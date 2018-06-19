using Scrubbler.Interfaces;
using Microsoft.Win32;

namespace Scrubbler.Models
{
  /// <summary>
  /// Service for showing a <see cref="SaveFileDialog"/>.
  /// </summary>
  class SaveFileDialogService : IFileDialog
  {
    #region Properties

    /// <summary>
    /// The file from the last <see cref="ShowDialog"/> operation.
    /// </summary>
    public string FileName => _saveFileDialog.FileName;

    /// <summary>
    /// Filter string to apply to the dialog.
    /// </summary>
    public string Filter { get => _saveFileDialog.Filter; set => _saveFileDialog.Filter = value; }

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual <see cref="SaveFileDialog"/>.
    /// </summary>
    private SaveFileDialog _saveFileDialog;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public SaveFileDialogService()
    {
      _saveFileDialog = new SaveFileDialog();
    }

    #endregion Construction

    /// <summary>
    /// Shows the file dialog.
    /// </summary>
    /// <returns>True, if the dialog was handled successfully.
    /// False if not.</returns>
    public bool ShowDialog()
    {
      return _saveFileDialog.ShowDialog() ?? false;
    }
  }
}