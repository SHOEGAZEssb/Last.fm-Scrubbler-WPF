using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using Scrubbler.Interfaces;
using Scrubbler.Models;

namespace Scrubbler.Test.ModelTests
{
  /// <summary>
  /// Tests for the <see cref="UserScrobbler"/>.
  /// </summary>
  [TestFixture]
  class UserScrobblerTest
  {
    /// <summary>
    /// Tests if the scrobble cap is correctly enforced.
    /// </summary>
    [Test]
    public void CapTest()
    {
      // given: mocks
      User user = new User("TestUser", "TestToken", false);

      Scrobble[] scrobbles = new Scrobble[User.MAXSCROBBLESPERDAY];
      for(int i = 0; i < User.MAXSCROBBLESPERDAY; i++)
      {
        scrobbles[i] = new Scrobble();
      }

      user.AddScrobbles(scrobbles);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      // when: trying to scrobble
      // then: exception
      Assert.That(async () => await userScrobbler.ScrobbleAsync(new Scrobble()), Throws.InvalidOperationException);
    }
  }
}