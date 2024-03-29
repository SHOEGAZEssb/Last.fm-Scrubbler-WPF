using Newtonsoft.Json.Serialization;
using Scrubbler.Scrobbling.Data;
using System.Collections.Generic;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Custom json property resolver.
  /// </summary>
  class CustomJSONContractResolver : DefaultContractResolver
  {
    /// <summary>
    /// Dictionary containing the mapped properties.
    /// </summary>
    private readonly Dictionary<string, string> _propertyMappings;

    /// <summary>
    /// Constructor.
    /// </summary>
    public CustomJSONContractResolver()
    {
      _propertyMappings = new Dictionary<string, string>()
      {
        { nameof(DatedScrobble.TrackName), Properties.Settings.Default.JSONTrackNameProperty },
        { nameof(DatedScrobble.ArtistName), Properties.Settings.Default.JSONArtistNameProperty },
        { nameof(DatedScrobble.AlbumName), Properties.Settings.Default.JSONAlbumNameProperty },
        { nameof(DatedScrobble.AlbumArtist), Properties.Settings.Default.JSONAlbumArtistNameProperty },
        { nameof(DatedScrobble.Played), Properties.Settings.Default.JSONTimestampProperty },
        { nameof(DatedScrobble.Duration), Properties.Settings.Default.JSONDurationProperty },
        { nameof(PlayLengthDatedScrobble.MillisecondsPlayed), Properties.Settings.Default.JSONMillisecondsPlayedProperty }
      };
    }

    /// <summary>
    /// Resolves the name of the property with the given <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">Name of the property to resolve.</param>
    /// <returns>Resolved name.</returns>
    protected override string ResolvePropertyName(string propertyName)
    {
      return _propertyMappings.TryGetValue(propertyName, out string resolvedName) ? resolvedName : base.ResolvePropertyName(propertyName);
    }
  }
}