using Caliburn.Micro;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Extended <see cref="WindowManager"/> with
  /// MessageBox, Open- and SaveFileDialog functionality.
  /// </summary>
  public class ExtendedWindowManager : WindowManager, IExtendedWindowManager
  {
    #region Properties

    /// <summary>
    /// Service that can display MessageBoxes and dialogs.
    /// </summary>
    public IMessageBoxService MessageBoxService { get; private set; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public ExtendedWindowManager()
    {
      MessageBoxService = new MessageBoxService();
    }

    #endregion Construction

    /// <summary>
    /// Create a OpenFileDialog service object.
    /// </summary>
    public IOpenFileDialog CreateOpenFileDialog()
    {
      return new OpenFileDialogService();
    }

    /// <summary>
    /// Creates a SaveFileDialog service object.
    /// </summary>
    public IFileDialog CreateSaveFileDialog()
    {
      return new SaveFileDialogService();
    }
  }
}