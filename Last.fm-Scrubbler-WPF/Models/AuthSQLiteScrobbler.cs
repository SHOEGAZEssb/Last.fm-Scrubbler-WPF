using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using IF.Lastfm.SQLite;
using Scrubbler.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scrubbler.Models
{
  /// <summary>
  /// Scrobbler that is authenticated
  /// via a <see cref="ILastAuth"/> and can
  /// cache scrobbles.
  /// </summary>
  class AuthSQLiteScrobbler : IAuthScrobbler, IDisposable
  {
    #region Properties

    /// <summary>
    /// Last.fm authentication object.
    /// </summary>
    public ILastAuth Auth => _scrobbler.Auth;

    /// <summary>
    /// If the scrobbler caches scrobbles.
    /// </summary>
    public bool CacheEnabled => _scrobbler.CacheEnabled;

    #endregion Properties

    #region Member

    /// <summary>
    /// The actual scrobbler.
    /// </summary>
    private SQLiteScrobbler _scrobbler;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="auth">Last.fm authentication object.</param>
    /// <param name="dbFile">Path of the cache database file.</param>
    /// <param name="httpClient">HttpClient to use for connections.</param>
    public AuthSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      _scrobbler = new SQLiteScrobbler(auth, dbFile, httpClient);
    }

    #endregion Construction

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