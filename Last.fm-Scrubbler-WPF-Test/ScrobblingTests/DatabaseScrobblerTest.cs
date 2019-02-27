using DiscogsClient;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using Scrubbler.Helper;
using Scrubbler.Scrobbling;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Test.ScrobblingTests
{
  /// <summary>
  /// Tests for the <see cref="DatabaseScrobbleViewModel"/>.
  /// </summary>
  [TestFixture]
  class DatabaseScrobblerTest
  {
    /// <summary>
    /// Tests the <see cref="DatabaseScrobbleViewModel.Scrobble"/> function.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleTest()
    {
      // given: mocks
      string artistSearchText = "TestArtist";

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      IEnumerable<LastArtist> artists = TestHelper.CreateGenericArtists(3);
      IEnumerable<LastAlbum> albums = TestHelper.CreateGenericAlbums(3);

      Mock<IArtistApi> artistAPIMock = new Mock<IArtistApi>(MockBehavior.Strict);
      artistAPIMock.Setup(a => a.SearchAsync(artistSearchText, It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(Task.Run(() => PageResponse<LastArtist>.CreateSuccessResponse(artists)));
      artistAPIMock.Setup(a => a.GetTopAlbumsAsync(artists.First().Name, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(Task.Run(() => PageResponse<LastAlbum>.CreateSuccessResponse(albums)));

      DateTime scrobbleTime = DateTime.Now;
      List<Scrobble> expectedScrobbles = new List<Scrobble>();
      for(int i = 0; i < 3; i++)
      {
        // -3 minutes because that is the default if the song has no duration
        expectedScrobbles.Add(new Scrobble("TestArtist", "TestAlbum", "TestTrack", scrobbleTime.Subtract(TimeSpan.FromMinutes(i * 3))));
      }

      LastAlbum albumToScrobble = albums.First();
      albumToScrobble.Tracks = expectedScrobbles.ToLastTracks();

      Mock<IAlbumApi> albumAPIMock = new Mock<IAlbumApi>(MockBehavior.Strict);
      albumAPIMock.Setup(a => a.GetInfoAsync(albums.First().ArtistName, albums.First().Name, It.IsAny<bool>(), It.IsAny<string>()))
                  .Returns(Task.Run(() => LastResponse<LastAlbum>.CreateSuccessResponse(albumToScrobble)));

      IEnumerable<Scrobble> actual = null;
      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>(MockBehavior.Strict);
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>(), false)).Callback<IEnumerable<Scrobble>, bool>((s, c) => actual = s)
                                                                                  .Returns(Task.Run(() => new ScrobbleResponse()));
      scrobblerMock.Setup(u => u.IsAuthenticated).Returns(true);

      var discocsClientMock = new Mock<IDiscogsDataBaseClient>(MockBehavior.Strict);

      var vm = new DatabaseScrobbleViewModel(windowManagerMock.Object, artistAPIMock.Object, albumAPIMock.Object, discocsClientMock.Object)
      {
        DatabaseToSearch = Database.LastFm,
        Scrobbler = scrobblerMock.Object,
        SearchType = SearchType.Artist,
        SearchText = artistSearchText,
      };
      vm.ScrobbleTimeVM.UseCurrentTime = false;
      vm.ScrobbleTimeVM.Time = scrobbleTime;

      // when searching and scrobbling
      await vm.Search();

      // now we should have artists
      // we "click" the first one, this should trigger the album search
      (vm.ActiveItem as ArtistResultViewModel).Items.First().Clicked();

      // now we should have albums
      // we "click" the first one, this should trigger the tracklist fetching
      (vm.ActiveItem as ReleaseResultViewModel).Items.First().Clicked();

      vm.CheckAll();
      await vm.Scrobble();

      // then: correct items where scrobbles
      Assert.That(actual.IsEqualScrobble(expectedScrobbles), Is.True);
    }
  }
}