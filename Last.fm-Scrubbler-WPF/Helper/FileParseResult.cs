using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.Generic;

namespace Scrubbler.Helper
{
  public class FileParseResult
  {
    public readonly IEnumerable<DatedScrobble> Scrobbles;
    public readonly IEnumerable<string> Errors;

    public FileParseResult(IEnumerable<DatedScrobble> scrobbles, IEnumerable<string> errors)
    {
      Scrobbles = scrobbles ?? throw new ArgumentNullException(nameof(scrobbles));
      Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }
  }
}
