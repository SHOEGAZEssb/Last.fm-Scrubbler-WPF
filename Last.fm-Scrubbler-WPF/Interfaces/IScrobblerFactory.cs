using IF.Lastfm.Core.Api;
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
  }
}