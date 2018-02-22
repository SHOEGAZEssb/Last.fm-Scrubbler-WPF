using Last.fm_Scrubbler_WPF.Interfaces;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Interface indicating the need for a scrobbler.
  /// </summary>
  interface INeedScrobbler
  {
    IAuthScrobbler Scrobbler { get; set; }
  }
}