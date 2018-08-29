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

    public string Artist => _fetchedSetlist.Artist.Name;
    public string City => _fetchedSetlist.Venue.City.Name;
    public string Venue => _fetchedSetlist.Venue.Name;
    public string Tour => _fetchedSetlist.Tour;
    public DateTime Date => _fetchedSetlist.EventDate;

    #endregion Properties

    #region Member

    /// <summary>
    /// The fetched setlist.
    /// </summary>
    private readonly Setlist _fetchedSetlist;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedSetlist">The fetched setlist.</param>
    public FetchedSetlistViewModel(Setlist fetchedSetlist)
    {
      _fetchedSetlist = fetchedSetlist;
    }

    /// <summary>
    /// Fires the <see cref="SetlistClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      SetlistClicked?.Invoke(_fetchedSetlist, EventArgs.Empty);
    }
  }
}