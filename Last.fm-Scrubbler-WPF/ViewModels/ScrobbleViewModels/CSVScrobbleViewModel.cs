using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.Models;
using Last.fm_Scrubbler_WPF.Properties;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using Last.fm_Scrubbler_WPF.Views.ScrobbleViews;
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
  /// <summary>
  /// Scrobble mode.
  /// </summary>
  public enum CSVScrobbleMode
  {
    /// <summary>
    /// Scrobble the tracks based on the parsed timestamp.
    /// </summary>
    Normal,

    /// <summary>
    /// Set the timestamp by setting <see cref="ScrobbleTimeViewModelBase.Time"/>.
    /// </summary>
    ImportMode
  }

  /// <summary>
  /// ViewModel for the <see cref="Views.CSVScrobbleView"/>.
  /// </summary>
  public class CSVScrobbleViewModel : ScrobbleTimeViewModelBase
  {
    #region Properties

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
        NotifyOfPropertyChange(() => CanParse);
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

    /// <summary>
    /// The selected <see cref="CSVScrobbleMode"/>.
    /// </summary>
    public CSVScrobbleMode ScrobbleMode
    {
      get { return _scrobbleMode; }
      set
      {
        _scrobbleMode = value;

        if (Scrobbles.Count > 0)
        {
          if (MessageBox.Show("Do you want to switch the Scrobble Mode? The CSV file will be parsed again!", "Change Scrobble Mode", MessageBoxButtons.YesNo) == DialogResult.Yes)
            ParseCSVFile().Forget();
        }

        NotifyOfPropertyChange(() => ScrobbleMode);
        NotifyOfPropertyChange(() => ShowImportModeSettings);
      }
    }
    private CSVScrobbleMode _scrobbleMode;

    /// <summary>
    /// Duration between scrobbles in seconds.
    /// </summary>
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

    /// <summary>
    /// Gets if the Scrobble button is enabled on the UI.
    /// </summary>
    public override bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && Scrobbles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return Scrobbles.Any(i => i.ToScrobble) && EnableControls; }
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
    /// Gets if the "Parse" button is enabled.
    /// </summary>
    public bool CanParse
    {
      get { return CSVFilePath != null && CSVFilePath != string.Empty; }
    }

    /// <summary>
    /// Gets if the import mode settings should be visible on the UI.
    /// </summary>
    public bool ShowImportModeSettings
    {
      get { return ScrobbleMode == CSVScrobbleMode.ImportMode; }
    }

    /// <summary>
    /// Gets/sets if certain controls on the UI should be enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyCanProperties();
      }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Different formats to try in case TryParse fails.
    /// </summary>
    private static string[] _formats = new string[] { "M/dd/yyyy h:mm" };

    /// <summary>
    /// Dispatcher used to invoke from another thread.
    /// </summary>
    private Dispatcher _dispatcher;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public CSVScrobbleViewModel(IWindowManager windowManager, IAuthScrobbler scrobbler)
      : base(windowManager, scrobbler)
    {
      Scrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();
      Duration = 1;
      UseCurrentTime = true;
      _dispatcher = Dispatcher.CurrentDispatcher;
      ScrobbleMode = CSVScrobbleMode.ImportMode;
    }

    /// <summary>
    /// Shows a dialog to open a csv file.
    /// </summary>
    public void LoadCSVFileDialog()
    {
      using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV Files|*.csv" })
      {
        if (ofd.ShowDialog() == DialogResult.OK)
          CSVFilePath = ofd.FileName;
      }
    }

    /// <summary>
    /// Loads and parses a csv file.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task ParseCSVFile()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Reading CSV file...");
        Scrobbles.Clear();

        TextFieldParser parser = new TextFieldParser(CSVFilePath)
        {
          HasFieldsEnclosedInQuotes = true
        };
        parser.SetDelimiters(Settings.Default.CSVDelimiters.Select(x => new string(x, 1)).ToArray());

        string[] fields = new string[0];
        List<string> errors = new List<string>();

        await Task.Run(() =>
        {
          while (!parser.EndOfData)
          {
            try
            {
              fields = parser.ReadFields();

              DateTime date = DateTime.Now;
              string dateString = fields[Settings.Default.TimestampFieldIndex];

              // check for 'now playing'
              if (dateString == "" && ScrobbleMode == CSVScrobbleMode.Normal)
                continue;

              if (!DateTime.TryParse(dateString, out date))
              {
                bool parsed = false;
                // try different formats until succeeded
                foreach (string format in _formats)
                {
                  parsed = DateTime.TryParseExact(dateString, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
                  if (parsed)
                    break;
                }

                if (!parsed && ScrobbleMode == CSVScrobbleMode.Normal)
                  throw new Exception("Timestamp could not be parsed!");
              }

              // try to get optional parameters first
              string album = fields.ElementAtOrDefault(Settings.Default.AlbumFieldIndex);
              string albumArtist = fields.ElementAtOrDefault(Settings.Default.AlbumArtistFieldIndex);
              string duration = fields.ElementAtOrDefault(Settings.Default.DurationFieldIndex);
              TimeSpan time = TimeSpan.FromSeconds(0);

              if (!string.IsNullOrEmpty(duration))
              {
                try
                {
                  time = TimeSpan.Parse(duration);
                }
                catch
                {
                  // swallow
                }
              }

              DatedScrobble parsedScrobble = new DatedScrobble(date.AddSeconds(1), fields[Settings.Default.TrackFieldIndex],
                                                              fields[Settings.Default.ArtistFieldIndex], album,
                                                              albumArtist, time);
              ParsedCSVScrobbleViewModel vm = new ParsedCSVScrobbleViewModel(parsedScrobble, ScrobbleMode);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              _dispatcher.Invoke(() => Scrobbles.Add(vm));
            }
            catch (Exception ex)
            {
              string errorString = "CSV line number: " + parser.LineNumber + ",";
              foreach (string s in fields)
              {
                errorString += s + ",";
              }

              errorString += ex.Message;
              errors.Add(errorString);
            }
          }
        });

        if (errors.Count == 0)
          OnStatusUpdated("Successfully parsed CSV file. Parsed " + Scrobbles.Count + " rows");
        else
        {
          OnStatusUpdated("Partially parsed CSV file. " + errors.Count + " rows could not be parsed");
          if (MessageBox.Show("Some rows could not be parsed. Do you want to save a text file with the rows that could not be parsed?", "Error parsing rows", MessageBoxButtons.YesNo) == DialogResult.Yes)
          {
            SaveFileDialog sfd = new SaveFileDialog()
            {
              Filter = "Text Files|*.txt"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
              File.WriteAllLines(sfd.FileName, errors.ToArray());
          }
        }
      }
      catch (Exception ex)
      {
        Scrobbles.Clear();
        OnStatusUpdated("Error parsing CSV file: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Notifies the UI that "ToScrobble" has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyCanProperties();
    }

    /// <summary>
    /// Notifies the UI of possible "Can-Property" changes.
    /// </summary>
    private void NotifyCanProperties()
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    /// <summary>
    /// Scrobbles the selected scrobbles.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      EnableControls = false;
      OnStatusUpdated("Trying to scrobble selected tracks...");

      var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
      if (response.Success)
        OnStatusUpdated("Successfully scrobbled!");
      else
        OnStatusUpdated("Error while scrobbling!");

      EnableControls = true;
    }

    /// <summary>
    /// Create a list with tracks that will be scrobbled.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      List<Scrobble> scrobbles = new List<Scrobble>();

      if (ScrobbleMode == CSVScrobbleMode.Normal)
      {
        foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
        {
          scrobbles.Add(new Scrobble(vm.ParsedScrobble.ArtistName, vm.ParsedScrobble.AlbumName, vm.ParsedScrobble.TrackName, vm.ParsedScrobble.Played));
        }
      }
      else if (ScrobbleMode == CSVScrobbleMode.ImportMode)
      {
        DateTime time = Time;
        foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
        {
          scrobbles.Add(new Scrobble(vm.ParsedScrobble.ArtistName, vm.ParsedScrobble.AlbumArtist, vm.ParsedScrobble.TrackName, time));
          time = time.Subtract(TimeSpan.FromSeconds(Duration));
        }
      }

      return scrobbles;
    }

    /// <summary>
    /// Opens the <see cref="ConfigureCSVParserView"/>
    /// </summary>
    public void OpenCSVParserSettings()
    {
      ConfigureCSVParserView view = new ConfigureCSVParserView();
      view.ShowDialog();
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