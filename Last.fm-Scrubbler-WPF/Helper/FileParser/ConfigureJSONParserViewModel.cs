using Caliburn.Micro;
using Scrubbler.Properties;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// ViewModel for configuring the json file parser.
  /// </summary>
  class ConfigureJSONParserViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Name of the "Track Name" property
    /// in the JSON file.
    /// </summary>
    public string TrackNameProperty
    {
      get => _trackNameProperty;
      set
      {
        if (TrackNameProperty != value)
        {
          _trackNameProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _trackNameProperty;

    /// <summary>
    /// Name of the "Artist Name" property
    /// in the JSON file.
    /// </summary>
    public string ArtistNameProperty
    {
      get => _artistNameProperty;
      set
      {
        if (ArtistNameProperty != value)
        {
          _artistNameProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _artistNameProperty;

    /// <summary>
    /// Name of the "Album Name" property
    /// in the JSON file.
    /// </summary>
    public string AlbumNameProperty
    {
      get => _albumNameProperty;
      set
      {
        if (AlbumNameProperty != value)
        {
          _albumNameProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _albumNameProperty;

    /// <summary>
    /// Name of the "Album Artist Name" property
    /// in the JSON file.
    /// </summary>
    public string AlbumArtistNameProperty
    {
      get => _albumArtistNameProperty;
      set
      {
        if (AlbumArtistNameProperty != value)
        {
          _albumArtistNameProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _albumArtistNameProperty;

    /// <summary>
    /// Name of the "Timestamp" property
    /// in the JSON file.
    /// </summary>
    public string TimestampProperty
    {
      get => _timestampProperty;
      set
      {
        if (TimestampProperty != value)
        {
          _timestampProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _timestampProperty;

    /// <summary>
    /// Name of the "Duration" property
    /// in the JSON file.
    /// </summary>
    public string DurationProperty
    {
      get => _durationProperty;
      set
      {
        if (DurationProperty != value)
        {
          _durationProperty = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private string _durationProperty;

    #endregion Properties

    #region Construction

    public ConfigureJSONParserViewModel()
    {
      ReadSettings();
    }

    #endregion Construction

    public void SaveAndClose()
    {
      Settings.Default.JSONTrackNameProperty = TrackNameProperty;
      Settings.Default.JSONArtistNameProperty = ArtistNameProperty;
      Settings.Default.JSONAlbumNameProperty = AlbumNameProperty;
      Settings.Default.JSONAlbumArtistNameProperty = AlbumArtistNameProperty;
      Settings.Default.JSONTimestampProperty = TimestampProperty;
      Settings.Default.JSONDurationProperty = DurationProperty;
      TryClose(true);
    }

    public void Cancel()
    {
      TryClose(false);
    }

    public void LoadDefaults()
    {
      TrackNameProperty = Settings.Default.Properties[nameof(Settings.Default.JSONTrackNameProperty)].DefaultValue.ToString();
      ArtistNameProperty = Settings.Default.Properties[nameof(Settings.Default.JSONArtistNameProperty)].DefaultValue.ToString();
      AlbumNameProperty = Settings.Default.Properties[nameof(Settings.Default.JSONAlbumNameProperty)].DefaultValue.ToString();
      AlbumArtistNameProperty = Settings.Default.Properties[nameof(Settings.Default.JSONAlbumArtistNameProperty)].DefaultValue.ToString();
      TimestampProperty = Settings.Default.Properties[nameof(Settings.Default.JSONTimestampProperty)].DefaultValue.ToString();
      DurationProperty = Settings.Default.Properties[nameof(Settings.Default.JSONDurationProperty)].DefaultValue.ToString();
    }

    private void ReadSettings()
    {
      TrackNameProperty = Settings.Default.JSONTrackNameProperty;
      ArtistNameProperty = Settings.Default.JSONArtistNameProperty;
      AlbumNameProperty = Settings.Default.JSONAlbumNameProperty;
      AlbumArtistNameProperty = Settings.Default.JSONAlbumArtistNameProperty;
      TimestampProperty = Settings.Default.JSONTimestampProperty;
      DurationProperty = Settings.Default.JSONDurationProperty;
    }
  }
}