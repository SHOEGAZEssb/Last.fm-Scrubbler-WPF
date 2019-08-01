using Microsoft.VisualBasic.FileIO;
using Scrubbler.Properties;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Parses a .csv file.
  /// </summary>
  class CSVFileParser : IFileParser
  {
    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="scrobbleMode"></param>
    /// <returns>Parse result.</returns>
    public FileParseResult Parse(string file, ScrobbleMode scrobbleMode)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException(nameof(file));

      var scrobbles = new List<DatedScrobble>();
      var errors = new List<string>();
      string[] fields = null;

      using (var parser = new TextFieldParser(file))
      {
        parser.SetDelimiters(Settings.Default.CSVDelimiters.Select(x => new string(x, 1)).ToArray());

        while (!parser.EndOfData)
        {
          fields = parser.ReadFields();

          try
          {
            string dateString = fields.ElementAtOrDefault(Settings.Default.TimestampFieldIndex);

            // check for 'now playing'
            if (dateString == "" && scrobbleMode == ScrobbleMode.Normal)
              continue;

            DateTime date = DateTime.Now;
            if (!FileParserBase.TryParseDateString(dateString, out date) && scrobbleMode == ScrobbleMode.Normal)
              throw new Exception("Timestamp could not be parsed!");

            // try to get optional parameters first
            string album = fields.ElementAtOrDefault(Settings.Default.AlbumFieldIndex);
            string albumArtist = fields.ElementAtOrDefault(Settings.Default.AlbumArtistFieldIndex);
            string duration = fields.ElementAtOrDefault(Settings.Default.DurationFieldIndex);
            TimeSpan time;
            if (!TimeSpan.TryParse(duration, out time))
              time = TimeSpan.FromSeconds(3); // todo: use user provided duration

            scrobbles.Add(new DatedScrobble(date.AddSeconds(1), fields[Settings.Default.TrackFieldIndex],
                                                            fields[Settings.Default.ArtistFieldIndex], album,
                                                            albumArtist, time));
          }
          catch (Exception ex)
          {
            string errorString = $"CSV line number: {parser.LineNumber},";
            foreach (string s in fields)
            {
              errorString += $"{s},";
            }

            errorString += ex.Message;
            errors.Add(errorString);
          }
        }
      }

      return new FileParseResult(scrobbles, errors);
    }
  }
}
