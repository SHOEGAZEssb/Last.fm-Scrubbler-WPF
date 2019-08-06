using Newtonsoft.Json.Serialization;
using Scrubbler.Scrobbling.Data;
using System.Collections.Generic;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Custom json property resolver.
  /// </summary>
  class CustomContractResolver : DefaultContractResolver
  {
    /// <summary>
    /// Dictionary containing the mapped properties.
    /// </summary>
    private readonly IDictionary<string, string> _propertyMappings;

    /// <summary>
    /// Constructor.
    /// </summary>
    public CustomContractResolver()
    {
      _propertyMappings = new Dictionary<string, string>()
      {
        { nameof(DatedScrobble.Played), "time" }
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