using Scrubbler.Interfaces;
using Microsoft.Win32;

namespace Scrubbler.Models
{
  /// <summary>
  /// Service for showing a <see cref="OpenFileDialog"/>.
  /// </summary>
  class OpenFileDialogService : IOpenFileDialog
  {
    #region Properties

    /// <summary>
    /// If multiple files can be selected.
    /// </summary>
    public bool Multiselect { get => _openFileDialog.Multiselect; set => _openFileDialog.Multiselect = value; }

    /// <summary>
    /// The selected files.
    /// </summary>
    public string[] FileNames => _openFileDialog.FileNames;

    /// <summary>
    /// The file from the last <see cref="ShowDialog"/> operation.
    /// </summary>
    public string FileName => _openFileDialog.FileName;

    /// <summary>
    /// Filter string to apply to the dialog.
    /// </summary>
    public string Filter { get => _openFileDialog.Filter; set => _openFileDialog.Filter = value; }

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual <see cref="OpenFileDialog"/>.
    /// </summary>
    private OpenFileDialog _openFileDialog;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public OpenFileDialogService()
    {
      _openFileDialog = new OpenFileDialog();
    }

    #endregion Construction

    /// <summary>
    /// Shows the file dialog.
    /// </summary>
    /// <returns>True, if the dialog was handled successfully.
    /// False if not.</returns>
    public bool ShowDialog()
    {
      return _openFileDialog.ShowDialog() ?? false;
    }
  }
}