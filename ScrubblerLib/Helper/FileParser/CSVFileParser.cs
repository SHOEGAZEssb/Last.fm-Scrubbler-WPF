using Microsoft.VisualBasic.FileIO;
using ScrubblerLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrubblerLib.Helper.FileParser
{
  /// <summary>
  /// Parses a .csv file.
  /// </summary>
  class CSVFileParser : IFileParser<CSVFileParserConfiguration>
  {
    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="defaultDuration">Default duration for tracks.</param>
    /// <param name="scrobbleMode"></param>
    /// <returns>Parse result.</returns>
    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode, CSVFileParserConfiguration config)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException(nameof(file));

      var scrobbles = new List<DatedScrobble>();
      var errors = new List<string>();
      string[] fields = null;

      using (var parser = new TextFieldParser(file, Encoding.GetEncoding(config.EncodingID)))
      {
        parser.SetDelimiters(config.Delimiters.Select(x => new string(x, 1)).ToArray());
        parser.HasFieldsEnclosedInQuotes = config.HasFieldsInQuotes;

        while (!parser.EndOfData)
        {
          try
          {
            fields = parser.ReadFields();
            string dateString = fields.ElementAtOrDefault(config.TimestampFieldIndex);

            // check for 'now playing'
            if (string.IsNullOrEmpty(dateString) && scrobbleMode == ScrobbleMode.Normal)
              continue;

            DateTime date = DateTime.Now;
            if (!FileParserBase<CSVFileParserConfiguration>.TryParseDateString(dateString, out date) && scrobbleMode == ScrobbleMode.Normal)
              throw new Exception("Timestamp could not be parsed!");

            // try to get optional parameters first
            string album = fields.ElementAtOrDefault(config.AlbumFieldIndex);
            string albumArtist = fields.ElementAtOrDefault(config.AlbumArtistFieldIndex);
            string duration = fields.ElementAtOrDefault(config.DurationFieldIndex);
            string timePlayedMS = fields.ElementAtOrDefault(config.MillisecondsPlayedFieldIndex);

            if (!TimeSpan.TryParse(duration, out TimeSpan time))
              time = TimeSpan.FromSeconds(3); // todo: use user provided duration

            // filter short played songs
            if (config.FilterShortPlayedSongs &&
                TimeSpan.TryParse(timePlayedMS, out TimeSpan msPlayed) &&
                msPlayed.TotalMilliseconds <= config.MillisecondsPlayedThreshold)
              continue;
            else
              scrobbles.Add(new DatedScrobble(date.AddSeconds(1), fields[config.TrackFieldIndex],
                                                              fields[config.ArtistFieldIndex], album,
                                                              albumArtist, time));
          }
          catch (Exception ex)
          {
            string errorString = $"CSV line number: {parser.LineNumber - 1},";

            // fields is old in this case
            if (!(ex is MalformedLineException))
            {
              foreach (string s in fields)
              {
                errorString += $"{s},";
              }
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