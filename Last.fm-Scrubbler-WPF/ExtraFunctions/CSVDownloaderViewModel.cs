using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel for the <see cref="CSVDownloaderView"/>
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanDownload);
      }
    }

    /// <summary>
    /// Gets if the 'Download' button is enabled on the ui.
    /// </summary>
    public bool CanDownload
    {
      get { return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(FilePath) && FilePath.EndsWith(".csv"); }
    }

    #endregion Properties

    #region Member

    /// <summary>
    /// Tracks to fetch per page.
    /// </summary>
    private const int TRACKSPERPAGE = 1000;

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// Last.fm user api used to fetch top artists and albums.
    /// </summary>
    private readonly IUserApi _userAPI;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user api used to fetch top artists and albums.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    public CSVDownloaderViewModel(IExtendedWindowManager windowManager, IUserApi userAPI, IFileOperator fileOperator)
      : base("CSV Downloader")
    {
      _windowManager = windowManager;
      _userAPI = userAPI;
      _fileOperator = fileOperator;
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
      try
      {
        EnableControls = false;
        var userResponse = await _userAPI.GetInfoAsync(Username);
        if (userResponse.Success && userResponse.Status == LastResponseStatus.Successful)
        {
          int pages = (int)Math.Ceiling(userResponse.Content.Playcount / (double)TRACKSPERPAGE);
          string error = null;
          IReadOnlyList<LastTrack>[] scrobbles = new IReadOnlyList<LastTrack>[pages];
          for (int i = 1; i <= pages; i++)
          {
            OnStatusUpdated($"Fetching page {i} / {pages}");
            var pageResponse = await _userAPI.GetRecentScrobbles(Username, null, i, TRACKSPERPAGE);
            if (pageResponse.Success && pageResponse.Status == LastResponseStatus.Successful)
              scrobbles[i - 1] = pageResponse.Content.Where(s => !s.IsNowPlaying.HasValue || !s.IsNowPlaying.Value).ToArray();
            else
            {
              error = pageResponse.Status.ToString();
              break;
            }
          }

          if (!string.IsNullOrEmpty(error))
            OnStatusUpdated($"Error while fetching scrobble data: {error}");
          else
          {
            try
            {
              OnStatusUpdated("Creating .csv file...");
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

                  string newLine = $"{artist},{album},{track},{timeStamp},{""},{duration}";
                  csv.AppendLine(newLine);
                }
              }

              _fileOperator.WriteAllText(FilePath, csv.ToString());
              OnStatusUpdated("Successfully wrote .csv file.");
            }
            catch (Exception ex)
            {
              OnStatusUpdated($"Error writing .csv file: {ex.Message}");
            }
          }
        }
        else
          OnStatusUpdated($"Error while fetching user info: {userResponse.Status}");
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
      return str.Contains(',') ? $"\"{str}\"" : str;
    }
  }
}