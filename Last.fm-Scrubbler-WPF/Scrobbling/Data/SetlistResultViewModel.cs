using SetlistFmApi.Model.Music;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for displaying <see cref="FetchedSetlistViewModel"/>s.
  /// </summary>
  class SetlistResultViewModel : ViewModelBase, IDisposable
  {
    #region Properties

    /// <summary>
    /// Event that is fired when a setlist is clicked.
    /// </summary>
    public event EventHandler SetlistClicked;

    /// <summary>
    /// The fetched setlists.
    /// </summary>
    public IEnumerable<FetchedSetlistViewModel> Setlists { get; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="setlists">The fetched setlists.</param>
    public SetlistResultViewModel(IEnumerable<Setlist> setlists)
    {
      Setlists = CreateSetlistViewModels(setlists);
    }

    #endregion Construction

    /// <summary>
    /// Deactivates all items.
    /// </summary>
    /// <param name="close">True if the items should be
    /// closed completely.</param>
    protected override void OnDeactivate(bool close)
    {
      foreach (var setlist in Setlists)
      {
        setlist.SetlistClicked -= Setlist_Clicked;
      }
    }

    private List<FetchedSetlistViewModel> CreateSetlistViewModels(IEnumerable<Setlist> setlists)
    {
      var vms = new List<FetchedSetlistViewModel>();
      foreach (var setlist in setlists)
      {
        var vm = new FetchedSetlistViewModel(setlist);
        vm.SetlistClicked += Setlist_Clicked;
        vms.Add(vm);
      }

      return vms;
    }

    /// <summary>
    /// Fires the <see cref="SetlistClicked"/> event
    /// when a setlist is clicked.
    /// </summary>
    /// <param name="sender">The clicked setlist.</param>
    /// <param name="e">Ignored.</param>
    private void Setlist_Clicked(object sender, EventArgs e)
    {
      SetlistClicked?.Invoke(sender, e);
    }

    #region IDisposable Implementation

    /// <summary>
    /// Used to detect redundant dispose calls.
    /// </summary>
    private bool _disposedValue = false;

    /// <summary>
    /// Disposes this object.
    /// </summary>
    /// <param name="disposing">If called by the user.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          foreach (var item in Setlists.ToList())
          {
            item.SetlistClicked -= SetlistClicked;
          }
        }

        _disposedValue = true;
      }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~SetlistResultViewModel()
    {
      Dispose(false);
    }

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation
  }
}