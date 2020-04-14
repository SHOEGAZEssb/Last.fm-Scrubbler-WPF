using Caliburn.Micro;
using Scrubbler.Properties;
using System;
using System.Windows;

namespace Scrubbler
{
  /// <summary>
  /// ViewModel for the <see cref="SystemTrayView"/>.
  /// </summary>
  class SystemTrayViewModel : Screen
  {
    #region Member

    /// <summary>
    /// WindowManager used to show the <see cref="_screenToShow"/>.
    /// </summary>
    private readonly IWindowManager _windowManager;

    /// <summary>
    /// The actual "main" screen that will be shown.
    /// </summary>
    private readonly IScreen _screenToShow;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to show the <paramref name="screenToShow"/>.</param>
    /// <param name="screenToShow">The actual "main" screen that will be shown.</param>
    public SystemTrayViewModel(IWindowManager windowManager, IScreen screenToShow)
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _screenToShow = screenToShow ?? throw new ArgumentNullException(nameof(screenToShow));
      _screenToShow.Deactivated += ScreenToShow_Deactivated;
    }

    #endregion Construction

    /// <summary>
    /// Shows the main application screen.
    /// </summary>
    public void ShowScreen()
    {
      if(!_screenToShow.IsActive)
        _windowManager.ShowWindow(_screenToShow);
    }

    /// <summary>
    /// Saves settings and exits the application.
    /// </summary>
    public void Exit()
    {
      if (_screenToShow is IDisposable screen)
        screen.Dispose();

      Settings.Default.Save();
      Application.Current.Shutdown();
    }

    /// <summary>
    /// Shows the main application screen,
    /// if the application shouldn't start minimized.
    /// </summary>
    protected override void OnActivate()
    {
      if (!Settings.Default.StartMinimized)
        ShowScreen();
    }

    /// <summary>
    /// Exits the application if the main application screen
    /// was closed and we do not minimize to tray.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">EventArgs.</param>
    private void ScreenToShow_Deactivated(object sender, DeactivationEventArgs e)
    {
      if (e.WasClosed && !Settings.Default.MinimizeToTray)
        Exit();
    }
  }
}