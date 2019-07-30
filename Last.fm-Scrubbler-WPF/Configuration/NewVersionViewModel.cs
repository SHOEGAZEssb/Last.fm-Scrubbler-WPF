using Caliburn.Micro;
using Octokit;
using Scrubbler.Helper;
using System;

namespace Scrubbler.Configuration
{
  /// <summary>
  /// ViewModel for the <see cref="NewVersionView"/>.
  /// </summary>
  public class NewVersionViewModel : Screen
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
    private readonly Release _newRelease;

    /// <summary>
    /// ProcessManager for working with processor functions.
    /// </summary>
    private readonly IProcessManager _processManager;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="newRelease">The new release.</param>
    /// <param name="processManager">ProcessManager for working with processor functions.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="newRelease"/> is null.</exception>
    public NewVersionViewModel(Release newRelease, IProcessManager processManager)
    {
      _newRelease = newRelease ?? throw new ArgumentNullException(nameof(newRelease));
      _processManager = processManager ?? throw new ArgumentNullException(nameof(processManager));
    }

    #endregion Construction

    /// <summary>
    /// Opens the release page.
    /// </summary>
    public void OpenReleasePage()
    {
      _processManager.Start(_newRelease.HtmlUrl);
    }

    /// <summary>
    /// Opens the direct download page of
    /// the release.
    /// </summary>
    public void DownloadRelease()
    {
      _processManager.Start(_newRelease.Assets[0].BrowserDownloadUrl);
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