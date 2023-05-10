using Identity.Contracts;
using Identity.Dto;
using Identity.Entity;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controller
{
    public class UserController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserController(
            IPublishEndpoint publishEndpoint,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this.publishEndpoint = publishEndpoint;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IResult> Register([FromBody] RegistrationDto registrationDto)
        {
            var user = new ApplicationUser
            {
                UserName = registrationDto.UserName,
                Email = registrationDto.Email,
                DiscordId = registrationDto.DiscordId
            };
            var createResult = await userManager.CreateAsync(user, registrationDto.Password);
            if (!createResult.Succeeded)
            {
                return Results.BadRequest(createResult.Errors.FirstOrDefault()?.Description);
            }

            await signInManager.SignInAsync(user, true);
            
            await publishEndpoint.Publish(new UserRegistered(Guid.Parse(user.Id), user.UserName, user.Email, user.DiscordId));
            return Results.Ok();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IResult> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Results.BadRequest("User not found");
            }

            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
            if (!result.Succeeded)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("/logout")]
        public async Task<IResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
    }
}
