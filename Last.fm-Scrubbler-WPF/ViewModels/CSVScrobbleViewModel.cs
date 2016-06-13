using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	public enum CSVScrobbleMode
	{
		Normal,
		ImportMode
	}

	/// <summary>
	/// ViewModel for the <see cref="Views.CSVScrobbleView"/>.
	/// </summary>
	class CSVScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		public static string[] Formats = new string[] { "M/dd/yyyy h:mm" };

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

		public CSVScrobbleMode ScrobbleMode
		{
			get { return _scrobbleMode; }
			set
			{
				if (Scrobbles.Count > 0)
				{
					if (MessageBox.Show("Do you want to switch the Scrobble Mode? The CSV file will be parsed again!", "Change Scrobble Mode", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						_scrobbleMode = value;
						LoadCSVFile(CSVFilePath);
					}
				}
				else
					_scrobbleMode = value;

				NotifyOfPropertyChange(() => ScrobbleMode);
				NotifyOfPropertyChange(() => ShowImportModeSettings);
			}
		}
		private CSVScrobbleMode _scrobbleMode;

		public DateTime FinishingTime
		{
			get { return _finishingTime; }
			set
			{
				_finishingTime = value;
				NotifyOfPropertyChange(() => FinishingTime);
			}
		}
		private DateTime _finishingTime;

		public int Duration
		{
			get { return _duration; }
			set
			{
				_duration = value;
				NotifyOfPropertyChange(() => Duration);
			}
		}
		private int _duration;

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

		public bool ShowImportModeSettings
		{
			get { return ScrobbleMode == CSVScrobbleMode.ImportMode; }
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

		private Dispatcher _dispatcher;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CSVScrobbleViewModel()
		{
			Scrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();
			MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
			Duration = 1;
			FinishingTime = DateTime.Now;
			EnableControls = true;
			_dispatcher = Dispatcher.CurrentDispatcher;
		}

		private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => CanScrobble);
		}

		/// <summary>
		/// Loads a csv file and parses it to scrobbles.
		/// </summary>
		public void LoadCSVFileDialog()
		{

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "CSV Files|*.csv";
			if (ofd.ShowDialog() == DialogResult.OK)
				LoadCSVFile(ofd.FileName);
		}

		private async void LoadCSVFile(string path)
		{

			try
			{
				EnableControls = false;
				StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Reading CSV file..."));

				CSVFilePath = path;
				Scrobbles.Clear();

				TextFieldParser parser = new TextFieldParser(CSVFilePath);
				parser.HasFieldsEnclosedInQuotes = true;
				parser.SetDelimiters(",");

				string[] fields = new string[0];
				List<string> errors = new List<string>();

				await Task.Run(() =>
				{
					while (!parser.EndOfData)
					{
						try
						{
							// csv should be "Artist, Album, Track, Date"
							fields = parser.ReadFields();

							if (fields.Length != 4)
								throw new Exception("Parsed row has wrong number of fields!");

							DateTime date = DateTime.Now;
							string dateString = fields[3];

							// check for 'now playing'
							if (fields[3] == "" && ScrobbleMode == CSVScrobbleMode.Normal)
								continue;

							if (DateTime.TryParse(dateString, out date))
							{

							}
							else
							{
								bool parsed = false;
								// try different formats until succeeded
								foreach (string format in Formats)
								{
									parsed = DateTime.TryParseExact(dateString, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
									if (parsed)
										break;
								}

								if (!parsed && ScrobbleMode == CSVScrobbleMode.Normal)
									throw new Exception("Timestamp could not be parsed!");
							}

							CSVScrobble parsedScrobble = new CSVScrobble(fields[0], fields[1], fields[2], date.AddSeconds(1));
							ParsedCSVScrobbleViewModel vm = new ParsedCSVScrobbleViewModel(parsedScrobble, ScrobbleMode);
							vm.ToScrobbleChanged += ToScrobbleChanged;
							_dispatcher.Invoke(() => Scrobbles.Add(vm));
						}
						catch (Exception ex)
						{
							string errorString = "CSV line number: " + parser.LineNumber + ",";
							foreach(string s in fields)
							{
								errorString += s + ",";
							}

							errorString += ex.Message;
							errors.Add(errorString);
						}
					}
				});

				if (errors.Count == 0)
					StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully parsed CSV file. Parsed " + Scrobbles.Count + " rows"));
				else
				{
					StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Partially parsed CSV file. " + errors.Count + " rows could not be parsed"));
					if(MessageBox.Show("Some rows could not be parsed. Do you want to save a text file with the rows that could not be parsed?", "Error parsing rows", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Filter = "Text Files|*.txt";
						if(sfd.ShowDialog() == DialogResult.OK)
							File.WriteAllLines(sfd.FileName, errors.ToArray());
					}
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

			if (ScrobbleMode == CSVScrobbleMode.Normal)
			{
				foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
				{
					scrobbles.Add(new Scrobble(vm.ParsedScrobble.Artist, vm.ParsedScrobble.Album, vm.ParsedScrobble.Track, vm.ParsedScrobble.DateTime));
				}
			}
			else if (ScrobbleMode == CSVScrobbleMode.ImportMode)
			{
				DateTime time = FinishingTime;
				foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
				{
					scrobbles.Add(new Scrobble(vm.ParsedScrobble.Artist, vm.ParsedScrobble.Album, vm.ParsedScrobble.Track, time));
					time = time.Subtract(TimeSpan.FromSeconds(Duration));
				}
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
			foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
			{
				vm.ToScrobble = true;
			}
		}

		/// <summary>
		/// Marks all scrobbles as not "ToScrobble".
		/// </summary>
		public void SelectNone()
		{
			foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
			{
				vm.ToScrobble = false;
			}
		}
	}
}