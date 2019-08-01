using Caliburn.Micro;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// Interface for a ViewModel managing a <see cref="IFileParser"/>.
  /// </summary>
  public interface IFileParserViewModel : INotifyPropertyChangedEx
  {
    /// <summary>
    /// Name of the parser.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Filter for file dialogs.
    /// </summary>
    string FileFilter { get; }

    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="scrobbleMode">Scrobble mode to use.</param>
    /// <returns>Parse result.</returns>
    FileParseResult Parse(string file, ScrobbleMode scrobbleMode);
  }
}