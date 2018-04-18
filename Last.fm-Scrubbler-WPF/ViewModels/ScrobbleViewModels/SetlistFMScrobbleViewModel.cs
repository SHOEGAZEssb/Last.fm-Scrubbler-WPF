using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.ViewModels.SubViewModels;
using Scrubbler.Views;
using Scrubbler.Views.SubViews;
using SetlistFmApi.Model.Music;
using SetlistFmApi.SearchOptions.Music;
using SetlistFmApi.SearchResults.Music;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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
  /// Viewmodel for the <see cref="Views.ScrobbleViews.SetlistFMScrobbleView"/>
  /// </summary>
  public class SetlistFMScrobbleViewModel : ScrobbleTimeViewModelBase
  {
    #region Properties

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
          AlbumString = "";

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
        if(_lastClickedArtist != null)
          ArtistClicked(_lastClickedArtist, EventArgs.Empty);
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
    /// List of fetched artists.
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
    /// List of fetched setlists.
    /// </summary>
    public ObservableCollection<FetchedSetlistViewModel> FetchedSetlists
    {
      get { return _fetchedSetlists; }
      private set
      {
        _fetchedSetlists = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<FetchedSetlistViewModel> _fetchedSetlists;

    /// <summary>
    /// List of fetched tracks.
    /// </summary>
    public ObservableCollection<FetchedTrackViewModel> FetchedTracks
    {
      get { return _fetchedTracks; }
      private set
      {
        _fetchedTracks = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
        NotifyOfPropertyChange(() => CanScrobble);
      }
    }
    private ObservableCollection<FetchedTrackViewModel> _fetchedTracks;

    /// <summary>
    /// The UserControl that is currently shown in the UI.
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
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanPreview);
      }
    }

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return true; }
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
    /// Object used to communicate with the Setlist.fm api.
    /// </summary>
    private SetlistFmApi.SetlistFmApi _setlistFMClient;

    /// <summary>
    /// View that shows the results of the artist search.
    /// </summary>
    private ArtistResultView _artistResultView;

    /// <summary>
    /// View that shows the setlists of an artist.
    /// </summary>
    private SetlistResultView _setlistResultView;

    /// <summary>
    /// View that shows the tracks of a setlist.
    /// </summary>
    private TrackResultView _trackResultView;

    /// <summary>
    /// The last clicked <see cref="Artist"/>.
    /// </summary>
    private Models.Artist _lastClickedArtist;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public SetlistFMScrobbleViewModel(IExtendedWindowManager windowManager)
      : base(windowManager, "Setlist.fm Scrobbler")
    {
      _setlistFMClient = new SetlistFmApi.SetlistFmApi("23b3fd98-f5c7-49c6-a7d2-28498c0c2283");
      _artistResultView = new ArtistResultView() { DataContext = this };
      _setlistResultView = new SetlistResultView() { DataContext = this };
      _trackResultView = new TrackResultView() { DataContext = this };
      AlbumString = "";
      FetchedArtists = new ObservableCollection<FetchedArtistViewModel>();
      FetchedSetlists = new ObservableCollection<FetchedSetlistViewModel>();
      FetchedTracks = new ObservableCollection<FetchedTrackViewModel>();
      SetlistResultPage = 1;
      ArtistResultPage = 1;
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
      EnableControls = false;

      try
      {
        OnStatusUpdated(string.Format("Searching for artist '{0}'", SearchText));
        ArtistSearchResult asr = null;
        await Task.Run(() => asr = _setlistFMClient.FindArtists(new ArtistSearchOptions() { Name = SearchText, Page = ArtistResultPage }));

        if (asr != null)
        {
          ObservableCollection<FetchedArtistViewModel> vms = new ObservableCollection<FetchedArtistViewModel>();
          foreach (var artist in asr.Artists)
          {
            Uri imgUri = null;
            var response = await MainViewModel.Client.Artist.GetInfoAsync(artist.Name);
            if (response.Success)
              imgUri = response.Content.MainImage.ExtraLarge;

            FetchedArtistViewModel vm = new FetchedArtistViewModel(new Models.Artist(artist.Name, artist.MbId, imgUri));
            vm.ArtistClicked += ArtistClicked;
            vms.Add(vm);
          }

          if (vms.Count > 0)
          {
            FetchedArtists = new ObservableCollection<FetchedArtistViewModel>(vms);
            CurrentView = _artistResultView;
            OnStatusUpdated("Successfully fetched artists");
          }
          else
            OnStatusUpdated("Found no artists");
        }
        else
          OnStatusUpdated("Found no artists");

      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while searching for artist: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Shows the setlist of an artist.
    /// </summary>
    /// <param name="sender">The artist to show setlists from.</param>
    /// <param name="e">Ignored.</param>
    private async void ArtistClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;

        try
        {
          OnStatusUpdated("Fetching setlists from selected artist...");
          Models.Artist clickedArtist = sender as Models.Artist;
          _lastClickedArtist = clickedArtist;

          SetlistSearchResult ssr = null;
          await Task.Run(() => ssr = _setlistFMClient.FindSetlistsByArtist(new SetlistByArtistSearchOptions() { MbId = clickedArtist.Mbid, Page = SetlistResultPage}));

          if (ssr != null)
          {
            ObservableCollection<FetchedSetlistViewModel> vms = new ObservableCollection<FetchedSetlistViewModel>();
            foreach (var setlist in ssr.Setlists)
            {
              FetchedSetlistViewModel vm = new FetchedSetlistViewModel(setlist);
              vm.SetlistClicked += SetlistClicked;
              vms.Add(vm);
            }

            if (vms.Count > 0)
            {
              OnStatusUpdated("Successfully fetched setlists");
              FetchedSetlists = vms;
              CurrentView = _setlistResultView;
            }
            else
              OnStatusUpdated("No setlists found");
          }
          else
            OnStatusUpdated("No setlists found");
        }
        catch (Exception ex)
        {
          OnStatusUpdated("Fatal error while fetching setlists: " + ex.Message);
        }
        finally
        {
          EnableControls = true;
        }
      }
    }

    /// <summary>
    /// Shows the tracks of a setlist.
    /// </summary>
    /// <param name="sender">The setlist to get tracks from.</param>
    /// <param name="e">Ignored.</param>
    private void SetlistClicked(object sender, EventArgs e)
    {
      if (EnableControls)
      {
        EnableControls = false;

        try
        {
          ObservableCollection<FetchedTrackViewModel> vms = new ObservableCollection<FetchedTrackViewModel>();
          var clickedSetlist = sender as Setlist;
          foreach(var set in clickedSetlist.Sets)
          {
            foreach(var song in set.Songs)
            {
              FetchedTrackViewModel vm = new FetchedTrackViewModel(new ScrobbleBase(song.Name, clickedSetlist.Artist.Name), null);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              vms.Add(vm);
            }
          }

          if (vms.Count > 0)
          {
            OnStatusUpdated("Successfully got tracks from setlist");
            FetchedTracks = vms;
            CurrentView = _trackResultView;
          }
          else
            OnStatusUpdated("Setlist has no tracks");
        }
        catch (Exception ex)
        {
          OnStatusUpdated("Fatal error while getting setlist tracks: " + ex.Message);
        }
        finally
        {
          EnableControls = true;

          // more or less a hack, but we cant do it before because EnableControls is false.
          NotifyOfPropertyChange(() => CanSelectAll);
          NotifyOfPropertyChange(() => CanSelectNone);
        }
      }
    }

    /// <summary>
    /// "Back" was clicked on the <see cref="_trackResultView"/>.
    /// Goes back to the <see cref="_setlistResultView"/>.
    /// </summary>
    public void BackFromTrackResult()
    {
      CurrentView = _setlistResultView;
    }

    /// <summary>
    /// Goes back to the <see cref="_artistResultView"/>.
    /// </summary>
    public void BackToArtists()
    {
      CurrentView = _artistResultView;
    }

    /// <summary>
    /// Notifies the ui that ToScrobble changed for any track.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <returns>Task.</returns>
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
      catch(Exception ex)
      {
        OnStatusUpdated("Fatal error while scrobbling: " + ex.Message);
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
      DateTime finishingTime = Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (FetchedTrackViewModel vm in FetchedTracks.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.FetchedTrack.ArtistName, AlbumString, vm.FetchedTrack.TrackName, finishingTime));
        if (vm.FetchedTrack.Duration.HasValue)
          finishingTime = finishingTime.Subtract(vm.FetchedTrack.Duration.Value);
        else
          finishingTime = finishingTime.Subtract(TimeSpan.FromMinutes(3.0));
      }

      return scrobbles;
    }

    /// <summary>
    /// Set ToScrobble to true for all <see cref="FetchedTracks"/>.
    /// </summary>
    public void SelectAll()
    {
      foreach (var vm in FetchedTracks)
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Set ToScrobble to false for all <see cref="FetchedTracks"/>.
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