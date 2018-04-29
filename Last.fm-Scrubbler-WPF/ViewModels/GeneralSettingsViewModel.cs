using Caliburn.Micro;
using Scrubbler.Properties;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.GeneralSettingsView"/>.
  /// </summary>
  class GeneralSettingsViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// If true, the application is minimized to tray
    /// when the main application screen is closed.
    /// </summary>
    public bool MinimizeToTray
    {
      get { return _minimizeToTray; }
      set
      {
        _minimizeToTray = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _minimizeToTray;

    /// <summary>
    /// If true, the application will start minimized,
    /// so no application screen will be shown, only
    /// a tray icon.
    /// </summary>
    public bool StartMinimized
    {
      get { return _startMinimized; }
      set
      {
        _startMinimized = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _startMinimized;

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public GeneralSettingsViewModel()
    {
      MinimizeToTray = Settings.Default.MinimizeToTray;
      StartMinimized = Settings.Default.StartMinimized;
    }

    #endregion Construction

    /// <summary>
    /// Sets the settings and closes
    /// the screen.
    /// </summary>
    public void OK()
    {
      Settings.Default.MinimizeToTray = MinimizeToTray;
      Settings.Default.StartMinimized = StartMinimized;
      TryClose(true);
    }

    /// <summary>
    /// Closes the screen.
    /// </summary>
    public void Cancel()
    {
      TryClose(false);
    }

    /// <summary>
    /// Restores default values.
    /// </summary>
    public void LoadDefaults()
    {
      MinimizeToTray = bool.Parse(Settings.Default.Properties[nameof(MinimizeToTray)].DefaultValue.ToString());
      StartMinimized = bool.Parse(Settings.Default.Properties[nameof(StartMinimized)].DefaultValue.ToString());
    }
  }
}