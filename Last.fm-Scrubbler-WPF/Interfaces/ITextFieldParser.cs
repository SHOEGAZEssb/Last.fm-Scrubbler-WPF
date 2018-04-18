using System;

namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for a text parser.
  /// </summary>
  public interface ITextFieldParser : IDisposable
  {
    /// <summary>
    /// If the text to parse has fields enclosed in quotes.
    /// </summary>
    bool HasFieldsEnclosedInQuotes { get; set; }

    /// <summary>
    /// Sets the delimiters to use when parsing the text.
    /// </summary>
    /// <param name="delimiters">Delimiters to use.</param>
    void SetDelimiters(params string[] delimiters);

    /// <summary>
    /// If the reader is at the end of the data to read.
    /// </summary>
    bool EndOfData { get; }

    /// <summary>
    /// Current line number.
    /// </summary>
    long LineNumber { get; }

    /// <summary>
    /// Reads the fields.
    /// </summary>
    /// <returns>Parsed fields.</returns>
    string[] ReadFields();
  }
}