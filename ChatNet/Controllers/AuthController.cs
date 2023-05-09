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
    /// <summary>
    /// Authentication controller
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepo;

        public AuthController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Login view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
                return Redirect("/home");

            return View("~/Views/Auth/Login.cshtml");
        }

        /// <summary>
        /// Logs in the user with the provided credentials
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Logs out the user session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/auth/login");
        }

        /// <summary>
        /// Register view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        /// <summary>
        /// Creates a new user on the system
        /// </summary>
        /// <param name="model">User information object</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Username must be provided");
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password must be provided");

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
