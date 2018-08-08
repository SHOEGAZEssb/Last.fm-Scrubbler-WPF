using Caliburn.Micro;
using SetlistFmApi.Model.Music;
using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a <see cref="Setlist"/>.
  /// </summary>
  public class FetchedSetlistViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this setlist is clicked.
    /// </summary>
    public event EventHandler SetlistClicked;

    /// <summary>
    /// The fetched setlist.
    /// </summary>
    public Setlist FetchedSetlist
    {
      get { return _fetchedSetlist; }
      private set
      {
        _fetchedSetlist = value;
        NotifyOfPropertyChange(() => FetchedSetlist);
      }
    }
    private Setlist _fetchedSetlist;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedSetlist">The fetched setlist.</param>
    public FetchedSetlistViewModel(Setlist fetchedSetlist)
    {
      FetchedSetlist = fetchedSetlist;
    }

    /// <summary>
    /// Fires the <see cref="SetlistClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      SetlistClicked?.Invoke(FetchedSetlist, EventArgs.Empty);
    }
  }
}