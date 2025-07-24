using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;

namespace ScrubblerLib
{
  /// <summary>
  /// Interface for a <see cref="IScrobbler"/> that can
  /// be authenticated.
  /// </summary>
  public interface IAuthScrobbler : IScrobbler
  {
    /// <summary>
    /// Authentication.
    /// </summary>
    ILastAuth Auth { get; }
  }
}