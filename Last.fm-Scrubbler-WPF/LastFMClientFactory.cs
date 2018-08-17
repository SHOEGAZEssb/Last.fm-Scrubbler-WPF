using System.Net.Http;

namespace Scrubbler
{
  /// <summary>
  /// Factory for creating <see cref="LastFMClient"/>s.
  /// </summary>
  class LastFMClientFactory : ILastFMClientFactory
  {
    /// <summary>
    /// Creates a new LastFMClient.
    /// </summary>
    /// <param name="apiKey">API key.</param>
    /// <param name="apiSecret">API secret.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns>Newly created LastFMClient.</returns>
    public ILastFMClient CreateClient(string apiKey, string apiSecret, HttpClient httpClient = null)
    {
      return new LastFMClient(apiKey, apiSecret, httpClient);
    }
  }
}