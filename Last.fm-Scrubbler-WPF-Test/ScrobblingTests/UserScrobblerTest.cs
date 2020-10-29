using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using Scrubbler.Login;
using Scrubbler.Scrobbling;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Test.ScrobblingTests
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
      // given: capped user and mocks
      User user = TestHelper.CreateCappedUser();

      var scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      var cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      // when: trying to scrobble
      // then: exception
      Assert.That(async () => await userScrobbler.ScrobbleAsync(new Scrobble()), Throws.InvalidOperationException);
    }

    /// <summary>
    /// Tests the scrobbling a single scrobble.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleTest()
    {
      // given: mocks
      User user = TestHelper.CreateUserWithRecentTracks(Enumerable.Empty<LastTrack>());

      var scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      var cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      var userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(1);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected.First(), false);

      // then: correctly scrobbled
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests the scrobbling of multiple scrobbles.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleMultipleTest()
    {
      // given: mocks
      User user = TestHelper.CreateUserWithRecentTracks(Enumerable.Empty<LastTrack>());

      var scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      var cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(10);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected, false);

      // then: correctly scrobbled
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests if scrobbling a single track with the
    /// caching scrobbler works.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleCachedTest()
    {
      // given: mocks
      User user = TestHelper.CreateUserWithRecentTracks(Enumerable.Empty<LastTrack>());

      var scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      var cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      cachingScrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      var userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(1);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected.First(), true);

      // then: correctly scrobbled
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests if scrobbling multiple tracks with the
    /// caching scrobbler works.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleMultipleCachedTest()
    {
      // given: mocks
      User user = TestHelper.CreateUserWithRecentTracks(Enumerable.Empty<LastTrack>());

      var scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      var cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      cachingScrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(10);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected, true);

      // then: correctly scrobbled
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests if the <see cref="UserScrobbler"/> is
    /// authenticated when its scrobblers are
    /// authenticated.
    /// </summary>
    [Test]
    public void AuthenticationTest()
    {
      // given: mocks
      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      User user = new User("TestUser", "TestToken", false, userApiMock.Object);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      Mock<ILastAuth> authMock = new Mock<ILastAuth>(MockBehavior.Strict);
      authMock.Setup(a => a.Authenticated).Returns(true);
      scrobblerMock.Setup(s => s.Auth).Returns(authMock.Object);

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      cachingScrobblerMock.Setup(s => s.Auth).Returns(authMock.Object);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      // when: checking the auth status
      // then: should be true
      Assert.That(userScrobbler.IsAuthenticated, Is.True);
    }

    /// <summary>
    /// Tests if the <see cref="UserScrobbler"/> is
    /// not authenticated when its scrobblers aren't
    /// authenticated.
    /// </summary>
    [Test]
    public void NoAuthenticationTest()
    {
      // given: mocks
      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      User user = new User("TestUser", "TestToken", false, userApiMock.Object);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      Mock<ILastAuth> authMock = new Mock<ILastAuth>(MockBehavior.Strict);
      authMock.Setup(a => a.Authenticated).Returns(false);
      scrobblerMock.Setup(s => s.Auth).Returns(authMock.Object);

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      cachingScrobblerMock.Setup(s => s.Auth).Returns(authMock.Object);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      // when: checking the auth status
      // then: should be false
      Assert.That(userScrobbler.IsAuthenticated, Is.False);
    }
  }
}