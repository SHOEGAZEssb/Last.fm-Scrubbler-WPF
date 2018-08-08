namespace Scrubbler.Helper
{
  /// <summary>
  /// Factory creating <see cref="CSVTextFieldParser"/>.
  /// </summary>
  class CSVTextFieldParserFactory : ITextFieldParserFactory
  {
    /// <summary>
    /// Creates an <see cref="CSVTextFieldParser"/>.
    /// </summary>
    /// <param name="path">Path of the file to parse.</param>
    /// <returns>Instance of an <see cref="CSVTextFieldParser"/>.</returns>
    public ITextFieldParser CreateParser(string path)
    {
      return new CSVTextFieldParser(path);
    }
  }
}