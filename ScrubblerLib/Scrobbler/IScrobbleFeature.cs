using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrubblerLib.Scrobbler
{
  internal interface IScrobbleFeature
  {
    IEnumerable<Scrobble> CreateScrobbles();
  }
}