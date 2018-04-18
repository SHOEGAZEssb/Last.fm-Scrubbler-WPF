using IF.Lastfm.Core.Api;
using Scrubbler.Interfaces;
using System.Net.Http;

namespace Scrubbler.Models
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
