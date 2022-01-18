using Scrubbler.Helper;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel for splitting a csv file.
  /// </summary>
  public class CSVSplitterViewModel : TabViewModel
  {
    #region Properties

    /// <summary>
    /// The csv file to split.
    /// </summary>
    public string FileToSplit
    {
      get => _fileToSplit;
      set
      {
        if (FileToSplit != value)
        {
          _fileToSplit = value;
          NotifyOfPropertyChange();
          NotifyOfPropertyChange(nameof(NumLines));
          (SplitCommand as DelegateCommand).RaiseCanExecuteChanged();
        }
      }
    }
    private string _fileToSplit;

    /// <summary>
    /// Amount of lines in the <see cref="FileToSplit"/>.
    /// </summary>
    public int NumLines
    {
      get
      {
        return (string.IsNullOrEmpty(FileToSplit) || !_fileOperator.Exists(FileToSplit)) ? 0 
          : _fileOperator.ReadAllLines(FileToSplit).Length;
      }
    }

    /// <summary>
    /// Amount of lines in each split file.
    /// </summary>
    public int LinesPerFile
    {
      get => _linesPerFile;
      set
      {
        if (LinesPerFile != value)
        {
          _linesPerFile = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private int _linesPerFile;

    /// <summary>
    /// Command for opening the file to split.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Command for the split process.
    /// </summary>
    public ICommand SplitCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="fileOperator"/> is null.</exception>
    public CSVSplitterViewModel(IExtendedWindowManager windowManager, IFileOperator fileOperator)
      : base("CSV Splitter")
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _fileOperator = fileOperator ?? throw new ArgumentNullException(nameof(fileOperator));
      LinesPerFile = 3000;
      OpenFileCommand = new DelegateCommand((o) => OpenFile());
      SplitCommand = new DelegateCommand((o) => Split(), (o) => !string.IsNullOrEmpty(FileToSplit) && FileToSplit.EndsWith(".csv"));
    }

    #endregion Construction

    private void OpenFile()
    {
      var ofd = _windowManager.CreateOpenFileDialog();
      ofd.Filter = "CSV Files|*.csv";
      ofd.Multiselect = false;
      if (ofd.ShowDialog())
      {
        FileToSplit = ofd.FileName;
      }
    }

    private void Split()
    {
      try
      {
        EnableControls = false;
        var lines = _fileOperator.ReadAllLines(FileToSplit);
        int numFiles = (int)Math.Ceiling(lines.Length / (double)LinesPerFile);
        for(int i = 0; i < numFiles; i++)
        {
          OnStatusUpdated($"Creating split file {i + 1} / {numFiles}");
          using (var fs = _fileOperator.Open(GetFreeFileName(), FileMode.Append))
          {
            using (var sw = new StreamWriter(fs))
            {
              foreach(var line in lines.Skip(i * LinesPerFile).Take(LinesPerFile))
              {
                sw.WriteLine(line);
              }
            }
          }
        }

        OnStatusUpdated("Successfully split csv files.");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while splitting csv file: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }

    private string GetFreeFileName()
    {
      string newFileName = FileToSplit;
      string extension = Path.GetExtension(FileToSplit);
      int counter = 0;
      while (_fileOperator.Exists(newFileName))
      {
        newFileName = FileToSplit.Replace(extension, $"_{counter++}{extension}");
      }

      return newFileName;
    }
  }
}
