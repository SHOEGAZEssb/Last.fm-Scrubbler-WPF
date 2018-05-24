using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Scrubbler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrubbler.Models
{
  class UserScrobbler : IUserScrobbler
  {
    public User User { get; private set; }

    public bool IsAuthenticated => _scrobbler.Auth.Authenticated && _cachingScrobbler.Auth.Authenticated;

    #region Member

    private IAuthScrobbler _scrobbler;
    private IAuthScrobbler _cachingScrobbler;

    #endregion Member

    public UserScrobbler(User user, IAuthScrobbler scrobbler, IAuthScrobbler cachingScrobbler)
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      _scrobbler = scrobbler ?? throw new ArgumentNullException(nameof(scrobbler));
      _cachingScrobbler = cachingScrobbler ?? throw new ArgumentNullException(nameof(cachingScrobbler));
    }

    public async Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble, bool needCaching = false)
    {
      return await ScrobbleAsync(new[] { scrobble }, needCaching);
    }

    public async Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles, bool needCaching = false)
    {
      User.UpdateRecentScrobbles();
      if (User.RecentScrobbles.Count + scrobbles.Count() > User.MAXSCROBBLESPERDAY)
        throw new Exception(string.Format("Scrobbling these tracks would break the daily scrobble cap! ({0})\r\nYou have scrobbled {1} tracks in the past 24 hours.",
                                          User.MAXSCROBBLESPERDAY, User.RecentScrobbles.Count));

      ScrobbleResponse response;
      if (needCaching)
        response = await _cachingScrobbler.ScrobbleAsync(scrobbles);
      else
        response = await _scrobbler.ScrobbleAsync(scrobbles);

      if (response.Success && response.Status == LastResponseStatus.Successful)
        User.AddScrobbles(scrobbles);
      return response;
    }

    public async Task<ScrobbleResponse> SendCachedScrobblesAsync()
    {
      return await _cachingScrobbler.SendCachedScrobblesAsync();
    }

    public async Task<IEnumerable<Scrobble>> GetCachedAsync()
    {
      return await _cachingScrobbler.GetCachedAsync();
    }
  }
}
