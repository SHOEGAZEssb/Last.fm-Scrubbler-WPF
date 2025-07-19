﻿using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using ScrubblerLib.Login;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScrubblerLib
{
  /// <summary>
  /// Interface for a scrobbler making sure
  /// that a <see cref="User"/> can not scrobble
  /// more than the scrobble cap.
  /// </summary>
  public interface IUserScrobbler
  {
    /// <summary>
    /// Gets if the scrobbler is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// User to check for scrobble cap.
    /// </summary>
    User User { get; }

    /// <summary>
    /// Scrobbles the given <paramref name="scrobble"/>.
    /// </summary>
    /// <param name="scrobble">Scrobble object to scrobble.</param>
    /// <returns>Response.</returns>
    Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble);

    /// <summary>
    /// Scrobbles the given <paramref name="scrobbles"/>.
    /// </summary>
    /// <param name="scrobbles">Collection of scrobble objects to scrobble.</param>
    /// <returns>Response.</returns>
    Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles);

    /// <summary>
    /// Sends the cached scrobbles.
    /// </summary>
    /// <returns>Response.</returns>
    Task<ScrobbleResponse> SendCachedScrobblesAsync();

    /// <summary>
    /// Gets the cached tracks.
    /// </summary>
    /// <returns>Cached tracks.</returns>
    Task<IEnumerable<Scrobble>> GetCachedAsync();
  }
}