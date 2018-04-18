using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;

namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for a client communicating with the last.fm api.
  /// </summary>
  public interface ILastFMClient : ILastFMApiBase
  {
    /// <summary>
    /// Album API.
    /// </summary>
    IAlbumApi Album { get; }

    /// <summary>
    /// Artist API.
    /// </summary>
    IArtistApi Artist { get; }

    /// <summary>
    /// Chart API.
    /// </summary>
    IChartApi Chart { get; }

    /// <summary>
    /// Library API.
    /// </summary>
    ILibraryApi Library { get; }

    /// <summary>
    /// Tag API.
    /// </summary>
    ITagApi Tag { get; }

    /// <summary>
    /// Track API.
    /// </summary>
    ITrackApi Track { get; }

    /// <summary>
    /// User API.
    /// </summary>
    IUserApi User { get; }

    /// <summary>
    /// Scrobbler used.
    /// </summary>
    IScrobbler Scrobbler { get; set; }
  }
}