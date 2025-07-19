using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ScrubblerLib.Data;

namespace ScrubblerLib.Helper.FileParser
{
  class JSONFileParser : IFileParser<JSONFileParserConfiguration>
  {
    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode, JSONFileParserConfiguration config)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException(nameof(file));

      var errors = new List<string>();
      var settings = new JsonSerializerSettings()
      {
        ContractResolver = new CustomJSONContractResolver(config),
        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
          errors.Add($"Object Number: {args.ErrorContext.Member} | Error: {args.ErrorContext.Error.Message}");
          args.ErrorContext.Handled = true;
        },
        NullValueHandling = NullValueHandling.Ignore
      };

      var f = File.ReadAllText(file);
      var parsedscrobbles = JsonConvert.DeserializeObject<PlayLengthDatedScrobble[]>(f, settings) ?? throw new Exception("Unknown error parsing json file (corrupted?)");
      if (config.FilerShortPlayedSongs)
        parsedscrobbles = parsedscrobbles.Where(s => s.MillisecondsPlayed == null || s.MillisecondsPlayed >= config.MillisecondsPlayedTreshold)
                                         .ToArray();
      return new FileParseResult(parsedscrobbles, errors);
    }
  }
}