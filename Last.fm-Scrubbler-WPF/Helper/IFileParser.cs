using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;

namespace Scrubbler.Helper
{
  interface IFileParser
  {
    FileParseResult Parse(string file, CSVScrobbleMode scrobbleMode);
  }
}