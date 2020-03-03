using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Scrubbler.Scrobbling.Scrobbler;

namespace Scrubbler.Helper.FileParser
{
  class JSONParserViewModel : PropertyChangedBase, IFileParserViewModel
  {
    public string Name => "JSON";

    public string FileFilter => "JSON Files (.json)|*.json";

    public IEnumerable<string> SupportedExtensions => new[] { ".json" };

    private IFileParser _parser;

    public JSONParserViewModel(IFileParser parser)
    {
      _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      return _parser.Parse(file, defaultDuration, scrobbleMode);
    }
  }
}
