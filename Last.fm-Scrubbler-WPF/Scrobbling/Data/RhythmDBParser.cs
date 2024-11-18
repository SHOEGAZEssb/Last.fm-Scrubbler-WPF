using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// Parser for a RhythmDB.xml file.
  /// </summary>
  internal static class RhythmDBParser
  {
    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">RhythmDB.xml file to parse.</param>
    /// <returns>List of parsed scrobbles.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="file"/> is null.</exception>
    /// <exception cref="Exception">If an error during parsing occurs.</exception>
    public static IEnumerable<MediaDBScrobble> Parse(string file)
    {
      if (file == null)
        throw new ArgumentNullException(nameof(file));

      var xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load(file);
      }
      catch (Exception ex)
      {
        throw new Exception($"Could not load Rhythmdb xml file: {ex.Message}");
      }

      IEnumerable<XmlNode> trackNodes = null;
      try
      {
        trackNodes = xmlDocument.ChildNodes[1].ChildNodes.OfType<XmlNode>().Where(i => i.Name == "entry");
      }
      catch (Exception ex) 
      {
        throw new Exception($"Could not get 'entry' nodes, file might be malformed: {ex.Message}");
      }

      var scrobbles = new List<MediaDBScrobble>();
      var errorNodes = new Dictionary<XmlNode, string>();
      foreach (XmlNode trackNode in trackNodes) 
      { 
        try
        {
          var childNodes = trackNode.ChildNodes.OfType<XmlNode>();
          // try to get playcount
          int playCount = 1;
          var playCountNode = childNodes.FirstOrDefault(i => i.Name == "play-count");
          if (playCountNode == null)
            continue; // no playcount node means track has not been played
          else
            playCount = int.Parse(playCountNode.InnerText);

          // mandatory fields
          var trackName = childNodes.First(i => i.Name == "title").InnerText;
          var artistName =  childNodes.First(i => i.Name == "artist").InnerText;

          // optional fields
          var albumName = childNodes.FirstOrDefault(i => i.Name == "album")?.InnerText;
          var albumArtistName = childNodes.FirstOrDefault(i => i.Name == "album-artist")?.InnerText;
          
          // try to get duration
          TimeSpan duration = TimeSpan.FromSeconds(1);
          try
          {
            duration = TimeSpan.FromSeconds(double.Parse(childNodes.First(i => i.Name == "duration").InnerText));
          }
          catch { }

          // try to get last played
          DateTime lastPlayed = DateTime.Now;
          try
          {
            lastPlayed = DateTimeOffset.FromUnixTimeSeconds(long.Parse(childNodes.First(i => i.Name == "mtime").InnerText)).UtcDateTime;
          }
          catch { }

          scrobbles.Add(new MediaDBScrobble(playCount, lastPlayed, trackName, artistName, albumName, albumArtistName, duration));
        }
        catch (Exception ex)
        {
          // corrupted or something, log and continue.
          errorNodes.Add(trackNode, ex.Message);
        }
      }

      return scrobbles;
    }
  }
}