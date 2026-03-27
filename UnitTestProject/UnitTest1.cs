using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFMaster.Pages;
using WPFMaster;

namespace UnitTestProject
{
    [TestClass]
    public class AuthUnitTest
    {
        [TestMethod]
        public void AuthTest()
        {
            var page = new SignInPage();
            Assert.IsTrue(AuthService.Auth("12", "12"));
            Assert.IsFalse(AuthService.Auth("Анастасия", "123123")); //<3
            Assert.IsFalse(AuthService.Auth("login", "password"));
        }

        [TestMethod]
        public void AuthTestSuccess()
        {
            foreach(Users user in Core.Context.Users)
            {
                Assert.IsTrue(AuthService.Auth(user.Login, user.Password));
            }
        }

        [TestMethod]
        public void AuthTestFail()
        {
            Assert.IsFalse(AuthService.Auth("", ""));
            Assert.IsFalse(AuthService.Auth("@#|5?.|/&%", ""));
        }
    }
}
