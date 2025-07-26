using Microsoft.Win32.TaskScheduler;
using ScrubblerLib;
using ScrubblerLib.Helper;
using ScrubblerLib.Helper.FileParser;
using ScrubblerLib.Login;
using ScrubblerLib.Scrobbler;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Security.Principal;

namespace ScrubblerCLI;

internal enum Mode
{
  Scrobble,
  CreateTask,
  RemoveTask
}

internal class Program
{
  private const string TASKNAME = "ScrobbleQueueTask";

  private static IUserScrobbler? _scrobbler;
  private static IFileParserConfiguration? _configuration;
  private static readonly FileParseScrobbleFeature _scrobbleFeature = new();

  internal static int Main(string[] args)
  {

    // main options
    Option<Mode> modeOption = new("--mode")
    {
      Description = "CLI Mode. Scrobble; CreateTask; RemoveTask",
      Required = false,
      DefaultValueFactory = (a) => Mode.Scrobble
    };

    Option<DirectoryInfo> fileDirectoryOption = new("--directory")
    {
      Description = "Path to the directory containing the files to parse (either .csv or .json)",
      Required = false
    };

    Option<FileInfo> userFileOption = new("--userFile")
    {
      Description = "Path to the last.fm user file",
      Required = false
    };

    // scrobble specific options
    Option<FileInfo> configFileOption = new("--configFile")
    {
      Description = "Path to the parser configuration file",
      Required = false
    };

    // create task specific options
    Option<DateTime?> taskTimeOption = new("--taskTime")
    {
      Description = "Time when the task should be run daily",
      Required = false
    };

    Option<int> dayIntervalOption = new("--dayInterval")
    {
      Description = "How many days between each task run",
      Required = false,
      DefaultValueFactory = (a) => 1
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
      var mode = parseResult.GetValue(modeOption);
      if (mode == Mode.Scrobble)
        return Scrobble(parseResult, fileDirectoryOption, userFileOption, configFileOption);
      else if (mode == Mode.CreateTask)
        return CreateTask(parseResult, fileDirectoryOption, userFileOption, taskTimeOption, dayIntervalOption);
      else if (mode == Mode.RemoveTask)
        return RemoveTask();
      else
        throw new Exception($"Unknown mode: {mode}");
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
      return 1;
    }
  }

  #region Scrobble

  private static int Scrobble(ParseResult parseResult, Option<DirectoryInfo> fileDirectoryOption, Option<FileInfo> userFileOption, Option<FileInfo> configFileOption)
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

    return 0;
  }

  private static IUserScrobbler CreateScrobbler(FileInfo? userFile)
  {
    ArgumentNullException.ThrowIfNull(userFile);

    var serializer = new DCSerializer();
    var user = serializer.Deserialize<User>(userFile.FullName);

    var factory = new ScrobblerFactory();
    var client = new LastFMClient("69fbfa5fdc2cc1a158ec3bffab4be7a7", "30a6ed8a75dad2aa6758fa607c53adb5");
    var auth = factory.CreateScrobbler(client.Auth);
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

  #endregion Scrobble

  #region Tasks

  #region CreateTask

  private static int CreateTask(ParseResult parseResult, Option<DirectoryInfo> fileDirectoryOption, Option<FileInfo> userFileOption,
                                Option<DateTime?> taskTimeOption, Option<int> dayIntervalOption)
  {
    if (!IsAdministrator())
      throw new Exception("Admin rights required");

    // value verification
    var directory = parseResult.GetValue(fileDirectoryOption);
    ArgumentNullException.ThrowIfNull(directory);
    if (!directory.Exists)
      throw new Exception($"Directory '{directory.FullName}' does not exist");

    var userFile = parseResult.GetValue(userFileOption);
    ArgumentNullException.ThrowIfNull(userFile);
    if (!userFile.Exists)
      throw new Exception($"User File '{userFile.FullName}' does not exist");

    var taskTime = parseResult.GetValue(taskTimeOption);
    if (!taskTime.HasValue)
      throw new ArgumentNullException(nameof(taskTimeOption));

    var dayInterval = parseResult.GetValue(dayIntervalOption);
    if (dayInterval <= 0)
      throw new Exception("Day interval is smaller than 1");

    using var ts = new TaskService();

    // actual task creation
    var task = ts.NewTask();
    task.RegistrationInfo.Description = "Scrobbles a parsed file";
    task.Triggers.Add(new DailyTrigger() { DaysInterval = (short)dayInterval, StartBoundary = taskTime.Value });
    task.Actions.Add(new ExecAction(
      Environment.ProcessPath,
      "--mode Scrobble " +
      "--directory " + directory.FullName +
      "--userFile " + userFile.FullName,
      null));

    ts.RootFolder.RegisterTaskDefinition(TASKNAME, task,
            TaskCreation.CreateOrUpdate,
            "SYSTEM", null,
            TaskLogonType.ServiceAccount);

    Console.WriteLine($"Task '{TASKNAME}' created successfully");
    return 0;
  }

  #endregion CreateTask

  #region RemoveTask

  private static int RemoveTask()
  {
    if (!IsAdministrator())
      throw new Exception("Admin rights required");

    using var ts = new TaskService();
    if (ts.GetTask(TASKNAME) != null)
    {
      ts.RootFolder.DeleteTask(TASKNAME);
      Console.WriteLine($"Deleted existing task '{TASKNAME}'.");
    }
    else
      throw new Exception("Task does not exist");

    return 0;
  }

  #endregion RemoveTask

  private static bool TaskExists(TaskService ts)
  {
    return ts.GetTask(TASKNAME) != null;
  }

  private static bool IsAdministrator()
  {
    var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
  }

  #endregion Tasks
}