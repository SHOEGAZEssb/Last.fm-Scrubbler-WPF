using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Scrubbler.Scrobbling.Scrobbler;

namespace Scrubbler.Helper.FileParser
{
  class JSONParserViewModel : PropertyChangedBase, IFileParserViewModel, IHaveSettings
  {
    #region Properties

    public string Name => "JSON";

    public string FileFilter => "JSON Files (.json)|*.json";

    public IEnumerable<string> SupportedExtensions => new[] { ".json" };

    #endregion Properties

    #region Member

    private readonly IFileParser _parser;
    private readonly IWindowManager _windowManager;

    #endregion Member

    #region Construction

    public JSONParserViewModel(IFileParser parser, IWindowManager windowManager)
    {
      _parser = parser ?? throw new ArgumentNullException(nameof(parser));
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
    }

    #endregion Construction

    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      return _parser.Parse(file, defaultDuration, scrobbleMode);
    }

    public void ShowSettings()
    {
      _windowManager.ShowDialog(new ConfigureJSONParserViewModel());
    }
  }
}
