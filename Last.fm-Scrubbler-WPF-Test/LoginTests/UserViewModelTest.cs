using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Scrubbler.Helper;
using Scrubbler.Login;
using ScrubblerLib.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
      var lastAuthMock = new Mock<ILastAuth>();
      lastAuthMock.Setup(l => l.Authenticated).Returns(true);
      bool isSubscriber = true;
      string username = "TestUsername";
      string token = "TestToken";
      lastAuthMock.Setup(l => l.UserSession).Returns(new LastUserSession() { IsSubscriber = isSubscriber, Token = token, Username = username });

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);

      var messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);

      var windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<LoginViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);
      windowManagerMock.SetupGet(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);

      var directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());

      var userSerializerMock = new Mock<ISerializer>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<IEnumerable<Type>>()));

      var vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, userApiMock.Object, null, directoryOperatorMock.Object, userSerializerMock.Object);

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
      var lastAuthMock = new Mock<ILastAuth>();
      lastAuthMock.Setup(l => l.Authenticated).Returns(true);
      bool isSubscriber = true;
      string username = "TestUsername";
      string token = "TestToken";
      lastAuthMock.Setup(l => l.UserSession).Returns(new LastUserSession() { IsSubscriber = isSubscriber, Token = token, Username = username });

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);

      var messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);

      var windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<LoginViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);
      windowManagerMock.SetupGet(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);

      var fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);
      fileOperatorMock.Setup(f => f.Delete(It.IsAny<string>()));

      var directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());

      var userSerializerMock = new Mock<ISerializer>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<IEnumerable<Type>>()));

      var vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, userApiMock.Object, fileOperatorMock.Object, directoryOperatorMock.Object, userSerializerMock.Object);

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
      var files = new string[]
      {
        "TestUser1.xml",
        "TestUser2.xml",
        "TestUser3.xml",
        "FileToBeIgnored.txt"
      };

      var directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(files);

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);

      var users = new User[]
      {
        new User("TestUser1", "TestToken1", true, userApiMock.Object),
        new User("TestUser2", "TestToken2", true, userApiMock.Object),
        new User("TestUser3", "TestToken3", true, userApiMock.Object)
      };

      var userSerializerMock = new Mock<ISerializer>(MockBehavior.Strict);
      for(int i = 0; i < users.Length; i++)
      {
        userSerializerMock.Setup(u => u.Deserialize<User>(files[i])).Returns(users[i]);
      }

      // when: creating the vm
      var vm = new UserViewModel(null, null, userApiMock.Object, null, directoryOperatorMock.Object, userSerializerMock.Object);

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
      var lastAuthMock = new Mock<ILastAuth>(MockBehavior.Strict);
      bool isSubscriber = true;
      string username = "TestUsername";
      string token = "TestToken";
      lastAuthMock.Setup(l => l.UserSession).Returns(new LastUserSession() { IsSubscriber = isSubscriber, Token = token, Username = username });
      lastAuthMock.Setup(l => l.LoadSession(It.IsAny<LastUserSession>())).Returns(true);
      lastAuthMock.Setup(l => l.Authenticated).Returns(true);

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 1, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 2, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                            It.IsAny<bool>(), 3, It.IsAny<int>()))
                                            .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));

      var messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);
      messageBoxServiceMock.Setup(m => m.ShowDialog(It.IsAny<string>())).Returns(IMessageBoxServiceResult.OK);

      var windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<LoginViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);
      windowManagerMock.SetupGet(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);

      var fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);

      var directoryOperatorMock = new Mock<IDirectoryOperator>(MockBehavior.Strict);
      directoryOperatorMock.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
      directoryOperatorMock.Setup(d => d.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());

      var userSerializerMock = new Mock<ISerializer>(MockBehavior.Strict);
      userSerializerMock.Setup(u => u.Serialize(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<IEnumerable<Type>>()));

      UserViewModel vm = new UserViewModel(windowManagerMock.Object, lastAuthMock.Object, userApiMock.Object, fileOperatorMock.Object, directoryOperatorMock.Object, userSerializerMock.Object);
      vm.AddUser();
      vm.SelectedUser = vm.AvailableUsers.First();

      // when: logging the user in
      vm.LoginUser();

      // then: active user changed
      Assert.That(vm.ActiveUser, Is.SameAs(vm.SelectedUser));
    }
  }
}