using Octokit;
using Scrubbler.Interfaces;
using Scrubbler.Properties;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.GeneralSettingsView"/>.
  /// </summary>
  class GeneralSettingsViewModel : ViewModelBase
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
    private IExtendedWindowManager _windowManager;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public GeneralSettingsViewModel(IExtendedWindowManager windowManager)
    {
      _windowManager = windowManager;
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

        var client = new GitHubClient(new ProductHeaderValue("Last.fm-Scrubbler-WPF"));
        var releases = await client.Repository.Release.GetAll("coczero", "Last.fm-Scrubbler-WPF");
        var newest = releases[0];

        Version newestVersion;
        if (newest.Prerelease)
          newestVersion = new Version(newest.TagName.Replace("B", ""));
        else
          newestVersion = new Version(newest.TagName);

        if (newestVersion > Assembly.GetExecutingAssembly().GetName().Version)
          _windowManager.ShowDialog(new NewVersionViewModel(newest));
        else if (manualCheck)
          _windowManager.MessageBoxService.ShowDialog("No new version available", "Update Check", IMessageBoxServiceButtons.OK);
      }
      catch(Exception ex)
      {
        _windowManager.MessageBoxService.ShowDialog(string.Format("Fatal error while checking for update: {0}", ex.Message, "Update Check",
                                                    IMessageBoxServiceButtons.OK));
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}