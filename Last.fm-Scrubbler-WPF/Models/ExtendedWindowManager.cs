using Caliburn.Micro;
using Scrubbler.Interfaces;

namespace Scrubbler.Models
{
  class ExtendedWindowManager : WindowManager, IExtendedWindowManager
  {
    public IMessageBoxService MessageBoxService => _messageBoxService;

    private MessageBoxService _messageBoxService;

    public ExtendedWindowManager()
    {
      _messageBoxService = new MessageBoxService();
    }

    public IOpenFileDialog CreateOpenFileDialog()
    {
      return new OpenFileDialogService();
    }

    public IFileDialog CreateSaveFileDialog()
    {
      return new SaveFileDialogService();
    }
  }
}