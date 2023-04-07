using Identity.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Repository;

namespace UserService.Controller
{
    public class UserController : ControllerBase
    {
        private readonly UserRepository repository;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IDataProtectionProvider protectionProvider;
        //private readonly UserManager<IdentityUser> userManager;
        //private readonly SignInManager<IdentityUser> signInManager;

        public UserController(UserRepository repository, IPasswordHasher<User> passwordHasher, IDataProtectionProvider protectionProvider/*, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager*/)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
            this.protectionProvider = protectionProvider;
            //this.userManager = userManager;
            //this.signInManager = signInManager;
        }

        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new User();
            user.Username = username;
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            await repository.Create(user);

            
            await HttpContext.SignInAsync(         
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    Convert(user)
                );

            return Ok();
        }

        [HttpGet]
        [Route("/login")]
        public async Task<ActionResult> Login(string username, string password)
        {
            var user = await repository.Get(username);
            if (user == null)
            {
                return BadRequest();
            }

            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return BadRequest();
            }

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    Convert(user)
                );
            return Ok("Logged in!");
        }

        [HttpGet]
        [Route("/promote")]
        public async Task<ActionResult> Promote(string username)
        {
            var user = await repository.Get(username);
            if (user == null)
            {
                return BadRequest();
            }

            user.Claims.Add(new UserClaim() { Type = "role", Value = "manager" });

            await repository.Create(user);

            return Ok("Promoted!");
        }

        [HttpGet]
        [Route("/start-password-reset")]
        public async Task<ActionResult> PasswordResetStart(string username)
        {
            var protector = protectionProvider.CreateProtector("PasswordReset");
            var user = await repository.Get(username);
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(protector.Protect(user.Username));
        }

        [HttpGet]
        [Route("/end-password-reset")]
        public async Task<ActionResult> PassWordResetEnd(string username, string password, string hash)
        {
            var protector = protectionProvider.CreateProtector("PasswordReset");
            var hashUserName = protector.Unprotect(hash);
            if (hashUserName != username)
            {
                return BadRequest();
            }

            var user = await repository.Get(username);
            if (user == null)
            {
                return BadRequest();
            }

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await repository.Create(user);

            return Ok("passwrd was reset");
        }

        [HttpGet]
        [Route("/protected")]
        [Authorize("manager")]
        public ActionResult Protected()
        {
            return Ok("Secret");
        }

        [HttpGet]
        [Route("/test")]
        public ActionResult Public()
        {

            return Ok("OK");
        }

        // TODO should be a service
        public static ClaimsPrincipal Convert(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("username", user.Username)
            };

            claims.AddRange(user.Claims.Select(c => new Claim(c.Type, c.Value)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
