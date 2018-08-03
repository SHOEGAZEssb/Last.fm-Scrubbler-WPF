using Caliburn.Micro;
using Scrubbler.Interfaces;
using Scrubbler.Login;
using Scrubbler.Models;
using Scrubbler.ViewModels;

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
    /// Configures the View-ViewModel type- and
    /// namespace mappings.
    /// </summary>
    protected override void Configure()
    {
      TypeMappingConfiguration map = new TypeMappingConfiguration()
      {
        DefaultSubNamespaceForViewModels = "Scrubbler.ViewModels",
        DefaultSubNamespaceForViews = "Scrubbler.Views"
      };

      ViewLocator.ConfigureTypeMappings(map);
      ViewLocator.AddSubNamespaceMapping("Scrubbler.ViewModels.ScrobbleViewModels", "Scrubbler.Views.ScrobbleViews");
      ViewLocator.AddSubNamespaceMapping("Scrubbler.ViewModels.SubViewModels", "Scrubbler.Views.SubViews");
      ViewModelLocator.ConfigureTypeMappings(map);
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
      ISerializer<User> userSerializer = new DCSerializer<User>();
      ILogger logger = new Logger("log.txt");
      MainViewModel mainVM = new MainViewModel(windowManager, lastFMClientFactory, scrobblerFactory, localFileFactory, fileOperator,
                                               directoryOperator, userSerializer, logger);

      windowManager.ShowWindow(new SystemTrayViewModel(windowManager, mainVM));
    }
  }
}