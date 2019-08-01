using Caliburn.Micro;
using Scrubbler.Scrobbling.Scrobbler;
using System;

namespace Scrubbler.Helper.FileParser
{
  class CSVFileParserViewModel : PropertyChangedBase, IFileParserViewModel, IHaveSettings
  {
    #region Properties

    public string Name => "CSV";

    public string FileFilter => "CSV Files (.csv)|*.csv";

    #endregion Properties

    #region Member

    private readonly IWindowManager _windowManager;
    private readonly CSVFileParser _parser;

    #endregion Member

    #region Construction

    public CSVFileParserViewModel(IWindowManager windowManager)
    {
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      _parser = new CSVFileParser();
    }

    #endregion Construction

    public FileParseResult Parse(string file, ScrobbleMode scrobbleMode)
    {
      return _parser.Parse(file, scrobbleMode);
    }

    public void ShowSettings()
    {
      _windowManager.ShowDialog(new ConfigureCSVParserViewModel());
    }
  }
}
