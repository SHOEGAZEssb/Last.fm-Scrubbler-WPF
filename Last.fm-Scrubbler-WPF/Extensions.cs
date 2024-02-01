using Hqub.MusicBrainz.API.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Scrubbler
{
  /// <summary>
  /// Extension methods for <see cref="Task"/>s.
  /// </summary>
  static class TastExtensions
  {
    /// <summary>
    /// Explicitly states that we don't want to
    /// do anything with the result of a task.
    /// </summary>
    /// <param name="t">Task whose result to forget.</param>
#pragma warning disable IDE0060 // Remove unused parameter
    public static void Forget(this Task t)
#pragma warning restore IDE0060 // Remove unused parameter
    { }
  }

  static class MusicBrainzReleaseExtensions
  {
    public static Uri GetMusicBrainzCoverArtUri(this ReleaseGroup release)
    {
      if (release.Releases.First()?.CoverArtArchive?.Front ?? false)
      {
        string url = $"https://coverartarchive.org/release/{release.Id}/front-250.jpg";
        return new Uri(url, UriKind.RelativeOrAbsolute);
      }

      return null;
    }
  }
}