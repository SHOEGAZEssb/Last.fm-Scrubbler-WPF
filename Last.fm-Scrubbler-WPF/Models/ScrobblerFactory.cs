using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using IF.Lastfm.SQLite;
using Last.fm_Scrubbler_WPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.Models
{
  class ScrobblerFactory : IScrobblerFactory
  {
    public IScrobbler CreateScrobbler(ILastAuth auth, HttpClient httpClient = null)
    {
      return new Scrobbler(auth);
    }

    public IScrobbler CreateSQLiteScrobbler(ILastAuth auth, string dbFile, HttpClient httpClient = null)
    {
      return new SQLiteScrobbler(auth, dbFile, httpClient);
    }
  }
}
