using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scrubbler.ExtraFunctions
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
  /// ViewModel for the <see cref="MilestoneCheckerView"/>
  /// </summary>
  public class MilestoneCheckerViewModel : TabViewModel
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
    /// If true, "import" timestamps (e.g. 1970...)
    /// are ignored.
    /// </summary>
    public bool IgnoreBrokenTimestamps
    {
      get { return _ignoreBrokenTimestamps; }
      set
      {
        _ignoreBrokenTimestamps = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _ignoreBrokenTimestamps;

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

    /// <summary>
    /// Command for creating the scrobble data.
    /// </summary>
    public ICommand CreateScrobbleDataCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Last.fm user api object with which to get user data.
    /// </summary>
    private readonly IUserApi _userAPI;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userAPI">Last.fm user api object with which to get user data.</param>
    public MilestoneCheckerViewModel(IUserApi userAPI)
      : base("Milestone Checker")
    {
      _userAPI = userAPI ?? throw new ArgumentNullException(nameof(userAPI));
      CreateScrobbleDataCommand = new DelegateCommand((o) => CreateScrobbleData().Forget());
    }

    /// <summary>
    /// Gets the defined milestones from the <see cref="ScrobbleData"/>.
    /// </summary>
    public void CreateMilestones()
    {
      try
      {
        EnableControls = false;

        List<LastTrack> tracksToProcess = ScrobbleData;
        if (IgnoreBrokenTimestamps)
          tracksToProcess = ScrobbleData.Where(i => i.TimePlayed.Value.Year != 1970).ToList();

        if (SelectedMilestoneType == MilestoneType.SpecificTrack)
          Milestones = new ObservableCollection<MilestoneViewModel>() { new MilestoneViewModel(tracksToProcess[Number - 1], Number) };
        else
        {
          var milestones = new List<MilestoneViewModel>();
          for(int i = 0; i < tracksToProcess.Count; i++)
          {
            if((i + 1) % Number == 0)
              milestones.Add(new MilestoneViewModel(tracksToProcess[i], i + 1));
          }

          Milestones = new ObservableCollection<MilestoneViewModel>(milestones);
        }

        OnStatusUpdated("Successfully got milestones");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while getting milestones: {ex.Message}");
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
    public async Task CreateScrobbleData()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated($"Getting user info of '{Username}'...");
        var response = await _userAPI.GetInfoAsync(Username);
        if (response.Success && response.Status == LastResponseStatus.Successful)
        {
          ScrobbleData = null;
          Milestones = null;
          int pagesToFetch = (int)Math.Ceiling(response.Content.Playcount / 1000.0);
          List<LastTrack> tracks = new List<LastTrack>();
          for (int i = 1; i <= pagesToFetch; i++)
          {
            OnStatusUpdated($"Getting scrobble data of '{Username}'... ({i} / {pagesToFetch}) pages");
            var pageResponse = await _userAPI.GetRecentScrobbles(Username, null, i, 1000);
            if (pageResponse.Success && pageResponse.Status == LastResponseStatus.Successful)
              tracks.AddRange(pageResponse.Content.Where(c => !c.IsNowPlaying.HasValue || !c.IsNowPlaying.Value).Reverse().ToList());
            else
            {
              OnStatusUpdated($"Error getting scrobble data of '{Username}': {pageResponse.Status}");
              return;
            }
          }

          ScrobbleData = tracks;
          OnStatusUpdated($"Successfully got scrobble data of '{Username}'");
        }
        else
          OnStatusUpdated($"Error getting user info: {response.Status}");
      }
      catch(Exception ex)
      {
        OnStatusUpdated($"Fatal error while getting scrobble data of '{Username}': {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}