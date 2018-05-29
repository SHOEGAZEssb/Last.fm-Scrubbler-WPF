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
  }
}