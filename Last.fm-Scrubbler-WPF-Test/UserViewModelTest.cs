using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.ViewModels;
using System.Collections.Generic;

namespace Scrubbler.Test
{
  /// <summary>
  /// Tests for the <see cref="UserViewModel"/>.
  /// </summary>
  [TestFixture]
  class UserViewModelTest
  {
    /// <summary>
    /// Tests the adding of an <see cref="User"/>.
    /// </summary>
    [Test]
    public void AddUserTest()
    {
      // given: mocks
      Mock<ILastAuth> lastAuthMock = new Mock<ILastAuth>();
      lastAuthMock.Setup(l => l.Authenticated).Returns(true);
      bool isSubscriber = true;
      string username = "TestUsername";
      string token = "TestToken";
      lastAuthMock.Setup(l => l.UserSession).Returns(new LastUserSession() { IsSubscriber = isSubscriber, Token = token, Username = username });

      Mock<IMessageBoxService> messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<LoginViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);
      windowManagerMock.SetupGet(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);

      Mock<IDirectoryOperator> directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(new string[0]);

      Mock<ISerializer<User>> userSerializerMock = new Mock<ISerializer<User>>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>()));

      UserViewModel vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, null, directoryOperatorMock.Object, userSerializerMock.Object);

      // when: adding the user
      vm.AddUser();

      // then: user with the values added
      Assert.That(vm.ActiveUser.IsSubscriber, Is.EqualTo(isSubscriber));
      Assert.That(vm.ActiveUser.Token, Is.EqualTo(token));
      Assert.That(vm.ActiveUser.Username, Is.EqualTo(username));
      // user added to list
      Assert.That(vm.AvailableUsers.Count, Is.EqualTo(1));
      // serialize was called
      Assert.That(() => userSerializerMock.Verify(u => u.Serialize(vm.ActiveUser, It.IsAny<string>())), Throws.Nothing);
    }
  }
}