using IF.Lastfm.Core.Objects;
using NUnit.Framework;
using Scrubbler.Login;
using System;
using System.Linq;

namespace Scrubbler.Test.LoginTests
{
  /// <summary>
  /// Tests for a <see cref="User"/>.
  /// </summary>
  [TestFixture]
  class UserTest
  {
    /// <summary>
    /// Tests the construction of
    /// a <see cref="User"/> object.
    /// </summary>
    [Test]
    public void ConstructionTest()
    {
      // given: example data
      string username = "TestUser";
      string token = "TestToken";
      bool subscriber = true;

      // when: creating a user
      User user = new User(username, token, subscriber);

      // then: data is set
      Assert.That(user.Username, Is.SameAs(username));
      Assert.That(user.Token, Is.SameAs(token));
      Assert.That(user.IsSubscriber, Is.EqualTo(subscriber));
      CollectionAssert.IsEmpty(user.RecentScrobbles);
    }

    /// <summary>
    /// Tests if scrobbles older than one day
    /// get cleared correctly.
    /// </summary>
    [Test]
    public void UpdateRecentScrobblesTest()
    {
      // given: user with recent scrobbles
      User user = new User("TestUser", "TestToken", false);

      int numScrobbles = 10;
      DateTime timePlayed = DateTime.Now.Subtract(TimeSpan.FromHours(25));
      Scrobble[] scrobbles = new Scrobble[numScrobbles];
      for(int i = 0; i < numScrobbles; i++)
      {
        // half of the scrobbles get a "normal" date that should
        // not be cleared
        if (i >= numScrobbles / 2)
          scrobbles[i] = new Scrobble("", "", "", DateTime.Now);
        else
          scrobbles[i] = new Scrobble("", "", "", timePlayed);
      }

      user.AddScrobbles(scrobbles);

      // when: updating recent scrobbles
      user.UpdateRecentScrobbles();

      // then: half of the scrobbles should be gone
      Assert.That(user.RecentScrobbles.Count, Is.EqualTo(numScrobbles / 2));
      // all recent scrobbles are newer than 1 day
      Assert.That(user.RecentScrobbles.All(s => s.Item2 > DateTime.Now.Subtract(TimeSpan.FromDays(1))));
    }

    /// <summary>
    /// Tests if the <see cref="User.RecentScrobblesChanged"/>
    /// event fires correctly.
    /// </summary>
    [Test]
    public void RecentScrobblesChangedTest()
    {
      // given: User
      User user = new User("", "", false);
      bool eventFired = false;
      user.RecentScrobblesChanged += (o, e) => eventFired = true;

      // when: adding a scrobble
      user.AddScrobbles(new[] { new Scrobble() });

      // then: event fired
      Assert.That(eventFired, Is.True);
    }
  }
}