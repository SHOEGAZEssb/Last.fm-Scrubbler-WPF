using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using Moq;
using NUnit.Framework;
using Scrubbler.Helper;
using Scrubbler.Login;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrubbler.Test.LoginTests
{
  /// <summary>
  /// Tests for the <see cref="LoginViewModel"/>.
  /// </summary>
  [TestFixture]
  class LoginViewModelTest
  {
    /// <summary>
    /// Tests if logging in with credentials work.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task LoginTest()
    {
      // given: mocks
      string user = "TestUser";
      string password = "TestPassword";
      PasswordBox pwBox = null;

      Thread thread = new Thread(() =>
      {
        pwBox = new PasswordBox()
        {
          Password = password
        };
      });
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();

      // wait for the pwBox to be initialized
      while (pwBox == null)
        await Task.Delay(10);

      Mock<ILastAuth> lastAuthMock = new Mock<ILastAuth>(MockBehavior.Strict);
      lastAuthMock.Setup(l => l.GetSessionTokenAsync(user, password)).Callback(() => lastAuthMock.SetupGet(l => l.Authenticated).Returns(true))
                                                                     .Returns(Task.Run(() => LastResponse.CreateSuccessResponse()));

      Mock<IMessageBoxService> messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);
      messageBoxServiceMock.Setup(m => m.ShowDialog(It.IsAny<string>())).Returns(IMessageBoxServiceResult.OK);

      LoginViewModel vm = new LoginViewModel(lastAuthMock.Object, messageBoxServiceMock.Object);

      // when: logging in
      await vm.Login(user, pwBox);

      // then: user is authenticated
      Assert.That(lastAuthMock.Object.Authenticated, Is.True);
    }
  }
}