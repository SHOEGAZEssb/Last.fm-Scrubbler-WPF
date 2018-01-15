using Caliburn.Micro;
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
      _container = new SimpleContainer();
      _container.Singleton<IWindowManager, WindowManager>();
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