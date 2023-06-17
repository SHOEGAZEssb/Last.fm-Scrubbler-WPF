using System;
using System.Threading.Tasks;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for ViewModels that need to use a "Starting" or "Finishing" time.
  /// </summary>
  public class ScrobbleTimeViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// Event that is fired when the <see cref="Time"/> changed.
    /// </summary>
    public event EventHandler TimeChanged;

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
        TimeChanged?.Invoke(this, EventArgs.Empty);
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
          Time = DateTime.Now;

        _useCurrentTime = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => Time);
        TimeChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _useCurrentTime;

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public ScrobbleTimeViewModel()
    {
      UseCurrentTime = true;
      UpdateCurrentTimeAsync().Forget();
    }

    #endregion Construction

    /// <summary>
    /// Gets if the selected <see cref="Time"/>
    /// is valid for a last.fm scrobble.
    /// </summary>
    /// <returns>True if time is "newer" than <see cref="MainViewModel.MinimumDateTime"/>,
    /// otherwise false.</returns>
    public bool IsTimeValid()
    {
      return Time >= MainViewModel.MinimumDateTime;
    }

    /// <summary>
    /// Task for notifying the UI that
    /// the <see cref="Time"/> has changed.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task UpdateCurrentTimeAsync()
    {
      while(true)
      {
        if(UseCurrentTime)
          NotifyOfPropertyChange(() => Time);

        await Task.Delay(1000);
      }
    }
  }
}