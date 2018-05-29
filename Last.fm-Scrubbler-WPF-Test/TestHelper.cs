using IF.Lastfm.Core.Objects;
using System.Collections.Generic;
using System.Linq;

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
    public static Scrobble CloneWithAddedSecond(this Scrobble scrobble)
    {
      return new Scrobble(scrobble.Artist, scrobble.Album, scrobble.Track, scrobble.TimePlayed.AddSeconds(1)) { AlbumArtist = scrobble.AlbumArtist, Duration = scrobble.Duration };
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
  }
}