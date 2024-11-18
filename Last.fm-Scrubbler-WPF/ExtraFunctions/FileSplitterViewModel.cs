using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// Base class for ViewModels that split files.
  /// </summary>
  public abstract class FileSplitterViewModel : TabViewModel
  {
    #region Properties

    /// <summary>
    /// The file to split.
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
          (SplitCommand as DelegateCommand).RaiseCanExecuteChanged();
        }
      }
    }
    private string _fileToSplit;

    /// <summary>
    /// Amount of entries in each split file.
    /// </summary>
    public int EntriesPerFile
    {
      get => _entriesPerFile;
      set
      {
        if (EntriesPerFile != value)
        {
          _entriesPerFile = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private int _entriesPerFile;

    /// <summary>
    /// Command for opening the file to split.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Command for the split process.
    /// </summary>
    public ICommand SplitCommand { get; }

    /// <summary>
    /// Filter for the OpenFileDialog.
    /// </summary>
    protected abstract string FileOpenFilter { get; }

    /// <summary>
    /// Allowed file extensions.
    /// </summary>
    protected abstract IEnumerable<string> AllowedFileExtensions { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    protected readonly IExtendedWindowManager _windowManager;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    protected readonly IFileOperator _fileOperator;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <param name="tabName">Name to display in the TabViewModel.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="windowManager"/> or
    /// <paramref name="fileOperator"/> is null.</exception>
    public FileSplitterViewModel(IExtendedWindowManager windowManager, IFileOperator fileOperator, string tabName)
      : base(tabName)
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _fileOperator = fileOperator ?? throw new ArgumentNullException(nameof(fileOperator));
      EntriesPerFile = 3000;
      OpenFileCommand = new DelegateCommand((o) => OpenFile());
      SplitCommand = new DelegateCommand((o) => Split(), (o) => !string.IsNullOrEmpty(FileToSplit) && AllowedFileExtensions.Any(e => FileToSplit.EndsWith(e)));
    }

    #endregion Construction

    /// <summary>
    /// Shows a dialog to open a file.
    /// </summary>
    protected virtual void OpenFile()
    {
      var ofd = _windowManager.CreateOpenFileDialog();
      ofd.Filter = FileOpenFilter;
      ofd.Multiselect = false;
      if (ofd.ShowDialog())
      {
        FileToSplit = ofd.FileName;
      }
    }

    /// <summary>
    /// Splits the <see cref="FileToSplit"/> into
    /// multiple files, each containing <see cref="EntriesPerFile"/> entries.
    /// </summary>
    protected abstract void Split();
  }
}
