using Caliburn.Micro;
using IF.Lastfm.Core.Api.Enums;
using System;
using System.Windows;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.PasteYourTasteView"/>.
  /// </summary>
  class PasteYourTasteViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// The name of the user whose taste to paste.
    /// </summary>
    public string Username
    {
      get { return _username; }
      set
      {
        _username = value;
        NotifyOfPropertyChange(() => Username);
      }
    }
    private string _username;

    /// <summary>
    /// Amount of artists to paste.
    /// </summary>
    public int Amount
    {
      get { return _amount; }
      set
      {
        _amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }
    private int _amount;

    /// <summary>
    /// The timespan from where to get the top artists.
    /// </summary>
    public LastStatsTimeSpan TimeSpan
    {
      get { return _timeSpan; }
      set
      {
        _timeSpan = value;
        NotifyOfPropertyChange(() => TimeSpan);
      }
    }
    private LastStatsTimeSpan _timeSpan;

    /// <summary>
    /// The taste text.
    /// </summary>
    public string TasteText
    {
      get { return _tasteText; }
      private set
      {
        _tasteText = value;
        NotifyOfPropertyChange(() => TasteText);
        NotifyOfPropertyChange(() => CanCopy);
      }
    }
    private string _tasteText;

    /// <summary>
    /// Gets if certain controls on the ui are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanCopy);
      }
    }

    /// <summary>
    /// Gets if the copy button on the ui is enabled.
    /// </summary>
    public bool CanCopy
    {
      get { return TasteText != string.Empty && EnableControls; }
    }

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    public PasteYourTasteViewModel()
    {
      Amount = 20;
      TimeSpan = LastStatsTimeSpan.Overall;
    }

    /// <summary>
    /// Gets the specified <see cref="Amount"/> of top artists from the given <see cref="TimeSpan"/>
    /// of the <see cref="Username"/>.
    /// </summary>
    public async void GetTopArtists()
    {
      EnableControls = false;
      OnStatusUpdated("Fetching top artists...");

      var response = await MainViewModel.Client.User.GetTopArtists(Username, TimeSpan, 0, Amount);
      if(response.Success)
      {
        string tasteText = GetIntroText();
        for(int i = 0; i < response.TotalItems; i++)
        {
          tasteText += response.Content[i];
          if (i + 1 != response.TotalItems)
            tasteText += ",";
        }

        TasteText = tasteText;
        OnStatusUpdated("Successfully fetched top artists");
      }
      else
        OnStatusUpdated("Error fetching top artists");

      EnableControls = true;
    }

    /// <summary>
    /// Stores the <see cref="TasteText"/> in the clipboard.
    /// </summary>
    public void CopyTasteText()
    {
      Clipboard.SetText(TasteText);
    }

    /// <summary>
    /// Gets the intro line of the taste text based on the
    /// selected <see cref="TimeSpan"/>.
    /// </summary>
    /// <returns>Intro text.</returns>
    private string GetIntroText()
    {
      switch(TimeSpan)
      {
        case LastStatsTimeSpan.Overall:
          return "I'm into ";
        case LastStatsTimeSpan.Week:
          return "The last 7 days I have been into ";
        case LastStatsTimeSpan.Month:
          return "The last month I have been into ";
        case LastStatsTimeSpan.Quarter:
          return "The last 3 months I have been into ";
        case LastStatsTimeSpan.Half:
          return "The last 6 months I have been into ";
        case LastStatsTimeSpan.Year:
          return "The last year I have been into ";
        default:
          return "";
      }
    }
  }
}
