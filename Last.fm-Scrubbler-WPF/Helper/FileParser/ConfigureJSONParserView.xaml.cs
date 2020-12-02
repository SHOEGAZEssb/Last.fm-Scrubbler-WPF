using System.Windows;
using System.Windows.Interop;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Interaction logic for ConfigureJSONParserView.xaml
  /// </summary>
  public partial class ConfigureJSONParserView : Window
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public ConfigureJSONParserView()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      NativeMethods.RemoveXFromWindow(new WindowInteropHelper(this).Handle);
    }
  }
}
