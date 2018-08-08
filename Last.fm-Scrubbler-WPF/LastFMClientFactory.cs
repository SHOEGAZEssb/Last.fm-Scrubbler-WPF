using System.Net.Http;

namespace Scrubbler
{
  class LastFMClientFactory : ILastFMClientFactory
  {
    public ILastFMClient CreateClient(string apiKey, string apiSecret, HttpClient httpClient = null)
    {
      return new LastFMClient(apiKey, apiSecret, httpClient);
    }
  }
}
