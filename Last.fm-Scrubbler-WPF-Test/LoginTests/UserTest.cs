using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using Scrubbler.Login;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Test.LoginTests
{
  /// <summary>
  /// Tests for a <see cref="User"/>.
  /// </summary>
  [TestFixture]
  class UserTest
  {
    /// <summary>
    /// Tests the construction of
    /// a <see cref="User"/> object.
    /// </summary>
    [Test]
    public void ConstructionTest()
    {
      // given: example data
      string username = "TestUser";
      string token = "TestToken";
      bool subscriber = true;
      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);

      // when: creating a user
      User user = new User(username, token, subscriber, userApiMock.Object);

      // then: data is set
      Assert.That(user.Username, Is.SameAs(username));
      Assert.That(user.Token, Is.SameAs(token));
      Assert.That(user.IsSubscriber, Is.EqualTo(subscriber));
      Assert.That(user.RecentScrobblesCache, Is.Null);
    }

    [Test]
    public async Task GetRecentScrobblesTest()
    {
      // given: example data
      string username = "TestUser";
      string token = "TestToken";
      bool subscriber = true;

      var tracks = TestHelper.CreateGenericScrobbles(50);
      var expected = tracks.ToLastTracks();
      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      // setup so first page returs the scrobbles
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 1, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse(expected)));
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 2, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                            It.IsAny<bool>(), 3, It.IsAny<int>()))
                                            .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));

      var user = new User(username, token, subscriber, userApiMock.Object);
      bool eventFired = false;
      user.RecentScrobblesCacheUpdated += (o, e) => eventFired = true;

      // when: getting recent scrobbles
      await user.UpdateRecentScrobbles();

      // then: Cache updated and event fired
      CollectionAssert.AreEqual(expected, user.RecentScrobblesCache);
      Assert.That(eventFired, Is.True);
    }
  }
}