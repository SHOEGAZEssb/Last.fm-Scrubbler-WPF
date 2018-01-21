using System.Net.Http;

namespace Last.fm_Scrubbler_WPF.Interfaces
{
  /// <summary>
  /// Factory creating a LastFMClient.
  /// </summary>
  public interface ILastFMClientFactory
  {
    /// <summary>
    /// Creates a new LastFMClient.
    /// </summary>
    /// <param name="apiKey">API key.</param>
    /// <param name="apiSecret">API secret.</param>
    /// <param name="httpClient">HttpClient used for requests.</param>
    /// <returns></returns>
    ILastFMClient CreateClient(string apiKey, string apiSecret, HttpClient httpClient = null);
  }
}