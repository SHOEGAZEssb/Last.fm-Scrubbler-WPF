using Caliburn.Micro;
using Octokit;
using Scrubbler.Helper;
using Scrubbler.Scrobbling;

namespace Scrubbler
{
  /// <summary>
  /// Bootstrapper used to connect View and ViewModel on startup.
  /// </summary>
  internal class AppBootstrapper : BootstrapperBase
  {
    #region Member

    /// <summary>
    /// The api key of this application.
    /// </summary>
    private const string APIKEY = "69fbfa5fdc2cc1a158ec3bffab4be7a7";

    /// <summary>
    /// The api secret of this application.
    /// </summary>
    private const string APISECRET = "30a6ed8a75dad2aa6758fa607c53adb5";

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public AppBootstrapper()
    {
      Initialize();
    }

    /// <summary>
    /// Displays the root View.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
    {
      IExtendedWindowManager windowManager = new ExtendedWindowManager();
      ILastFMClient client = new LastFMClient(APIKEY, APISECRET);
      IScrobblerFactory scrobblerFactory = new ScrobblerFactory();
      ILocalFileFactory localFileFactory = new LocalFileFactory();
      IFileOperator fileOperator = new FileOperator();
      IDirectoryOperator directoryOperator = new DirectoryOperator();
      ISerializer userSerializer = new DCSerializer();
      ILogger logger = new Logger("log.txt");
      IGitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("Last.fm-Scrubbler-WPF"));
      MainViewModel mainVM = new MainViewModel(windowManager, client, scrobblerFactory, localFileFactory, fileOperator,
                                               directoryOperator, userSerializer, logger, gitHubClient);

      windowManager.ShowWindow(new SystemTrayViewModel(windowManager, mainVM));
    }
  }
}