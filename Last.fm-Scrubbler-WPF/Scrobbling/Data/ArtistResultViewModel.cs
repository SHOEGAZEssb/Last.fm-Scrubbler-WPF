using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="ArtistResultView"/>.
  /// </summary>
  class ArtistResultViewModel : Conductor<FetchedArtistViewModel>.Collection.AllActive
  {
    /// <summary>
    /// Event that fires when one of the displayed
    /// artists is clicked.
    /// </summary>
    public event EventHandler ArtistClicked;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="artists">Fetched artists.</param>
    public ArtistResultViewModel(IEnumerable<Artist> artists)
    {
      foreach (var artist in artists)
      {
        var vm = new FetchedArtistViewModel(artist);
        vm.ArtistClicked += Artist_Clicked;
        ActivateItem(vm);
      }
    }

    /// <summary>
    /// Deactivates all items.
    /// </summary>
    /// <param name="close">True if the items should be
    /// closed completely.</param>
    protected override void OnDeactivate(bool close)
    {
      foreach (var item in Items.ToList())
      {
        item.ArtistClicked -= ArtistClicked;
        DeactivateItem(item, close);
      }
    }

    /// <summary>
    /// Triggers when an artist was clicked.
    /// Fires the <see cref="ArtistClicked"/> event.
    /// </summary>
    /// <param name="sender">The clicked artist.</param>
    /// <param name="e">Ignored.</param>
    private void Artist_Clicked(object sender, EventArgs e)
    {
      ArtistClicked?.Invoke(sender, e);
    }
  }
}