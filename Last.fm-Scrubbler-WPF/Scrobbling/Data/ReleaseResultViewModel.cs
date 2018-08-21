using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="ReleaseResultView"/>.
  /// </summary>
  class ReleaseResultViewModel : Conductor<FetchedReleaseViewModel>.Collection.AllActive
  {
    #region Properties

    /// <summary>
    /// Event that gets fired when one of the
    /// displayed releases is clicked.
    /// </summary>
    public event EventHandler ReleaseClicked;

    /// <summary>
    /// Event that gets fired when the origin
    /// artist results should be shown.
    /// </summary>
    public event EventHandler BackToArtistRequested;

    /// <summary>
    /// If true, the release data was fetched
    /// by clicking an artist.
    /// </summary>
    public bool FetchedThroughArtist
    {
      get => _fetchedThroughArtist;
      set
      {
        _fetchedThroughArtist = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _fetchedThroughArtist;

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="releases">Fetched releases.</param>
    /// <param name="fetchedThroughArtist">True if the <paramref name="releases"/> were
    /// fetched by clicking an artist.</param>
    public ReleaseResultViewModel(IEnumerable<Release> releases, bool fetchedThroughArtist)
    {
      FetchedThroughArtist = fetchedThroughArtist;
      foreach(var release in releases)
      {
        var vm = new FetchedReleaseViewModel(release);
        vm.ReleaseClicked += Release_Clicked;
        ActivateItem(vm);
      }
    }

    #endregion Construction

    /// <summary>
    /// Fires the <see cref="BackToArtistRequested"/> event.
    /// </summary>
    public void BackToArtist()
    {
      if(FetchedThroughArtist)
        BackToArtistRequested?.Invoke(this, EventArgs.Empty);
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
        item.ReleaseClicked -= ReleaseClicked;
        DeactivateItem(item, close);
      }
    }

    /// <summary>
    /// Fires the <see cref="ReleaseClicked"/> event.
    /// </summary>
    /// <param name="sender">Clicked release.</param>
    /// <param name="e">Ignored.</param>
    private void Release_Clicked(object sender, EventArgs e)
    {
      ReleaseClicked?.Invoke(sender, e);
    }
  }
}