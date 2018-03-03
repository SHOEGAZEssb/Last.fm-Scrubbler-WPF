using Caliburn.Micro;
using Last.fm_Scrubbler_WPF.Interfaces;
using Last.fm_Scrubbler_WPF.Models;
using Last.fm_Scrubbler_WPF.ViewModels;
using System;
using System.Collections.Generic;

namespace Last.fm_Scrubbler_WPF
{
  /// <summary>
  /// Bootstrapper used to connect View and ViewModel on startup.
  /// </summary>
  internal class AppBootstrapper : BootstrapperBase
  {
    private SimpleContainer _container;

    /// <summary>
    /// Constructor.
    /// </summary>
    public AppBootstrapper()
    {
      Initialize();
    }

    protected override void Configure()
    {
      TypeMappingConfiguration map = new TypeMappingConfiguration()
      {
        DefaultSubNamespaceForViewModels = "Last.fm_Scrubbler_WPF.ViewModels",
        DefaultSubNamespaceForViews = "Last.fm_Scrubbler_WPF.Views"
      };

      ViewLocator.ConfigureTypeMappings(map);
      ViewLocator.AddSubNamespaceMapping("fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels", "fm_Scrubbler_WPF.Views.ScrobbleViews");
      ViewLocator.AddSubNamespaceMapping("fm_Scrubbler_WPF.ViewModels.SubViewModels", "fm_Scrubbler_WPF.Views.SubViews");
      ViewModelLocator.ConfigureTypeMappings(map);

      _container = new SimpleContainer();
      _container.Singleton<IWindowManager, WindowManager>();
      _container.Singleton<IExtendedWindowManager, ExtendedWindowManager>();
      _container.Singleton<ILastFMClientFactory, LastFMClientFactory>();
      _container.Singleton<IScrobblerFactory, ScrobblerFactory>();
      _container.Singleton<ILocalFileFactory, LocalFileFactory>();
      _container.PerRequest<MainViewModel>();
    }

    protected override void BuildUp(object instance)
    {
      _container.BuildUp(instance);
    }

    protected override object GetInstance(Type service, string key)
    {
      var instance = _container.GetInstance(service, key);
      if (instance != null)
        return instance;
      throw new InvalidOperationException("Could not locate any instances.");
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
      return _container.GetAllInstances(service);
    }

    /// <summary>
    /// Displays the root View.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
    {
      DisplayRootViewFor<MainViewModel>();
    }
  }
}