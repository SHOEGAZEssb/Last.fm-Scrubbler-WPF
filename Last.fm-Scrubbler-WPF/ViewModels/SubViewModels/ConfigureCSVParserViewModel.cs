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

    #endregion Properties

    /// <summary>
    /// Saves the settings and closes the view.
    /// </summary>
    /// <param name="vm">View to close.</param>
    public void SaveAndClose(ConfigureCSVParserView vm)
    {
      SaveSettings();
      vm.Close();
    }

    private void SaveSettings()
    {
      int[] vals = new int[] { ArtistFieldIndex, AlbumFieldIndex, TrackFieldIndex, TimestampFieldIndex };
      if (vals.Distinct().Count() != vals.Length)
      {
        if (MessageBox.Show("Some of the indexes are equal, are you sure you want to save these values?", "Equal Indexes",
          MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
          return;
      }

      Settings.Default.Save();
    }

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
    /// Loads the default values for the field indexes.
    /// </summary>
    public void LoadDefaults()
    {
      ArtistFieldIndex = int.Parse((string)Settings.Default.Properties["ArtistFieldIndex"].DefaultValue);
      AlbumFieldIndex = int.Parse((string)Settings.Default.Properties["AlbumFieldIndex"].DefaultValue);
      TrackFieldIndex = int.Parse((string)Settings.Default.Properties["TrackFieldIndex"].DefaultValue);
      TimestampFieldIndex = int.Parse((string)Settings.Default.Properties["TimestampFieldIndex"].DefaultValue);
    }
  }
}