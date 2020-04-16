using DiscogsClient;
using DiscogsClient.Data.Query;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
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

    /// <summary>
    /// Search Discogs.com
    /// </summary>
    Discogs,

    /// <summary>
    /// search musicbrainz.org
    /// </summary>
    MusicBrainz
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
  public class DatabaseScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<FetchedTrackViewModel>
  {
    #region Properties

    /// <summary>
    /// The result that should be displayed.
    /// </summary>
    public ViewModelBase ActiveResult
    {
      get => _activeResult;
      private set
      {
        if(ActiveResult != value)
        {
          _activeResult = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private ViewModelBase _activeResult;

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
    /// Command for executing the search.
    /// </summary>
    public ICommand SearchCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Last.fm artist api used to search for artists.
    /// </summary>
    private readonly IArtistApi _lastfmArtistAPI;

    /// <summary>
    /// Last.fm album api used to search for albums.
    /// </summary>
    private readonly IAlbumApi _lastfmAlbumAPI;

    private readonly IDiscogsDataBaseClient _discogsClient;

    /// <summary>
    /// The last <see cref="ArtistResultViewModel"/>.
    /// </summary>
    private ArtistResultViewModel _artistResultVM;

    /// <summary>
    /// The last <see cref="ReleaseResultViewModel"/>.
    /// </summary>
    private ReleaseResultViewModel _releaseResultVM;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="lastfmArtistAPI">Last.fm artist api used to search for artists.</param>
    /// <param name="lastfmAlbumAPI">Last.fm album api used to search for albums.</param>
    /// <param name="discogsClient">Client used to interact with Discogs.com</param>
    public DatabaseScrobbleViewModel(IExtendedWindowManager windowManager, IArtistApi lastfmArtistAPI, IAlbumApi lastfmAlbumAPI, IDiscogsDataBaseClient discogsClient)
      : base(windowManager, "Database Scrobbler")
    {
      _lastfmArtistAPI = lastfmArtistAPI;
      _lastfmAlbumAPI = lastfmAlbumAPI;
      _discogsClient = discogsClient ?? throw new ArgumentNullException(nameof(discogsClient));
      DatabaseToSearch = Database.LastFm;
      SearchType = SearchType.Artist;
      MaxResults = 25;
      Scrobbles = new ObservableCollection<FetchedTrackViewModel>();

      SearchCommand = new DelegateCommand((o) => Search());
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
        OnStatusUpdated($"Trying to search for artist '{SearchText}'...");

        var fetchedArtists = Enumerable.Empty<Artist>();
        if (DatabaseToSearch == Database.LastFm)
          fetchedArtists = await SearchArtistLastFm();
        else if (DatabaseToSearch == Database.Discogs)
          fetchedArtists = await SearchArtistDiscogs();
        else if (DatabaseToSearch == Database.MusicBrainz)
          fetchedArtists = await SearchArtistMusicBrainz();

        if (fetchedArtists.Any())
        {
          // clean up old vm
          if (_artistResultVM != null)
          {
            _artistResultVM.ArtistClicked -= ArtistClicked;
            _artistResultVM.Dispose();
          }

          _artistResultVM = new ArtistResultViewModel(fetchedArtists);
          _artistResultVM.ArtistClicked += ArtistClicked;
          ActiveResult = _artistResultVM;
          //ActivateItem(_artistResultVM);
          OnStatusUpdated($"Found {fetchedArtists.Count()} artists");
        }
        else
          OnStatusUpdated("Found no artists");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while searching for artist '{SearchText}': {ex.Message}");
      }
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Artist>> SearchArtistLastFm()
    {
      var response = await _lastfmArtistAPI.SearchAsync(SearchText, 1, MaxResults);
      if (response.Success)
        return response.Content.Select(a => new Artist(a));
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>
    /// on discogs.com.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Artist>> SearchArtistDiscogs()
    {
      var result = _discogsClient.SearchAsEnumerable(new DiscogsSearch() { query = SearchText, type = DiscogsEntityType.artist }, MaxResults);
      var artists = new List<Artist>();
      await Task.Run(() =>
      {
        foreach (var a in result)
          artists.Add(new Artist(a.title, a.id.ToString(), string.IsNullOrEmpty(a.thumb) ? null : new Uri(a.thumb)));
      });
      return artists;
    }

    /// <summary>
    /// Searches for artists with the entered <see cref="SearchText"/>
    /// on musicbrainz.org.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Artist>> SearchArtistMusicBrainz()
    {
      var found = await Hqub.MusicBrainz.API.Entities.Artist.SearchAsync(SearchText, MaxResults);
      return found.Items.Select(i => new Artist(i.Name, i.Id, null));
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchRelease()
    {
      try
      {
        OnStatusUpdated($"Trying to search for release '{SearchText}'");

        var releases = Enumerable.Empty<Release>();
        if (DatabaseToSearch == Database.LastFm)
          releases = await SearchReleaseLastFm();
        else if (DatabaseToSearch == Database.Discogs)
          releases = await SearchReleaseDiscogs();
        else if (DatabaseToSearch == Database.MusicBrainz)
          releases = await SearchReleaseMusicBrainz();

        if (releases.Any())
        {
          ActivateNewReleaseResultViewModel(releases, false);
          OnStatusUpdated($"Found {releases.Count()} releases");
        }
        else
          OnStatusUpdated("Found no releases");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while searching for release '{SearchText}': {ex.Message}");
      }
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>
    /// on Last.fm.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Release>> SearchReleaseLastFm()
    {
      var response = await _lastfmAlbumAPI.SearchAsync(SearchText, 1, MaxResults);
      if (response.Success)
        return response.Content.Select(r => new Release(r));
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>
    /// on discogs.com.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Release>> SearchReleaseDiscogs()
    {
      var searchResults = _discogsClient.SearchAsEnumerable(new DiscogsSearch() { query = SearchText, type = DiscogsEntityType.master }, MaxResults);
      var releases = new List<Release>();
      await Task.Run(async () =>
      {
        foreach (var r in searchResults)
        {
          int id = r.id;
          if (r.type == DiscogsEntityType.master)
          {
            var rel = await _discogsClient.GetMasterAsync(r.id);
            if (rel.main_release != 0)
              id = rel.main_release;
          }
          releases.Add(new Release(r.title, null, id.ToString(), string.IsNullOrEmpty(r.thumb) ? null : new Uri(r.thumb)));
        }
      });
      return releases;
    }

    /// <summary>
    /// Searches for releases with the entered <see cref="SearchText"/>
    /// on musicbrainz.org
    /// </summary>
    /// <returns></returns>
    private async Task<IEnumerable<Release>> SearchReleaseMusicBrainz()
    {
      var found = await Hqub.MusicBrainz.API.Entities.ReleaseGroup.SearchAsync(SearchText, MaxResults);
      return found.Items.Select(i => new Release(i));
    }

    /// <summary>
    /// Fetches the release list of the clicked artist.
    /// </summary>
    /// <param name="sender">Clicked artist as <see cref="LastArtist"/>.</param>
    /// <param name="e">Ignored.</param>
    private async void ArtistClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;
        var artist = sender as Artist;

        try
        {
          OnStatusUpdated($"Trying to fetch releases from artist '{artist.Name}'");

          var releases = Enumerable.Empty<Release>();
          if (DatabaseToSearch == Database.LastFm)
            releases = await ArtistClickedLastFm(artist);
          else if (DatabaseToSearch == Database.Discogs)
            releases = await ArtistClickedDiscogs(artist);
          else if (DatabaseToSearch == Database.MusicBrainz)
            releases = await ArtistClickedMusicBrainz(artist);

          if (releases.Any())
          {
            ActivateNewReleaseResultViewModel(releases, true);
            OnStatusUpdated($"Successfully fetched releases from artist '{artist.Name}'");
          }
          else
            OnStatusUpdated($"Artist '{artist.Name}' has no releases");
        }
        catch (Exception ex)
        {
          OnStatusUpdated($"Fatal error while fetching releases from artist '{artist.Name}': {ex.Message}");
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Creates and activates a new <see cref="ReleaseResultViewModel"/>.
    /// </summary>
    /// <param name="releases">Release to create <see cref="ReleaseResultViewModel"/> for.</param>
    /// <param name="fetchedThroughArtist">If the <paramref name="releases"/> were fetched
    /// by clicking an artist.</param>
    private void ActivateNewReleaseResultViewModel(IEnumerable<Release> releases, bool fetchedThroughArtist)
    {
      // clean up old vm
      if (_releaseResultVM != null)
      {
        _releaseResultVM.ReleaseClicked -= ReleaseClicked;
        _releaseResultVM.BackToArtistRequested -= Release_BackToArtistRequested;
        _releaseResultVM.Dispose();
      }

      _releaseResultVM = new ReleaseResultViewModel(releases, fetchedThroughArtist);
      _releaseResultVM.ReleaseClicked += ReleaseClicked;
      _releaseResultVM.BackToArtistRequested += Release_BackToArtistRequested;
      ActiveResult = _releaseResultVM;
    }

    /// <summary>
    /// Goes back to the artist when the user requests it.s
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Release_BackToArtistRequested(object sender, EventArgs e)
    {
      BackToArtist();
    }

    /// <summary>
    /// Fetches the release list of the clicked artist via Last.fm.
    /// </summary>
    /// <param name="artist">Artist to fetch releases of.</param>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Release>> ArtistClickedLastFm(Artist artist)
    {
      var response = await _lastfmArtistAPI.GetTopAlbumsAsync(artist.Name, false, 1, MaxResults);
      if (response.Success && response.Status == LastResponseStatus.Successful)
        return response.Content.Select(r => new Release(r));
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Fetches the release list of the clicked artist via discogs.
    /// </summary>
    /// <param name="artist">Artist to fetch releases of.</param>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Release>> ArtistClickedDiscogs(Artist artist)
    {
      var a = _discogsClient.GetArtistReleaseAsEnumerable(int.Parse(artist.Mbid), null, MaxResults);
      var releases = new List<Release>();
      await Task.Run(() =>
      {
        foreach (var r in a)
          releases.Add(new Release(r.title, r.artist, (r.type == "master" && r.main_release != 0) ? r.main_release.ToString() : r.id.ToString(), string.IsNullOrEmpty(r.thumb) ? null : new Uri(r.thumb)));
      });

      return releases;
    }

    /// <summary>
    /// Fetches the release list of the clicked artist via musicbrainz.org.
    /// </summary>
    /// <param name="artist">Artist to fetch releases of.</param>
    /// <returns>Task.</returns>
    private async Task<IEnumerable<Release>> ArtistClickedMusicBrainz(Artist artist)
    {
      var r = await Hqub.MusicBrainz.API.Entities.ReleaseGroup.BrowseAsync("artist", artist.Mbid, MaxResults, 0, "artist-credits");
      return r.Items.Select(i => new Release(i));
    }

    /// <summary>
    /// Fetches the tracklist of the clicked release and displays it.
    /// </summary>
    /// <param name="sender">Clicked release as <see cref="LastAlbum"/>.</param>
    /// <param name="e">Ignored.</param>
    private async void ReleaseClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;
        var release = sender as Release;

        try
        {
          OnStatusUpdated($"Trying to fetch tracklist from release '{release.Name}'");

          var tracks = Enumerable.Empty<Track>();
          if (DatabaseToSearch == Database.LastFm)
            tracks = await FetchTracksLastFM(release);
          else if (DatabaseToSearch == Database.Discogs)
            tracks = await FetchTracksDiscogs(release);
          else if (DatabaseToSearch == Database.MusicBrainz)
            tracks = await FetchTracksMusicBrainz(release);

          foreach (var track in tracks)
          {
            var vm = new FetchedTrackViewModel(new ScrobbleBase(track), track.Image);
            Scrobbles.Add(vm);
          }

          if (Scrobbles.Count != 0)
            OnStatusUpdated($"Successfully fetched tracklist from release '{release.Name}'");
          else
            OnStatusUpdated($"Release '{release.Name}' has no tracks");
        }
        catch (Exception ex)
        {
          OnStatusUpdated($"Fatal error while fetching tracklist from release '{release.Name}': {ex.Message}");
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Fetches tracks of the given <paramref name="release"/> from Last.fm.
    /// </summary>
    /// <param name="release">Release to get tracks for.</param>
    /// <returns>Enumerable tracks of the given <paramref name="release"/>.</returns>
    private async Task<IEnumerable<Track>> FetchTracksLastFM(Release release)
    {
      LastResponse<LastAlbum> response = null;
      if (!string.IsNullOrEmpty(release.Mbid))
        response = await _lastfmAlbumAPI.GetInfoByMbidAsync(release.Mbid);
      else
        response = await _lastfmAlbumAPI.GetInfoAsync(release.ArtistName, release.Name);

      if (response.Success && response.Status == LastResponseStatus.Successful)
        return response.Content.Tracks.Select(t => new Track(t));
      else
        throw new Exception(response.Status.ToString());
    }

    /// <summary>
    /// Fetches tracks of the given <paramref name="release"/> from discogs.com.
    /// </summary>
    /// <param name="release">Release to get tracks for.</param>
    /// <returns>Enumerable tracks of the given <paramref name="release"/>.</returns>
    private async Task<IEnumerable<Track>> FetchTracksDiscogs(Release release)
    {
      var r = await _discogsClient.GetReleaseAsync(int.Parse(release.Mbid));
      return r.tracklist.Select(i => new Track(i.title, i.artists?.FirstOrDefault()?.name ?? r.artists.First().name, r.title, string.IsNullOrEmpty(r.thumb) ? null : new Uri(r.thumb)));
    }

    /// <summary>
    /// Fetches tracks of the given <paramref name="release"/> from musicbrainz.org.
    /// </summary>
    /// <param name="release">Release to get tracks for.</param>
    /// <returns>Enumerable tracks of the given <paramref name="release"/>.</returns>
    private static async Task<IEnumerable<Track>> FetchTracksMusicBrainz(Release release)
    {
      var t = await Hqub.MusicBrainz.API.Entities.ReleaseGroup.GetAsync(release.Mbid, "releases");
      var r = await Hqub.MusicBrainz.API.Entities.Release.GetAsync(t.Releases.First().Id, "media", "recordings");
      return r.Media.First().Tracks.Select(i => new Track(i.Recording.Title, release.ArtistName, release.Name, release.Image));
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
        OnStatusUpdated($"Fatal error while scrobbling selected tracks: {ex.Message}");
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
        finishingTime = vm.Duration.HasValue ? finishingTime.Subtract(vm.Duration.Value) : finishingTime.Subtract(TimeSpan.FromMinutes(3.0));
      }

      return scrobbles;
    }

    /// <summary>
    /// Switches the <see cref="ActiveResult"/> to the artist view.
    /// </summary>
    public void BackToArtist()
    {
      if (ActiveResult is IDisposable d)
        d.Dispose();
      ActiveResult = _artistResultVM;
    }

    /// <summary>
    /// Switches the <see cref="ActiveResult"/> to the release view.
    /// </summary>
    public void BackFromTrackResult()
    {
      Scrobbles.Clear();
    }
  }
}