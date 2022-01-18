using Microsoft.VisualBasic.FileIO;
using Scrubbler.Properties;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// <param name="defaultDuration">Default duration for tracks.</param>
    /// <param name="scrobbleMode"></param>
    /// <returns>Parse result.</returns>
    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      if (string.IsNullOrEmpty(file))
        throw new ArgumentNullException(nameof(file));

      var scrobbles = new List<DatedScrobble>();
      var errors = new List<string>();
      string[] fields = null;

      //var lines = System.IO.File.ReadAllLines(file);
      //using(var s = System.IO.File.OpenWrite("test.csv"))
      //{
      //  using (var sw = new System.IO.StreamWriter(s))
      //  {
      //    for (int i = 0; i < 200; i++)
      //    {
      //      sw.WriteLine(lines[i]);
      //    }
      //  }
      //}

      using (var parser = new TextFieldParser(file, Encoding.GetEncoding(Settings.Default.CSVEncoding)))
      {
        parser.SetDelimiters(Settings.Default.CSVDelimiters.Select(x => new string(x, 1)).ToArray());
        parser.HasFieldsEnclosedInQuotes = Settings.Default.CSVHasFieldsInQuotes;

        while (!parser.EndOfData)
        {
          try
          {
            fields = parser.ReadFields();
            string dateString = fields.ElementAtOrDefault(Settings.Default.TimestampFieldIndex);

            // check for 'now playing'
            if (string.IsNullOrEmpty(dateString) && scrobbleMode == ScrobbleMode.Normal)
              continue;

            DateTime date = DateTime.Now;
            if (!FileParserBase.TryParseDateString(dateString, out date) && scrobbleMode == ScrobbleMode.Normal)
              throw new Exception("Timestamp could not be parsed!");

            // try to get optional parameters first
            string album = fields.ElementAtOrDefault(Settings.Default.AlbumFieldIndex);
            string albumArtist = fields.ElementAtOrDefault(Settings.Default.AlbumArtistFieldIndex);
            string duration = fields.ElementAtOrDefault(Settings.Default.DurationFieldIndex);

            if (!TimeSpan.TryParse(duration, out TimeSpan time))
              time = TimeSpan.FromSeconds(3); // todo: use user provided duration

            scrobbles.Add(new DatedScrobble(date.AddSeconds(1), fields[Settings.Default.TrackFieldIndex],
                                                            fields[Settings.Default.ArtistFieldIndex], album,
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
