using IF.Lastfm.Core.Api;
using Scrubbler.Interfaces;
using System.Net.Http;

namespace Scrubbler.Models
{
  class ScrobblerFactory : IScrobblerFactory
  {
    public IAuthScrobbler CreateScrobbler(ILastAuth auth, HttpClient httpClient = null)
    {
      return new AuthScrobbler(auth);
    }

    public IAuthScrobbler CreateSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      return new AuthSQLiteScrobbler(auth, dbFile, httpClient);
    }

    /// <summary>
    /// Creates a scrobbler that checks <see cref="Models.User"/>
    /// scrobble count.
    /// </summary>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <param name="cachingScrobbler">Scrobbler that caches.</param>
    /// <returns>Newly created scrobbler.</returns>
    public IUserScrobbler CreateUserScrobbler(User user, IAuthScrobbler scrobbler, IAuthScrobbler cachingScrobbler)
    {
      return new UserScrobbler(user, scrobbler, cachingScrobbler);
    }
  }
}
