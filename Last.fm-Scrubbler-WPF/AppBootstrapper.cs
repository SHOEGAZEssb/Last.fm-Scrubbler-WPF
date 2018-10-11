using Caliburn.Micro;
using Scrubbler.Helper;
using Scrubbler.Login;
using Scrubbler.Scrobbling;

namespace Scrubbler
{
  /// <summary>
  /// Bootstrapper used to connect View and ViewModel on startup.
  /// </summary>
  internal class AppBootstrapper : BootstrapperBase
  {
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
      ILastFMClientFactory lastFMClientFactory = new LastFMClientFactory();
      IScrobblerFactory scrobblerFactory = new ScrobblerFactory();
      ILocalFileFactory localFileFactory = new LocalFileFactory();
      IFileOperator fileOperator = new FileOperator();
      IDirectoryOperator directoryOperator = new DirectoryOperator();
      ISerializer userSerializer = new DCSerializer();
      ILogger logger = new Logger("log.txt");
      MainViewModel mainVM = new MainViewModel(windowManager, lastFMClientFactory, scrobblerFactory, localFileFactory, fileOperator,
                                               directoryOperator, userSerializer, logger);

      windowManager.ShowWindow(new SystemTrayViewModel(windowManager, mainVM));
    }
  }
}