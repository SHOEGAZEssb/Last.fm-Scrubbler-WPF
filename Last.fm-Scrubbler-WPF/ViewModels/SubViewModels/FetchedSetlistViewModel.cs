using Caliburn.Micro;
using SetlistFmApi.Model.Music;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels.SubViewModels
{
  public class FetchedSetlistViewModel : PropertyChangedBase
  {
    #region Properties

    public event EventHandler SetlistClicked;

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

    public FetchedSetlistViewModel(Setlist fetchedSetlist)
    {
      FetchedSetlist = fetchedSetlist;
    }

    public void Clicked()
    {
      SetlistClicked?.Invoke(FetchedSetlist, EventArgs.Empty);
    }
  }
}