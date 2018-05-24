using IF.Lastfm.Core.Api;
using Scrubbler.Models;
using System.Net.Http;

namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for a factory that creates scrobblers.
  /// </summary>
  public interface IScrobblerFactory
  {
    /// <summary>
    /// Creates a normal scrobbler.
    /// </summary>
    /// <param name="auth">Authentication.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns>Newly created scrobbler.</returns>
    IAuthScrobbler CreateScrobbler(ILastAuth auth, HttpClient httpClient = null);

    /// <summary>
    /// Creates a scrobbler that caches into a sqlite database.
    /// </summary>
    /// <param name="auth">Authentication.</param>
    /// <param name="dbFile">Database file to write cached scrobbles to.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns>Newly created scrobbler.</returns>
    IAuthScrobbler CreateSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null);

    /// <summary>
    /// Creates a scrobbler that checks <see cref="User"/>
    /// scrobble count.
    /// </summary>
    /// <param name="user">User to check scrobbles for.</param>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <param name="cachingScrobbler">Scrobbler that caches.</param>
    /// <returns>Newly created scrobbler.</returns>
    IUserScrobbler CreateUserScrobbler(User user, IAuthScrobbler scrobbler, IAuthScrobbler cachingScrobbler);
  }
}