using Last.fm_Scrubbler_WPF.ViewModels;
using System.Windows;

namespace Last.fm_Scrubbler_WPF.Views
{
  /// <summary>
  /// Interaction logic for ScrobblePreviewView.xaml
  /// </summary>
  public partial class ScrobblePreviewView : Window
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="vm">ViewModel to use as DataContext.</param>
    public ScrobblePreviewView(ScrobblePreviewViewModel vm)
    {
      InitializeComponent();
      DataContext = vm;
    }
  }
}