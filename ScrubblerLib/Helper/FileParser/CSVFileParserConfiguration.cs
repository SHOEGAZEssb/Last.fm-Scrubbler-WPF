namespace ScrubblerLib.Helper.FileParser
{
  public struct CSVFileParserConfiguration : IFileParserConfiguration
  {
    #region Properties

    public int EncodingID { get; set; }

    public string Delimiters { get; set; }

    public bool HasFieldsInQuotes { get; set; }

    public int TimestampFieldIndex { get; set; }

    public int TrackFieldIndex { get; set; }

    public int ArtistFieldIndex { get; set; }

    public int AlbumFieldIndex { get; set; }

    public int AlbumArtistFieldIndex { get; set; }

    public int DurationFieldIndex { get; set; }

    public int MillisecondsPlayedFieldIndex { get; set; }

    public bool FilterShortPlayedSongs { get; set; }

    public int MillisecondsPlayedThreshold { get; set; }

    #endregion Properties

    #region Construction

    public CSVFileParserConfiguration(int encodingID, string delimiters, bool hasFieldsInQuotes, int timestampFieldIndex,
                                      int trackFieldIndex, int artistFieldIndex, int albumFieldIndex, int albumArtistFieldIndex, int durationFieldIndex, int millisecondsPlayedFieldIndex,
                                      bool filterShortPlayedSongs, int millisecondsPlayedThreshold)
    {
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