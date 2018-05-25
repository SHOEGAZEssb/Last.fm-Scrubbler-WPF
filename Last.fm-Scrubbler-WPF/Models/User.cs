using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Event that fires when the <see cref="RecentScrobbles"/>
    /// change.
    /// </summary>
    public event EventHandler RecentScrobblesChanged;

    /// <summary>
    /// Allowed scrobbles per day.
    /// </summary>
    public const int MAXSCROBBLESPERDAY = 2800;

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

    /// <summary>
    /// List of recent scrobbles.
    /// Scrobbles older than a day should be removed from this.
    /// No more than 2.8k scrobbles should be in here.
    /// </summary>
    public IReadOnlyList<Tuple<Scrobble, DateTime>> RecentScrobbles => _recentScrobbles.AsReadOnly();
    [DataMember]
    private List<Tuple<Scrobble, DateTime>> _recentScrobbles;

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
      _recentScrobbles = new List<Tuple<Scrobble, DateTime>>(MAXSCROBBLESPERDAY);
    }

    /// <summary>
    /// Updates the <see cref="RecentScrobbles"/>,
    /// removing scrobbles older than 1 day.
    /// </summary>
    public void UpdateRecentScrobbles()
    {
      int oldCount = RecentScrobbles.Count;
      _recentScrobbles = RecentScrobbles.Where(i => (DateTime.Now - i.Item2) <= TimeSpan.FromDays(1)).ToList();

      if(oldCount != RecentScrobbles.Count)
        RecentScrobblesChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds the given <paramref name="scrobbles"/> to
    /// the <see cref="RecentScrobbles"/>.
    /// </summary>
    /// <param name="scrobbles">Scrobbles to add.</param>
    /// <param name="timeScrobbled">The time this scrobble was scrobbled.
    /// Leave null if you want to use the TimePlayed of the scrobble.</param>
    public void AddScrobbles(IEnumerable<Scrobble> scrobbles, DateTime? timeScrobbled = null)
    {
      foreach (var scrobble in scrobbles)
        _recentScrobbles.Add(new Tuple<Scrobble, DateTime>(scrobble, timeScrobbled ?? scrobble.TimePlayed.DateTime));
      RecentScrobblesChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}