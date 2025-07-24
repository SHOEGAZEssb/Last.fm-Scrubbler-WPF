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
    /// <returns>Parse result.</returns>
    FileParseResult Parse(string file, T configuration);
  }
}