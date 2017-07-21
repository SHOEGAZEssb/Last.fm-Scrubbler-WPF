using System;
using System.Timers;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Base class for ViewModels that need to use a "Starting" or "Finishing" time.
  /// </summary>
  abstract class ScrobbleTimeViewModelBase : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// The selected time.
    /// </summary>
    public DateTime Time
    {
      get
      {
        if (UseCurrentTime)
          return DateTime.Now;
        else
          return _time;
      }
      set
      {
        _time = value;
        NotifyOfPropertyChange(() => Time);
      }
    }
    private DateTime _time;

    /// <summary>
    /// If <see cref="DateTime.Now"/> should be used
    /// for <see cref="Time"/>.
    /// </summary>
    public bool UseCurrentTime
    {
      get { return _useCurrentTime; }
      set
      {
        if (!value)
        {
          Time = DateTime.Now;
          _currentTimeTimer.Stop();
        }
        else
          _currentTimeTimer.Start();

        _useCurrentTime = value;
        NotifyOfPropertyChange(() => UseCurrentTime);
        NotifyOfPropertyChange(() => Time);
      }
    }
    private bool _useCurrentTime;

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Timer used to update the current time.
    /// </summary>
    private Timer _currentTimeTimer;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public ScrobbleTimeViewModelBase()
    {
      _currentTimeTimer = new Timer(1000);
      _currentTimeTimer.Elapsed += _currentTimeTimer_Elapsed;
      UseCurrentTime = true;
    }

    /// <summary>
    /// Notifies the UI that the current time has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _currentTimeTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      NotifyOfPropertyChange(() => Time);
    }
  }
}