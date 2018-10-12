using Octokit;
using System.Collections.Generic;

namespace Scrubbler.Test.MockData
{
  /// <summary>
  /// A fake <see cref="Release"/>.
  /// </summary>
  class ReleaseMock : Release
  {
    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A fake name.</param>
    /// <param name="body">A fake body.</param>
    /// <param name="htmlUrl">A fake url.</param>
    /// <param name="asset">A fake asset.</param>
    /// <param name="tagName">A fake tag name.</param>
    public ReleaseMock(string name, string body, string htmlUrl, ReleaseAssetMock asset, string tagName)
    {
      Name = name;
      Body = body;
      HtmlUrl = htmlUrl;
      Assets = new List<ReleaseAsset>(new[] { asset });
      TagName = tagName;
    }

    #endregion Construction
  }
}