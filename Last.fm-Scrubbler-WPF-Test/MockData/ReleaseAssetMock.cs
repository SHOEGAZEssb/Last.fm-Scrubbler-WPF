using Octokit;

namespace Scrubbler.Test.MockData
{
  /// <summary>
  /// A fake <see cref="ReleaseAsset"/>.
  /// </summary>
  class ReleaseAssetMock : ReleaseAsset
  {
    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="browserDownloadUrl">A fake url.</param>
    public ReleaseAssetMock(string browserDownloadUrl)
    {
      BrowserDownloadUrl = browserDownloadUrl;
    }

    #endregion Construction
  }
}