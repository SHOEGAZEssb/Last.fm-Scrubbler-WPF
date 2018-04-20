using Caliburn.Micro;
using IF.Lastfm.Core.Api;
using Scrubbler.Interfaces;
using System;

namespace Scrubbler.ViewModels.ExtraFunctions
{
  /// <summary>
  /// ViewModel managing all extra function ViewModels.
  /// </summary>
  class ExtraFunctionsViewModel : Conductor<ViewModelBase>.Collection.OneActive
  {
    #region Properties

    /// <summary>
    /// Event that fires when the status of a ViewModel changes.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user API.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    public ExtraFunctionsViewModel(IExtendedWindowManager windowManager, IUserApi userAPI, IFileOperator fileOperator)
    {
      DisplayName = "Extra Functions";
      CreateViewModels(windowManager, userAPI, fileOperator);
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user API.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    private void CreateViewModels(IExtendedWindowManager windowManager, IUserApi userAPI, IFileOperator fileOperator)
    {
      var pasteYourTasteVM = new PasteYourTasteViewModel();
      pasteYourTasteVM.StatusUpdated += VM_StatusUpdated; ;
      var csvDownloaderVM = new CSVDownloaderViewModel(windowManager, userAPI, fileOperator);
      csvDownloaderVM.StatusUpdated += VM_StatusUpdated;
      var collageCreatorVM = new CollageCreatorViewModel(windowManager, userAPI);
      collageCreatorVM.StatusUpdated += VM_StatusUpdated;

      ActivateItem(pasteYourTasteVM);
      ActivateItem(csvDownloaderVM);
      ActivateItem(collageCreatorVM);

      // should be selected
      ActivateItem(pasteYourTasteVM);
    }

    /// <summary>
    /// Fires the <see cref="StatusUpdated"/> event.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">NEw status.</param>
    private void VM_StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      StatusUpdated?.Invoke(this, e);
    }
  }
}