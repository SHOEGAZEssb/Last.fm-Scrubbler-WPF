using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
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
  /// ViewModel for the <see cref="FriendScrobbleView"/>.
  /// </summary>
  public class FriendScrobbleViewModel : ScrobbleMultipleViewModelBase<FetchedFriendTrackViewModel>
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
      }
    }
    private int _amount;

    /// <summary>
    /// Command for fetching the scrobbles.
    /// </summary>
    public ICommand FetchCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// The last.fm api object to get the scrobbles of an user.
    /// </summary>
    private readonly IUserApi _userApi;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userApi">The last.fm api object to get the scrobbles of an user.</param>
    public FriendScrobbleViewModel(IExtendedWindowManager windowManager, IUserApi userApi)
      : base(windowManager, "Friend Scrobbler")
    {
      _userApi = userApi;
      Scrobbles = new ObservableCollection<FetchedFriendTrackViewModel>();
      Amount = 20;
      FetchCommand = new DelegateCommand((o) => FetchScrobbles().Forget());
    }

    /// <summary>
    /// Fetches the recent scrobbles of the user with the given <see cref="Username"/>.
    /// </summary>
    public async Task FetchScrobbles()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated($"Trying to fetch scrobbles of '{Username}'...");
        Scrobbles.Clear();
        var response = await _userApi.GetRecentScrobbles(Username, null, 1, Amount);
        if (response.Success)
        {
          foreach (var s in response)
          {
            if (!s.IsNowPlaying.HasValue || !s.IsNowPlaying.Value)
            {
              FetchedFriendTrackViewModel vm = new FetchedFriendTrackViewModel(s);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              Scrobbles.Add(vm);
            }
          }

          OnStatusUpdated($"Successfully fetched scrobbles of '{Username}'");
        }
        else
          OnStatusUpdated($"Failed to fetch scrobbles of '{Username}': {response.Status}");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while fetching scrobbles of '{Username}': {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
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
      NotifyOfPropertyChange(() => CanCheckAll);
      NotifyOfPropertyChange(() => CanUncheckAll);
    }

    /// <summary>
    /// Creates a list with scrobbles that will be scrobbled.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
      {
        scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, vm.TimePlayed.AddSeconds(1)) { Duration = vm.Duration });
      }

      return scrobbles;
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
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
    /// Marks all fetched scrobbles as "ToScrobble".
    /// </summary>
    public override void CheckAll()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all fetched scrobbles as not "ToScrobble".
    /// </summary>
    public override void UncheckAll()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = false;
      }
    }
  }
}