using Octokit;
using System;

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
      : base("", 0, "", "", "", "", "", 0, 0, DateTimeOffset.Now, DateTimeOffset.Now, browserDownloadUrl, null)
    { }

    #endregion Construction
  }
}