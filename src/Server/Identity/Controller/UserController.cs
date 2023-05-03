using Identity.Contracts;
using Identity.Dto;
using Identity.Entity;
using Identity.Repository;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Controller
{
    public class UserController : ControllerBase
    {
        private readonly UserRepository repository;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IDataProtectionProvider protectionProvider;
        private readonly IPublishEndpoint publishEndpoint;

        public UserController(
            UserRepository repository,
            IPasswordHasher<User> passwordHasher,
            IDataProtectionProvider protectionProvider,
            IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
            this.protectionProvider = protectionProvider;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new User();
            user.Username = username;
            user.Email = username;
            user.Id = Guid.NewGuid();
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            await repository.Create(user);

            await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    Convert(user)
                );

            await publishEndpoint.Publish(new UserRegistered(user.Id, user.Username, user.Email, user.DiscordId));
            return NoContent();
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register2([FromBody] RegistrationDto registrationDto)
        {
            var user = new User();
            user.Id = Guid.NewGuid();
            user.Username = registrationDto.UserName;
            user.DiscordId = registrationDto.DiscordId;
            user.Email = registrationDto.Email;
            user.PasswordHash = passwordHasher.HashPassword(user, registrationDto.Password);

            await repository.Create(user);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                Convert(user)
                );

            await publishEndpoint.Publish(new UserRegistered(user.Id, user.Username, user.Email, user.DiscordId));

            return NoContent();
        }

        [HttpGet]
        [Route("/login")]
        public async Task<ActionResult> Login(string email, string password)
        {
            var user = await repository.Get(email);
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

            return NoContent();
        }

        // TODO sample stuff
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

        public static ClaimsPrincipal Convert(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", user.Id.ToString())
            };

            claims.AddRange(user.Claims.Select(c => new Claim(c.Type, c.Value)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
