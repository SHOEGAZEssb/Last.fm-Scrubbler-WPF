using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF_Test
{
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
        actual.TimePlayed == expected.TimePlayed &&
        actual.AlbumArtist == expected.AlbumArtist &&
        actual.Duration == expected.Duration;
    }
  }
}