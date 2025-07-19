using Newtonsoft.Json.Serialization;
using ScrubblerLib.Data;
using System.Collections.Generic;

namespace ScrubblerLib.Helper.FileParser
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
    public CustomJSONContractResolver(JSONFileParserConfiguration config)
    {
      _propertyMappings = new Dictionary<string, string>()
      {
        { nameof(DatedScrobble.TrackName), config.TrackFieldName },
        { nameof(DatedScrobble.ArtistName), config.ArtistFieldName },
        { nameof(DatedScrobble.AlbumName), config.AlbumFieldName },
        { nameof(DatedScrobble.AlbumArtist), config.AlbumArtistFieldName },
        { nameof(DatedScrobble.Played), config.TimestampFieldName },
        { nameof(DatedScrobble.Duration), config.DurationFieldName },
        { nameof(PlayLengthDatedScrobble.MillisecondsPlayed), config.MillisecondsPlayedFieldName }
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