namespace Scrubbler.Helper
{
  /// <summary>
  /// Possible result values of a <see cref="IMessageBoxService"/> dialog.
  /// </summary>
  public enum IMessageBoxServiceResult
  {
    /// <summary>
    /// The user pressed 'OK'.
    /// </summary>
    OK,

    /// <summary>
    /// The user pressed 'Yes'.
    /// </summary>
    Yes,

    /// <summary>
    /// The user pressed 'No'.
    /// </summary>
    No,

    /// <summary>
    /// The user canceled the dialog.
    /// </summary>
    Cancel,

    /// <summary>
    /// Undefined result.
    /// </summary>
    None
  }

  /// <summary>
  /// Possible button combinations of a <see cref="IMessageBoxService"/> dialog.
  /// </summary>
  public enum IMessageBoxServiceButtons
  {
    /// <summary>
    /// The user can only press 'OK'.
    /// </summary>
    OK,

    /// <summary>
    /// The user can press 'Yes' and 'No'.
    /// </summary>
    YesNo,

    /// <summary>
    /// The user can press 'Yes', 'No' and 'Cancel'.
    /// </summary>
    YesNoCancel
  }

  /// <summary>
  /// Possible icons to show on a <see cref="IMessageBoxService"/> dialog.
  /// </summary>
  public enum IMessageBoxServiceIcon
  {
    /// <summary>
    /// No icon is shown.
    /// </summary>
    None,

    /// <summary>
    /// A warning icon is shown.
    /// </summary>
    Warning,

    /// <summary>
    /// A error icon is shown.
    /// </summary>
    Error
  }

  /// <summary>
  /// Interface for showing a MessageBox like dialog.
  /// </summary>
  public interface IMessageBoxService
  {
    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <returns>Result of the dialog</returns>
    IMessageBoxServiceResult ShowDialog(string text);

    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content
    /// and the given <paramref name="title"/> in the title bar.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <param name="title">Text to display in the title of the dialog.</param>
    /// <returns>Result of the dialog</returns>
    IMessageBoxServiceResult ShowDialog(string text, string title);

    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content
    /// and the given <paramref name="title"/> in the title bar.
    /// Gives the user the given <paramref name="buttons"/> to press.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <param name="title">Text to display in the title of the dialog.</param>
    /// <param name="buttons">Buttons displayed in the dialog.</param>
    /// <returns>Result of the dialog</returns>
    IMessageBoxServiceResult ShowDialog(string text, string title, IMessageBoxServiceButtons buttons);

    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content
    /// and the given <paramref name="title"/> in the title bar.
    /// Gives the user the given <paramref name="buttons"/> to press.
    /// Also displays an icon next to the text.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <param name="title">Text to display in the title of the dialog.</param>
    /// <param name="buttons">Buttons displayed in the dialog.</param>
    /// <param name="icon">Icon to display in the dialog.</param>
    /// <returns>Result of the dialog</returns>
    IMessageBoxServiceResult ShowDialog(string text, string title, IMessageBoxServiceButtons buttons, IMessageBoxServiceIcon icon);
  }
}