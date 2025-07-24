using ScrubblerLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="ReleaseResultView"/>.
  /// </summary>
  public class ReleaseResultViewModel : ViewModelBase, IDisposable
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
    /// Fetched results.
    /// </summary>
    public IEnumerable<FetchedReleaseViewModel> Results { get; }

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
      if (releases == null)
        throw new ArgumentNullException(nameof(releases));

      FetchedThroughArtist = fetchedThroughArtist;
      Results = CreateResultViewModels(releases);
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

    private List<FetchedReleaseViewModel> CreateResultViewModels(IEnumerable<Release> releases)
    {
      var vms = new List<FetchedReleaseViewModel>();
      foreach (var release in releases)
      {
        var vm = new FetchedReleaseViewModel(release);
        vm.ReleaseClicked += Release_Clicked;
        vms.Add(vm);
      }

      return vms;
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
          foreach (var item in Results.ToList())
          {
            item.ReleaseClicked -= ReleaseClicked;
          }
        }

        _disposedValue = true;
      }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~ReleaseResultViewModel()
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