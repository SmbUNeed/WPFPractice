using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFMaster.Pages;
using WPFMaster;

namespace RegisterUnitProject
{
    [TestClass]
    public class RegisterUnitTest
    {
        [TestMethod]
        public void NegativeTests()
        {
            var page = new SignUpPage();
            Assert.IsFalse(page.TrySignUp("lik", "", "123", "bb@m.ru"));
            Assert.IsTrue(page.TrySignUp("lik421dsfs#$@#", "123", "123", "bb@m.ru"));
            Assert.IsFalse(page.TrySignUp("lik", "123", "123", "bbru"));
        }

        [TestMethod]
        public void PositiveTests()
        {
            var page = new SignUpPage();
            Assert.IsTrue(page.TrySignUp("lik", "123", "123", "bb@m.ru"));
            Assert.IsFalse(page.TrySignUp("lik421ds", "", "123", "bb@m.ru"));
            Assert.IsFalse(page.TrySignUp("likerrrr", "", "", "bb@ru"));
        }
    }
}