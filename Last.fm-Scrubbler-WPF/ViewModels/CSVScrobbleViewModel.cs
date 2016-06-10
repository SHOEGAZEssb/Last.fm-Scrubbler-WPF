using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	/// <summary>
	/// ViewModel for the <see cref="Views.CSVScrobbleView"/>.
	/// </summary>
	class CSVScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		/// <summary>
		/// Event that triggers when the status should be changed.
		/// </summary>
		public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

		/// <summary>
		/// The path to the csv file.
		/// </summary>
		public string CSVFilePath
		{
			get { return _csvFilePath; }
			private set
			{
				_csvFilePath = value;
				NotifyOfPropertyChange(() => CSVFilePath);
			}
		}
		private string _csvFilePath;

		/// <summary>
		/// The parsed scrobbles from the csv file.
		/// </summary>
		public ObservableCollection<ParsedCSVScrobbleViewModel> Scrobbles
		{
			get { return _scrobbles; }
			private set
			{
				_scrobbles = value;
				NotifyOfPropertyChange(() => Scrobbles);
			}
		}
		private ObservableCollection<ParsedCSVScrobbleViewModel> _scrobbles;

		public bool CanScrobble
		{
			get { return MainViewModel.Client.Auth.Authenticated && Scrobbles.Any(i => i.ToScrobble) && EnableControls; }
		}

		/// <summary>
		/// Gets if the "Select All" button is enabled.
		/// </summary>
		public bool CanSelectAll
		{
			get { return !Scrobbles.All(i => i.ToScrobble) && EnableControls; }
		}

		/// <summary>
		/// Gets if the "Select None" button is enabled.
		/// </summary>
		public bool CanSelectNone
		{
			get { return Scrobbles.Any(i => i.ToScrobble) && EnableControls; }
		}

		/// <summary>
		/// Gets/sets if certain controls on the UI should be enabled.
		/// </summary>
		public bool EnableControls
		{
			get { return _enableControls; }
			private set
			{
				_enableControls = value;
				NotifyOfPropertyChange(() => EnableControls);
				NotifyOfPropertyChange(() => CanScrobble);
				NotifyOfPropertyChange(() => CanSelectAll);
				NotifyOfPropertyChange(() => CanSelectNone);
			}
		}
		private bool _enableControls;

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		public CSVScrobbleViewModel()
		{
			Scrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();
			MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
		}

		private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => CanScrobble);
		}

		/// <summary>
		/// Loads a csv file and parses it to scrobbles.
		/// </summary>
		public void LoadCSVFile()
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "CSV Files|*.csv";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					EnableControls = false;
					StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Reading CSV file..."));

					CSVFilePath = ofd.FileName;
					Scrobbles.Clear();
					string[] fileContents = File.ReadAllLines(CSVFilePath);

					foreach (string row in fileContents)
					{
						// csv should be "Artist, Album, Track, Date"
						string[] scrobble = row.Split(',');

						// check for 'now playing'
						if (scrobble[3] == "")
							continue;

						CSVScrobble parsedScrobble = new CSVScrobble(scrobble[0], scrobble[1], scrobble[2], DateTime.Parse(scrobble[3]).ToLocalTime());
						ParsedCSVScrobbleViewModel vm = new ParsedCSVScrobbleViewModel(parsedScrobble);
						vm.ToScrobbleChanged += ToScrobbleChanged;
						Scrobbles.Add(vm);
					}

					StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully parsed CSV file"));
				}
			}
			catch (Exception ex)
			{
				Scrobbles.Clear();
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error parsing CSV file: " + ex.Message));
			}
			finally
			{
				EnableControls = true;
			}
		}

		private void ToScrobbleChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => CanScrobble);
			NotifyOfPropertyChange(() => CanSelectAll);
			NotifyOfPropertyChange(() => CanSelectNone);
		}

		/// <summary>
		/// Scrobbles the selected scrobbles.
		/// </summary>
		/// <returns>Task.</returns>
		public async Task Scrobble()
		{
			EnableControls = false;
			StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble selected tracks"));
			List<Scrobble> scrobbles = new List<Scrobble>();
			foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
			{
				scrobbles.Add(new Scrobble(vm.ParsedScrobble.Artist, vm.ParsedScrobble.Album, vm.ParsedScrobble.Track, vm.ParsedScrobble.DateTime));
			}

			var response = await MainViewModel.Scrobbler.ScrobbleAsync(scrobbles);
			if (response.Success)
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
			else
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));

			EnableControls = true;
		}

		/// <summary>
		/// Marks all scrobbles as "ToScrobble".
		/// </summary>
		public void SelectAll()
		{
			foreach (var vm in Scrobbles)
			{
				vm.ToScrobble = true;
			}
		}

		/// <summary>
		/// Marks all scrobbles as not "ToScrobble".
		/// </summary>
		public void SelectNone()
		{
			foreach (var vm in Scrobbles)
			{
				vm.ToScrobble = false;
			}
		}
	}
}