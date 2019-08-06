namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Factory for creating <see cref="IFileParser"/>.
  /// </summary>
  class FileParserFactory : IFileParserFactory
  {
    /// <summary>
    /// Creates a parser that can parse .csv files.
    /// </summary>
    /// <returns>Parser that can parse .csv files.</returns>
    public IFileParser CreateCSVFileParser()
    {
      return new CSVFileParser();
    }

    /// <summary>
    /// Creates a parser that can parse .json files.
    /// </summary>
    /// <returns>Parser that can parse .json files.</returns>
    public IFileParser CreateJSONFileParser()
    {
      return new JSONFileParser();
    }
  }
}