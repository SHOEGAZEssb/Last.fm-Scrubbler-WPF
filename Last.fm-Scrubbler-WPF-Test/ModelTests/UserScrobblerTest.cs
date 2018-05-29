using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

      var scrobbles = TestHelper.CreateGenericScrobbles(User.MAXSCROBBLESPERDAY);
      user.AddScrobbles(scrobbles, DateTime.Now);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

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
      User user = new User("TestUser", "TestToken", false);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(1);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected.First(), false);

      // then: correctly scrobbled and saved to the user
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(expected.Length));
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
      User user = new User("TestUser", "TestToken", false);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(10);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected, false);

      // then: correctly scrobbled and saved to the user
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(expected.Length));
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests if a failed scrobble request
    /// doesn't add scrobbles to the user.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleFailTest()
    {
      // given: mocks
      User user = new User("TestUser", "TestToken", false);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Unknown)));

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var scrobbles = TestHelper.CreateGenericScrobbles(10);

      // when: trying to scrobble
      var response = await userScrobbler.ScrobbleAsync(scrobbles, false);

      // then: no scrobbles added to the user, because scrobbling failed
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(0));
      Assert.That(response.Success, Is.False);
      Assert.That(response.Status == LastResponseStatus.Unknown);
    }

    /// <summary>
    /// Tests if scrobbling a single track with the
    /// caching scrobbler works.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleChachedTest()
    {
      // given: mocks
      User user = new User("TestUser", "TestToken", false);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      cachingScrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(1);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected.First(), true);

      // then: correctly scrobbled and saved to the user
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(expected.Length));
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Tests if scrobbling multiple tracks with the
    /// caching scrobbler works.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleMultipleChachedTest()
    {
      // given: mocks
      User user = new User("TestUser", "TestToken", false);

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);      

      Mock<IAuthScrobbler> cachingScrobblerMock = new Mock<IAuthScrobbler>(MockBehavior.Strict);
      IEnumerable<Scrobble> actual = null;
      cachingScrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      UserScrobbler userScrobbler = new UserScrobbler(user, scrobblerMock.Object, cachingScrobblerMock.Object);

      var expected = TestHelper.CreateGenericScrobbles(10);

      // when: scrobbling
      await userScrobbler.ScrobbleAsync(expected, true);

      // then: correctly scrobbled and saved to the user
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(expected.Length));
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
      User user = new User("TestUser", "TestToken", false);

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
      User user = new User("TestUser", "TestToken", false);

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