using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.Generic;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// The result of a <see cref="IFileParser.Parse(string, TimeSpan, Scrobbling.Scrobbler.ScrobbleMode)"/> operation.
  /// </summary>
  public class FileParseResult
  {
    /// <summary>
    /// The parsed scrobbles.
    /// </summary>
    public readonly IEnumerable<DatedScrobble> Scrobbles;

    /// <summary>
    /// Errors encountered during parsing.
    /// </summary>
    public readonly IEnumerable<string> Errors;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="scrobbles">The parsed scrobbles.</param>
    /// <param name="errors">Errors encountered during parsing.</param>
    public FileParseResult(IEnumerable<DatedScrobble> scrobbles, IEnumerable<string> errors)
    {
      Scrobbles = scrobbles ?? throw new ArgumentNullException(nameof(scrobbles));
      Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }
  }
}