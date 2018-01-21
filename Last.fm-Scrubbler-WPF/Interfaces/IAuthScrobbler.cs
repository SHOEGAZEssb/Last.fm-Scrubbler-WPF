using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.Interfaces
{
  public interface IAuthScrobbler : IScrobbler
  {
    ILastAuth Auth { get; }
  }
}