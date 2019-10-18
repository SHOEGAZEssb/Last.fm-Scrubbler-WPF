using System.Collections.Generic;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for an OpenFileDialog.
  /// </summary>
  public interface IOpenFileDialog : IFileDialog
  {
    /// <summary>
    /// If multiple files can be selected.
    /// </summary>
    bool Multiselect { get; set; }

    /// <summary>
    /// The selected files.
    /// </summary>
    IEnumerable<string> FileNames { get; }
  }
}