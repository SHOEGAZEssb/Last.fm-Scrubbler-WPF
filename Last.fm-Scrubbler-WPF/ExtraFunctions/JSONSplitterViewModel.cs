using Newtonsoft.Json;
using Scrubbler.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel for splitting json files.
  /// </summary>
  public class JSONSplitterViewModel : FileSplitterViewModel
  {
    #region Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string FileOpenFilter => "JSON Files|*.json";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override IEnumerable<string> AllowedFileExtensions => new string[] { ".json" };

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="windowManager"/> or
    /// <paramref name="fileOperator"/> is null.</exception>
    public JSONSplitterViewModel(IExtendedWindowManager windowManager, IFileOperator fileOperator)
      : base(windowManager, fileOperator, "JSON Splitter")
    {
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
        // Read the input JSON file
        string jsonContent = _fileOperator.ReadAllText(FileToSplit);

        // Deserialize the JSON array into a list of objects
        var entries = JsonConvert.DeserializeObject<List<object>>(jsonContent);

        if (entries == null || entries.Count == 0)
        {
          OnStatusUpdated("No entries found in the JSON file.");
          return;
        }
        else if (entries.Count <= EntriesPerFile)
        {
          OnStatusUpdated($"JSON file has <= {EntriesPerFile} entries, does not need to be split.");
          return;
        }

        string inputFileName = Path.GetFileNameWithoutExtension(FileToSplit);
        string inputDirectory = Path.GetDirectoryName(FileToSplit);

        int fileNumber = 1;
        // Process records in chunks
        for (int i = 0; i < entries.Count; i += EntriesPerFile)
        {
          // Get a chunk of entries
          var chunk = entries.GetRange(i, Math.Min(EntriesPerFile, entries.Count - i));

          // Serialize the chunk to JSON
          string chunkJson = JsonConvert.SerializeObject(chunk, Formatting.Indented);

          // Write to a new file
          _fileOperator.WriteAllText(Path.Combine(inputDirectory, $"{inputFileName}_{fileNumber++}.json"), chunkJson);
        }

        OnStatusUpdated($"Successfully split json file into {fileNumber} files.");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while splitting json file: {ex.Message}");
      }
      finally
      {
        EnableControls = true;
      }
    }
  }
}
