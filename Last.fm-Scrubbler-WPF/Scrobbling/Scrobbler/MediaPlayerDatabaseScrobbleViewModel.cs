using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using ScrubblerLib.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Media player database to scrobble tracks from.
  /// </summary>
  public enum MediaPlayerDatabaseType
  {
    /// <summary>
    /// Scrobble from iTunes or Winamp database (.xml).
    /// </summary>
    [Description("iTunes / Winamp")]
    iTunesOrWinamp,

    /// <summary>
    /// Scrobble from a RhythmDB database (.xml).
    /// </summary>
    [Description("RhythmDB")]
    RhythmDB,

    /// <summary>
    /// Scrobble from Windows Media Player.
    /// </summary>
    [Description("Windows Media Player")]
    WMP
  }

  /// <summary>
  /// ViewModel for the <see cref="MediaPlayerDatabaseScrobbleView"/>.
  /// </summary>
  public class MediaPlayerDatabaseScrobbleViewModel : ScrobbleMultipleViewModelBase<MediaDBScrobbleViewModel>
  {
    #region Properties

    /// <summary>
    /// Path to the database file.
    /// </summary>
    public string DBFilePath
    {
      get { return _dbFilePath; }
      set
      {
        _dbFilePath = value;
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
      }
    }
    private MediaPlayerDatabaseType _mediaPlayerDatabaseType;

    /// <summary>
    /// If true will scrobble a track the amount of times
    /// it was played instead of just once.
    /// </summary>
    public bool ScrobblePlaycounts
    {
      get => _scrobblePlaycounts;
      set
      {
        if(ScrobblePlaycounts != value)
        {
          _scrobblePlaycounts = value;
          NotifyOfPropertyChange();
          NotifyOfPropertyChange(() => ToScrobbleCount);
          NotifyOfPropertyChange(() => MaxToScrobbleCount);
        }
      }
    }
    private bool _scrobblePlaycounts;

    /// <summary>
    /// Gets the amount of scrobbles that will be scrobbled.
    /// </summary>
    public override int ToScrobbleCount => ScrobblePlaycounts ? Scrobbles.Where(i => i.ToScrobble).Sum(i => i.PlayCount) : base.ToScrobbleCount;

    /// <summary>
    /// Maximum amount of scrobbable scrobbles.
    /// </summary>
    public override int MaxToScrobbleCount => ScrobblePlaycounts ? Scrobbles.Sum(i => i.PlayCount) : base.MaxToScrobbleCount;

    /// <summary>
    /// Command for parsing the file.
    /// </summary>
    public ICommand ParseFileCommand { get; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public MediaPlayerDatabaseScrobbleViewModel(IExtendedWindowManager windowManager)
      : base(windowManager, "Media Player Database Scrobbler")
    {
      Scrobbles = new ObservableCollection<MediaDBScrobbleViewModel>();
      ScrobblePlaycounts = true;
      ParseFileCommand = new DelegateCommand((o) => ParseFile().Forget());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void CheckFirst3000()
    {
      if (ScrobblePlaycounts)
      {
        var scrobbles = new List<MediaDBScrobbleViewModel>();
        int totalPlayCounts = 0;
        foreach (var item in Scrobbles)
        {
          if (scrobbles.Count >= 3000 || totalPlayCounts + item.PlayCount > 3000)
            break;

          totalPlayCounts += item.PlayCount;
          scrobbles.Add(item);
        }

        SetToScrobbleState(scrobbles, true);
      }
      else
        base.CheckFirst3000();
    }

    /// <summary>
    /// Shows a file dialog and lets the user
    /// select a database file.
    /// Filter will be applied depending on selected <see cref="MediaPlayerDatabaseType"/>.
    /// </summary>
    public void SelectFile()
    {
      IOpenFileDialog ofd = WindowManager.CreateOpenFileDialog();
      if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.iTunesOrWinamp)
        ofd.Filter = "iTunes/Winamp Database XML (*.xml) | *.xml";
      else if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.RhythmDB)
        ofd.Filter = "RhythmDB XML (*.xml) | *.xml";

      if (ofd.ShowDialog())
        DBFilePath = ofd.FileName;
    }

    /// <summary>
    /// Parses the <see cref="DBFilePath"/>.
    /// </summary>
    public async Task ParseFile()
    {
      if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.iTunesOrWinamp)
        await ParseItunesConformXML();
      else if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.RhythmDB)
        await ParseRhythmDBDatabase();
      else if (MediaPlayerDatabaseType == MediaPlayerDatabaseType.WMP)
        await ParseWMPDatabase();
    }

    /// <summary>
    /// Parse the <see cref="DBFilePath"/>.
    /// A lot of media players use the same xml format.
    /// </summary>
    private async Task ParseItunesConformXML()
    {
      try
      {
        EnableControls = false;

        var xmlDocument = new XmlDocument();
        try
        {
          xmlDocument.Load(DBFilePath);
        }
        catch (Exception ex)
        {
          throw new Exception($"Could not load database file: {ex.Message}");
        }

        // node that points to all tracks
        XmlNode trackDictNode = null;
        try
        {
          trackDictNode = xmlDocument.ChildNodes[2].ChildNodes[0].ChildNodes.OfType<XmlNode>().First(i => i.Name == "dict");
          if (trackDictNode == null)
            throw new Exception("Could not find 'Tracks' node in xml file");
        }
        catch (Exception ex)
        {
          throw new Exception($"Error parsing database file: {ex.Message}");
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
              string albumArtist = xmlNodes.FirstOrDefault(i => i.InnerText == "Album Artist")?.NextSibling.InnerText;
              TimeSpan duration = TimeSpan.FromMilliseconds(int.Parse(xmlNodes.FirstOrDefault(i => i.InnerText == "Total Time")?.NextSibling.InnerText));
              DateTime lastPlayed = DateTime.Parse(xmlNodes.FirstOrDefault(i => i.InnerText == "Play Date UTC")?.NextSibling.InnerText);

              var vm = new MediaDBScrobbleViewModel(new MediaDBScrobble(playCount, lastPlayed, trackName, artistName, albumName, albumArtist, duration));
              vm.ToScrobbleChanged += ToScrobbleChanged;
              scrobbles.Add(vm);
            }
            catch (Exception ex)
            {
              // corrupted or something, log and continue.
              errorNodes.Add(trackNode, ex.Message);
            }
            finally
            {
              OnStatusUpdated($"Parsing database file... {count++} / {dictNodes.Count()}");
            }
          });
        }

        OnStatusUpdated("Successfully parsed database file");
        Scrobbles = new ObservableCollection<MediaDBScrobbleViewModel>(scrobbles);
      }
      catch (Exception ex)
      {
        OnStatusUpdated(ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Parses the <see cref="DBFilePath"/> as
    /// a RhythmDB xml.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task ParseRhythmDBDatabase()
    {
      try
      {
        EnableControls = false;
        await Task.Run(() =>
        {
          OnStatusUpdated("Parsing RhythmDB library...");
          var scrobbles = RhythmDBParser.Parse(DBFilePath);
          var scrobbleVMs = new List<MediaDBScrobbleViewModel>();
          foreach (var scrobble in scrobbles)
          {
            var vm = new MediaDBScrobbleViewModel(scrobble);
            vm.ToScrobbleChanged += ToScrobbleChanged;
            scrobbleVMs.Add(vm);
          }

          Scrobbles = new ObservableCollection<MediaDBScrobbleViewModel>(scrobbleVMs);
          OnStatusUpdated("Successfully parsed RhythmDB xml");
        });
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while parsing RhythmDB library: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Parses the windows media player database.
    /// </summary>
    private async Task ParseWMPDatabase()
    {
      try
      {
        EnableControls = false;
        await Task.Run(() =>
        {
          OnStatusUpdated("Parsing Windows Media Player library...");
          using (var wmp = new WMP())
          {
            // todo: this can be better
            var scrobbles = wmp.GetMusicLibrary();
            var scrobbleVMs = new List<MediaDBScrobbleViewModel>();
            foreach (var scrobble in scrobbles)
            {
              var vm = new MediaDBScrobbleViewModel(scrobble);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              scrobbleVMs.Add(vm);
            }

            Scrobbles = new ObservableCollection<MediaDBScrobbleViewModel>(scrobbleVMs);
          }

          OnStatusUpdated("Successfully parsed Windows Media Player library");
        });
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while parsing Windows Media Player library: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to scrobble selected tracks...");

        var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success && response.Status == LastResponseStatus.Successful)
          OnStatusUpdated("Successfully scrobbled selected tracks");
        else
          OnStatusUpdated($"Error while scrobbling selected tracks: {response.Status}");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while trying to scrobble selected tracks: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates a list with scrobbles that will be scrobbled.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      var scrobbles = new List<Scrobble>();

      var time = DateTime.Now; ;
      foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
      {
        for (int i = 0; i < (ScrobblePlaycounts ? vm.PlayCount : 1); i++)
        {
          scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, time) { AlbumArtist = vm.AlbumArtist, Duration = vm.Duration });
          time = time.Subtract(TimeSpan.FromSeconds(1));
        }
      }

      return scrobbles;
    }

    /// <summary>
    /// Notifies the UI that ToScrobble has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanCheckAll);
      NotifyOfPropertyChange(() => CanUncheckAll);
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
    }
  }
}