using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
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
  public class SetlistFMScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<FetchedTrackViewModel>
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
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && Scrobbles.Any(s => s.ToScrobble); }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return Scrobbles.Any(i => i.ToScrobble); }
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

    /// <summary>
    /// Last.fm API object for getting artist information.
    /// </summary>
    private IArtistApi _artistAPI;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="artistAPI">Last.fm API object for getting artist information.</param>
    public SetlistFMScrobbleViewModel(IExtendedWindowManager windowManager, IArtistApi artistAPI)
      : base(windowManager, "Setlist.fm Scrobbler")
    {
      _artistAPI = artistAPI;
      _setlistFMClient = new SetlistFmApi.SetlistFmApi("23b3fd98-f5c7-49c6-a7d2-28498c0c2283");
      _artistResultView = new ArtistResultView() { DataContext = this };
      _setlistResultView = new SetlistResultView() { DataContext = this };
      _trackResultView = new TrackResultView() { DataContext = this };
      AlbumString = "";
      FetchedArtists = new ObservableCollection<FetchedArtistViewModel>();
      FetchedSetlists = new ObservableCollection<FetchedSetlistViewModel>();
      Scrobbles = new ObservableCollection<FetchedTrackViewModel>();
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
            var response = await _artistAPI.GetInfoAsync(artist.Name);
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
        OnStatusUpdated(string.Format("Fatal error while searching for artist '{0}': {1}", SearchText, ex.Message));
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
          Models.Artist clickedArtist = sender as Models.Artist;
          OnStatusUpdated(string.Format("Fetching setlists from artist '{0}'...", clickedArtist.Name));
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
              OnStatusUpdated(string.Format("Successfully fetched setlists from artist '{0}'", clickedArtist.Name));
              FetchedSetlists = vms;
              CurrentView = _setlistResultView;
            }
            else
              OnStatusUpdated(string.Format("No setlists found for artist '{0}'", clickedArtist.Name));
          }
          else
            OnStatusUpdated(string.Format("No setlists found for artist '{0}'", clickedArtist.Name));
        }
        catch (Exception ex)
        {
          OnStatusUpdated(string.Format("Fatal error while fetching setlists: {0}", ex.Message));
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
              vms.Add(new FetchedTrackViewModel(new ScrobbleBase(song.Name, clickedSetlist.Artist.Name), null));
            }
          }

          if (vms.Count > 0)
          {
            OnStatusUpdated("Successfully got tracks from setlist");
            Scrobbles = vms;
            CurrentView = _trackResultView;
          }
          else
            OnStatusUpdated("Setlist has no tracks");
        }
        catch (Exception ex)
        {
          OnStatusUpdated(string.Format("Fatal error while getting setlist tracks: {0}", ex.Message));
        }
        finally
        {
          EnableControls = true;
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
          OnStatusUpdated(string.Format("Error while scrobbling selected tracks: {0}", response.Status));
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while scrobbling: {0}", ex.Message));
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
      foreach (FetchedTrackViewModel vm in Scrobbles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.FetchedTrack.ArtistName, AlbumString, vm.FetchedTrack.TrackName, finishingTime));
        if (vm.FetchedTrack.Duration.HasValue)
          finishingTime = finishingTime.Subtract(vm.FetchedTrack.Duration.Value);
        else
          finishingTime = finishingTime.Subtract(TimeSpan.FromMinutes(3.0));
      }

      return scrobbles;
    }
  }
}