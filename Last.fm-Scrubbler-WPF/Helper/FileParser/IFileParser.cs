using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Interface for an object that parses files to scrobbles.
  /// </summary>
  public interface IFileParser
  {
    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="scrobbleMode">Scrobble mode to use.</param>
    /// <returns>Parse result.</returns>
    FileParseResult Parse(string file, ScrobbleMode scrobbleMode);
  }
}