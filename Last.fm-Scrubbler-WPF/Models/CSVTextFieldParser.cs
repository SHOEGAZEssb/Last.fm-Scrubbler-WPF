using Scrubbler.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Scrubbler.Models
{
  /// <summary>
  /// Text field parser that parses a csv file.
  /// </summary>
  class CSVTextFieldParser : ITextFieldParser
  {
    #region Properties

    /// <summary>
    /// If the text to parse has fields enclosed in quotes.
    /// </summary>
    public bool HasFieldsEnclosedInQuotes { get => _parser.HasFieldsEnclosedInQuotes; set => _parser.HasFieldsEnclosedInQuotes = value; }

    /// <summary>
    /// If the reader is at the end of the data to read.
    /// </summary>
    public bool EndOfData => _parser.EndOfData;

    /// <summary>
    /// Current line number.
    /// </summary>
    public long LineNumber => _parser.LineNumber;

    #endregion Properties

    #region Member

    /// <summary>
    /// Actual text field parser.
    /// </summary>
    private TextFieldParser _parser;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="path">Path of the file to parse.</param>
    public CSVTextFieldParser(string path)
    {
      _parser = new TextFieldParser(path);
    }

    /// <summary>
    /// Sets the delimiters to use when parsing the text.
    /// </summary>
    /// <param name="delimiters">Delimiters to use.</param>
    public void SetDelimiters(params string[] delimiters)
    {
      _parser.SetDelimiters(delimiters);
    }

    /// <summary>
    /// Reads the fields.
    /// </summary>
    /// <returns>Parsed fields.</returns>
    public string[] ReadFields()
    {
      return _parser.ReadFields();
    }

    /// <summary>
    /// Disposes the text field parser.
    /// </summary>
    public void Dispose()
    {
      _parser?.Dispose();
    }
  }
}