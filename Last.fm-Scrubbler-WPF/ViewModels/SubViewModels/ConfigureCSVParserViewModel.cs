using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Properties;
using Last.fm_Scrubbler_WPF.Views.ScrobbleViews;
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
      get { return Settings.Default.ArtistFieldIndex; }
      set
      {
        Settings.Default.ArtistFieldIndex = value;
        NotifyOfPropertyChange(() => ArtistFieldIndex);
      }
    }

    /// <summary>
    /// Index of the album field in the csv file.
    /// </summary>
    public int AlbumFieldIndex
    {
      get { return Settings.Default.AlbumFieldIndex; }
      set
      {
        Settings.Default.AlbumFieldIndex = value;
        NotifyOfPropertyChange(() => AlbumFieldIndex);
      }
    }

    /// <summary>
    /// Index of the artist field in the csv file.
    /// </summary>
    public int TrackFieldIndex
    {
      get { return Settings.Default.TrackFieldIndex; }
      set
      {
        Settings.Default.TrackFieldIndex = value;
        NotifyOfPropertyChange(() => TrackFieldIndex);
      }
    }

    /// <summary>
    /// Index of the artist field in the csv file.
    /// </summary>
    public int TimestampFieldIndex
    {
      get { return Settings.Default.TimestampFieldIndex; }
      set
      {
        Settings.Default.TimestampFieldIndex = value;
        NotifyOfPropertyChange(() => TimestampFieldIndex);
      }
    }

    /// <summary>
    /// Index of the album artist field in the csv file.
    /// </summary>
    public int AlbumArtistFieldIndex
    {
      get { return Settings.Default.AlbumArtistFieldIndex; }
      set
      {
        Settings.Default.AlbumArtistFieldIndex = value;
        NotifyOfPropertyChange(() => AlbumArtistFieldIndex);
      }
    }

    /// <summary>
    /// Index of the duration field in the csv file.
    /// </summary>
    public int DurationFieldIndex
    {
      get { return Settings.Default.DurationFieldIndex; }
      set
      {
        Settings.Default.DurationFieldIndex = value;
        NotifyOfPropertyChange(() => DurationFieldIndex);
      }
    }

    /// <summary>
    /// Delimiters to use when parsing csv file.
    /// </summary>
    public string Delimiters
    {
      get { return Settings.Default.CSVDelimiters; }
      set
      {
        Settings.Default.CSVDelimiters = value;
        NotifyOfPropertyChange(() => Delimiters);
      }
    }

    #endregion Properties

    /// <summary>
    /// Saves the settings and closes the view.
    /// </summary>
    /// <param name="vm">View to close.</param>
    public void SaveAndClose(ConfigureCSVParserView vm)
    {
      if(CheckAndSaveSettings())
        vm.Close();
    }

    /// <summary>
    /// Saves the settings.
    /// </summary>
    /// <returns>True if settings were saved, false if not.</returns>
    private bool CheckAndSaveSettings()
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

      Settings.Default.Save();
      return true;
    }

    /// <summary>
    /// Reloads the saved values of the settings.
    /// </summary>
    private void ReloadSettings()
    {
      Settings.Default.Reload();
    }

    /// <summary>
    /// Reloads the original settings and closes the view.
    /// </summary>
    /// <param name="vm">View to close.</param>
    public void Cancel(ConfigureCSVParserView vm)
    {
      ReloadSettings();
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