namespace Last.fm_Scrubbler_WPF.Interfaces
{
  /// <summary>
  /// Interface for a factory creating <see cref="ITextFieldParser"/>s.
  /// </summary>
  public interface ITextFieldParserFactory
  {
    /// <summary>
    /// Creates an <see cref="ITextFieldParser"/>.
    /// </summary>
    /// <param name="path">Path of the file to parse.</param>
    /// <returns>Instance of an <see cref="ITextFieldParser"/>.</returns>
    ITextFieldParser CreateParser(string path);
  }
}