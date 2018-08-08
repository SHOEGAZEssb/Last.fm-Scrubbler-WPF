using IF.Lastfm.Core.Api;
using Scrubbler.Login;
using System.Net.Http;

namespace Scrubbler.Scrobbling
{
  class ScrobblerFactory : IScrobblerFactory
  {
    /// <summary>
    /// Creates a normal scrobbler.
    /// </summary>
    /// <param name="auth">Authentication.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns>Newly created scrobbler.</returns>
    public IAuthScrobbler CreateScrobbler(ILastAuth auth, HttpClient httpClient = null)
    {
      return new AuthScrobbler(auth);
    }

    /// <summary>
    /// Creates a scrobbler that caches into a sqlite database.
    /// </summary>
    /// <param name="auth">Authentication.</param>
    /// <param name="dbFile">Database file to write cached scrobbles to.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns>Newly created scrobbler.</returns>
    public IAuthScrobbler CreateSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      return new AuthSQLiteScrobbler(auth, dbFile, httpClient);
    }

    /// <summary>
    /// Creates a scrobbler that checks <see cref="User"/>
    /// scrobble count.
    /// </summary>
    /// <param name="user">User to count scrobbles for.</param>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <param name="cachingScrobbler">Scrobbler that caches.</param>
    /// <returns>Newly created scrobbler.</returns>
    public IUserScrobbler CreateUserScrobbler(User user, IAuthScrobbler scrobbler, IAuthScrobbler cachingScrobbler)
    {
      return new UserScrobbler(user, scrobbler, cachingScrobbler);
    }
  }
}