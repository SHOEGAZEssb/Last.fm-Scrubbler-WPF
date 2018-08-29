using Caliburn.Micro;
using SetlistFmApi.Model.Music;
using System;
using System.Collections.Generic;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for displaying <see cref="FetchedSetlistViewModel"/>s.
  /// </summary>
  class SetlistResultViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Event that is fired when a setlist is clicked.
    /// </summary>
    public event EventHandler SetlistClicked;

    /// <summary>
    /// The fetched setlists.
    /// TODO: can we put them in a conductor?
    /// </summary>
    public List<FetchedSetlistViewModel> Setlists
    {
      get => _setlists;
      private set
      {
        _setlists = value;
        NotifyOfPropertyChange();
      }
    }
    private List<FetchedSetlistViewModel> _setlists;

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="setlists">The fetched setlists.</param>
    public SetlistResultViewModel(IEnumerable<Setlist> setlists)
    {
      Setlists = new List<FetchedSetlistViewModel>();
      foreach(var setlist in setlists)
      {
        var vm = new FetchedSetlistViewModel(setlist);
        vm.SetlistClicked += Setlist_Clicked;
        Setlists.Add(vm);
      }
    }

    #endregion Construction

    public void BackToArtists()
    {

    }

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
  }
}