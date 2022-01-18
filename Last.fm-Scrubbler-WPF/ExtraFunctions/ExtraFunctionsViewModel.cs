using IF.Lastfm.Core.Api;
using Scrubbler.Helper;
using System.Collections.Generic;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel managing all extra function ViewModels.
  /// </summary>
  class ExtraFunctionsViewModel : TabViewModel
  {
    #region Properties

    /// <summary>
    /// Available extra functions.
    /// </summary>
    public IEnumerable<TabViewModel> ExtraFunctions { get; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user API.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    public ExtraFunctionsViewModel(IExtendedWindowManager windowManager, IUserApi userAPI, IFileOperator fileOperator)
      : base("Extra Functions")
    {
      ExtraFunctions = CreateViewModels(windowManager, userAPI, fileOperator);
    }

    /// <summary>
    /// Creates the ViewModels.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user API.</param>
    /// <param name="fileOperator">FileOperator used to interface with files.</param>
    private TabViewModel[] CreateViewModels(IExtendedWindowManager windowManager, IUserApi userAPI, IFileOperator fileOperator)
    {
      var pasteYourTasteVM = new PasteYourTasteViewModel(userAPI);
      pasteYourTasteVM.StatusUpdated += VM_StatusUpdated;
      var csvSplitterVM = new CSVSplitterViewModel(windowManager, fileOperator);
      csvSplitterVM.StatusUpdated += VM_StatusUpdated;
      var csvDownloaderVM = new CSVDownloaderViewModel(windowManager, userAPI, fileOperator);
      csvDownloaderVM.StatusUpdated += VM_StatusUpdated;
      var collageCreatorVM = new CollageCreatorViewModel(windowManager, userAPI);
      collageCreatorVM.StatusUpdated += VM_StatusUpdated;
      var milestoneCheckerVM = new MilestoneCheckerViewModel(userAPI);
      milestoneCheckerVM.StatusUpdated += VM_StatusUpdated;

      return new TabViewModel[] { pasteYourTasteVM, csvSplitterVM, csvDownloaderVM, collageCreatorVM, milestoneCheckerVM };
    }

    /// <summary>
    /// Fires the StatusUpdated event.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">NEw status.</param>
    private void VM_StatusUpdated(object sender, UpdateStatusEventArgs e)
    {
      OnStatusUpdated(e.NewStatus);
    }
  }
}