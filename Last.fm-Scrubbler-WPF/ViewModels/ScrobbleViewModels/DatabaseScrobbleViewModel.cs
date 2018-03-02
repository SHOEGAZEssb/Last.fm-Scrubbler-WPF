using Caliburn.Micro;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using Last.fm_Scrubbler_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// Available databases to search.
  /// </summary>
  public enum Database
  {
    /// <summary>
    /// Search Last.fm.
    /// </summary>
    LastFm,
  }

  /// <summary>
  /// Available search types.
  /// </summary>
  public enum SearchType
  {
    /// <summary>
    /// Search for an artist.
    /// </summary>
    Artist,

    /// <summary>
    /// Search for a release.
    /// </summary>
    Release
  }

  /// <summary>
  /// ViewModel for the <see cref="DatabaseScrobbleView"/>.
  /// </summary>
  public class DatabaseScrobbleViewModel : ScrobbleTimeViewModelBase
  {
    #region Properties

    /// <summary>
    /// String to search.
    /// </summary>
    public string SearchText
    {
      get { return _searchText; }
      set
      {
        _searchText = value;
        NotifyOfPropertyChange();
      }
    }
    private string _searchText;

    /// <summary>
    /// Which database to search for infos.
    /// </summary>
    public Database DatabaseToSearch
    {
      get { return _databaseToSearch; }
      set
      {
        _databaseToSearch = value;
        NotifyOfPropertyChange();
      }
    }
    private Database _databaseToSearch;

    /// <summary>
    /// Which type of data to search.
    /// </summary>
    public SearchType SearchType
    {
      get { return _searchType; }
      set
      {
        _searchType = value;
        NotifyOfPropertyChange();
      }
    }
    private SearchType _searchType;

    /// <summary>
    /// Maximum amount of search results.
    /// </summary>
    public int MaxResults
    {
      get { return _maxResults; }
      set
      {
        _maxResults = value;
        NotifyOfPropertyChange();
      }
    }
    private int _maxResults;

    /// <summary>
    /// The current view that is displayed.
    /// </summary>
    public UserControl CurrentView
    {
      get { return _currentView; }
      private set
      {
        _currentView = value;
        NotifyOfPropertyChange();
      }
    }
    private UserControl _currentView;

    /// <summary>
    /// The list of fetched artists.
    /// </summary>
    public ObservableCollection<FetchedArtistViewModel> FetchedArtists
    {
      get { return _fetchedArtists; }
      private set
      {
        _fetchedArtists = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<FetchedArtistViewModel> _fetchedArtists;

    /// <summary>
    /// The list of fetched releases.
    /// </summary>
    public ObservableCollection<FetchedReleaseViewModel> FetchedReleases
    {
      get { return _fetchedAlbums; }
      private set
      {
        _fetchedAlbums = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<FetchedReleaseViewModel> _fetchedAlbums;

    /// <summary>
    /// The list of fetched tracks.
    /// </summary>
    public ObservableCollection<FetchedTrackViewModel> FetchedTracks
    {
      get { return _fetchedTracks; }
      private set
      {
        _fetchedTracks = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<FetchedTrackViewModel> _fetchedTracks;

    /// <summary>
    /// Gets if certain controls should be enabled on the UI.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
      }
    }

    /// <summary>
    /// Gets if the currently fetched releases has been fetched
    /// through the click on an artist.
    /// </summary>
    public bool FetchedReleaseThroughArtist
    {
      get { return _fetchedReleaseThroughArtist; }
      private set
      {
        _fetchedReleaseThroughArtist = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _fetchedReleaseThroughArtist;

    /// <summary>
    /// Gets if the scrobble button is enabled on the UI.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && FetchedTracks.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return FetchedTracks.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select All" button is enabled.
    /// </summary>
    public bool CanSelectAll
    {
      get { return !FetchedTracks.All(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select None" button is enabled.
    /// </summary>
    public bool CanSelectNone
    {
      get { return FetchedTracks.Any(i => i.ToScrobble) && EnableControls; }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// The view that displays the fetched aritsts.
    /// </summary>
    private ArtistResultView _artistResultView;

    /// <summary>
    /// The view that displays the fetched releases.
    /// </summary>
    private ReleaseResultView _releaseResultView;

    /// <summary>
    /// The view that displays the fetched tracks.
    /// </summary>
    private TrackResultView _trackResultView;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public DatabaseScrobbleViewModel(IWindowManager windowManager)
      : base(windowManager, "Database Scrobbler")
    {
      SearchText = "";
      DatabaseToSearch = Database.LastFm;
      SearchType = SearchType.Artist;
      MaxResults = 25;
      FetchedArtists = new ObservableCollection<FetchedArtistViewModel>();
      FetchedReleases = new ObservableCollection<FetchedReleaseViewModel>();
      FetchedTracks = new ObservableCollection<FetchedTrackViewModel>();
      _artistResultView = new ArtistResultView() { DataContext = this };
      _releaseResultView = new ReleaseResultView() { DataContext = this };
      _trackResultView = new TrackResultView() { DataContext = this };
      UseCurrentTime = true;
    }

    /// <summary>
    /// Searches the entered <see cref="SearchText"/>.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task Search()
    {
      EnableControls = false;

      try
      {
        if (SearchType == SearchType.Artist)
          await SearchArtist();
        else if (SearchType == SearchType.Release)
          await SearchRelease();
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchArtist()
    {
      try
      {
        OnStatusUpdated("Trying to search for artist '" + SearchText + "'...");

        FetchedArtists.Clear();

        if (DatabaseToSearch == Database.LastFm)
          await SearchArtistLastFm();

        if (FetchedArtists.Count != 0)
        {
          CurrentView = _artistResultView;
          OnStatusUpdated("Found " + FetchedArtists.Count + " artists");
        }
        else
          OnStatusUpdated("Found no artists");
      }
      catch(Exception ex)
      {
        OnStatusUpdated("Error while searching for artist '" + SearchText + "': " + ex.Message);
      }
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchArtistLastFm()
    {
      var response = await MainViewModel.Client.Artist.SearchAsync(SearchText, 1, MaxResults);
      if (response.Success)
      {
        foreach (var s in response.Content)
        {
          AddArtistViewModel(s.Name, s.Mbid, s.MainImage.Large);
        }
      }
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Adds a new <see cref="FetchedArtistViewModel"/> to the <see cref="FetchedArtists"/>.
    /// </summary>
    /// <param name="name">Name of the artist.</param>
    /// <param name="mbid">Mbid of the artist.</param>
    /// <param name="image">Image of the artist.</param>
    private void AddArtistViewModel(string name, string mbid, Uri image)
    {
      FetchedArtistViewModel vm = new FetchedArtistViewModel(new Artist(name, mbid, image));
      vm.ArtistClicked += ArtistClicked;
      FetchedArtists.Add(vm);
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchRelease()
    {
      try
      {
        OnStatusUpdated("Trying to search for release '" + SearchText + "'");

        FetchedReleases.Clear();

        if (DatabaseToSearch == Database.LastFm)
          await SearchReleaseLastFm();

        if (FetchedReleases.Count != 0)
        {
          FetchedReleaseThroughArtist = false;
          CurrentView = _releaseResultView;
          OnStatusUpdated("Found " + FetchedReleases.Count + " releases");
        }
        else
          OnStatusUpdated("Found no releases");
      }
      catch(Exception ex)
      {
        OnStatusUpdated("Error while searching for release '" + SearchText + "': " + ex.Message);
      }
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchReleaseLastFm()
    {
      var response = await MainViewModel.Client.Album.SearchAsync(SearchText, 1, MaxResults);
      if (response.Success)
      {
        foreach (var s in response.Content)
        {
          AddReleaseViewModel(s.Name, s.ArtistName, s.Mbid, s.Images.Large);
        }
      }
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Adds a new <see cref="FetchedReleaseViewModel"/> to the <see cref="FetchedReleases"/>.
    /// </summary>
    /// <param name="name">Name of the release.</param>
    /// <param name="artistName">Name of the artist.</param>
    /// <param name="mbid">Mbid of the release.</param>
    /// <param name="image">Image of the release.</param>
    private void AddReleaseViewModel(string name, string artistName, string mbid, Uri image)
    {
      FetchedReleaseViewModel vm = new FetchedReleaseViewModel(new Release(name, artistName, mbid, image));
      vm.ReleaseClicked += ReleaseClicked;
      FetchedReleases.Add(vm);
    }

    /// <summary>
    /// Fetches the release list of the clicked artist.
    /// </summary>
    /// <param name="sender">Clicked artist as <see cref="LastArtist"/>.</param>
    /// <param name="e">Ignored.</param>
    public async void ArtistClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;

        try
        {
          var artist = sender as Artist;

          OnStatusUpdated("Trying to fetch releases from '" + artist.Name + "'");

          if (DatabaseToSearch == Database.LastFm)
            await ArtistClickedLastFm(artist);
        }
        catch (Exception ex)
        {
          OnStatusUpdated("Fatal error while fetching releases from artist: " + ex.Message);
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Fetches the release list of the clicked artist via Last.fm.
    /// </summary>
    /// <param name="artist"></param>
    /// <returns>Task.</returns>
    private async Task ArtistClickedLastFm(Artist artist)
    {
      var response = await MainViewModel.Client.Artist.GetTopAlbumsAsync(artist.Name, false, 1, MaxResults);
      if (response.Success)
      {
        FetchedReleases.Clear();
        foreach (var s in response.Content)
        {
          FetchedReleaseViewModel vm = new FetchedReleaseViewModel(new Release(s.Name, s.ArtistName, s.Mbid, s.Images.Large));
          vm.ReleaseClicked += ReleaseClicked;
          FetchedReleases.Add(vm);
        }

        if (FetchedReleases.Count != 0)
        {
          FetchedReleaseThroughArtist = true;
          CurrentView = _releaseResultView;
          OnStatusUpdated("Successfully fetched releases from '" + artist.Name + "'");
        }
        else
          OnStatusUpdated("'" + artist.Name + "'" + " has no releases");
      }
      else
        OnStatusUpdated("Error while fetching releases from '" + artist.Name + "'");
    }

    /// <summary>
    /// Fetches the tracklist of the clicked release and displays it.
    /// </summary>
    /// <param name="sender">Clicked release as <see cref="LastAlbum"/>.</param>
    /// <param name="e">Ignored.</param>
    public async void ReleaseClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;

        try
        {
          var release = sender as Release;

          OnStatusUpdated("Trying to fetch tracklist from '" + release.Name + "'");

          LastResponse<LastAlbum> detailedRelease = null;
          if (release.Mbid != null && release.Mbid != "")
            detailedRelease = await MainViewModel.Client.Album.GetInfoByMbidAsync(release.Mbid);
          else
            detailedRelease = await MainViewModel.Client.Album.GetInfoAsync(release.ArtistName, release.Name);

          if (detailedRelease.Success)
          {
            FetchedTracks.Clear();
            foreach (var t in detailedRelease.Content.Tracks)
            {
              FetchedTrackViewModel vm = new FetchedTrackViewModel(new ScrobbleBase(t.Name, t.ArtistName, t.AlbumName, "", t.Duration), release.Image);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              FetchedTracks.Add(vm);
            }

            if (FetchedTracks.Count != 0)
            {
              CurrentView = _trackResultView;
              OnStatusUpdated("Successfully fetched tracklist from '" + release.Name + "'");
            }
            else
              OnStatusUpdated("'" + release.Name + "' has no tracks");
          }
          else
            OnStatusUpdated("Error while fetching tracklist from '" + release.Name + "'");
        }
        catch(Exception ex)
        {
          OnStatusUpdated("Fatal error while fetching tracklist from release: " + ex.Message);
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Notifies <see cref="CanScrobble"/> that one or more
    /// ToScrobbles have been changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <remarks>
    /// Scrobbles will be 'reversed' meaning track 1 of the release
    /// will be scrobbled last.
    /// The first track to be scrobbled will have the <see cref="ScrobbleTimeViewModelBase.Time"/>
    /// as timestamp. The last track (track 1) will have the <see cref="ScrobbleTimeViewModelBase.Time"/>
    /// minus all the durations of the scrobbles before. 3 minute default duration.
    /// </remarks>
    public override async Task Scrobble()
    {
      EnableControls = false;

      try
      {
        OnStatusUpdated("Trying to scrobble selected tracks...");

        var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success)
          OnStatusUpdated("Successfully scrobbled!");
        else
          OnStatusUpdated("Error while scrobbling!");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while scrobbling: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates the list of scrobbles.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      DateTime finishingTime = Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (FetchedTrackViewModel vm in FetchedTracks.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.FetchedTrack.ArtistName, vm.FetchedTrack.AlbumName, vm.FetchedTrack.TrackName, finishingTime));
        if (vm.FetchedTrack.Duration.HasValue)
          finishingTime = finishingTime.Subtract(vm.FetchedTrack.Duration.Value);
        else
          finishingTime = finishingTime.Subtract(TimeSpan.FromMinutes(3.0));
      }

      return scrobbles;
    }

    /// <summary>
    /// Switches the <see cref="CurrentView"/> to the <see cref="_artistResultView"/>.
    /// </summary>
    public void BackToArtist()
    {
      CurrentView = _artistResultView;
    }

    /// <summary>
    /// Switches the <see cref="CurrentView"/> to the <see cref="_releaseResultView"/>.
    /// </summary>
    public void BackFromTrackResult()
    {
      CurrentView = _releaseResultView;
    }

    /// <summary>
    /// Marks all tracks as "ToScrobble".
    /// </summary>
    public void SelectAll()
    {
      foreach (var vm in FetchedTracks)
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all tracks as not "ToScrobble";
    /// </summary>
    public void SelectNone()
    {
      foreach (var vm in FetchedTracks)
      {
        vm.ToScrobble = false;
      }
    }
  }
}