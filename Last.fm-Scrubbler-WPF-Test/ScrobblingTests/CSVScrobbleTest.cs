using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubbler.Scrobbling.Scrobbler;
using Scrubbler.Helper;
using System.Linq;
using ScrubblerLib.Helper.FileParser;
using ScrubblerLib.Data;
using ScrubblerLib;

namespace Scrubbler.Test.ScrobblingTests
{
  /// <summary>
  /// Tests for the <see cref="FileParseScrobbleViewModel"/>.
  /// </summary>
  [TestFixture]
  class CSVScrobbleTest
  {
    /// <summary>
    /// Tests scrobbling with <see cref="ScrobbleMode.Normal"/>.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task NormalScrobbleTest()
    {
      // given: CSVScrobbleViewModel with needed mocks to scrobble
      IEnumerable<Scrobble> actual = null;
      var scrobblerMock = new Mock<IUserScrobbler>(MockBehavior.Strict);
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>((s) => actual = s)
                                                                                  .Returns(Task.Run(() => new ScrobbleResponse()));
      scrobblerMock.Setup(u => u.IsAuthenticated).Returns(true);

      var expected = new List<Scrobble>()
      {
        new Scrobble("TestArtist", "TestAlbum", "TestTrack", DateTime.Now) { AlbumArtist = "TestAlbumArtist", Duration = TimeSpan.FromSeconds(30)},
        new Scrobble("TestArtist2", "TestAlbum2", "TestTrack2", DateTime.Now.AddSeconds(1)) { AlbumArtist = "TestAlbumArtist2", Duration = TimeSpan.FromSeconds(31)},
        new Scrobble("TestArtist3", "TestAlbum3", "TestTrack3", DateTime.Now.AddSeconds(2)) { AlbumArtist = "TestAlbumArtist2", Duration = TimeSpan.FromSeconds(0)}
      };

      var parserFactoryMock = new Mock<IFileParserFactory>(MockBehavior.Strict);

      var csvParserMock = new Mock<IFileParser<CSVFileParserConfiguration>>(MockBehavior.Strict);
      csvParserMock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ScrobbleMode>(), It.IsAny<CSVFileParserConfiguration>())).Returns(
                                                                 new FileParseResult(expected.Select(i => new DatedScrobble(i)), Enumerable.Empty<string>()));

      parserFactoryMock.Setup(p => p.CreateCSVFileParser()).Returns(csvParserMock.Object);

      var jsonParserMock = new Mock<IFileParser<JSONFileParserConfiguration>>(MockBehavior.Strict);
      parserFactoryMock.Setup(p => p.CreateJSONFileParser()).Returns(jsonParserMock.Object);

      Mock<IFileOperator> fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);
      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      FileParseScrobbleViewModel vm = new FileParseScrobbleViewModel(windowManagerMock.Object, parserFactoryMock.Object, fileOperatorMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        FilePath = "C:\\TestFile.csv",
        ScrobbleMode = ScrobbleMode.Normal
      };

      await vm.ParseFile();
      vm.CheckAll();

      // when: scrobbling
      await vm.Scrobble();

      // then: expected tracks scrobbled
      Assert.That(actual.IsEqualScrobble(expected));
    }
  }
}