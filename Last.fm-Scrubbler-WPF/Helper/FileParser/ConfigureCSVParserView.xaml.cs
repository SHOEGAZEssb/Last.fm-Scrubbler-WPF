using System.Windows;
using System.Windows.Interop;

namespace Scrubbler.Helper.FileParser
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
    }

    /// <summary>
    /// Removes the 'X' button from the window.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      NativeMethods.RemoveXFromWindow(new WindowInteropHelper(this).Handle);
    }
  }
}