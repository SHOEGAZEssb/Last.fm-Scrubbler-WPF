using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// Custom ListView for displaying <see cref="IScrobbableObjectViewModel"/>s.
  /// </summary>
  class ScrobbableObjectListView : ListView
  {
    private readonly List<IScrobbableObjectViewModel> selectedItems = new List<IScrobbableObjectViewModel>();

    /// <summary>
    /// Manually select the items...
    /// </summary>
    /// <param name="e">SelectionChangedEventArgs</param>
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);

      bool isVirtualizing = VirtualizingPanel.GetIsVirtualizing(this);
      bool isMultiSelect = (SelectionMode != SelectionMode.Single);

      if (isVirtualizing && isMultiSelect)
      {
        var newSelectedItems = SelectedItems.Cast<IScrobbableObjectViewModel>();

        foreach (var deselectedItem in selectedItems.Except(newSelectedItems))
        {
          deselectedItem.UpdateIsSelectedSilent(false);
        }

        selectedItems.Clear();
        selectedItems.AddRange(newSelectedItems);

        foreach (var newlySelectedItem in selectedItems)
        {
          newlySelectedItem.UpdateIsSelectedSilent(true);
        }

        // todo: kinda hacky way to update the view...
        var lastItem = selectedItems.LastOrDefault();
        if(lastItem != null)
          lastItem.IsSelected = true;
      }
    }
  }
}