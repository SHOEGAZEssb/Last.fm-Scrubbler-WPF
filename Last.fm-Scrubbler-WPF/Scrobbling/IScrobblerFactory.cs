using IF.Lastfm.Core.Api;
using Scrubbler.Login;
using System.Net.Http;

namespace Scrubbler.Scrobbling
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
    /// Creates a scrobbler that checks <see cref="User"/>
    /// scrobble count.
    /// </summary>
    /// <param name="user">User to check scrobbles for.</param>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <returns>Newly created scrobbler.</returns>
    IUserScrobbler CreateUserScrobbler(User user, IAuthScrobbler scrobbler);
  }
}