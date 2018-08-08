using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scrubbler.Scrobbling
{
  /// <summary>
  /// Scrobbler that is authenticated
  /// via a <see cref="ILastAuth"/>.
  /// </summary>
  class AuthScrobbler : IAuthScrobbler, IDisposable
  {
    #region Properties

    /// <summary>
    /// Last.fm authentication object.
    /// </summary>
    public ILastAuth Auth => _scrobbler.Auth;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobbler.
    /// </summary>
    private MemoryScrobbler _scrobbler;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="auth">Last.fm authentication object.</param>
    /// <param name="httpClient">HttpClient to use for connections.</param>
    public AuthScrobbler(ILastAuth auth, HttpClient httpClient = null)
    {
      _scrobbler = new MemoryScrobbler(auth, httpClient);
    }

    /// <summary>
    /// Gets the cached tracks.
    /// </summary>
    /// <returns>Cached tracks.</returns>
    public Task<IEnumerable<Scrobble>> GetCachedAsync()
    {
      return _scrobbler.GetCachedAsync();
    }

    /// <summary>
    /// Scrobbles the given <paramref name="scrobble"/>.
    /// </summary>
    /// <param name="scrobble">Scrobble object to scrobble.</param>
    /// <returns>Response.</returns>
    public Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble)
    {
      return _scrobbler.ScrobbleAsync(scrobble);
    }

    /// <summary>
    /// Scrobbles the given <paramref name="scrobbles"/>.
    /// </summary>
    /// <param name="scrobbles">Collection of scrobble objects to scrobble.</param>
    /// <returns>Response.</returns>
    public Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles)
    {
      return _scrobbler.ScrobbleAsync(scrobbles);
    }

    /// <summary>
    /// Sends the cached scrobbles.
    /// </summary>
    /// <returns>Response.</returns>
    public Task<ScrobbleResponse> SendCachedScrobblesAsync()
    {
      return _scrobbler.SendCachedScrobblesAsync();
    }

    /// <summary>
    /// Disposes this scrobbler.
    /// </summary>
    public void Dispose()
    {
      _scrobbler.Dispose();
    }
  }
}