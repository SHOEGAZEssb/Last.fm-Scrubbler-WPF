using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Scrobbler;
using ScrubblerLib;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Test.ScrobblingTests
{
  /// <summary>
  /// Tests for the <see cref="CacheScrobblerViewModel"/>.
  /// </summary>
  [TestFixture]
  class CacheScrobblerTest
  {
    /// <summary>
    /// Tests the <see cref="CacheScrobblerViewModel.Scrobble"/> function.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleTest()
    {
      // given: CacheScrobbleViewModel and mocks
      var scrobbles = TestHelper.CreateGenericScrobbles(3);

      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>(MockBehavior.Strict);
      scrobblerMock.Setup(u => u.GetCachedAsync()).Returns(Task.Run(() => scrobbles.AsEnumerable()));
      scrobblerMock.Setup(u => u.SendCachedScrobblesAsync()).Returns(Task.Run(() => new ScrobbleResponse()));
      scrobblerMock.Setup(u => u.IsAuthenticated).Returns(true);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      CacheScrobblerViewModel vm = new CacheScrobblerViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object
      };

      await vm.GetCachedScrobbles();

      // when: scrobbling the cached tracks
      await vm.Scrobble();

      // then: send has been called
      Assert.That(() => scrobblerMock.Verify(u => u.SendCachedScrobblesAsync(), Times.Once), Throws.Nothing);
    }

    /// <summary>
    /// Tests the <see cref="CacheScrobblerViewModel.GetCachedScrobbles"/> function.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task GetCachedScrobblesTest()
    {
      // given: CacheScrobbleViewModel and mocks
      var expected = TestHelper.CreateGenericScrobbles(3);

      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>(MockBehavior.Strict);
      scrobblerMock.Setup(u => u.GetCachedAsync()).Returns(Task.Run(() => expected.AsEnumerable()));

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      CacheScrobblerViewModel vm = new CacheScrobblerViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object
      };

      // when: getting the cached scrobbles
      await vm.GetCachedScrobbles();

      // then: scrobbles are the expected ones
      Assert.That(vm.Scrobbles.IsEqualScrobble(expected));
    }
  }
}