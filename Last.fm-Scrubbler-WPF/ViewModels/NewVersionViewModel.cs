using Caliburn.Micro;
using Octokit;
using System;
using System.Diagnostics;

namespace Scrubbler.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.NewVersionView"/>.
  /// </summary>
  class NewVersionViewModel : Screen
  {
    #region Properties

    /// <summary>
    /// Full name of the new version.
    /// </summary>
    public string VersionName => _newRelease.Name;

    /// <summary>
    /// Description of the new release.
    /// </summary>
    public string Description => _newRelease.Body;

    #endregion Properties

    #region Member

    /// <summary>
    /// The new release.
    /// </summary>
    private Release _newRelease;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="newRelease">The new release.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="newRelease"/> is null.</exception>
    public NewVersionViewModel(Release newRelease)
    {
      _newRelease = newRelease ?? throw new ArgumentNullException(nameof(newRelease));
    }

    #endregion Construction

    /// <summary>
    /// Opens the release page.
    /// </summary>
    public void OpenReleasePage()
    {
      Process.Start(_newRelease.HtmlUrl);
    }

    /// <summary>
    /// Opens the direct download page of
    /// the release.
    /// </summary>
    public void DownloadRelease()
    {
      Process.Start(_newRelease.Assets[0].BrowserDownloadUrl);
    }

    /// <summary>
    /// Closes this screen.
    /// </summary>
    public void Cancel()
    {
      TryClose(false);
    }
  }
}