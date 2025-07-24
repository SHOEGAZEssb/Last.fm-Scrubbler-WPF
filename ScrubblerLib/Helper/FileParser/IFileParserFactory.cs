namespace ScrubblerLib.Helper.FileParser
{
  /// <summary>
  /// Interface for a factory for creating <see cref="IFileParser"/>.
  /// </summary>
  public interface IFileParserFactory
  {
    /// <summary>
    /// Creates a parser that can parse .csv files.
    /// </summary>
    /// <returns>Parser that can parse .csv files.</returns>
    IFileParser<CSVFileParserConfiguration> CreateCSVFileParser();

    /// <summary>
    /// Creates a parser that can parse .json files.
    /// </summary>
    /// <returns>Parser that can parse .json files.</returns>
    IFileParser<JSONFileParserConfiguration> CreateJSONFileParser();
  }
}