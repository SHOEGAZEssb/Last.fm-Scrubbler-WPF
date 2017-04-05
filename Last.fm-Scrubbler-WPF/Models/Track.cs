using System;

namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// Track of a release.
  /// </summary>
  class Track
  {
    #region Properties

    /// <summary>
    /// Name of this track.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Name of the artist of this track.
    /// </summary>
    public string ArtistName { get; private set; }

    /// <summary>
    /// Name of the release this track is on.
    /// </summary>
    public string ReleaseName { get; private set; }

    /// <summary>
    /// Length of this track.
    /// </summary>
    public TimeSpan? Duration { get; private set; }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of this track.</param>
    /// <param name="artistName">Name of the artist of this track.</param>
    /// <param name="releaseName">Name of the release this track is on.</param>
    /// <param name="duration">Length of this track.</param>
    public Track(string name, string artistName, string releaseName, TimeSpan? duration)
    {
      Name = name;
      ArtistName = artistName;
      ReleaseName = releaseName;
      Duration = duration;
    }
  }
}