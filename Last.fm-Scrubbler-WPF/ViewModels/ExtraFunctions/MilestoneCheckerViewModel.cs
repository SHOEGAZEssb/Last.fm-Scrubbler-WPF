using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ExtraFunctions
{
  /// <summary>
  /// Milestone type to fetch.
  /// </summary>
  public enum MilestoneType
  {
    /// <summary>
    /// Show every nth track.
    /// </summary>
    [Description("Every Nth Track")]
    EveryNthTrack,

    /// <summary>
    /// Show one specific track.
    /// </summary>
    [Description("Specific Track")]
    SpecificTrack
  }

  /// <summary>
  /// ViewModel for the <see cref="Views.ExtraFunctions.MilestoneCheckerView"/>
  /// </summary>
  public class MilestoneCheckerViewModel : ExtraFunctionViewModelBase
  {
    #region Properties

    /// <summary>
    /// User whose milestones to get.
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
    /// The selected <see cref="MilestoneType"/>.
    /// </summary>
    public MilestoneType SelectedMilestoneType
    {
      get { return _selectedMilestoneType; }
      set
      {
        _selectedMilestoneType = value;
        NotifyOfPropertyChange();
      }
    }
    private MilestoneType _selectedMilestoneType;

    /// <summary>
    /// The number used for milestone calculation.
    /// </summary>
    public int Number
    {
      get { return _number; }
      set
      {
        _number = value;
        NotifyOfPropertyChange();
      }
    }
    private int _number;

    /// <summary>
    /// The scrobble data of the <see cref="Username"/>.
    /// </summary>
    public List<LastTrack> ScrobbleData
    {
      get { return _scrobbleData; }
      private set
      {
        _scrobbleData = value;
        NotifyOfPropertyChange();
      }
    }
    private List<LastTrack> _scrobbleData;

    /// <summary>
    /// The calculated milestones.
    /// </summary>
    public ObservableCollection<MilestoneViewModel> Milestones
    {
      get { return _milestones; }
      private set
      {
        _milestones = value;
        NotifyOfPropertyChange();
      }
    }
    private ObservableCollection<MilestoneViewModel> _milestones;

    #endregion Properties

    #region Member

    /// <summary>
    /// Last.fm user api object with which to get user data.
    /// </summary>
    private IUserApi _userAPI;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userAPI">Last.fm user api object with which to get user data.</param>
    public MilestoneCheckerViewModel(IUserApi userAPI)
      : base("Milestone Checker")
    {
      _userAPI = userAPI;
    }

    /// <summary>
    /// Gets the defined milestones from the <see cref="ScrobbleData"/>.
    /// </summary>
    public void GetMilestones()
    {
      try
      {
        EnableControls = false;

        List<LastTrack> tracksToProcess = new List<LastTrack>();
        if (SelectedMilestoneType == MilestoneType.SpecificTrack)
          Milestones = new ObservableCollection<MilestoneViewModel>() { new MilestoneViewModel(ScrobbleData[Number - 1], Number) };
        else
        {
          List<MilestoneViewModel> milestones = new List<MilestoneViewModel>();
          for(int i = 0; i < ScrobbleData.Count; i++)
          {
            if((i + 1) % Number == 0)
              milestones.Add(new MilestoneViewModel(_scrobbleData[i], i + 1));
          }

          Milestones = new ObservableCollection<MilestoneViewModel>(milestones);
        }

        OnStatusUpdated("Successfully got milestones");
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while getting milestones: {0}", ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Gets the scrobble data from the <see cref="Username"/>.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task GetScrobbleData()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated(string.Format("Getting user info of '{0}'...", Username));
        var response = await _userAPI.GetInfoAsync(Username);
        if (response.Success && response.Status == LastResponseStatus.Successful)
        {
          ScrobbleData = null;
          Milestones = null;
          int pagesToFetch = (int)Math.Ceiling(response.Content.Playcount / 1000.0);
          List<LastTrack> tracks = new List<LastTrack>();
          for (int i = 1; i <= pagesToFetch; i++)
          {
            OnStatusUpdated(string.Format("Getting scrobble data of '{0}'... ({1} / {2}) pages", Username, i, pagesToFetch));
            var pageResponse = await _userAPI.GetRecentScrobbles(Username, null, i, 1000);
            if (pageResponse.Success)
              tracks.AddRange(pageResponse.Content.Where(c => !c.IsNowPlaying.HasValue || !c.IsNowPlaying.Value).Reverse().ToList());
            else
            {
              OnStatusUpdated(string.Format("Error getting scrobble data of '{0}': {1}", Username, pageResponse.Status));
              return;
            }
          }

          ScrobbleData = tracks;
          OnStatusUpdated(string.Format("Successfully got scrobble data of '{0}'", Username));
        }
        else
          OnStatusUpdated(string.Format("Error getting user info: {0}", response.Status));
      }
      catch(Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while getting scrobble data of '{0}': {1}", Username, ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}