using Caliburn.Micro;
using Scrubbler.Scrobbling.Scrobbler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrubbler.Helper.FileParser
{
  public interface IFileParserViewModel : INotifyPropertyChangedEx
  {
    string Name { get; }
    string FileFilter { get; }
    FileParseResult Parse(string file, ScrobbleMode scrobbleMode);
  }
}