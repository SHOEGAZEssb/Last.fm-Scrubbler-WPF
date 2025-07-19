using System;

namespace ScrubblerLib.Helper.FileParser
{
  /// <summary>
  /// Interface for an object that parses files to scrobbles.
  /// </summary>
  public interface IFileParser<T> where T : IFileParserConfiguration
  {
    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="defaultDuration">Default duration for tracks.</param>
    /// <param name="scrobbleMode">Scrobble mode to use.</param>
    /// <returns>Parse result.</returns>
    FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode, T configuration);
  }
}