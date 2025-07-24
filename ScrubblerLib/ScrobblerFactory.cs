using IF.Lastfm.Core.Api;
using ScrubblerLib.Login;
using System.Net.Http;

namespace ScrubblerLib
{
  public class ScrobblerFactory : IScrobblerFactory
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
    /// Creates a scrobbler that checks <see cref="User"/>
    /// scrobble count.
    /// </summary>
    /// <param name="user">User to count scrobbles for.</param>
    /// <param name="scrobbler">Normal scrobbler.</param>
    /// <returns>Newly created scrobbler.</returns>
    public IUserScrobbler CreateUserScrobbler(User user, IAuthScrobbler scrobbler)
    {
      return new UserScrobbler(user, scrobbler);
    }
  }
}