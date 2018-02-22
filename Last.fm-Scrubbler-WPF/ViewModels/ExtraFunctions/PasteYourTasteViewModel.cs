using IF.Lastfm.Core.Api.Enums;
using System;
using System.Windows;

namespace Last.fm_Scrubbler_WPF.ViewModels.ExtraFunctions
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ExtraFunctions.PasteYourTasteView"/>.
  /// </summary>
  public class PasteYourTasteViewModel : ExtraFunctionViewModelBase
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
        NotifyOfPropertyChange(() => CanFetch);
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
    /// If the profile link should be added before
    /// the artists.
    /// </summary>
    public bool AddProfileLink
    {
      get { return _addProfileLink; }
      set
      {
        _addProfileLink = value;
        NotifyOfPropertyChange(() => AddProfileLink);
      }
    }
    private bool _addProfileLink;

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
        NotifyOfPropertyChange(() => CanFetch);
        NotifyOfPropertyChange(() => CanCopy);
      }
    }

    /// <summary>
    /// Gets if the fetch button on the ui is enabled.
    /// </summary>
    public bool CanFetch
    {
      get { return Username != null && Username != string.Empty && EnableControls; }
    }

    /// <summary>
    /// Gets if the copy button on the ui is enabled.
    /// </summary>
    public bool CanCopy
    {
      get { return TasteText != null && TasteText != string.Empty && EnableControls; }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Base URL to a Last.fm user profile.
    /// </summary>
    private const string LASTFMPROFILEURL = "https://www.last.fm/user/";

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public PasteYourTasteViewModel()
      : base("Paste Your Taste")
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

      try
      {
        OnStatusUpdated("Fetching top artists...");

        var response = await MainViewModel.Client.User.GetTopArtists(Username, TimeSpan, 1, Amount);
        if (response.Success)
        {
          string tasteText = "";
          if (AddProfileLink)
            tasteText += LASTFMPROFILEURL + Username + Environment.NewLine + Environment.NewLine;

          tasteText += GetIntroText();
          for (int i = 0; i < response.Content.Count; i++)
          {
            tasteText += response.Content[i].Name;
            if (i + 1 != response.Content.Count)
              tasteText += ", ";
          }

          TasteText = tasteText;
          OnStatusUpdated("Successfully fetched top artists");
        }
        else
          OnStatusUpdated("Error fetching top artists");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while fetching top artists: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
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
      switch (TimeSpan)
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