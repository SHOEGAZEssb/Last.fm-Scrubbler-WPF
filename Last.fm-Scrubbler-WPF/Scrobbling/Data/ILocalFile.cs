using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// Interface for a local audio file.
  /// </summary>
  public interface ILocalFile
  {
    /// <summary>
    /// The artist name.
    /// </summary>
    string Artist { get; }

    /// <summary>
    /// The album name.
    /// </summary>
    string Album { get; }

    /// <summary>
    /// The track name.
    /// </summary>
    string Track { get; }

    /// <summary>
    /// The album artist name.
    /// </summary>
    string AlbumArtist { get; }

    /// <summary>
    /// The duration of the track.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// The number of the track.
    /// </summary>
    int TrackNumber { get; }
  }
}