using IF.Lastfm.Core.Objects;
using System.Collections.Generic;

namespace ScrubblerLib.Scrobbler
{
  internal interface IScrobbleFeature
  {
    IEnumerable<Scrobble> CreateScrobbles();
  }
}