using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using Scrubbler.Helper;
using Scrubbler.Login;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Test.LoginTests
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

      // user added to list
      Assert.That(vm.AvailableUsers.Count, Is.EqualTo(1));
      // then: user with the values added
      Assert.That(vm.AvailableUsers.First().IsSubscriber, Is.EqualTo(isSubscriber));
      Assert.That(vm.AvailableUsers.First().Token, Is.SameAs(token));
      Assert.That(vm.AvailableUsers.First().Username, Is.SameAs(username));
      // then: added user is active
      Assert.That(vm.ActiveUser, Is.SameAs(vm.AvailableUsers.First()));
      Assert.That(vm.Username, Is.SameAs(vm.AvailableUsers.First().Username));
      // serialize was called
      Assert.That(() => userSerializerMock.Verify(u => u.Serialize(vm.ActiveUser, It.IsAny<string>())), Throws.Nothing);
    }

    /// <summary>
    /// Tests the removal of an <see cref="User"/>.
    /// </summary>
    [Test]
    public void RemoveUserTest()
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

      Mock<IFileOperator> fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);
      fileOperatorMock.Setup(f => f.Delete(It.IsAny<string>()));

      Mock<IDirectoryOperator> directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(new string[0]);

      Mock<ISerializer<User>> userSerializerMock = new Mock<ISerializer<User>>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>()));

      UserViewModel vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, fileOperatorMock.Object, directoryOperatorMock.Object, userSerializerMock.Object);

      vm.AddUser();
      vm.SelectedUser = vm.AvailableUsers.First();

      // when: removing the selected user
      vm.RemoveUser();

      // then: ActiveUser is null and was removed
      Assert.That(vm.ActiveUser, Is.Null);
      CollectionAssert.IsEmpty(vm.AvailableUsers);
      Assert.That(() => fileOperatorMock.Verify(f => f.Delete(It.IsAny<string>()), Times.Once), Throws.Nothing);
    }

    /// <summary>
    /// Tests the deserialization of users.
    /// </summary>
    [Test]
    public void DeserializeUsersTest()
    {
      // given: needed mocks
      string[] files = new string[]
      {
        "TestUser1.xml",
        "TestUser2.xml",
        "TestUser3.xml",
        "FileToBeIgnored.txt"
      };

      Mock<IDirectoryOperator> directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(files);

      User[] users = new User[]
      {
        new User("TestUser1", "TestToken1", true),
        new User("TestUser2", "TestToken2", true),
        new User("TestUser3", "TestToken3", true)
      };

      Mock<ISerializer<User>> userSerializerMock = new Mock<ISerializer<User>>(MockBehavior.Strict);
      for(int i = 0; i < users.Length; i++)
      {
        userSerializerMock.Setup(u => u.Deserialize(files[i])).Returns(users[i]);
      }

      // when: creating the vm
      UserViewModel vm = new UserViewModel(null, null, null, directoryOperatorMock.Object, userSerializerMock.Object);

      // then: users have been deserialized
      Assert.That(vm.AvailableUsers.Count, Is.EqualTo(users.Length));
      CollectionAssert.AreEqual(users, vm.AvailableUsers);
    }

    /// <summary>
    /// Tests if logging a user in works.
    /// </summary>
    [Test]
    public void LoginTest()
    {
      // given: mocks
      Mock<ILastAuth> lastAuthMock = new Mock<ILastAuth>(MockBehavior.Strict);
      bool isSubscriber = true;
      string username = "TestUsername";
      string token = "TestToken";
      lastAuthMock.Setup(l => l.UserSession).Returns(new LastUserSession() { IsSubscriber = isSubscriber, Token = token, Username = username });
      lastAuthMock.Setup(l => l.LoadSession(It.IsAny<LastUserSession>())).Returns(true);
      lastAuthMock.Setup(l => l.Authenticated).Returns(true);

      Mock<IMessageBoxService> messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);
      messageBoxServiceMock.Setup(m => m.ShowDialog(It.IsAny<string>())).Returns(IMessageBoxServiceResult.OK);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<LoginViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);
      windowManagerMock.SetupGet(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);

      Mock<IFileOperator> fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);
      fileOperatorMock.Setup(f => f.Delete(It.IsAny<string>()));

      Mock<IDirectoryOperator> directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(new string[0]);

      Mock<ISerializer<User>> userSerializerMock = new Mock<ISerializer<User>>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>()));

      UserViewModel vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, fileOperatorMock.Object, directoryOperatorMock.Object, userSerializerMock.Object);
      vm.AddUser();
      vm.SelectedUser = vm.AvailableUsers.First();

      // when: logging the user in
      vm.LoginUser();

      // then: active user changed
      Assert.That(vm.ActiveUser, Is.SameAs(vm.SelectedUser));
    }
  }
}