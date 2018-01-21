using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;

namespace Last.fm_Scrubbler_WPF.Interfaces
{
  public interface IAuthScrobbler : IScrobbler
  {
    ILastAuth Auth { get; }
  }
}