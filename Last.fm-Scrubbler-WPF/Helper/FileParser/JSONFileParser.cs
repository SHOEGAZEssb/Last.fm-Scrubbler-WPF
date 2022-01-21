using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Scrubbler.Properties;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;

namespace Scrubbler.Helper.FileParser
{
  class JSONFileParser : IFileParser
  {
    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException(nameof(file));

      var errors = new List<string>();
      var settings = new JsonSerializerSettings()
      {
        ContractResolver = new CustomJSONContractResolver(),
        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
          errors.Add($"Object Number: {args.ErrorContext.Member} | Error: {args.ErrorContext.Error.Message}");
          args.ErrorContext.Handled = true;
        }
      };

      var parsedscrobbles = JsonConvert.DeserializeObject<PlayLengthDatedScrobble[]>(File.ReadAllText(file), settings);
      if (Settings.Default.JSONFilterShortPlayedSongs)
        parsedscrobbles = parsedscrobbles.Where(s => s.MillisecondsPlayed >= Settings.Default.JSONPlayedMillisecondsThreshold)
                                         .ToArray();
      return new FileParseResult(parsedscrobbles, errors);
    }
  }
}