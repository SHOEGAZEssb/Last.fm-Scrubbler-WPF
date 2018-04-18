using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ExtraFunctions
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ExtraFunctions.CSVDownloaderView"/>
  /// </summary>
  public class CSVDownloaderViewModel : ExtraFunctionViewModelBase
  {
    #region Properties

    /// <summary>
    /// Name of the user whose scrobbles should
    /// be downloaded as csv.
    /// </summary>
    public string Username
    {
      get { return _username; }
      set
      {
        _username = value;
        NotifyOfPropertyChange(() => Username);
        NotifyOfPropertyChange(() => CanDownload);
      }
    }
    private string _username;

    /// <summary>
    /// Path of the csv file that will be created.
    /// </summary>
    public string FilePath
    {
      get { return _filePath; }
      set
      {
        _filePath = value;
        NotifyOfPropertyChange(() => FilePath);
        NotifyOfPropertyChange(() => CanDownload);
      }
    }
    private string _filePath;

    /// <summary>
    /// Gets if certain controls on the ui are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanDownload);
      }
    }

    /// <summary>
    /// Gets if the 'Download' button is enabled on the ui.
    /// </summary>
    public bool CanDownload
    {
      get { return EnableControls && Username != string.Empty && FilePath != string.Empty && FilePath.EndsWith(".csv"); }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Tracks to fetch per page.
    /// </summary>
    private const int TRACKSPERPAGE = 1000;

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private IExtendedWindowManager _windowManager;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public CSVDownloaderViewModel(IExtendedWindowManager windowManager)
      : base("CSV Downloader")
    {
      _windowManager = windowManager;
      Username = string.Empty;
      FilePath = string.Empty;
    }

    /// <summary>
    /// Lets the user select the file path of
    /// the csv file.
    /// </summary>
    public void SelectFilePath()
    {
      IFileDialog sfd = _windowManager.CreateSaveFileDialog();
      sfd.Filter = "CSV Files (*.csv) | *.csv";
      if (sfd.ShowDialog())
        FilePath = sfd.FileName;
    }

    /// <summary>
    /// Downloads the scrobbles of the given <see cref="Username"/>
    /// and saves them in the csv file in the given <see cref="FilePath"/>.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task Download()
    {
      EnableControls = false;

      try
      {
        var userResponse = await MainViewModel.Client.User.GetInfoAsync(Username);
        if (userResponse.Success)
        {
          int pages = (int)Math.Ceiling(userResponse.Content.Playcount / (double)TRACKSPERPAGE);
          bool error = false;
          IReadOnlyList<LastTrack>[] scrobbles = new IReadOnlyList<LastTrack>[pages];
          for (int i = 1; i <= pages; i++)
          {
            OnStatusUpdated("Fetching page " + i + " / " + pages);
            var pageResponse = await MainViewModel.Client.User.GetRecentScrobbles(Username, null, i, TRACKSPERPAGE);
            if (pageResponse.Success)
              scrobbles[i - 1] = pageResponse.Content.Where(s => !s.IsNowPlaying.HasValue || !s.IsNowPlaying.Value).ToArray();
            else
            {
              error = true;
              break;
            }
          }

          if (error)
            OnStatusUpdated("Error while fetching scrobble data.");
          else
          {
            OnStatusUpdated("Creating .csv file...");

            try
            {
              StringBuilder csv = new StringBuilder();
              foreach (var page in scrobbles)
              {
                foreach (var scrobble in page)
                {
                  string artist = EncloseComma(scrobble.ArtistName);
                  string album = EncloseComma(scrobble.AlbumName);
                  string track = EncloseComma(scrobble.Name);
                  string duration = EncloseComma(scrobble.Duration.ToString());
                  var timeStamp = scrobble.TimePlayed;

                  string newLine = string.Format("{0},{1},{2},{3},{4},{5}", artist, album, track, timeStamp, "", duration);
                  csv.AppendLine(newLine);
                }
              }

              File.WriteAllText(FilePath, csv.ToString());
              OnStatusUpdated("Successfully wrote .csv file.");
            }
            catch (Exception ex)
            {
              OnStatusUpdated("Error writing .csv file: " + ex.Message);
            }
          }
        }
        else
          OnStatusUpdated("Error while fetching user info.");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Encloses the given <paramref name="str"/> with
    /// quotation marks if it contains a ','.
    /// </summary>
    /// <param name="str">String to enclose.</param>
    /// <returns>Enclosed string.</returns>
    private string EncloseComma(string str)
    {
      if(str.Contains(','))
        return "\"" + str + "\"";
      return str;
    }
  }
}