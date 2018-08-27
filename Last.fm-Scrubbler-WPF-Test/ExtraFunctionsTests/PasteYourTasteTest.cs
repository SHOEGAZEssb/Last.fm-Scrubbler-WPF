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
    public async Task CreateTasteTextTest()
    {
      // given: user api mock
      string username = "TestUser";
      LastStatsTimeSpan timeSpan = LastStatsTimeSpan.Overall;

      // create test data
      int amount = 3;
      IEnumerable<LastArtist> responseArtists = TestHelper.CreateGenericArtists(amount);

      Mock<IUserApi> userApiMock = new Mock<IUserApi>(MockBehavior.Strict);
      userApiMock.Setup(u => u.GetTopArtists(username, timeSpan, It.IsAny<int>(), amount))
                       .Returns(Task.Run(() => PageResponse<LastArtist>.CreateSuccessResponse(responseArtists)));

      PasteYourTasteViewModel vm = new PasteYourTasteViewModel(userApiMock.Object)
      {
        Username = username,
        Amount = amount
      };

      // when: getting the taste text
      await vm.GetTopArtists();

      // then: taste text contains artists
      foreach(var artist in responseArtists)
      {
        Assert.That(vm.TasteText.Contains(artist.Name), Is.True);
      }
    }
  }
}