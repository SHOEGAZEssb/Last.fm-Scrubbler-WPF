using System;

namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for an object that can be scrobbled.
  /// </summary>
  public interface IScrobbableObject
  {
    /// <summary>
    /// If true, this object should be
    /// scrobbled.
    /// </summary>
    bool ToScrobble { get; set; }

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/>
    /// changes.
    /// </summary>
    event EventHandler ToScrobbleChanged;
  }
}