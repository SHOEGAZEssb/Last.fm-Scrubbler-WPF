namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface indicating the need for a scrobbler.
  /// </summary>
  interface INeedScrobbler
  {
    IAuthScrobbler Scrobbler { get; set; }
  }
}