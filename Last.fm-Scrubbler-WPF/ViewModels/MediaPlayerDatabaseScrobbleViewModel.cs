using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// Media player database to scrobble tracks from.
  /// </summary>
  public enum MediaPlayerDatabaseType
  {
    /// <summary>
    /// Scrobble from iTunes database (.xml).
    /// </summary>
    iTunes
  }

  /// <summary>
  /// ViewModel for the <see cref="Views.MediaPlayerDatabaseScrobbleView"/>.
  /// </summary>
  class MediaPlayerDatabaseScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when the status should be changed.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    /// <summary>
    /// Path to the database file.
    /// </summary>
    public string DBFilePath
    {
      get { return _dbFilePath; }
      set
      {
        _dbFilePath = value;
        NotifyOfPropertyChange(() => DBFilePath);
      }
    }
    private string _dbFilePath;

    /// <summary>
    /// Media player database to scrobble tracks from.
    /// </summary>
    public MediaPlayerDatabaseType MediaPlayerDatabaseType
    {
      get { return _mediaPlayerDatabaseType; }
      set
      {
        _mediaPlayerDatabaseType = value;
        NotifyOfPropertyChange(() => MediaPlayerDatabaseType);
      }
    }
    private MediaPlayerDatabaseType _mediaPlayerDatabaseType;

    /// <summary>
    /// List of parsed scrobbles.
    /// </summary>
    public ObservableCollection<MediaDBScrobbleViewModel> ParsedScrobbles
    {
      get { return _parsedScrobbles; }
      private set
      {
        _parsedScrobbles = value;
        NotifyOfPropertyChange(() => ParsedScrobbles);
      }
    }
    private ObservableCollection<MediaDBScrobbleViewModel> _parsedScrobbles;

    /// <summary>
    /// Gets if certain controls are enabled on the UI:
    /// </summary>
    public bool EnableControls
    {
      get { return _enableControls; }
      private set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
      }
    }
    private bool _enableControls;

    /// <summary>
    /// Gets if the "Scrobble" button is enabled on the UI.
    /// </summary>
    public bool CanScrobble
    {
      get { return ParsedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select ALL" button is enabled on the UI.
    /// </summary>
    public bool CanSelectAll
    {
      get { return ParsedScrobbles.Any(i => !i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select NONE" button is enabled on the UI.
    /// </summary>
    public bool CanSelectNone
    {
      get { return ParsedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public MediaPlayerDatabaseScrobbleViewModel()
    {
      ParsedScrobbles = new ObservableCollection<MediaDBScrobbleViewModel>();
      EnableControls = true;
    }

    /// <summary>
    /// Shows an <see cref="OpenFileDialog"/> and lets the user
    /// select a database file.
    /// Filter will be applied depending on selected <see cref="MediaPlayerDatabaseType"/>.
    /// </summary>
    public void SelectFile()
    {
      OpenFileDialog ofd = new OpenFileDialog();

      if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.iTunes)
        ofd.Filter = "iTunes Database XML (*.xml) | *.xml";

      if (ofd.ShowDialog() == DialogResult.OK)
        DBFilePath = ofd.FileName;
    }

    /// <summary>
    /// Parses the <see cref="DBFilePath"/>.
    /// </summary>
    public void ParseFile()
    {
      if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.iTunes)
        ParseItunes();
    }

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    public void SelectAll()
    {
      foreach (MediaDBScrobbleViewModel s in ParsedScrobbles.Where(i => !i.ToScrobble))
      {
        s.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public void SelectNone()
    {
      foreach (MediaDBScrobbleViewModel s in ParsedScrobbles.Where(i => i.ToScrobble))
      {
        s.ToScrobble = false;
      }
    }

    /// <summary>
    /// Parse the <see cref="DBFilePath"/>.
    /// </summary>
    private async void ParseItunes()
    {
      EnableControls = false;

      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load(DBFilePath);
      }
      catch (Exception ex)
      {
        EnableControls = true;
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Could not load database file: " + ex.Message));
        return;
      }

      // node that points to all tracks
      XmlNode trackDictNode = null;
      try
      {
        trackDictNode = xmlDocument.ChildNodes[2].ChildNodes[0].ChildNodes[15];
      }
      catch (Exception)
      {
        EnableControls = true;
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Could not find 'Tracks' node in xml file"));
        return;
      }

      List<MediaDBScrobbleViewModel> scrobbles = new List<MediaDBScrobbleViewModel>();
      Dictionary<XmlNode, string> errorNodes = new Dictionary<XmlNode, string>();

      int count = 1;
      var dictNodes = trackDictNode.ChildNodes.OfType<XmlNode>().Where(i => i.Name == "dict");
      foreach (XmlNode trackNode in dictNodes)
      {
        await Task.Run(() =>
        {
          try
          {
            var xmlNodes = trackNode.ChildNodes.OfType<XmlNode>();
            int playCount = int.Parse(xmlNodes.First(i => i.InnerText == "Play Count").NextSibling.InnerText);
            string trackName = xmlNodes.First(i => i.InnerText == "Name").NextSibling.InnerText;
            string artistName = xmlNodes.First(i => i.InnerText == "Artist").NextSibling.InnerText;
            string albumName = xmlNodes.FirstOrDefault(i => i.InnerText == "Album")?.NextSibling.InnerText;

            MediaDBScrobbleViewModel vm = new MediaDBScrobbleViewModel(new MediaDBScrobble(trackName, artistName, albumName, playCount));
            vm.ToScrobbleChanged += ToScrobbleChanged;
            scrobbles.Add(vm);
          }
          catch (Exception ex)
          {
            // todo: corrupted or something, log and continue.
            //errorNodes.Add(trackNode, ex.Message);
          }
          finally
          {
            StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Parsing iTunes library... " + count + " / " + dictNodes.Count()));
            count++;
          }
        });
      }

      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully parsed iTunes library"));
      ParsedScrobbles = new ObservableCollection<MediaDBScrobbleViewModel>(scrobbles);
      EnableControls = true;
    }

    public async Task Scrobble()
    {
      EnableControls = false;
      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble selected tracks"));
      List<Scrobble> scrobbles = new List<Scrobble>();

      DateTime time = DateTime.Now; ;
      foreach (var vm in ParsedScrobbles.Where(i => i.ToScrobble))
      {
        for(int i = 0; i < vm.Scrobble.PlayCount; i++)
        {
          scrobbles.Add(new Scrobble(vm.Scrobble.ArtistName, vm.Scrobble.AlbumName, vm.Scrobble.TrackName, time));
          time = time.Subtract(TimeSpan.FromSeconds(1));
        }
      }

      var response = await MainViewModel.Scrobbler.ScrobbleAsync(scrobbles);
      if (response.Success)
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
      else
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));

      EnableControls = true;
    }

    /// <summary>
    /// Notifies the UI that ToScrobble has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
      NotifyOfPropertyChange(() => CanScrobble);
    }
  }
}