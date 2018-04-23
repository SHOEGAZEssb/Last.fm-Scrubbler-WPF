using System.Runtime.Serialization;

namespace Scrubbler.Models
{
  /// <summary>
  /// Represents a last.fm user.
  /// </summary>
  [DataContract]
  public class User
  {
    #region Properties

    /// <summary>
    /// Username of this user.
    /// </summary>
    [DataMember]
    public string Username { get; private set; }

    /// <summary>
    /// Login token of this user.
    /// </summary>
    [DataMember]
    public string Token { get; private set; }

    /// <summary>
    /// If this user is a subscriber.
    /// </summary>
    [DataMember]
    public bool IsSubscriber { get; private set; }

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
  }
}