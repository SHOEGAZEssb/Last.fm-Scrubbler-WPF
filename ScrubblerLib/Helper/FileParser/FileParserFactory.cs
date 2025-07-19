namespace ScrubblerLib.Helper.FileParser
{
  /// <summary>
  /// Factory for creating <see cref="IFileParser"/>.
  /// </summary>
  public class FileParserFactory : IFileParserFactory
  {
    /// <summary>
    /// Creates a parser that can parse .csv files.
    /// </summary>
    /// <returns>Parser that can parse .csv files.</returns>
    public IFileParser<CSVFileParserConfiguration> CreateCSVFileParser()
    {
      return new CSVFileParser();
    }

    /// <summary>
    /// Creates a parser that can parse .json files.
    /// </summary>
    /// <returns>Parser that can parse .json files.</returns>
    public IFileParser<JSONFileParserConfiguration> CreateJSONFileParser()
    {
      return new JSONFileParser();
    }
  }
}