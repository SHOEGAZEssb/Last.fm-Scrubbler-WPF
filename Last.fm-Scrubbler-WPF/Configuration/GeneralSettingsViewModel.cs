using Octokit;
using Scrubbler.Helper;
using Scrubbler.Properties;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Scrubbler.Configuration
{
  /// <summary>
  /// ViewModel for the <see cref="GeneralSettingsView"/>.
  /// </summary>
  public class GeneralSettingsViewModel : ViewModelBase
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

    /// <summary>
    /// If true, the application will call
    /// <see cref="CheckForUpdates"/> on startup.
    /// </summary>
    public bool StartupUpdateCheck
    {
      get { return _startupUpdateCheck; }
      set
      {
        _startupUpdateCheck = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _startupUpdateCheck;

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// GitHub client to check for updates.
    /// </summary>
    private readonly IGitHubClient _gitHubClient;

    /// <summary>
    /// ProcessManager for working with processor functions.
    /// </summary>
    private readonly IProcessManager _processManager;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="gitHubClient">GitHub client to check for updates.</param>
    /// <param name="processManager">ProcessManager for working with processor functions.</param>
    public GeneralSettingsViewModel(IExtendedWindowManager windowManager, IGitHubClient gitHubClient, IProcessManager processManager)
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(windowManager));
      _processManager = processManager ?? throw new ArgumentNullException(nameof(windowManager));
      MinimizeToTray = Settings.Default.MinimizeToTray;
      StartMinimized = Settings.Default.StartMinimized;
      StartupUpdateCheck = Settings.Default.StartupUpdateCheck;

      if(Settings.Default.StartupUpdateCheck)
        CheckForUpdates(false).Forget();
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
      Settings.Default.StartupUpdateCheck = StartupUpdateCheck;
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
      StartupUpdateCheck = bool.Parse(Settings.Default.Properties[nameof(StartupUpdateCheck)].DefaultValue.ToString());
    }

    /// <summary>
    /// Checks for updates of the application.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task CheckForUpdates(bool manualCheck)
    {
      try
      {
        EnableControls = false;
        var releases = await _gitHubClient.Repository.Release.GetAll("coczero", "Last.fm-Scrubbler-WPF");
        var newestRelease = releases[0];

        Version newestVersion;
        if (newestRelease.Prerelease)
          newestVersion = new Version(newestRelease.TagName.Replace("B", ""));
        else
          newestVersion = new Version(newestRelease.TagName);

        if (newestVersion > Assembly.GetExecutingAssembly().GetName().Version)
          _windowManager.ShowDialog(new NewVersionViewModel(newestRelease, _processManager));
        else if (manualCheck)
          _windowManager.MessageBoxService.ShowDialog("No new version available", "Update Check", IMessageBoxServiceButtons.OK);
      }
      catch(Exception ex)
      {
        _windowManager.MessageBoxService.ShowDialog($"Fatal error while checking for update: {ex.Message}", "Update Check Error", IMessageBoxServiceButtons.OK, IMessageBoxServiceIcon.Error);
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}