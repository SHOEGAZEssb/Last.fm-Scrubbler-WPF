using Scrubbler.Interfaces;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Interface indicating the need for a scrobbler.
  /// </summary>
  interface INeedScrobbler
  {
    IAuthScrobbler Scrobbler { get; set; }
  }
}