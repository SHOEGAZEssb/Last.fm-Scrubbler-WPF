using IF.Lastfm.Core.Api;
using Moq;
using NUnit.Framework;
using Scrubbler.Login;
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
      // given: user
      var tracks = TestHelper.CreateGenericScrobbles(50);
      var expected = tracks.ToLastTracks();

      var user = TestHelper.CreateUserWithRecentTracks(expected);

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