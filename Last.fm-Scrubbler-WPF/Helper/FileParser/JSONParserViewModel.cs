using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Scrubbler.Properties;
using ScrubblerLib.Helper.FileParser;

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

    private readonly IFileParser<JSONFileParserConfiguration> _parser;
    private readonly IWindowManager _windowManager;

    #endregion Member

    #region Construction

    public JSONParserViewModel(IFileParser<JSONFileParserConfiguration> parser, IWindowManager windowManager)
    {
      _parser = parser ?? throw new ArgumentNullException(nameof(parser));
      _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
    }

    #endregion Construction

    public FileParseResult Parse(string file, TimeSpan defaultDuration, ScrobbleMode scrobbleMode)
    {
      return _parser.Parse(file, MakeConfig(scrobbleMode, defaultDuration));
    }

    public void ShowSettings()
    {
      _windowManager.ShowDialog(new ConfigureJSONParserViewModel());
    }

    private static JSONFileParserConfiguration MakeConfig(ScrobbleMode scrobbleMode, TimeSpan defaultDuration)
    {
      return new JSONFileParserConfiguration(scrobbleMode, defaultDuration, Settings.Default.JSONTrackNameProperty, Settings.Default.JSONArtistNameProperty, Settings.Default.JSONAlbumNameProperty,
        Settings.Default.JSONAlbumArtistNameProperty, Settings.Default.JSONTimestampProperty, Settings.Default.JSONDurationProperty, Settings.Default.JSONMillisecondsPlayedProperty,
        Settings.Default.JSONFilterShortPlayedSongs, Settings.Default.JSONPlayedMillisecondsThreshold);
    }
  }
}
