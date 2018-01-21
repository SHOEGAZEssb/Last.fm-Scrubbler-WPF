using Last.fm_Scrubbler_WPF.ViewModels.SubViewModels;
using System.Windows;
using System.Windows.Interop;

namespace Last.fm_Scrubbler_WPF.Views.ScrobbleViews
{
  /// <summary>
  /// Interaction logic for ConfigureCSVParserView.xaml
  /// </summary>
  public partial class ConfigureCSVParserView : Window
  {
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
      NativeMethods.RemoveXFromWindow(hwnd);
    }
  }
}