using System;

namespace ScrubblerLib.Helper.FileParser
{
  public struct JSONFileParserConfiguration : IFileParserConfiguration
  {
    #region Properties

    public ScrobbleMode ScrobbleMode { get; set; }

    public TimeSpan DefaultDuration { get; set; }

    public string TrackFieldName { get; set; }
    public string ArtistFieldName { get; set; }
    public string AlbumFieldName { get; set; }
    public string AlbumArtistFieldName { get; set; }
    public string TimestampFieldName { get; set; }
    public string DurationFieldName { get; set; }
    public string MillisecondsPlayedFieldName { get; set; }
    public bool FilerShortPlayedSongs { get; set; }
    public int MillisecondsPlayedTreshold { get; set; }

    #endregion Properties

    #region Construction

    public JSONFileParserConfiguration(ScrobbleMode scrobbleMode, TimeSpan defaultDuration, string trackFieldName, string artistFieldName, string albumFieldName, string albumArtistFieldName, string timestampFieldName,
                                   string durationFieldName, string millisecondsPlayedFieldName, bool filterShortPlayedSongs, int millsecondsPlayedThreshold)
    {
      ScrobbleMode = scrobbleMode;
      DefaultDuration = defaultDuration;
      TrackFieldName = trackFieldName;
      ArtistFieldName = artistFieldName;
      AlbumFieldName = albumFieldName;
      AlbumArtistFieldName = albumArtistFieldName;
      TimestampFieldName = timestampFieldName;
      DurationFieldName = durationFieldName;
      MillisecondsPlayedFieldName = millisecondsPlayedFieldName;
      FilerShortPlayedSongs = filterShortPlayedSongs;
      MillisecondsPlayedTreshold = millsecondsPlayedThreshold;
    }

    #endregion Construction
  }
}
