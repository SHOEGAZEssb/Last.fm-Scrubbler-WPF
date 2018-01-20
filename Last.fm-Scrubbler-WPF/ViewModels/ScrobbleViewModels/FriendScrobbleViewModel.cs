using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="FriendScrobbleView"/>.
  /// </summary>
  public class FriendScrobbleViewModel : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// The username of the user to fetch scrobbles from.
    /// </summary>
    public string Username
    {
      get { return _username; }
      set
      {
        _username = value;
        NotifyOfPropertyChange(() => Username);
        NotifyOfPropertyChange(() => CanFetch);
      }
    }
    private string _username;

    /// <summary>
    /// The amount of scrobbles to be fetched.
    /// </summary>
    public int Amount
    {
      get { return _amount; }
      set
      {
        _amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }
    private int _amount;

    /// <summary>
    /// List of fetched scrobbles.
    /// </summary>
    public ObservableCollection<FetchedFriendTrackViewModel> FetchedScrobbles
    {
      get { return _fetchedScrobbles; }
      private set
      {
        _fetchedScrobbles = value;
        NotifyOfPropertyChange(() => FetchedScrobbles);
      }
    }
    private ObservableCollection<FetchedFriendTrackViewModel> _fetchedScrobbles;

    /// <summary>
    /// Gets/sets if certain controls on the UI should be enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanPreview);
        NotifyOfPropertyChange(() => CanFetch);
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
      }
    }

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && FetchedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return FetchedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the fetch button is enabled.
    /// </summary>
    public bool CanFetch
    {
      get { return Username.Length > 0 && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select All" button is enabled.
    /// </summary>
    public bool CanSelectAll
    {
      get { return !FetchedScrobbles.All(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select None" button is enabled.
    /// </summary>
    public bool CanSelectNone
    {
      get { return FetchedScrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public FriendScrobbleViewModel()
    {
      Username = "";
      FetchedScrobbles = new ObservableCollection<FetchedFriendTrackViewModel>();
      Amount = 20;
    }

    /// <summary>
    /// Fetches the recent scrobbles of the user with the given <see cref="Username"/>.
    /// </summary>
    public async void FetchScrobbles()
    {
      EnableControls = false;
      OnStatusUpdated("Trying to fetch scrobbles of user " + Username + "...");
      FetchedScrobbles.Clear();
      var response = await MainViewModel.Client.User.GetRecentScrobbles(Username, null, 1, Amount);
      if (response.Success)
      {
        OnStatusUpdated("Successfully fetched scrobbles of user " + Username);
        foreach (var s in response)
        {
          if (!s.IsNowPlaying.HasValue || !s.IsNowPlaying.Value)
          {
            FetchedFriendTrackViewModel vm = new FetchedFriendTrackViewModel(s);
            vm.ToScrobbleChanged += ToScrobbleChanged;
            FetchedScrobbles.Add(vm);
          }
        }
      }
      else
        OnStatusUpdated("Failed to fetch scrobbles of user " + Username);

      EnableControls = true;
    }

    /// <summary>
    /// Signals that the "ToScrobble" property of a scrobble has changed.
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
    /// Creates a list with scrobbles that will be scrobbled.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    private List<Scrobble> CreateScrobbles()
    {
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (var vm in FetchedScrobbles.Where(i => i.ToScrobble))
      {
        scrobbles.Add(new Scrobble(vm.Track.ArtistName, vm.Track.AlbumName, vm.Track.Name, vm.Track.TimePlayed.Value.LocalDateTime.AddSeconds(1)) { Duration = vm.Track.Duration });
      }

      return scrobbles;
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    public override async Task Scrobble()
    {
      EnableControls = false;

      try
      {
        OnStatusUpdated("Trying to scrobble selected tracks");

        var response = await MainViewModel.Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success)
          OnStatusUpdated("Successfully scrobbled!");
        else
          OnStatusUpdated("Error while scrobbling!");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while trying to scrobble selected tracks. Error: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Previews the tracks that will be scrobbled.
    /// </summary>
    public override void Preview()
    {
      ScrobblePreviewView spv = new ScrobblePreviewView(new ScrobblePreviewViewModel(CreateScrobbles()));
      spv.ShowDialog();
    }

    /// <summary>
    /// Marks all fetched scrobbles as "ToScrobble".
    /// </summary>
    public void SelectAll()
    {
      foreach (var vm in FetchedScrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all fetched scrobbles as not "ToScrobble".
    /// </summary>
    public void SelectNone()
    {
      foreach (var vm in FetchedScrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = false;
      }
    }
  }
}