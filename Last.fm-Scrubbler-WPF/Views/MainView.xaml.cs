using Last.fm_Scrubbler_WPF.ViewModels;
using System.Windows;

namespace Last.fm_Scrubbler_WPF.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}
	}
}