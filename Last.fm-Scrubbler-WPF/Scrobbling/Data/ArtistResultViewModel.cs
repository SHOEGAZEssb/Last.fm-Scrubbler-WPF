using ScrubblerLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="ArtistResultView"/>.
  /// </summary>
  public class ArtistResultViewModel : ViewModelBase, IDisposable
  {
    /// <summary>
    /// Event that fires when one of the displayed
    /// artists is clicked.
    /// </summary>
    public event EventHandler ArtistClicked;

    /// <summary>
    /// The fetched results.
    /// </summary>
    public IEnumerable<FetchedArtistViewModel> Results { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="artists">Fetched artists.</param>
    public ArtistResultViewModel(IEnumerable<Artist> artists)
    {
      if (artists == null)
        throw new ArgumentNullException(nameof(artists));

      Results = CreateResultViewModels(artists);
    }

    private List<FetchedArtistViewModel> CreateResultViewModels(IEnumerable<Artist> artists)
    {
      var results = new List<FetchedArtistViewModel>();
      foreach (var artist in artists)
      {
        var vm = new FetchedArtistViewModel(artist);
        vm.ArtistClicked += Artist_Clicked;
        results.Add(vm);
      }

      return results;
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
            item.ArtistClicked -= ArtistClicked;
          }
        }

        _disposedValue = true;
      }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~ArtistResultViewModel()
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