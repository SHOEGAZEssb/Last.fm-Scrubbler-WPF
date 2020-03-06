using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using SetlistFmApi.Model.Music;
using SetlistFmApi.SearchOptions.Music;
using SetlistFmApi.SearchResults.Music;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Search type.
  /// </summary>
  public enum SetlistSearchType
  {
    /// <summary>
    /// Search for artists.
    /// </summary>
    Artist
  }

  /// <summary>
  /// Viewmodel for the <see cref="SetlistFMScrobbleView"/>
  /// </summary>
  public class SetlistFMScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<FetchedTrackViewModel>
  {
    #region Properties

    /// <summary>
    /// Event that triggers when the activation of a new item has been processed.
    /// </summary>
    public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

    /// <summary>
    /// The result that should be displayed.
    /// </summary>
    public ViewModelBase ActiveResult
    {
      get => _activeResult;
      private set
      {
        if (ActiveResult != value)
        {
          _activeResult = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private ViewModelBase _activeResult;

    /// <summary>
    /// Text to search.
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
    /// The selected type to search.
    /// </summary>
    public SetlistSearchType SelectedSearchType
    {
      get { return _selectedSearchType; }
      set
      {
        _selectedSearchType = value;
        NotifyOfPropertyChange();
      }
    }
    private SetlistSearchType _selectedSearchType;

    /// <summary>
    /// The album string to use.
    /// Needed because Setlist.fm does not provide
    /// album strings for setlists.
    /// </summary>
    public string AlbumString
    {
      get { return _albumString; }
      set
      {
        _albumString = value;
        NotifyOfPropertyChange();
      }
    }
    private string _albumString;

    /// <summary>
    /// Indicates if a custom album string is used.
    /// </summary>
    public bool CustomAlbumString
    {
      get { return _customAlbumString; }
      set
      {
        _customAlbumString = value;
        if (!value)
          AlbumString = string.Empty;

        NotifyOfPropertyChange();
      }
    }
    private bool _customAlbumString;

    /// <summary>
    /// The number of the result page of setlists to get.
    /// </summary>
    public int SetlistResultPage
    {
      get { return _setlistResultPage; }
      set
      {
        _setlistResultPage = value;
        NotifyOfPropertyChange();

        // todo: if we ever fetch setlists other than by clicking an
        // artist we need to change this I guess.
        if (_lastClickedArtist != null)
          Artist_Clicked(_lastClickedArtist, EventArgs.Empty);
      }
    }
    private int _setlistResultPage;

    /// <summary>
    /// The number of the result page of artists to get.
    /// </summary>
    public int ArtistResultPage
    {
      get { return _artistResultPage; }
      set
      {
        _artistResultPage = value;
        NotifyOfPropertyChange();
      }
    }
    private int _artistResultPage;

    /// <summary>
    /// Command for executing the search.
    /// </summary>
    public ICommand SearchCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Object used to communicate with the Setlist.fm api.
    /// </summary>
    private readonly SetlistFmApi.SetlistFmApi _setlistFMClient;

    /// <summary>
    /// The last clicked <see cref="Artist"/>.
    /// </summary>
    private Data.Artist _lastClickedArtist;

    /// <summary>
    /// Last.fm API object for getting artist information.
    /// </summary>
    private readonly IArtistApi _artistAPI;

    /// <summary>
    /// The last used artist result viewmodel.
    /// </summary>
    private ArtistResultViewModel _artistResultVM;

    /// <summary>
    /// The last used setlist result viewmodel.
    /// </summary>
    private SetlistResultViewModel _setlistResultVM;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="artistAPI">Last.fm API object for getting artist information.</param>
    public SetlistFMScrobbleViewModel(IExtendedWindowManager windowManager, IArtistApi artistAPI)
      : base(windowManager, "Setlist.fm Scrobbler")
    {
      Scrobbles = new ObservableCollection<FetchedTrackViewModel>();
      _artistAPI = artistAPI;
      _setlistFMClient = new SetlistFmApi.SetlistFmApi("23b3fd98-f5c7-49c6-a7d2-28498c0c2283");
      AlbumString = "";
      SetlistResultPage = 1;
      ArtistResultPage = 1;
      SearchCommand = new DelegateCommand((o) => Search().Forget());
    }

    /// <summary>
    /// Searches for info.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task Search()
    {
      if (SelectedSearchType == SetlistSearchType.Artist)
        await SearchArtists();
    }

    /// <summary>
    /// Searches for artists with the <see cref="SearchText"/>.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task SearchArtists()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated($"Searching for artist '{SearchText}'");
        IEnumerable<Data.Artist> artists = await GetArtists();

        if (artists.Any())
        {
          if (_artistResultVM != null)
          {
            _artistResultVM.ArtistClicked -= Artist_Clicked;
            _artistResultVM.Dispose();
          }

          _artistResultVM = new ArtistResultViewModel(artists);
          _artistResultVM.ArtistClicked += Artist_Clicked;
          ActiveResult = _artistResultVM;
          OnStatusUpdated("Successfully fetched artists");
        }
        else
          OnStatusUpdated("Found no artists");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while searching for artist '{SearchText}': {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Gets the artists by searching for them.
    /// </summary>
    /// <returns>Found artists.</returns>
    private async Task<IEnumerable<Data.Artist>> GetArtists()
    {
      ArtistSearchResult asr = null;
      await Task.Run(() => asr = _setlistFMClient.FindArtists(new ArtistSearchOptions() { Name = SearchText, Page = ArtistResultPage }));

      if (asr != null)
      {
        // todo: can we do a cool select query on asr.Artists to create the artist IEnumerable?
        List<Data.Artist> artists = new List<Data.Artist>();
        foreach (var artist in asr.Artists)
        {
          Uri imageUri = await GetImageForArtist(artist.Name);
          artists.Add(new Data.Artist(artist.Name, artist.MbId, imageUri));
        }

        return artists;
      }
      else
        return Enumerable.Empty<Data.Artist>();
    }

    /// <summary>
    /// Gets the Uri of the image of the given <paramref name="artistName"/>.
    /// </summary>
    /// <param name="artistName">Artist to get image for.</param>
    /// <returns>Uri of the artist image.</returns>
    private async Task<Uri> GetImageForArtist(string artistName)
    {
      var response = await _artistAPI.GetInfoAsync(artistName);
      if (response.Success && response.Status == LastResponseStatus.Successful)
        return response.Content.MainImage?.ExtraLarge;
      return null;
    }

    /// <summary>
    /// Shows the setlist of an artist.
    /// </summary>
    /// <param name="sender">The artist to show setlists from.</param>
    /// <param name="e">Ignored.</param>
    private async void Artist_Clicked(object sender, EventArgs e)
    {
      try
      {
        EnableControls = false;
        var clickedArtist = sender as Data.Artist;
        _lastClickedArtist = clickedArtist;
        OnStatusUpdated($"Fetching setlists from artist '{clickedArtist.Name}'...");
        IEnumerable<Setlist> setlists = await GetSetlists(clickedArtist);

        if (setlists.Any())
        {
          if(_setlistResultVM != null)
          {
            _setlistResultVM.SetlistClicked -= Setlist_Clicked;
            _setlistResultVM.Dispose();
          }

          _setlistResultVM = new SetlistResultViewModel(setlists);
          _setlistResultVM.SetlistClicked += Setlist_Clicked;
          ActiveResult = _setlistResultVM;
          OnStatusUpdated($"Successfully fetched setlists from artist '{clickedArtist.Name}'");
        }
        else
          OnStatusUpdated($"No setlists found for artist '{clickedArtist.Name}'");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while fetching setlists: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Gets the setlists for the given <paramref name="artist"/>.
    /// </summary>
    /// <param name="artist">Artist to get setlists for.</param>
    /// <returns>Setlists of the given <paramref name="artist"/>.</returns>
    private async Task<IEnumerable<Setlist>> GetSetlists(Data.Artist artist)
    {
      SetlistSearchResult ssr = null;
      await Task.Run(() => ssr = _setlistFMClient.FindSetlistsByArtist(new SetlistByArtistSearchOptions() { MbId = artist.Mbid, Page = SetlistResultPage }));

      if (ssr != null)
        return ssr.Setlists;
      else
        return Enumerable.Empty<Setlist>();
    }

    /// <summary>
    /// Shows the tracks of a setlist.
    /// </summary>
    /// <param name="sender">The setlist to get tracks from.</param>
    /// <param name="e">Ignored.</param>
    private void Setlist_Clicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;

        try
        {
          ObservableCollection<FetchedTrackViewModel> vms = new ObservableCollection<FetchedTrackViewModel>();
          var clickedSetlist = sender as Setlist;
          foreach (var set in clickedSetlist.Sets)
          {
            foreach (var song in set.Songs)
            {
              vms.Add(new FetchedTrackViewModel(new ScrobbleBase(song.Name, clickedSetlist.Artist.Name), null));
            }
          }

          if (vms.Count > 0)
          {
            OnStatusUpdated("Successfully got tracks from setlist");
            if (ActiveResult is IDisposable d)
              d.Dispose();
            Scrobbles = vms;
          }
          else
            OnStatusUpdated("Setlist has no tracks");
        }
        catch (Exception ex)
        {
          OnStatusUpdated($"Fatal error while getting setlist tracks: {ex.Message}");
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Goes back to the found setlists.
    /// </summary>
    public void BackFromTrackResult()
    {
      Scrobbles.Clear();
      if (ActiveResult is IDisposable d)
        d.Dispose();
      ActiveResult = _setlistResultVM;
    }

    /// <summary>
    /// Goes back to the found artists.
    /// </summary>
    public void BackToArtists()
    {
      if (ActiveResult is IDisposable d)
        d.Dispose();
      ActiveResult = _artistResultVM;
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
        OnStatusUpdated($"Fatal error while scrobbling: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates a list with scrobbles.
    /// </summary>
    /// <returns>List with scrobbles that would be
    /// scrobbled with the current settings.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      DateTime finishingTime = ScrobbleTimeVM.Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (FetchedTrackViewModel vm in Scrobbles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.ArtistName, AlbumString, vm.TrackName, finishingTime));
        if (vm.Duration.HasValue)
          finishingTime = finishingTime.Subtract(vm.Duration.Value);
        else
          finishingTime = finishingTime.Subtract(TimeSpan.FromMinutes(3.0));
      }

      return scrobbles;
    }
  }
}