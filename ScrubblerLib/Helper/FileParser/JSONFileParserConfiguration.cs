using System;
using System.Runtime.Serialization;

namespace ScrubblerLib.Helper.FileParser
{
  [DataContract]
  public struct JSONFileParserConfiguration : IFileParserConfiguration
  {
    #region Properties

    [DataMember]
    public ScrobbleMode ScrobbleMode { get; set; }

    [DataMember]
    public TimeSpan DefaultDuration { get; set; }

    [DataMember]
    public string TrackFieldName { get; set; }

    [DataMember]
    public string ArtistFieldName { get; set; }

    [DataMember]
    public string AlbumFieldName { get; set; }

    [DataMember]
    public string AlbumArtistFieldName { get; set; }

    [DataMember]
    public string TimestampFieldName { get; set; }

    [DataMember]
    public string DurationFieldName { get; set; }

    [DataMember]
    public string MillisecondsPlayedFieldName { get; set; }

    [DataMember]
    public bool FilerShortPlayedSongs { get; set; }

    [DataMember]
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
