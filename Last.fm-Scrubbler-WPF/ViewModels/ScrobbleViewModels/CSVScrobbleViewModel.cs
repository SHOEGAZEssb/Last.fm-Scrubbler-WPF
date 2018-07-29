using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.Models;
using Scrubbler.Properties;
using Scrubbler.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.ViewModels.ScrobbleViewModels
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
    /// Set the timestamp by setting <see cref="ScrobbleTimeViewModel.Time"/>.
    /// </summary>
    ImportMode
  }

  /// <summary>
  /// ViewModel for the <see cref="Views.ScrobbleViews.CSVScrobbleView"/>.
  /// </summary>
  public class CSVScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<ParsedCSVScrobbleViewModel>
  {
    #region Properties

    /// <summary>
    /// The path to the csv file.
    /// </summary>
    public string CSVFilePath
    {
      get { return _csvFilePath; }
      set
      {
        _csvFilePath = value;
        NotifyOfPropertyChange();
      }
    }
    private string _csvFilePath;

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
          if (_windowManager.MessageBoxService.ShowDialog("Do you want to switch the Scrobble Mode? The CSV file will be parsed again!",
                                                          "Change Scrobble Mode", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
            ParseCSVFile().Forget();
        }

        NotifyOfPropertyChange();
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

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Different formats to try in case TryParse fails.
    /// </summary>
    private static readonly string[] _formats = new string[] { "M/dd/yyyy h:mm" };

    /// <summary>
    /// The factory used to create <see cref="ITextFieldParser"/>.
    /// </summary>
    private ITextFieldParserFactory _parserFactory;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private IFileOperator _fileOperator;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="parserFactory">The factory used to create <see cref="ITextFieldParser"/>.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    public CSVScrobbleViewModel(IExtendedWindowManager windowManager, ITextFieldParserFactory parserFactory, IFileOperator fileOperator)
      : base(windowManager, "CSV Scrobbler")
    {
      _parserFactory = parserFactory;
      _fileOperator = fileOperator;
      Scrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();
      Duration = 1;
      ScrobbleMode = CSVScrobbleMode.ImportMode;
    }

    /// <summary>
    /// Shows a dialog to open a csv file.
    /// </summary>
    public void LoadCSVFileDialog()
    {
      IOpenFileDialog ofd = _windowManager.CreateOpenFileDialog();
      ofd.Filter = "CSV Files|*.csv";
      if(ofd.ShowDialog())
        CSVFilePath = ofd.FileName;
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

        using (ITextFieldParser parser = _parserFactory.CreateParser(CSVFilePath))
        {

          parser.SetDelimiters(Settings.Default.CSVDelimiters.Select(x => new string(x, 1)).ToArray());

          string[] fields = null;
          List<string> errors = new List<string>();
          ObservableCollection<ParsedCSVScrobbleViewModel> parsedScrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();

          await Task.Run(() =>
          {
            while (!parser.EndOfData)
            {
              try
              {
                fields = parser.ReadFields();

                string dateString = fields[Settings.Default.TimestampFieldIndex];

                // check for 'now playing'
                if (dateString == "" && ScrobbleMode == CSVScrobbleMode.Normal)
                  continue;

                DateTime date = DateTime.Now;
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
                TimeSpan time = TimeSpan.FromSeconds(Duration);
                TimeSpan.TryParse(duration, out time);

                DatedScrobble parsedScrobble = new DatedScrobble(date.AddSeconds(1), fields[Settings.Default.TrackFieldIndex],
                                                                fields[Settings.Default.ArtistFieldIndex], album,
                                                                albumArtist, time);

                parsedScrobbles.Add(new ParsedCSVScrobbleViewModel(parsedScrobble, ScrobbleMode));
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
            OnStatusUpdated(string.Format("Successfully parsed CSV file. Parsed {0} rows", parsedScrobbles.Count));
          else
          {
            OnStatusUpdated(string.Format("Partially parsed CSV file. {0} rows could not be parsed", errors.Count));
            if (_windowManager.MessageBoxService.ShowDialog("Some rows could not be parsed. Do you want to save a text file with the rows that could not be parsed?",
                                                            "Error parsing rows", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
            {
              IFileDialog sfd = _windowManager.CreateSaveFileDialog();
              sfd.Filter = "Text Files|*.txt";
              if (sfd.ShowDialog())
                _fileOperator.WriteAllLines(sfd.FileName, errors.ToArray());
            }
          }

          Scrobbles = parsedScrobbles;
        }
      }
      catch (Exception ex)
      {
        Scrobbles.Clear();
        OnStatusUpdated(string.Format("Error parsing CSV file: {0}", ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Scrobbles the selected scrobbles.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to scrobble selected tracks...");

        var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success && response.Status == LastResponseStatus.Successful)
          OnStatusUpdated("Successfully scrobbled selected tracks");
        else
          OnStatusUpdated(string.Format("Error while scrobbling selected tracks: {0}", response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while scrobbling selected tracks: {0}", ex.Message));
      }
      finally
      {
        EnableControls = true;
      }
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
          scrobbles.Add(new Scrobble(vm.ParsedScrobble.ArtistName, vm.ParsedScrobble.AlbumName, vm.ParsedScrobble.TrackName, vm.ParsedScrobble.Played)
          { AlbumArtist = vm.ParsedScrobble.AlbumArtist, Duration = vm.ParsedScrobble.Duration });
        }
      }
      else if (ScrobbleMode == CSVScrobbleMode.ImportMode)
      {
        DateTime time = ScrobbleTimeVM.Time;
        foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
        {
          scrobbles.Add(new Scrobble(vm.ParsedScrobble.ArtistName, vm.ParsedScrobble.AlbumName, vm.ParsedScrobble.TrackName, time)
          {
            AlbumArtist = vm.ParsedScrobble.AlbumArtist,
            Duration = vm.ParsedScrobble.Duration
          });

          time = time.Subtract(TimeSpan.FromSeconds(Duration));
        }
      }

      return scrobbles;
    }

    /// <summary>
    /// Opens the <see cref="Views.SubViews.ConfigureCSVParserView"/>
    /// </summary>
    public void OpenCSVParserSettings()
    {
      _windowManager.ShowDialog(new ConfigureCSVParserViewModel());
    }

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    public override void CheckAll()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public override void UncheckAll()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled))
      {
        vm.ToScrobble = false;
      }
    }

    /// <summary>
    /// Marks all selected scrobbles as "ToScrobble".
    /// </summary>
    public override void CheckSelected()
    {
      foreach (var vm in Scrobbles.Where(i => i.IsEnabled && i.IsSelected))
      {
        vm.ToScrobble = false;
      }
    }

    /// <summary>
    /// Marks all selected scrobbles as not "ToScrobble".
    /// </summary>
    public override void UncheckSelected()
    {
      foreach (var s in Scrobbles.Where(s => s.IsEnabled && s.IsSelected))
      {
        s.ToScrobble = false;
      }
    }
  }
}