using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        ContractResolver = new CustomContractResolver(),
        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
          errors.Add($"Object Number: {args.ErrorContext.Member} | Error: {args.ErrorContext.Error.Message}");
          args.ErrorContext.Handled = true;
        }
      };

      var parsedscrobbles = JsonConvert.DeserializeObject<DatedScrobble[]>(File.ReadAllText(file), settings);
      return new FileParseResult(parsedscrobbles, errors);
    }
  }
}