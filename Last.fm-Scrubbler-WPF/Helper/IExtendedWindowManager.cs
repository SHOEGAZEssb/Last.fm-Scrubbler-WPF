using Caliburn.Micro;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Service that manages windows and dialogs.
  /// </summary>
  public interface IExtendedWindowManager : IWindowManager
  {
    /// <summary>
    /// Service that manages an OpenFileDialog.
    /// </summary>
    IOpenFileDialog CreateOpenFileDialog();

    /// <summary>
    /// Service that manages a SaveFileDialog.
    /// </summary>
    IFileDialog CreateSaveFileDialog();

    /// <summary>
    /// Service that manages dialogs.
    /// </summary>
    IMessageBoxService MessageBoxService { get; }
  }
}