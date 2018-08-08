using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using Scrubbler.ExtraFunctions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.Test.ExtraFunctionsTests
{
  /// <summary>
  /// Tests for the <see cref="PasteYourTasteViewModel"/>.
  /// </summary>
  [TestFixture]
  class PasteYourTasteTest
  {
    /// <summary>
    /// Tests the basic creation of a taste text.
    /// </summary>
    [Test]
    public void CreateTasteTextTest()
    {
      // given: user api mock
      string username = "TestUser";
      int amount = 3;
      LastStatsTimeSpan timeSpan = LastStatsTimeSpan.Overall;

      // create test data
      List<LastArtist> responseArtists = new List<LastArtist>();
      for(int i = 0; i < amount; i++)
      {
        responseArtists.Add(new LastArtist() { Name = "TestArtist" + i });
      }

      Mock<IUserApi> userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      userApiMock.Setup(u => u.GetTopArtists(username, timeSpan, It.IsAny<int>(), amount))
                       .Returns(Task.Run(() => PageResponse<LastArtist>.CreateSuccessResponse(responseArtists)));

      PasteYourTasteViewModel vm = new PasteYourTasteViewModel(userApiMock.Object)
      {
        Username = username,
        Amount = amount
      };

      // when: getting the taste text
      vm.GetTopArtists();

      // then: taste text contains artists
      foreach(var artist in responseArtists)
      {
        Assert.That(vm.TasteText.Contains(artist.Name), Is.True);
      }
    }
  }
}