using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.ViewModels.SubViewModels;
using Scrubbler.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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
  /// ViewModel for the <see cref="Views.ScrobbleViews.DatabaseScrobbleView"/>.
  /// </summary>
  public class DatabaseScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<FetchedTrackViewModel>
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

    #endregion Properties

    #region Member

    /// <summary>
    /// The view that displays the fetched artists.
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

    /// <summary>
    /// Last.fm artist api used to search for artists.
    /// </summary>
    private IArtistApi _lastfmArtistAPI;

    /// <summary>
    /// Last.fm album api used to search for albums.
    /// </summary>
    private IAlbumApi _lastfmAlbumAPI;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="lastfmArtistAPI">Last.fm artist api used to search for artists.</param>
    /// <param name="lastfmAlbumAPI">Last.fm album api used to search for albums.</param>
    public DatabaseScrobbleViewModel(IExtendedWindowManager windowManager, IArtistApi lastfmArtistAPI, IAlbumApi lastfmAlbumAPI)
      : base(windowManager, "Database Scrobbler")
    {
      _lastfmArtistAPI = lastfmArtistAPI;
      _lastfmAlbumAPI = lastfmAlbumAPI;
      DatabaseToSearch = Database.LastFm;
      SearchType = SearchType.Artist;
      MaxResults = 25;
      FetchedArtists = new ObservableCollection<FetchedArtistViewModel>();
      FetchedReleases = new ObservableCollection<FetchedReleaseViewModel>();
      Scrobbles = new ObservableCollection<FetchedTrackViewModel>();
      _artistResultView = new ArtistResultView() { DataContext = this };
      _releaseResultView = new ReleaseResultView() { DataContext = this };
      _trackResultView = new TrackResultView() { DataContext = this };
    }

    #endregion Construction

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
        OnStatusUpdated(string.Format("Trying to search for artist '{0}'...", SearchText));

        FetchedArtists.Clear();

        if (DatabaseToSearch == Database.LastFm)
          await SearchArtistLastFm();

        if (FetchedArtists.Count != 0)
        {
          CurrentView = _artistResultView;
          OnStatusUpdated(string.Format("Found {0} artists", FetchedArtists.Count));
        }
        else
          OnStatusUpdated("Found no artists");
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while searching for artist '{0}': {1}", SearchText, ex.Message));
      }
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchArtistLastFm()
    {
      var response = await _lastfmArtistAPI.SearchAsync(SearchText, 1, MaxResults);
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
        OnStatusUpdated(string.Format("Trying to search for release '{0}'", SearchText));

        FetchedReleases.Clear();

        if (DatabaseToSearch == Database.LastFm)
          await SearchReleaseLastFm();

        if (FetchedReleases.Count != 0)
        {
          FetchedReleaseThroughArtist = false;
          CurrentView = _releaseResultView;
          OnStatusUpdated(string.Format("Found {0} releases", FetchedReleases.Count));
        }
        else
          OnStatusUpdated("Found no releases");
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while searching for release '{0}': {1}", SearchText,  ex.Message));
      }
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchReleaseLastFm()
    {
      var response = await _lastfmAlbumAPI.SearchAsync(SearchText, 1, MaxResults);
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
          OnStatusUpdated(string.Format("Trying to fetch releases from artist '{0}'", artist.Name));
          if (DatabaseToSearch == Database.LastFm)
            await ArtistClickedLastFm(artist);
        }
        catch (Exception ex)
        {
          OnStatusUpdated(string.Format("Fatal error while fetching releases from artist: {0}", ex.Message));
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
      var response = await _lastfmArtistAPI.GetTopAlbumsAsync(artist.Name, false, 1, MaxResults);
      if (response.Success && response.Status == LastResponseStatus.Successful)
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
          OnStatusUpdated(string.Format("Successfully fetched releases from artist '{0}'", artist.Name));
        }
        else
          OnStatusUpdated(string.Format("Artist '{0} has no releases", artist.Name));
      }
      else
        OnStatusUpdated(string.Format("Error while fetching releases from artist '{0}': {1}", artist.Name, response.Status));
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
          OnStatusUpdated(string.Format("Trying to fetch tracklist from release '{0}'", release.Name));

          LastResponse<LastAlbum> response = null;
          if (!string.IsNullOrEmpty(release.Mbid))
            response = await _lastfmAlbumAPI.GetInfoByMbidAsync(release.Mbid);
          else
            response = await _lastfmAlbumAPI.GetInfoAsync(release.ArtistName, release.Name);

          if (response.Success && response.Status == LastResponseStatus.Successful)
          {
            Scrobbles.Clear();
            foreach (var t in response.Content.Tracks)
            {
              FetchedTrackViewModel vm = new FetchedTrackViewModel(new ScrobbleBase(t.Name, t.ArtistName, t.AlbumName, "", t.Duration), release.Image);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              Scrobbles.Add(vm);
            }

            if (Scrobbles.Count != 0)
            {
              CurrentView = _trackResultView;
              OnStatusUpdated(string.Format("Successfully fetched tracklist from release '{0}'", release.Name));
            }
            else
              OnStatusUpdated(string.Format("Release '{0}' has no tracks", release.Name));
          }
          else
            OnStatusUpdated(string.Format("Error while fetching tracklist from release '{0}': {1}", release.Name, response.Status));
        }
        catch(Exception ex)
        {
          OnStatusUpdated(string.Format("Fatal error while fetching tracklist from release: {0}", ex.Message));
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Notifies that one or more
    /// ToScrobbles have been changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <remarks>
    /// Scrobbles will be 'reversed' meaning track 1 of the release
    /// will be scrobbled last.
    /// The first track to be scrobbled will have the <see cref="ScrobbleTimeViewModel.Time"/>
    /// as timestamp. The last track (track 1) will have the <see cref="ScrobbleTimeViewModel.Time"/>
    /// minus all the durations of the scrobbles before. 3 minute default duration.
    /// </remarks>
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
          OnStatusUpdated(string.Format("Error while scrobbling selected tracks: {0}", response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while scrobbling selected tracks: {0}", ex.Message));
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
      DateTime finishingTime = ScrobbleTimeVM.Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (FetchedTrackViewModel vm in Scrobbles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, finishingTime));
        if (vm.Duration.HasValue)
          finishingTime = finishingTime.Subtract(vm.Duration.Value);
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
  }
}