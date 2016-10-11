namespace Last.fm_Scrubbler_WPF.Models
{
  /// <summary>
  /// Represents a last.fm user.
  /// </summary>
  public class User
  {
    #region Properties

    /// <summary>
    /// Username of this user.
    /// </summary>
    public string Username
    {
      get { return _username; }
      set { _username = value; }
    }
    private string _username;

    /// <summary>
    /// Login token of this username.
    /// </summary>
    public string Token
    {
      get { return _token; }
      set { _token = value; }
    }
    private string _token;

    /// <summary>
    /// If this user is a subscriber.
    /// </summary>
    public bool IsSubscriber
    {
      get { return _isSubscriber; }
      set { _isSubscriber = value; }
    }
    private bool _isSubscriber;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="username">Username of the user.</param>
    /// <param name="token">Login token.</param>
    /// <param name="isSubscriber">If this user is a subscriber.</param>
    public User(string username, string token, bool isSubscriber)
    {
      Username = username;
      Token = token;
      IsSubscriber = isSubscriber;
    }

    /// <summary>
    /// Constructor for serializing.
    /// </summary>
    public User()
    { }
  }
}