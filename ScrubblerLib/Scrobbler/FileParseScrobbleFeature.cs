using IF.Lastfm.Core.Objects;
using ScrubblerLib.Helper.FileParser;
using System;
using System.Collections.Generic;

namespace ScrubblerLib.Scrobbler
{
  public class FileParseScrobbleFeature : IScrobbleFeature
  {
    #region Properties

    public string FilePath { get; set; }

    public IFileParserConfiguration ParserConfiguration { get; set; }

    public IEnumerable<string> LastParseErrors { get; private set; }

    #endregion Properties

    public IEnumerable<Scrobble> CreateScrobbles()
    {
      FileParseResult result;
      if (ParserConfiguration is CSVFileParserConfiguration csvFileParserConfiguration)
      {
        var parser = new CSVFileParser();
        result = parser.Parse(FilePath, csvFileParserConfiguration);

      }
      else if (ParserConfiguration is JSONFileParserConfiguration jsonFileParserConfiguration)
      {
        var parser = new JSONFileParser();
        result = parser.Parse(FilePath, jsonFileParserConfiguration);
      }
      else
        throw new ArgumentException("Unsupported parser configuration");

      LastParseErrors = result.Errors;
      return (IEnumerable<Scrobble>)result.Scrobbles;
    }
  }
}
