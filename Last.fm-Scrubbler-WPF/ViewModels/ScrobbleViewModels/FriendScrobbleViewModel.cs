using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ScrobbleViews.FriendScrobbleView"/>.
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

    #endregion Properties

    #region Member

    /// <summary>
    /// The last.fm api object to get the scrobbles of an user.
    /// </summary>
    private IUserApi _userApi;

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
    }

    /// <summary>
    /// Fetches the recent scrobbles of the user with the given <see cref="Username"/>.
    /// </summary>
    public async Task FetchScrobbles()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated(string.Format("Trying to fetch scrobbles of '{0}' ...", Username));
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

          OnStatusUpdated(string.Format("Successfully fetched scrobbles of '{0}'", Username));
        }
        else
          OnStatusUpdated(string.Format("Failed to fetch scrobbles of '{0}': {1}", Username, response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while fetching scrobbles of '{0}': {1}", Username, ex.Message));
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
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
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
        scrobbles.Add(new Scrobble(vm.Track.ArtistName, vm.Track.AlbumName, vm.Track.Name, vm.Track.TimePlayed.Value.LocalDateTime.AddSeconds(1)) { Duration = vm.Track.Duration });
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
          OnStatusUpdated(string.Format("Error while scrobbling selected tracks: {0}", response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while trying to scrobble selected tracks: {0}", ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Marks all fetched scrobbles as "ToScrobble".
    /// </summary>
    public override void SelectAll()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all fetched scrobbles as not "ToScrobble".
    /// </summary>
    public override void SelectNone()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = false;
      }
    }
  }
}