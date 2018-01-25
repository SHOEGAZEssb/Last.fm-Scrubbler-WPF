using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF_Test.ScrobblerTests
{
  [TestFixture]
  class ManualScrobblerTest
  {
    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.Scrobble"/>.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleTest()
    {
      DateTime expectedTime = DateTime.Now;
      Scrobble expected = new Scrobble("TestArtist", "TestAlbum", "TestTrack", expectedTime) { AlbumArtist = "TestAlbumArtist", Duration = TimeSpan.FromSeconds(30) };

      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>();
      Scrobble actual = null;
      scrobblerMock.Setup(i => i.ScrobbleAsync(It.IsAny<Scrobble>())).Callback<Scrobble>(s => actual = s);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(null, scrobblerMock.Object)
      {
        UseCurrentTime = false,

        Artist = expected.Artist,
        Album = expected.Album,
        Track = expected.Track,
        Time = expectedTime,
        AlbumArtist = expected.AlbumArtist,
        Duration = expected.Duration.Value
      };

      await vm.Scrobble();

      Assert.That(actual.IsEqualScrobble(expected), Is.True);
    }

    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.CanScrobble"/> is false
    /// when no auth is done.
    /// </summary>
    [Test]
    public void CanScrobbleNoAuthTest()
    {
      // given: ManualScrobbleViewModel without auth
      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>();
      scrobblerMock.Setup(s => s.Auth.Authenticated).Returns(false);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(null, scrobblerMock.Object)
      {
        // set artist and track
        Artist = "TestArtist",
        Track = "TestTrack"
      };

      // then: CanScrobble should be false
      Assert.That(vm.CanScrobble, Is.False);

      // make sure it really was the auth
      scrobblerMock.Setup(s => s.Auth.Authenticated).Returns(true);
      Assert.That(vm.CanScrobble, Is.True);
    }

    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.CanScrobble"/> is false
    /// when no artist is given.
    /// </summary>
    [Test]
    public void CanScrobbleNoArtistTest()
    {
      // given: ManualScrobbleViewModel without given artist.
      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>();
      scrobblerMock.Setup(s => s.Auth.Authenticated).Returns(true);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(null, scrobblerMock.Object)
      {
        // set track
        Track = "TestTrack"
      };

      // then: CanScrobble should be false
      Assert.That(vm.CanScrobble, Is.False);

      // make sure it really was the artist
      vm.Artist = "TestArtist";
      Assert.That(vm.CanScrobble, Is.True);
    }

    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.CanScrobble"/> is false
    /// when no track is given.
    /// </summary>
    [Test]
    public void CanScrobbleNoTrackTest()
    {
      // given: ManualScrobbleViewModel without given artist.
      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>();
      scrobblerMock.Setup(s => s.Auth.Authenticated).Returns(true);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(null, scrobblerMock.Object)
      {
        // set artist
        Artist = "TestArtist"
      };

      // then: CanScrobble should be false
      Assert.That(vm.CanScrobble, Is.False);

      // make sure it really was the track
      vm.Track = "TestTrack";
      Assert.That(vm.CanScrobble, Is.True);
    }
  }
}