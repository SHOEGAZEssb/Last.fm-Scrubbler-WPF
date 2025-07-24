using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;

namespace ScrubblerLib.Scrobbler
{
  public class ManualScrobbleFeature : IScrobbleFeature
  {
    #region Properties

    public string Artist { get; set; }

    public string Track { get; set; }

    public string Album { get; set; }

    public string AlbumArtist { get; set; }

    public int Amount { get; set; } = 1;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public TimeSpan Duration { get; set; }

    #endregion Properties

    public IEnumerable<Scrobble> CreateScrobbles()
    {
      var scrobbles = new List<Scrobble>();
      var time = Timestamp;
      for (int i = 0; i < Amount; i++)
      {
        scrobbles.Add(new Scrobble(Artist, Album, Track, time) { AlbumArtist = AlbumArtist, Duration = Duration });
        time = time.Subtract(TimeSpan.FromSeconds(1));
      }

      return scrobbles;
    }
  }
}