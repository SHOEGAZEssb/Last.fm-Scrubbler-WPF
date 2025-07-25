using System;
using System.Runtime.Serialization;

namespace ScrubblerLib.Helper.FileParser
{
  [DataContract]
  public struct CSVFileParserConfiguration : IFileParserConfiguration
  {
    #region Properties

    [DataMember]
    public ScrobbleMode ScrobbleMode { get; set; }

    [DataMember]
    public TimeSpan DefaultDuration { get; set; }

    [DataMember]
    public int EncodingID { get; set; }

    [DataMember]
    public string Delimiters { get; set; }

    [DataMember]
    public bool HasFieldsInQuotes { get; set; }

    [DataMember]
    public int TimestampFieldIndex { get; set; }

    [DataMember]
    public int TrackFieldIndex { get; set; }

    [DataMember]
    public int ArtistFieldIndex { get; set; }

    [DataMember]
    public int AlbumFieldIndex { get; set; }

    [DataMember]
    public int AlbumArtistFieldIndex { get; set; }

    [DataMember]
    public int DurationFieldIndex { get; set; }

    [DataMember]
    public int MillisecondsPlayedFieldIndex { get; set; }

    [DataMember]
    public bool FilterShortPlayedSongs { get; set; }

    [DataMember]
    public int MillisecondsPlayedThreshold { get; set; }

    #endregion Properties

    #region Construction

    public CSVFileParserConfiguration(ScrobbleMode scrobbleMode, TimeSpan defaultDuration, int encodingID, string delimiters, bool hasFieldsInQuotes, int timestampFieldIndex,
                                      int trackFieldIndex, int artistFieldIndex, int albumFieldIndex, int albumArtistFieldIndex, int durationFieldIndex, int millisecondsPlayedFieldIndex,
                                      bool filterShortPlayedSongs, int millisecondsPlayedThreshold)
    {
      ScrobbleMode = scrobbleMode;
      DefaultDuration = defaultDuration;
      EncodingID = encodingID;
      Delimiters = delimiters;
      HasFieldsInQuotes = hasFieldsInQuotes;
      TimestampFieldIndex = timestampFieldIndex;
      TrackFieldIndex = trackFieldIndex;
      ArtistFieldIndex = artistFieldIndex;
      AlbumFieldIndex = albumFieldIndex;
      AlbumArtistFieldIndex = albumArtistFieldIndex;
      DurationFieldIndex = durationFieldIndex;
      MillisecondsPlayedFieldIndex = millisecondsPlayedFieldIndex;
      FilterShortPlayedSongs = filterShortPlayedSongs;
      MillisecondsPlayedThreshold = millisecondsPlayedThreshold;
    }

    #endregion Construction
  }
}