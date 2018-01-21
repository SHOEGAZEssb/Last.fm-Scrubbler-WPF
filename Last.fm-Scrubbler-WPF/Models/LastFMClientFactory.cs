using Last.fm_Scrubbler_WPF.Interfaces;
using System.Net.Http;

namespace Last.fm_Scrubbler_WPF.Models
{
  class LastFMClientFactory : ILastFMClientFactory
  {
    public ILastFMClient CreateClient(string apiKey, string apiSecret, HttpClient httpClient = null)
    {
      return new LastFMClient(apiKey, apiSecret, httpClient);
    }
  }
}
