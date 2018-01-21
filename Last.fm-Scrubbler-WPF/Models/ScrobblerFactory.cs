using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using IF.Lastfm.SQLite;
using Last.fm_Scrubbler_WPF.Interfaces;
using System.Net.Http;

namespace Last.fm_Scrubbler_WPF.Models
{
  class ScrobblerFactory : IScrobblerFactory
  {
    public IAuthScrobbler CreateScrobbler(ILastAuth auth, HttpClient httpClient = null)
    {
      return new AuthScrobbler(auth);
    }

    public IAuthScrobbler CreateSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      return new AuthSQLiteScrobbler(auth, dbFile, httpClient);
    }
  }
}
