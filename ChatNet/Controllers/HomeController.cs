using ChatNet.Models;
using ChatNet.Utils.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatNet.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    [Route("[controller]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Main view (/index)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var userData = IdentityUtility.GetIdentityUserData(HttpContext.User?.Identity);
            ViewData["username"] = userData?.Username;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}