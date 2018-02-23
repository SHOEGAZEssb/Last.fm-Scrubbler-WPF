using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Properties;
using Last.fm_Scrubbler_WPF.Views.SubViews;
using System.Linq;
using System.Windows;

namespace Last.fm_Scrubbler_WPF.ViewModels.SubViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="ConfigureCSVParserView"/>
  /// </summary>
  class ConfigureCSVParserViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Index of the artist field in the csv file.
    /// </summary>
    public int ArtistFieldIndex
    {
      get { return _artistFieldIndex; }
      set
      {
        _artistFieldIndex = value;
        NotifyOfPropertyChange(() => ArtistFieldIndex);
      }
    }
    private int _artistFieldIndex;

    /// <summary>
    /// Index of the album field in the csv file.
    /// </summary>
    public int AlbumFieldIndex
    {
      get { return _albumFieldIndex; }
      set
      {
        _albumFieldIndex = value;
        NotifyOfPropertyChange(() => AlbumFieldIndex);
      }
    }
    private int _albumFieldIndex;

    /// <summary>
    /// Index of the artist field in the csv file.
    /// </summary>
    public int TrackFieldIndex
    {
      get { return _trackFieldIndex; }
      set
      {
        _trackFieldIndex = value;
        NotifyOfPropertyChange(() => TrackFieldIndex);
      }
    }
    private int _trackFieldIndex;

    /// <summary>
    /// Index of the artist field in the csv file.
    /// </summary>
    public int TimestampFieldIndex
    {
      get { return _timestampFieldIndex; }
      set
      {
        _timestampFieldIndex = value;
        NotifyOfPropertyChange(() => TimestampFieldIndex);
      }
    }
    private int _timestampFieldIndex;

    /// <summary>
    /// Index of the album artist field in the csv file.
    /// </summary>
    public int AlbumArtistFieldIndex
    {
      get { return _albumArtistFieldIndex; }
      set
      {
        _albumArtistFieldIndex = value;
        NotifyOfPropertyChange(() => AlbumArtistFieldIndex);
      }
    }
    private int _albumArtistFieldIndex;

    /// <summary>
    /// Index of the duration field in the csv file.
    /// </summary>
    public int DurationFieldIndex
    {
      get { return _durationFieldIndex; }
      set
      {
        _durationFieldIndex = value;
        NotifyOfPropertyChange(() => DurationFieldIndex);
      }
    }
    private int _durationFieldIndex;

    /// <summary>
    /// Delimiters to use when parsing csv file.
    /// </summary>
    public string Delimiters
    {
      get { return _delimiters; }
      set
      {
        _delimiters = value;
        NotifyOfPropertyChange(() => Delimiters);
      }
    }
    private string _delimiters;

    #endregion Properties

    public ConfigureCSVParserViewModel()
    {
      ReadSettings();
    }

    private void ReadSettings()
    {
      ArtistFieldIndex = Settings.Default.ArtistFieldIndex;
      AlbumFieldIndex = Settings.Default.AlbumFieldIndex;
      TrackFieldIndex = Settings.Default.TrackFieldIndex;
      TimestampFieldIndex = Settings.Default.TimestampFieldIndex;
      AlbumArtistFieldIndex = Settings.Default.AlbumArtistFieldIndex;
      DurationFieldIndex = Settings.Default.DurationFieldIndex;
      Delimiters = Settings.Default.CSVDelimiters;
    }

    /// <summary>
    /// Saves the settings and closes the view.
    /// </summary>
    /// <param name="vm">View to close.</param>
    public void SaveAndClose(ConfigureCSVParserView vm)
    {
      if (CheckSettings())
      {
        Settings.Default.ArtistFieldIndex = ArtistFieldIndex;
        Settings.Default.AlbumFieldIndex = AlbumFieldIndex;
        Settings.Default.TrackFieldIndex = TrackFieldIndex;
        Settings.Default.TimestampFieldIndex = TimestampFieldIndex;
        Settings.Default.AlbumArtistFieldIndex = AlbumArtistFieldIndex;
        Settings.Default.DurationFieldIndex = DurationFieldIndex;
        Settings.Default.CSVDelimiters = Delimiters;
        vm.Close();
      }
    }

    /// <summary>
    /// Checks the settings for errors.
    /// </summary>
    /// <returns>True if user wants to use these settings, false if not.</returns>
    private bool CheckSettings()
    {
      string errors = string.Empty;

      int[] vals = new int[] { ArtistFieldIndex, AlbumFieldIndex, TrackFieldIndex, TimestampFieldIndex };
      if (vals.Distinct().Count() != vals.Length)
        errors += "Some of the indexes are equal!";

      if (Delimiters == string.Empty)
        errors += "\rThere are no delimiters defined!";

      if(errors != string.Empty)
      {
       if (MessageBox.Show("There are errors:\r" + errors + "\rAre you sure you want to save these values?", "Errors",
          MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Reloads the original settings and closes the view.
    /// </summary>
    /// <param name="vm">View to close.</param>
    public void Cancel(ConfigureCSVParserView vm)
    {
      vm.Close();
    }

    /// <summary>
    /// Loads the default values for the settings.
    /// </summary>
    public void LoadDefaults()
    {
      ArtistFieldIndex = int.Parse((string)Settings.Default.Properties["ArtistFieldIndex"].DefaultValue);
      AlbumFieldIndex = int.Parse((string)Settings.Default.Properties["AlbumFieldIndex"].DefaultValue);
      TrackFieldIndex = int.Parse((string)Settings.Default.Properties["TrackFieldIndex"].DefaultValue);
      TimestampFieldIndex = int.Parse((string)Settings.Default.Properties["TimestampFieldIndex"].DefaultValue);
      AlbumArtistFieldIndex = int.Parse((string)Settings.Default.Properties["AlbumArtistFieldIndex"].DefaultValue);
      DurationFieldIndex = int.Parse((string)Settings.Default.Properties["DurationFieldIndex"].DefaultValue);
      Delimiters = (string)Settings.Default.Properties["CSVDelimiters"].DefaultValue;
    }
  }
}