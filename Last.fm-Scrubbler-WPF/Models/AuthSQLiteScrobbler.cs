using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using IF.Lastfm.SQLite;
using Scrubbler.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scrubbler.Models
{
  class AuthSQLiteScrobbler : IAuthScrobbler
  {
    public ILastAuth Auth => _scrobbler.Auth;

    public bool CacheEnabled => _scrobbler.CacheEnabled;

    private SQLiteScrobbler _scrobbler;

    public Task<IEnumerable<Scrobble>> GetCachedAsync()
    {
      return _scrobbler.GetCachedAsync();
    }

    public Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble)
    {
      return _scrobbler.ScrobbleAsync(scrobble);
    }

    public Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles)
    {
      return _scrobbler.ScrobbleAsync(scrobbles);
    }

    public Task<ScrobbleResponse> SendCachedScrobblesAsync()
    {
      return _scrobbler.SendCachedScrobblesAsync();
    }

    public AuthSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      _scrobbler = new SQLiteScrobbler(auth, dbFile, httpClient);
    }
  }
}
