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
          Time = DateTime.Now;

        _useCurrentTime = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => Time);
      }
    }
    private bool _useCurrentTime;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public ScrobbleTimeViewModel()
    {
      UseCurrentTime = true;
      UpdateCurrentTimeAsync().Forget();
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