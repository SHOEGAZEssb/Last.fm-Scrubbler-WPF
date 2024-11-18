using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel for splitting a csv file.
  /// </summary>
  public class CSVSplitterViewModel : FileSplitterViewModel
  {
    #region Properties

    /// <summary>
    /// Amount of lines in the <see cref="FileSplitterViewModel.FileToSplit"/>.
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
    /// <inheritdoc/>
    /// </summary>
    protected override string FileOpenFilter => "CSV Files|*.csv";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override IEnumerable<string> AllowedFileExtensions => new string[] { ".csv" };

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="fileOperator"/> is null.</exception>
    public CSVSplitterViewModel(IExtendedWindowManager windowManager, IFileOperator fileOperator)
      : base(windowManager, fileOperator, "CSV Splitter")
    {
      PropertyChanged += FileSplitterViewModel_PropertyChanged;
    }

    #endregion Construction

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void Split()
    {
      try
      {
        EnableControls = false;
        var lines = _fileOperator.ReadAllLines(FileToSplit);
        int numFiles = (int)Math.Ceiling(lines.Length / (double)EntriesPerFile);
        for(int i = 0; i < numFiles; i++)
        {
          OnStatusUpdated($"Creating split file {i + 1} / {numFiles}");
          using (var fs = _fileOperator.Open(GetFreeFileName(), FileMode.Append))
          {
            using (var sw = new StreamWriter(fs))
            {
              foreach(var line in lines.Skip(i * EntriesPerFile).Take(EntriesPerFile))
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

    private void FileSplitterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(FileToSplit))
        NotifyOfPropertyChange(() => NumLines);
    }
  }
}
