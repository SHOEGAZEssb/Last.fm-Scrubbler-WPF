using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Scrubbler.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.Interfaces
{
  public interface IUserScrobbler
  {
    bool IsAuthenticated { get; }
    User User { get; }
    Task<ScrobbleResponse> ScrobbleAsync(Scrobble scrobble, bool needCaching = false);
    Task<ScrobbleResponse> ScrobbleAsync(IEnumerable<Scrobble> scrobbles, bool needCaching = false);
    Task<ScrobbleResponse> SendCachedScrobblesAsync();
    Task<IEnumerable<Scrobble>> GetCachedAsync();
  }
}