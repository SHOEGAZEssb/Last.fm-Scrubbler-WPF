using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Moq;
using Octokit;
using Scrubbler.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Test
{
  /// <summary>
  /// Helper methods for tests.
  /// </summary>
  static class TestHelper
  {
    /// <summary>
    /// Checks if this Scrobble has equal values as the <paramref name="expected"/> scrobble.
    /// </summary>
    /// <param name="actual">Actual scrobble values.</param>
    /// <param name="expected">Scrobble values to check.</param>
    /// <returns>True if scrobbles are equal, false if not.</returns>
    public static bool IsEqualScrobble(this Scrobble actual, Scrobble expected)
    {
      return actual.Artist == expected.Artist &&
        actual.Album == expected.Album &&
        actual.Track == expected.Track &&
        actual.TimePlayed.ToString() == expected.TimePlayed.ToString() &&
        actual.AlbumArtist == expected.AlbumArtist &&
        actual.Duration == expected.Duration;
    }

    /// <summary>
    /// Checks if this scrobble list is equal to the given <paramref name="expected"/>.
    /// </summary>
    /// <param name="actual">Actual scrobble list.</param>
    /// <param name="expected">Expected scrobble list.</param>
    /// <returns>True if scrobbles are equal, false if not.</returns>
    public static bool IsEqualScrobble(this IEnumerable<Scrobble> actual, IEnumerable<Scrobble> expected)
    {
      for(int i = 0; i < actual.Count(); i++)
      {
        if (!actual.ElementAt(i).IsEqualScrobble(expected.ElementAt(i)))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Creates <see cref="LastTrack"/>s with the same values as the
    /// given <paramref name="scrobbles"/>.
    /// </summary>
    /// <param name="scrobbles">Scrobbles to create <see cref="LastTrack"/>s for.</param>
    /// <returns>List with <see cref="LastTrack"/>s.</returns>
    public static IEnumerable<LastTrack> ToLastTracks(this IEnumerable<Scrobble> scrobbles)
    {
      List<LastTrack> tracks = new List<LastTrack>();
      foreach(var s in scrobbles)
      {
        tracks.Add(new LastTrack() { ArtistName = s.Artist, AlbumName = s.Album, Name = s.Track, TimePlayed = s.TimePlayed, Duration = s.Duration });
      }

      return tracks;
    }

    /// <summary>
    /// Clones the given <paramref name="scrobble"/> with an added
    /// second to the <see cref="Scrobble.TimePlayed"/>. This is used
    /// because some scrobblers add an extra second to be able
    /// to scrobble duplicates.
    /// </summary>
    /// <param name="scrobble">The scrobble to clone with an additional second.</param>
    /// <returns>Cloned scrobble.</returns>
    public static Scrobble CloneWithAddedTime(this Scrobble scrobble, TimeSpan timeToAdd)
    {
      return new Scrobble(scrobble.Artist, scrobble.Album, scrobble.Track, scrobble.TimePlayed.Add(timeToAdd))
                         { AlbumArtist = scrobble.AlbumArtist, Duration = scrobble.Duration };
    }

    public static IEnumerable<Scrobble> CloneWithAddedTime(this IEnumerable<Scrobble> scrobbles, TimeSpan timeToAdd)
    {
      return scrobbles.Select(s => s.CloneWithAddedTime(timeToAdd));
    }

    /// <summary>
    /// Creates generic scrobbles (without information).
    /// </summary>
    /// <param name="amount">Amount of scrobbles to create.</param>
    /// <returns>Newly created, generic scrobbles.</returns>
    public static Scrobble[] CreateGenericScrobbles(int amount)
    {
      Scrobble[] scrobbles = new Scrobble[amount];
      for (int i = 0; i < scrobbles.Length; i++)
      {
        scrobbles[i] = new Scrobble();
      }

      return scrobbles;
    }

    /// <summary>
    /// Creates generic artists.
    /// Only the name is set to "TestArtist" + ID.
    /// </summary>
    /// <param name="amount">Amount of artists to create.</param>
    /// <returns>Newly created artists.</returns>
    public static LastArtist[] CreateGenericArtists(int amount)
    {
      LastArtist[] artists = new LastArtist[amount];
      for(int i = 0; i < artists.Length; i++)
      {
        artists[i] = new LastArtist() { Name = $"TestArtist{i}" };
      }

      return artists;
    }

    /// <summary>
    /// Creates generic albums.
    /// </summary>
    /// <param name="amount">Amount of albums to create.</param>
    /// <returns>Newly created albums.</returns>
    public static LastAlbum[] CreateGenericAlbums(int amount)
    {
      LastAlbum[] albums = new LastAlbum[amount];
      for(int i = 0; i < albums.Length; i++)
      {
        albums[i] = new LastAlbum() { ArtistName = $"TestArtist{i}", Name = $"TestAlbum{i}" };
      }

      return albums;
    }

    /// <summary>
    /// Creates a generic user that has the given <paramref name="tracks"/>
    /// after <see cref="Login.User.UpdateRecentScrobbles"/> is called.
    /// </summary>
    /// <param name="tracks">Tracks of the user.</param>
    /// <returns>User.</returns>
    public static Login.User CreateUserWithRecentTracks(IEnumerable<LastTrack> tracks)
    {
      return CreateUserWithRecentTracks("TestUser", "TestToken", true, tracks);
    }

    public static Login.User CreateUserWithRecentTracks(string username, string token, bool isSubscriber, IEnumerable<LastTrack> tracks)
    {
      if (tracks == null || tracks.Count() > 1000)
        throw new ArgumentException(nameof(tracks), "tracks is null or has more than 1000 tracks");

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      // setup so first page returs the scrobbles
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 1, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse(tracks)));
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 2, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));
      userApiMock.Setup(u => u.GetRecentScrobbles(username, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                            It.IsAny<bool>(), 3, It.IsAny<int>()))
                                            .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse()));

      return new Login.User(username, token, isSubscriber, userApiMock.Object);
    }

    /// <summary>
    /// Creates a user that has hit the daily scrobble limit.
    /// </summary>
    /// <returns>User with reached limit.</returns>
    public static Login.User CreateCappedUser()
    {
      var tracks1 = CreateGenericScrobbles(1000).ToLastTracks();
      var tracks2 = CreateGenericScrobbles(1000).ToLastTracks();
      var tracks3 = CreateGenericScrobbles(1000).ToLastTracks();

      var userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      // setup so first page returs the scrobbles
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 1, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse(tracks1)));
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                                  It.IsAny<bool>(), 2, It.IsAny<int>()))
                                                  .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse(tracks2)));
      userApiMock.Setup(u => u.GetRecentScrobbles(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                                            It.IsAny<bool>(), 3, It.IsAny<int>()))
                                            .Returns(() => Task.Run(() => PageResponse<LastTrack>.CreateSuccessResponse(tracks3)));

      return new Login.User("TestUser", "TestToken", true, userApiMock.Object);
    }
  }
}