using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Helper;
using Scrubbler.Helper.FileParser;
using Scrubbler.Scrobbling.Data;
using ScrubblerLib.Helper;
using ScrubblerLib.Helper.FileParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="FileParseScrobbleView"/>.
  /// </summary>
  public class FileParseScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<ParsedCSVScrobbleViewModel>
  {
    #region Properties

    /// <summary>
    /// List of available file parser.
    /// </summary>
    public IEnumerable<IFileParserViewModel> AvailableParser { get; }

    /// <summary>
    /// The selected file parser.
    /// </summary>
    public IFileParserViewModel SelectedParser
    {
      get => _selectedParser;
      set
      {
        if (SelectedParser != value)
        {
          _selectedParser = value;
          NotifyOfPropertyChange();
          NotifyOfPropertyChange(() => CanParse);
          NotifyOfPropertyChange(() => CanShowSettings);
        }
      }
    }
    private IFileParserViewModel _selectedParser;

    /// <summary>
    /// The path to the file.
    /// </summary>
    public string FilePath
    {
      get { return _csvFilePath; }
      set
      {
        _csvFilePath = value;
        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanParse);
      }
    }
    private string _csvFilePath;

    /// <summary>
    /// The selected <see cref="Scrobbler.ScrobbleMode"/>.
    /// </summary>
    public ScrobbleMode ScrobbleMode
    {
      get { return _scrobbleMode; }
      set
      {
        if (Scrobbles.Count > 0)
        {
          if (WindowManager.MessageBoxService.ShowDialog("Do you want to switch the Scrobble Mode? The CSV file will be parsed again!",
                                                          "Change Scrobble Mode", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
          {
            ParseFile().Forget();
          }
          else
            return;
        }

        _scrobbleMode = value;
        NotifyOfPropertyChange();
      }
    }
    private ScrobbleMode _scrobbleMode;

    /// <summary>
    /// Duration between scrobbles in seconds.
    /// </summary>
    public int Duration
    {
      get { return _duration; }
      set
      {
        _duration = value;
        NotifyOfPropertyChange();
      }
    }
    private int _duration;

    /// <summary>
    /// Gets if the current configuration can be parsed.
    /// </summary>
    public bool CanParse => !string.IsNullOrEmpty(FilePath) && SelectedParser.SupportedExtensions.Contains(Path.GetExtension(FilePath));

    /// <summary>
    /// Gets if the <see cref="SelectedParser"/> has settings.
    /// </summary>
    public bool CanShowSettings => SelectedParser is IHaveSettings;

    /// <summary>
    /// Command for parsing the file.
    /// </summary>
    public ICommand ParseCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    private readonly ISerializer _serializer;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="parserFactory">The factory used to create <see cref="IFileParser"/>.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    public FileParseScrobbleViewModel(IExtendedWindowManager windowManager, IFileParserFactory parserFactory, IFileOperator fileOperator, ISerializer serializer)
      : base(windowManager, "File Parse Scrobbler")
    {
      if (parserFactory == null)
        throw new ArgumentNullException(nameof(parserFactory));

      _fileOperator = fileOperator ?? throw new ArgumentNullException(nameof(fileOperator));
      _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
      AvailableParser = new List<IFileParserViewModel>()
      {
        new CSVFileParserViewModel(parserFactory.CreateCSVFileParser(), WindowManager),
        new JSONParserViewModel(parserFactory.CreateJSONFileParser(), windowManager)
      };
      SelectedParser = AvailableParser.FirstOrDefault();

      Scrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>();
      Duration = 1;
      ScrobbleMode = ScrobbleMode.ImportMode;
      ParseCommand = new DelegateCommand((o) => ParseFile().Forget());
    }

    /// <summary>
    /// Shows a dialog to open a file.
    /// </summary>
    public void LoadFileDialog()
    {
      IOpenFileDialog ofd = WindowManager.CreateOpenFileDialog();
      ofd.Filter = SelectedParser.FileFilter;
      if (ofd.ShowDialog())
        FilePath = ofd.FileName;
    }

    /// <summary>
    /// Loads and parses the <see cref="FilePath"/>.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task ParseFile()
    {
      if (!CanParse)
        return;

      try
      {
        EnableControls = false;
        OnStatusUpdated("Parsing file...");

        IEnumerable<string> errors = null;
        ObservableCollection<ParsedCSVScrobbleViewModel> parsedScrobbles = null;

        await Task.Run(() =>
        {
          var res = SelectedParser.Parse(FilePath, TimeSpan.FromSeconds(Duration), ScrobbleMode);
          errors = res.Errors;
          parsedScrobbles = new ObservableCollection<ParsedCSVScrobbleViewModel>(res.Scrobbles.Select(i => new ParsedCSVScrobbleViewModel(i, ScrobbleMode)));
        });

        if (!errors.Any())
          OnStatusUpdated($"Successfully parsed file. Parsed {parsedScrobbles.Count} scrobbles");
        else
        {
          OnStatusUpdated($"Partially parsed file. Scrobbles: {parsedScrobbles.Count} | Errors: {errors.Count()}");
          if (WindowManager.MessageBoxService.ShowDialog("Some scrobbles could not be parsed. Do you want to save a text file with the rows that could not be parsed?",
                                                          "Error parsing scrobbles", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
          {
            IFileDialog sfd = WindowManager.CreateSaveFileDialog();
            sfd.Filter = "Text Files|*.txt";
            if (sfd.ShowDialog())
              _fileOperator.WriteAllLines(sfd.FileName, errors.ToArray());
          }
        }

        Scrobbles = parsedScrobbles;
      }
      catch (Exception ex)
      {
        Scrobbles.Clear();
        OnStatusUpdated($"Fatal error parsing file: {ex.Message}");
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
          OnStatusUpdated($"Error while scrobbling selected tracks: {response.Status}");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while scrobbling selected tracks: {ex.Message}");
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
      var scrobbles = new List<Scrobble>();

      if (ScrobbleMode == ScrobbleMode.Normal)
      {
        foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
        {
          scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, vm.Played)
          { AlbumArtist = vm.AlbumArtist, Duration = vm.Duration });
        }
      }
      else if (ScrobbleMode == ScrobbleMode.ImportMode)
      {
        DateTime time = ScrobbleTimeVM.Time;
        foreach (var vm in Scrobbles.Where(i => i.ToScrobble))
        {
          scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, time)
          {
            AlbumArtist = vm.AlbumArtist,
            Duration = vm.Duration
          });

          time = time.Subtract(TimeSpan.FromSeconds(Duration));
        }
      }

      return scrobbles;
    }

    /// <summary>
    /// Opens the settings of the <see cref="SelectedParser"/>.
    /// </summary>
    public void OpenParserSettings()
    {
      if (CanShowSettings)
      {
        if ((SelectedParser as IHaveSettings).ShowSettings() ?? false)
        {
          try
          {
            SaveParserConfig();
          }
          catch
          {
            // ignore
          }
        }
      }
    }

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    public override void CheckAll()
    {
      SetToScrobbleState(Scrobbles.Where(i => i.CanScrobble), true);
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public override void UncheckAll()
    {
      SetToScrobbleState(Scrobbles.Where(i => i.CanScrobble), false);
    }

    /// <summary>
    /// Marks all selected scrobbles as "ToScrobble".
    /// </summary>
    public override void CheckSelected()
    {
      SetToScrobbleState(Scrobbles.Where(i => i.IsSelected && i.CanScrobble), true);
    }

    /// <summary>
    /// Marks all selected scrobbles as not "ToScrobble".
    /// </summary>
    public override void UncheckSelected()
    {
      SetToScrobbleState(Scrobbles.Where(i => i.IsSelected && i.CanScrobble), false);
    }

    private void SaveParserConfig()
    {
      var config = SelectedParser.MakeConfig(ScrobbleMode, TimeSpan.FromSeconds(Duration));
      _serializer.Serialize(config, $"{config.GetType().Name}.xml", new[] { typeof(CSVFileParserConfiguration), typeof(JSONFileParserConfiguration) });
    }
  }
}