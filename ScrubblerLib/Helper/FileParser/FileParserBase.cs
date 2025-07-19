using System;
using System.Globalization;

namespace ScrubblerLib.Helper.FileParser
{
  /// <summary>
  /// Scrobble mode.
  /// </summary>
  public enum ScrobbleMode
  {
    /// <summary>
    /// Scrobble the tracks based on the parsed timestamp.
    /// </summary>
    Normal,

    /// <summary>
    /// Set the timestamp by setting <see cref="ScrobbleTimeViewModel.Time"/>.
    /// </summary>
    ImportMode
  }

  /// <summary>
  /// Base class for all file parsers.
  /// </summary>
  public abstract class FileParserBase<T> : IFileParser<T> where T : IFileParserConfiguration
  {
    /// <summary>
    /// Different formats to try in case TryParse fails.
    /// </summary>
    private static readonly string[] _formats = new string[] { "M/dd/yyyy h:mm" };

    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="defaultDuration">Default duration for tracks.</param>
    /// <param name="scrobbleMode">Scrobble mode to use.</param>
    /// <returns>Parse result.</returns>
    public abstract FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode, T config);

    /// <summary>
    /// Tries to parse a string to a DateTime.
    /// </summary>
    /// <param name="dateString">String to parse.</param>
    /// <param name="dateTime">Outgoing DateTime.</param>
    /// <returns>True if <paramref name="dateString"/> was successfully parsed,
    /// otherwise false.</returns>
    public static bool TryParseDateString(string dateString, out DateTime dateTime)
    {
      if (!DateTime.TryParse(dateString, out dateTime))
      {
        bool parsed;
        // try different formats until succeeded
        foreach (string format in _formats)
        {
          parsed = DateTime.TryParseExact(dateString, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);
          if (parsed)
            return true;
        }

        return false;
      }

      return true;
    }
  }
}
