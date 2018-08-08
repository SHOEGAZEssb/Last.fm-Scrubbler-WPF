using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using System;
using System.Windows;

namespace Scrubbler.ExtraFunctions
{
  /// <summary>
  /// ViewModel for the <see cref="PasteYourTasteView"/>.
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
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
        NotifyOfPropertyChange();
      }
    }
    private string _tasteText;

    #endregion Properties

    #region Member

    /// <summary>
    /// Base URL to a Last.fm user profile.
    /// </summary>
    private const string LASTFMPROFILEURL = "https://www.last.fm/user/";

    /// <summary>
    /// The last.fm api object to get the scrobbles of an user.
    /// </summary>
    private IUserApi _userAPI;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userAPI">The last.fm api object to get the scrobbles of an user.</param>
    public PasteYourTasteViewModel(IUserApi userAPI)
      : base("Paste Your Taste")
    {
      _userAPI = userAPI;
      Amount = 20;
      TimeSpan = LastStatsTimeSpan.Overall;
    }

    /// <summary>
    /// Gets the specified <see cref="Amount"/> of top artists from the given <see cref="TimeSpan"/>
    /// of the <see cref="Username"/>.
    /// </summary>
    public async void GetTopArtists()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated(string.Format("Fetching top artists of '{0}'...", Username));

        var response = await _userAPI.GetTopArtists(Username, TimeSpan, 1, Amount);
        if (response.Success && response.Status == LastResponseStatus.Successful)
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
          OnStatusUpdated(string.Format("Successfully fetched top artists of '{0}'", Username));
        }
        else
          OnStatusUpdated(string.Format("Error fetching top artists of '{0}': {1}", Username, response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while fetching top artists of '{0}': {1}", Username, ex.Message));
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