using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Interfaces;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels.ExtraFunctions
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
    public ExtraFunctionsViewModel(IExtendedWindowManager windowManager)
    {
      DisplayName = "Extra Functions";
      CreateViewModels(windowManager);
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    private void CreateViewModels(IExtendedWindowManager windowManager)
    {
      var pasteYourTasteVM = new PasteYourTasteViewModel();
      pasteYourTasteVM.StatusUpdated += VM_StatusUpdated; ;
      var csvDownloaderVM = new CSVDownloaderViewModel(windowManager);
      csvDownloaderVM.StatusUpdated += VM_StatusUpdated;
      var collageCreatorVM = new CollageCreatorViewModel(MainViewModel.Client.User);
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