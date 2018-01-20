using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Scrobblers;
using Last.fm_Scrubbler_WPF.Interfaces;
using System.Net.Http;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// Implementation of the <see cref="LastfmClient"/>
  /// using the <see cref="ILastFMClient"/> interface.
  /// </summary>
  class LastFMClient : ILastFMClient
  {
    /// <summary>
    /// Album API.
    /// </summary>
    public IAlbumApi Album => _client.Album;

    /// <summary>
    /// Artist API.
    /// </summary>
    public IArtistApi Artist => _client.Artist;

    /// <summary>
    /// Chart API.
    /// </summary>
    public IChartApi Chart => _client.Chart;

    /// <summary>
    /// Library API.
    /// </summary>
    public ILibraryApi Library => _client.Library;

    /// <summary>
    /// Tag API.
    /// </summary>
    public ITagApi Tag => _client.Tag;

    /// <summary>
    /// Track API.
    /// </summary>
    public ITrackApi Track => _client.Track;

    /// <summary>
    /// User API.
    /// </summary>
    public IUserApi User => _client.User;

    /// <summary>
    /// Scrobbler used.
    /// </summary>
    public IScrobbler Scrobbler { get => _client.Scrobbler; set => _client.Scrobbler = (ScrobblerBase)value; }

    /// <summary>
    /// Authentication.
    /// </summary>
    public ILastAuth Auth => _client.Auth;

    /// <summary>
    /// HttpClient used for requests.
    /// </summary>
    public HttpClient HttpClient => _client.HttpClient;

    /// <summary>
    /// The last.fm client.
    /// </summary>
    private LastfmClient _client;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="apiKey">API key.</param>
    /// <param name="apiSecret">API secret.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    public LastFMClient(string apiKey, string apiSecret, HttpClient httpClient = null)
    {
      _client = new LastfmClient(apiKey, apiSecret, httpClient);
    }

    /// <summary>
    /// Disposes the last.fm client.
    /// </summary>
    public void Dispose()
    {
      _client?.Dispose();
    }
  }
}