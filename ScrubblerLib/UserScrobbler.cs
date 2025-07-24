using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using ScrubblerLib.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrubblerLib
{
  /// <summary>
  /// Scrobbler making sure
  /// that a <see cref="User"/> can not scrobble
  /// more than the scrobble cap.
  /// </summary>
  public class UserScrobbler : IUserScrobbler
  {
    #region Properties

    /// <summary>
    /// User to check for scrobble cap.
    /// </summary>
    public User User { get; private set; }

    /// <summary>
    /// Gets if the scrobbler is authenticated.
    /// </summary>
    public bool IsAuthenticated => _scrobbler.Auth.Authenticated;

    #endregion Properties

    #region Member

    /// <summary>
    /// Actual normal scrobbler.
    /// </summary>
    private readonly IAuthScrobbler _scrobbler;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="user">User to count scrobbles for.</param>
    /// <param name="scrobbler">Normal scrobbler.</param>
    public UserScrobbler(User user, IAuthScrobbler scrobbler)
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      _scrobbler = scrobbler ?? throw new ArgumentNullException(nameof(scrobbler));
    }

    #endregion Construction

    /// <summary>
    /// Scrobbles the given <paramref name="scrobble"/>.
    /// </summary>
    /// <param name="scrobble">Scrobble object to scrobble.</param>
    /// <returns>Response.</returns>
    public async Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble)
    {
      return await ScrobbleAsync(new[] { scrobble });
    }

    /// <summary>
    /// Scrobbles the given <paramref name="scrobbles"/>.
    /// </summary>
    /// <param name="scrobbles">Collection of scrobble objects to scrobble.</param>
    /// <returns>Response.</returns>
    public async Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles)
    {
      await User.UpdateRecentScrobbles();
      int recentScrobblesCount = User.RecentScrobblesCache.Count();
      if (recentScrobblesCount + scrobbles.Count() > User.MAXSCROBBLESPERDAY)
        throw new InvalidOperationException($"Scrobbling these tracks would break the daily scrobble cap! " +
                                            $"({User.MAXSCROBBLESPERDAY})\r\nYou have scrobbled {recentScrobblesCount} tracks in the past 24 hours.");

      ScrobbleResponse response = await _scrobbler.ScrobbleAsync(scrobbles);
      if (response.Success && response.Status == LastResponseStatus.Successful)
        await User.UpdateRecentScrobbles();

      return response;
    }

    /// <summary>
    /// Sends the cached scrobbles.
    /// </summary>
    /// <returns>Response.</returns>
    public async Task<ScrobbleResponse> SendCachedScrobblesAsync()
    {
      return await _scrobbler.SendCachedScrobblesAsync();
    }

    /// <summary>
    /// Gets the cached tracks.
    /// </summary>
    /// <returns>Cached tracks.</returns>
    public async Task<IEnumerable<Scrobble>> GetCachedAsync()
    {
      return await _scrobbler.GetCachedAsync();
    }
  }
}