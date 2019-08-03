using Caliburn.Micro;
using Scrubbler.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// ViewModel for the <see cref="ConfigureCSVParserView"/>
  /// </summary>
  class ConfigureCSVParserViewModel : Screen
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
      }
    }
    private string _delimiters;

    public bool HasFieldsEnclosedInQuotes
    {
      get => _hasFieldsEnclosedInQuotes;
      set
      {
        if(HasFieldsEnclosedInQuotes != value)
        {
          _hasFieldsEnclosedInQuotes = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private bool _hasFieldsEnclosedInQuotes;

    /// <summary>
    /// Gets available encodings.
    /// </summary>
    public IEnumerable<Encoding> AvailableEncodings => typeof(Encoding).GetProperties().Where(i => i.PropertyType == typeof(Encoding)).Select(i => (Encoding)i.GetValue(null));

    /// <summary>
    /// The currently selected encoding.
    /// </summary>
    public Encoding SelectedEncoding
    {
      get => _selectedEncoding;
      set
      {
        if(SelectedEncoding != value)
        {
          _selectedEncoding = value;
          _encodingID = SelectedEncoding.CodePage;
          NotifyOfPropertyChange();
        }
      }
    }
    private Encoding _selectedEncoding;

    /// <summary>
    /// The code page of the <see cref="SelectedEncoding"/>
    /// </summary>
    private int _encodingID;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ConfigureCSVParserViewModel()
    {
      ReadSettings();
    }

    /// <summary>
    /// Reads the settings.
    /// </summary>
    private void ReadSettings()
    {
      ArtistFieldIndex = Settings.Default.ArtistFieldIndex;
      AlbumFieldIndex = Settings.Default.AlbumFieldIndex;
      TrackFieldIndex = Settings.Default.TrackFieldIndex;
      TimestampFieldIndex = Settings.Default.TimestampFieldIndex;
      AlbumArtistFieldIndex = Settings.Default.AlbumArtistFieldIndex;
      DurationFieldIndex = Settings.Default.DurationFieldIndex;
      Delimiters = Settings.Default.CSVDelimiters;
      HasFieldsEnclosedInQuotes = Settings.Default.CSVHasFieldsInQuotes;
      _encodingID = Settings.Default.CSVEncoding;
      SelectedEncoding = Encoding.GetEncoding(_encodingID);
    }

    /// <summary>
    /// Saves the settings and closes the view.
    /// </summary>
    public void SaveAndClose()
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
        Settings.Default.CSVHasFieldsInQuotes = HasFieldsEnclosedInQuotes;
        Settings.Default.CSVEncoding = _encodingID;
        TryClose(true);
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

      if (errors != string.Empty)
      {
        if (MessageBox.Show($"There are errors:\r{errors}\rAre you sure you want to save these values?", "Errors",
           MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Cancels the edit.
    /// </summary>
    public void Cancel()
    {
      TryClose(false);
    }

    /// <summary>
    /// Loads the default values for the settings.
    /// </summary>
    public void LoadDefaults()
    {
      ArtistFieldIndex = int.Parse(Settings.Default.Properties["ArtistFieldIndex"].DefaultValue.ToString());
      AlbumFieldIndex = int.Parse(Settings.Default.Properties["AlbumFieldIndex"].DefaultValue.ToString());
      TrackFieldIndex = int.Parse(Settings.Default.Properties["TrackFieldIndex"].DefaultValue.ToString());
      TimestampFieldIndex = int.Parse(Settings.Default.Properties["TimestampFieldIndex"].DefaultValue.ToString());
      AlbumArtistFieldIndex = int.Parse(Settings.Default.Properties["AlbumArtistFieldIndex"].DefaultValue.ToString());
      DurationFieldIndex = int.Parse(Settings.Default.Properties["DurationFieldIndex"].DefaultValue.ToString());
      Delimiters = Settings.Default.Properties["CSVDelimiters"].DefaultValue.ToString();
      HasFieldsEnclosedInQuotes = bool.Parse(Settings.Default.Properties["CSVHasFieldsInQuotes"].DefaultValue.ToString());
      _encodingID = int.Parse(Settings.Default.Properties["CSVEncoding"].DefaultValue.ToString());
      SelectedEncoding = Encoding.GetEncoding(_encodingID);
    }
  }
}