using ScrubblerLib;
using ScrubblerLib.Helper;
using ScrubblerLib.Helper.FileParser;
using ScrubblerLib.Login;
using ScrubblerLib.Scrobbler;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace ScrubblerCLI;

internal class Program
{
  private static IUserScrobbler? _scrobbler;
  private static IFileParserConfiguration? _configuration;
  private static readonly FileParseScrobbleFeature _scrobbleFeature = new();

  internal static int Main(string[] args)
  {
    Option<DirectoryInfo> fileDirectoryOption = new("--directory")
    {
      Description = "Path to the directory containing the files to parse (either .csv or .json)",
      Required = true
    };

    Option<FileInfo> configFileOption = new("--configFile")
    {
      Description = "Path to the parser configuration file",
      Required = true
    };

    Option<FileInfo> userFileOption = new("--userFile")
    {
      Description = "Path to the last.fm user file",
      Required = true
    };

    RootCommand rootCommand = new("Scrubbler Command Line Interface");
    rootCommand.Options.Add(fileDirectoryOption);
    rootCommand.Options.Add(configFileOption);
    rootCommand.Options.Add(userFileOption);

    ParseResult parseResult = rootCommand.Parse(args);
    if (parseResult.Errors.Count > 0)
    {
      foreach (ParseError parseError in parseResult.Errors)
      {
        Console.Error.WriteLine(parseError.Message);
      }

      return 1;
    }

    try
    {
      _scrobbler = CreateScrobbler(parseResult.GetValue(userFileOption));
      _configuration = CreateConfiguration(parseResult.GetValue(configFileOption));
      _scrobbleFeature.ParserConfiguration = _configuration;
      var file = SelectFile(parseResult.GetValue(fileDirectoryOption));
      _scrobbleFeature.FilePath = file.FullName;

      var scrobbles = _scrobbleFeature.CreateScrobbles();
      if (_scrobbleFeature.LastParseErrors?.Count() > 0)
      {
        foreach (var error in _scrobbleFeature.LastParseErrors)
        {
          Console.Error.WriteLine(error);
        }

        return 1;
      }

      if (!scrobbles.Any())
        throw new Exception("No scrobbles in file");
      else if (scrobbles.Count() > 3000)
        throw new Exception("File contains more than 3000 scrobbles");

        Console.WriteLine($"Scrobbling {scrobbles.Count()} tracks from file {file.FullName}");
      var response = _scrobbler.ScrobbleAsync(scrobbles).Result;
      if (response.Success)
      {
        MarkFileAsDone(file);
        Console.WriteLine("Successfully scrobbled!");
      }
      else
        throw new Exception($"Scrobbling failed with status: {response.Status}");
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
      return 1;
    }

    return 0;
  }

  private static IUserScrobbler CreateScrobbler(FileInfo? userFile)
  {
    ArgumentNullException.ThrowIfNull(userFile);

    var serializer = new DCSerializer();
    var user = serializer.Deserialize<User>(userFile.FullName);

    var factory = new ScrobblerFactory();
    var client = new LastFMClient("69fbfa5fdc2cc1a158ec3bffab4be7a7", "30a6ed8a75dad2aa6758fa607c53adb5");
    var auth = factory.CreateScrobbler(client.Auth) ?? throw new Exception("Could not create scrobbler");
    return factory.CreateUserScrobbler(user, auth);
  }

  private static IFileParserConfiguration CreateConfiguration(FileInfo? configurationFile)
  {
    ArgumentNullException.ThrowIfNull(configurationFile);

    var serializer = new DCSerializer();
    return serializer.Deserialize<IFileParserConfiguration>(configurationFile.FullName, [typeof(CSVFileParserConfiguration), typeof(JSONFileParserConfiguration)]);
  }

  private static FileInfo SelectFile(DirectoryInfo? directory)
  {
    ArgumentNullException.ThrowIfNull(directory);

    string extension;
    if (_configuration is CSVFileParserConfiguration)
      extension = ".csv";
    else
      extension = "json";

    FileInfo? file = directory.GetFiles().FirstOrDefault(f => f.Extension.Equals(extension, StringComparison.CurrentCultureIgnoreCase)) ?? throw new Exception("No more files to process");
    return file;
  }

  private static void MarkFileAsDone(FileInfo file)
  {
    ArgumentNullException.ThrowIfNull(file);

    var newPath = file.FullName + ".done";
    File.Move(file.FullName, newPath);
  }
}