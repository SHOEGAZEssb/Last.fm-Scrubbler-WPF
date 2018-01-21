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
    [Test]
    public async Task ManualScrobblerScrobbleTest()
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
  }
}