using LeavePortal.Areas.Identity.Data;
using LeavePortal.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
namespace LeavePortalTesting
{
    public class UnitTest3
    {
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<LoginModel>> _loggerMock;

        public UnitTest3()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

            _loggerMock = new Mock<ILogger<LoginModel>>();
        }

        private void SetupSignInManagerMock(SignInResult result)
        {
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                              .ReturnsAsync(result);
        }

        private void SetupUserManagerMock(ApplicationUser user, IList<string> roles)
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                            .ReturnsAsync(roles);
        }

        [Fact]
        public async Task TestUserLogin_Success_With_Admin_Role()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "adminuser", Email = "adminuser@example.com" };
            var roles = new List<string> { "Admin" };

            SetupSignInManagerMock(SignInResult.Success);
            SetupUserManagerMock(user, roles);

            var loginModel = new LoginModel(_signInManagerMock.Object, _loggerMock.Object, _userManagerMock.Object)
            {
                Input = new LoginModel.InputModel
                {
                    Email = "adminuser@example.com",
                    Password = "Test@123"
                }
            };

            // Act
            var result = await loginModel.OnPostAsync("/Index");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Admin", redirectToActionResult.ControllerName);
        }

        [Fact]
        public async Task TestUserLogin_Failure()
        {
            // Arrange
            SetupSignInManagerMock(SignInResult.Failed);

            var loginModel = new LoginModel(_signInManagerMock.Object, _loggerMock.Object, _userManagerMock.Object)
            {
                Input = new LoginModel.InputModel
                {
                    Email = "user@example.com",
                    Password = "WrongPassword"
                }
            };

            // Act
            var result = await loginModel.OnPostAsync("/Login");

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.True(loginModel.ModelState.ContainsKey(string.Empty));
            Assert.Equal("Invalid login attempt.", loginModel.ModelState[string.Empty].Errors[0].ErrorMessage);
        }
    }
}
