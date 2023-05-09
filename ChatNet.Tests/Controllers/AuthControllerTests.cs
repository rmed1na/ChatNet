using ChatNet.Controllers;
using ChatNet.Data.Models;
using ChatNet.Data.Repositories;
using ChatNet.Models;
using ChatNet.Utils.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace ChatNet.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<ITempDataDictionary> _mockTempData;

        #region Data sets
        private readonly List<User> _users = new();
        #endregion

        [SetUp]
        public void SetUp()
        {
            _mockTempData = new Mock<ITempDataDictionary>();
            _mockUserRepo = new Mock<IUserRepository>();
        }

        [Test]
        public void Login_Authenticated()
        {
            var controller = GetController();

            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.GivenName, "first"),
                new Claim(ClaimTypes.Surname, "last"),
                new Claim(ClaimTypes.Name, "first last"),
                new Claim("username", "mockUsername")
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            var result = controller.Login() as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo("/home"));
        }

        [Test]
        public void Login_NotAuthenticated()
        {
            var controller = GetController();
            var result = controller.Login() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Does.Contain("Login.cshtml"));
        }

        [Test]
        public async Task Login_Authenticate()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            _mockUserRepo
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Username = "username",
                    Password = PasswordUtility.HashPassword("pwd@123")
                });

            var controller = new AuthController(_mockUserRepo.Object)
            {
                TempData = _mockTempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };

            var result = await controller.Login(new LoginViewModel
            {
                Username = "username",
                Password = "pwd@123"
            }) as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo("/home"));
        }

        [Test]
        public async Task Login_MissingParams()
        {
            var controller = GetController();
            var result = await controller.Login(new LoginViewModel()) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value as string, Is.EqualTo($"{nameof(LoginViewModel.Username)} and {nameof(LoginViewModel.Password)} must have a value"));
        }

        [Test]
        public async Task Login_MissingUser()
        {
            var controller = GetController();
            var result = await controller.Login(new LoginViewModel
            {
                Username = "username",
                Password = "pwd@123"
            }) as NotFoundObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value as string, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task Login_InvalidCredentials()
        {
            _mockUserRepo
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Username = "username",
                    Password = PasswordUtility.HashPassword("not_the_password_you_expect")
                });

            var controller = GetController();
            var result = await controller.Login(new LoginViewModel
            {
                Username = "username",
                Password = "pwd@123"
            }) as UnauthorizedObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Invalid credentials"));
        }

        [Test]
        public async Task Logout_Ok()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var controller = new AuthController(_mockUserRepo.Object)
            {
                TempData = _mockTempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };

            var result = await controller.Logout() as RedirectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo("/auth/login"));
        }

        [Test]
        public void Register_View()
        {
            var controller = GetController();
            var result = controller.Register() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Does.Contain("Register.cshtml"));
        }

        [Test]
        public async Task Register_Ok()
        {
            _mockUserRepo
                .Setup(x => x.UsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockUserRepo
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback(() =>
                {
                    _users.Add(new User
                    {
                        Username = "username"
                    });
                });

            var controller = GetController();
            var result = await controller.Register(new UserRegistrationViewModel
            {
                Username = "username",
                FirstName = "first",
                LastName = "last",
                Password = "pwd@123"
            }) as RedirectResult;

            _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()));
            Assert.Multiple(() =>
            {
                Assert.That(_users.Any(x => x.Username == "username"), Is.True);
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Url, Is.EqualTo("/auth/login"));
            });
        }

        [Test]
        public async Task Register_InvalidUsername()
        {
            var controller = GetController();
            var result = await controller.Register(new UserRegistrationViewModel()) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value as string, Is.EqualTo("Username must be provided"));
        }

        [Test]
        public async Task Register_MissingPassword()
        {
            var controller = GetController();
            var result = await controller.Register(new UserRegistrationViewModel
            {
                Username = "username"
            }) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value as string, Is.EqualTo("Password must be provided"));
        }

        [Test]
        public async Task Register_TakenUsername()
        {
            _mockUserRepo
                .Setup(x => x.UsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = GetController();
            var result = await controller.Register(new UserRegistrationViewModel
            {
                Username = "username",
                Password = "pwd@123"
            }) as ConflictObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value as string, Is.EqualTo("Username is already taken. Please choose another one"));
        }

        private AuthController GetController()
        {
            return new AuthController(_mockUserRepo.Object)
            {
                TempData = _mockTempData.Object,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                }
            };
        }
    }
}
