using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  class SetlistFMScrobbleViewModel : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// Gets if certain controls that modify the
    /// scrobbling data are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanPreview);
      }
    }

    /// <summary>
    /// Gets if the scrobble button is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return true; }
    }

    #endregion Properties

    #region Private Member

    private SetlistFmApi.SetlistFmApi _setlistFMClient;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public SetlistFMScrobbleViewModel()
    {
      _setlistFMClient = new SetlistFmApi.SetlistFmApi("23b3fd98-f5c7-49c6-a7d2-28498c0c2283");
    }

    public override Task Scrobble()
    {
      throw new NotImplementedException();
    }

    public override void Preview()
    {
      throw new NotImplementedException();
    }
  }
}