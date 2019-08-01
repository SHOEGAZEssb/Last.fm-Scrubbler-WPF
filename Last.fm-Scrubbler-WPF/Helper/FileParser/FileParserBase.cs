using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;

namespace Scrubbler.Helper.FileParser
{
  public abstract class FileParserBase : IFileParser
  {
    /// <summary>
    /// Different formats to try in case TryParse fails.
    /// </summary>
    private static readonly string[] _formats = new string[] { "M/dd/yyyy h:mm" };

    public abstract FileParseResult Parse(string file, ScrobbleMode scrobbleMode);

    public static bool TryParseDateString(string dateString, out DateTime dateTime)
    {
      if (!DateTime.TryParse(dateString, out dateTime))
      {
        bool parsed = false;
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
