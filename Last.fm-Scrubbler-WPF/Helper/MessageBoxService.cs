using System.Windows;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Wrapper class for the <see cref="MessageBox"/>.
  /// </summary>
  class MessageBoxService : IMessageBoxService
  {
    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <returns>Result of the dialog</returns>
    public IMessageBoxServiceResult ShowDialog(string text)
    {
      return ParseMessageBoxResult(MessageBox.Show(text));
    }

    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content
    /// and the given <paramref name="title"/> in the title bar.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <param name="title">Text to display in the title of the dialog.</param>
    /// <returns>Result of the dialog</returns>
    public IMessageBoxServiceResult ShowDialog(string text, string title)
    {
      return ParseMessageBoxResult(MessageBox.Show(text, title));
    }

    /// <summary>
    /// Shows a dialog with the given <paramref name="text"/> as content
    /// and the given <paramref name="title"/> in the title bar.
    /// Gives the user the given <paramref name="buttons"/> to press.
    /// </summary>
    /// <param name="text">Text to display in the dialog.</param>
    /// <param name="title">Text to display in the title of the dialog.</param>
    /// <param name="buttons">Buttons displayed in the dialog.</param>
    /// <returns>Result of the dialog</returns>
    public IMessageBoxServiceResult ShowDialog(string text, string title, IMessageBoxServiceButtons buttons)
    {
      return ParseMessageBoxResult(MessageBox.Show(text, title, ParseIDialogServiceButtons(buttons)));
    }

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
    public IMessageBoxServiceResult ShowDialog(string text, string title, IMessageBoxServiceButtons buttons, IMessageBoxServiceIcon icon)
    {
      return ParseMessageBoxResult(MessageBox.Show(text, title, ParseIDialogServiceButtons(buttons), ParseIDialogServiceIcon(icon)));
    }

    /// <summary>
    /// Converts a <see cref="MessageBoxResult"/> to the
    /// corresponding <see cref="IMessageBoxServiceResult"/>.
    /// </summary>
    /// <param name="result">Result of the <see cref="MessageBox"/>.</param>
    /// <returns>Equivalent <see cref="IMessageBoxServiceResult"/>.</returns>
    private IMessageBoxServiceResult ParseMessageBoxResult(MessageBoxResult result)
    {
      switch(result)
      {
        case MessageBoxResult.OK:
          return IMessageBoxServiceResult.OK;
        case MessageBoxResult.Yes:
          return IMessageBoxServiceResult.Yes;
        case MessageBoxResult.No:
          return IMessageBoxServiceResult.No;
        case MessageBoxResult.Cancel:
          return IMessageBoxServiceResult.Cancel;
        default:
          return IMessageBoxServiceResult.None;
      }
    }

    private MessageBoxButton ParseIDialogServiceButtons(IMessageBoxServiceButtons buttons)
    {
      switch(buttons)
      {
        case IMessageBoxServiceButtons.OK:
          return MessageBoxButton.OK;
        case IMessageBoxServiceButtons.YesNo:
          return MessageBoxButton.YesNo;
        case IMessageBoxServiceButtons.YesNoCancel:
          return MessageBoxButton.YesNoCancel;
        default:
          return MessageBoxButton.OK;
      }
    }

    private MessageBoxImage ParseIDialogServiceIcon(IMessageBoxServiceIcon icon)
    {
      switch(icon)
      {
        case IMessageBoxServiceIcon.None:
          return MessageBoxImage.None;
        case IMessageBoxServiceIcon.Warning:
          return MessageBoxImage.Warning;
        case IMessageBoxServiceIcon.Error:
          return MessageBoxImage.Error;
        default:
          return MessageBoxImage.None;
      }
    }
  }
}