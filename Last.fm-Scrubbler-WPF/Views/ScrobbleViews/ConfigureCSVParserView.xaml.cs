using Last.fm_Scrubbler_WPF.ViewModels.SubViewModels;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Last.fm_Scrubbler_WPF.Views.ScrobbleViews
{
  /// <summary>
  /// Interaction logic for ConfigureCSVParserView.xaml
  /// </summary>
  public partial class ConfigureCSVParserView : Window
  {
    #region Remove 'X'

    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    #endregion Remove 'X'

    /// <summary>
    /// Constructor.
    /// </summary>
    public ConfigureCSVParserView()
    {
      InitializeComponent();
      DataContext = new ConfigureCSVParserViewModel();
    }

    /// <summary>
    /// Removes the 'X' button from the window.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var hwnd = new WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
  }
}