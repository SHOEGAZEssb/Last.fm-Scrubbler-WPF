namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for a dialog that handles files.
  /// </summary>
  public interface IFileDialog
  {
    /// <summary>
    /// Shows the file dialog.
    /// </summary>
    /// <returns>True, if the dialog was handled successfully.
    /// False if not.</returns>
    bool ShowDialog();

    /// <summary>
    /// The file from the last <see cref="ShowDialog"/> operation.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Filter string to apply to the dialog.
    /// </summary>
    string Filter { get; set; }
  }
}