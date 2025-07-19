using Caliburn.Micro;
using ScrubblerLib.Data;
using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for the <see cref="FetchedReleaseView"/>.
  /// </summary>
  public class FetchedReleaseViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Event that triggers when this release is clicked on the UI.
    /// </summary>
    public event EventHandler ReleaseClicked;

    /// <summary>
    /// Name of this release.
    /// </summary>
    public string Name => _fetchedRelease.Name;

    /// <summary>
    /// Image of this release.
    /// </summary>
    public Uri Image => _fetchedRelease.Image;

    #endregion Properties

    #region Member

    /// <summary>
    /// The fetched release.
    /// </summary>
    private readonly Release _fetchedRelease;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fetchedRelease">The fetched release.</param>
    public FetchedReleaseViewModel(Release fetchedRelease)
    {
      _fetchedRelease = fetchedRelease;
    }

    /// <summary>
    /// Triggers the <see cref="ReleaseClicked"/> event.
    /// </summary>
    public void Clicked()
    {
      ReleaseClicked?.Invoke(_fetchedRelease, EventArgs.Empty);
    }
  }
}