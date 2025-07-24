using System;

namespace ScrubblerLib.Helper.FileParser
{
  public interface IFileParserConfiguration
  {
    ScrobbleMode ScrobbleMode { get; set; }

    TimeSpan DefaultDuration { get; set; }
  }
}
