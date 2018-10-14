using System;
using System.Timers;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for ViewModels that need to use a "Starting" or "Finishing" time.
  /// </summary>
  public class ScrobbleTimeViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// The selected time.
    /// </summary>
    public DateTime Time
    {
      get => UseCurrentTime ? DateTime.Now : _time;
      set
      {
        _time = value;
        NotifyOfPropertyChange();
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
          _currentTimeTimer.Stop();
          Time = DateTime.Now;
        }
        else
          _currentTimeTimer.Start();

        _useCurrentTime = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => Time);
      }
    }
    private bool _useCurrentTime;

    #endregion Properties

    #region Member

    /// <summary>
    /// Timer used to update the current time.
    /// </summary>
    private Timer _currentTimeTimer;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public ScrobbleTimeViewModel()
    {
      _currentTimeTimer = new Timer(1000);
      _currentTimeTimer.Elapsed += _currentTimeTimer_Elapsed;
      UseCurrentTime = true;
    }

    /// <summary>
    /// Notifies the UI that the current time has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void _currentTimeTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      NotifyOfPropertyChange(() => Time);
    }
  }
}