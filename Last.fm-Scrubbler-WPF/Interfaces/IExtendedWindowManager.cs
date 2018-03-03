using Caliburn.Micro;

namespace Last.fm_Scrubbler_WPF.Interfaces
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