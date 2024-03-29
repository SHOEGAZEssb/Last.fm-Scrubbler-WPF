using Caliburn.Micro;
using DiscogsClient;
using DiscogsClient.Internal;
using Octokit;
using Scrubbler.Helper;
using Scrubbler.Helper.FileParser;
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
      var windowManager = new ExtendedWindowManager();
      var client = new LastFMClient(APIKEY, APISECRET);
      var scrobblerFactory = new ScrobblerFactory();
      var localFileFactory = new LocalFileFactory();
      var fileOperator = new FileOperator();
      var directoryOperator = new DirectoryOperator();
      var userSerializer = new DCSerializer();
      var logger = new Logger("log.txt");
      var gitHubClient = new GitHubClient(new ProductHeaderValue("Last.fm-Scrubbler-WPF"));
      var processManager = new ProcessManager();
      var discogsClient = new DiscogsClient.DiscogsClient(new TokenAuthenticationInformation("vcrTuxlCPCANcLDUDcbGSYBxbODkeyywIUtYAMxg"));
      var fileParserFactory = new FileParserFactory();

      MainViewModel mainVM = new MainViewModel(windowManager, client, scrobblerFactory, localFileFactory, fileOperator,
                                               directoryOperator, userSerializer, logger, gitHubClient, processManager, discogsClient, fileParserFactory);

      windowManager.ShowWindow(new SystemTrayViewModel(windowManager, mainVM));
    }
  }
}