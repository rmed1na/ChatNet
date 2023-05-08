using ChatNet.Data.Models;
using ChatNet.Data.Repositories;
using ChatNet.Models;
using ChatNet.Utils.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatNet.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepo;

        public AuthController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
                return Redirect("/home");

            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest($"{nameof(model.Username)} and {nameof(model.Password)} must have a value");

            var user = await _userRepo.GetAsync(model.Username);
            if (user == null)
                return NotFound("User not found");

            if (!PasswordUtility.VerifyPassword(model.Password, user.Password))
                return Unauthorized("Invalid credentials");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Name, user.GetFullName()),
                new Claim("username", model.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var props = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = false
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);

            return Redirect("/home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/auth/login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Username must be provided");
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password must be provided");

            // TODO: implement a password strength check (1 number, 1 special char, 1 upper case min, 8 min total length)
            var isUsernameTaken = await _userRepo.UsernameExistsAsync(model.Username);
            if (isUsernameTaken)
                return Conflict("Username is already taken. Please choose another one");

            var user = new User
            {
                Username = model.Username,
                Password = PasswordUtility.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            await _userRepo.AddAsync(user);
            return Redirect("/auth/login");
        }
    }
}
