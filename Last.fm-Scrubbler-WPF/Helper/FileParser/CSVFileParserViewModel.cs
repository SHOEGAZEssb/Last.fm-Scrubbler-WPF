using Caliburn.Micro;
using Scrubbler.Properties;
using ScrubblerLib.Helper.FileParser;
using System;
using System.Collections.Generic;

namespace Scrubbler.Helper.FileParser
{
  /// <summary>
  /// ViewModel for managing a parser that parses .csv files.
  /// </summary>
  class CSVFileParserViewModel : PropertyChangedBase, IFileParserViewModel, IHaveSettings
  {
    #region Properties

    /// <summary>
    /// Name of the parser.
    /// </summary>
    public string Name => "CSV";

    /// <summary>
    /// Filter for file dialogs.
    /// </summary>
    public string FileFilter => "CSV Files (.csv)|*.csv";

    /// <summary>
    /// Gets a list of supported file extensions.
    /// </summary>
    public IEnumerable<string> SupportedExtensions => new[] { ".csv" };

    #endregion Properties

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private readonly IWindowManager _windowManager;

    /// <summary>
    /// Parser used to parse .csv files.
    /// </summary>
    private readonly IFileParser<CSVFileParserConfiguration> _parser;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="parser">Parser to manage.</param>
    /// <param name="windowManager"></param>
    public CSVFileParserViewModel(IFileParser<CSVFileParserConfiguration> parser, IWindowManager windowManager)
    {
      _parser = parser ?? throw new ArgumentNullException(nameof(parser));
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
    }

    #endregion Construction

    /// <summary>
    /// Parses the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">File to parse.</param>
    /// <param name="defaultDuration">Default duration for tracks.</param>
    /// <param name="scrobbleMode">Scrobble mode to use.</param>
    /// <returns>Parse result.</returns>
    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      return _parser.Parse(file, defaultDuration, scrobbleMode, MakeConfig());
    }

    /// <summary>
    /// Shows the settings of the parser.
    /// </summary>
    public void ShowSettings()
    {
      _windowManager.ShowDialog(new ConfigureCSVParserViewModel());
    }

    private CSVFileParserConfiguration MakeConfig()
    {
      return new CSVFileParserConfiguration(Settings.Default.CSVEncoding, Settings.Default.CSVDelimiters, Settings.Default.CSVHasFieldsInQuotes,
        Settings.Default.TimestampFieldIndex, Settings.Default.TrackFieldIndex, Settings.Default.ArtistFieldIndex, Settings.Default.AlbumFieldIndex,
        Settings.Default.AlbumArtistFieldIndex, Settings.Default.DurationFieldIndex, Settings.Default.CSVMillisecondsPlayedFieldIndex,
        Settings.Default.CSVFilterShortPlayedSongs, Settings.Default.CSVPlayedMillisecondsThreshold);
    }
  }
}