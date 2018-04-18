using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Scrubbler.Interfaces;
using Scrubbler.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.Test.ScrobblerTests
{
  /// <summary>
  /// Tests for the <see cref="CSVScrobbleViewModel"/>.
  /// </summary>
  [TestFixture]
  class CSVScrobbleTest
  {
    /// <summary>
    /// Tests scrobbling with <see cref="CSVScrobbleMode.Normal"/>.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task NormalScrobbleTest()
    {
      // given: CSVScrobbleViewModel with needed mocks to scrobble
      Mock<IAuthScrobbler> scrobblerMock = new Mock<IAuthScrobbler>();
      scrobblerMock.Setup(s => s.Auth.Authenticated).Returns(true);
      IEnumerable<Scrobble> actual = null;
      scrobblerMock.Setup(s => s.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>())).Callback<IEnumerable<Scrobble>>(s => actual = s)
                                                                                  .Returns(Task.Run(() => new ScrobbleResponse()));

      List<Scrobble> expected = new List<Scrobble>()
      {
        new Scrobble("TestArtist", "TestAlbum", "TestTrack", DateTime.Now) { AlbumArtist = "TestAlbumArtist", Duration = TimeSpan.FromSeconds(30)},
        new Scrobble("TestArtist2", "TestAlbum2", "TestTrack2", DateTime.Now.AddSeconds(1)) { AlbumArtist = "TestAlbumArtist2", Duration = TimeSpan.FromSeconds(31)},
        new Scrobble("TestArtist3", "TestAlbum3", "TestTrack3", DateTime.Now.AddSeconds(2)) { AlbumArtist = "TestAlbumArtist2", Duration = TimeSpan.FromSeconds(0)}
      };

      Mock<ITextFieldParser> parserMock = new Mock<ITextFieldParser>();
      string[][] fields = new string[3][]
      {
        new string[] {expected[0].Artist, expected[0].Album, expected[0].Track, expected[0].TimePlayed.ToString(), expected[0].AlbumArtist, expected[0].Duration.ToString() },
        new string[] {expected[1].Artist, expected[1].Album, expected[1].Track, expected[1].TimePlayed.ToString(), expected[1].AlbumArtist, expected[1].Duration.ToString() },
        new string[] {expected[2].Artist, expected[2].Album, expected[2].Track, expected[2].TimePlayed.ToString(), expected[2].AlbumArtist, expected[2].Duration.ToString() },
      };
      int i = 0;
      parserMock.Setup(p => p.ReadFields()).Returns(() => fields[i++]).Callback(() =>
      {
        if (i == 3)
          parserMock.Setup(p => p.EndOfData).Returns(true);
      });

      Mock<ITextFieldParserFactory> factoryMock = new Mock<ITextFieldParserFactory>();
      factoryMock.Setup(f => f.CreateParser(It.IsAny<string>())).Returns(parserMock.Object);

      CSVScrobbleViewModel vm = new CSVScrobbleViewModel(null, factoryMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        CSVFilePath = "C:\\TestFile.csv",
        ScrobbleMode = CSVScrobbleMode.Normal
      };

      await vm.ParseCSVFile();
      vm.SelectAll();

      // when: scrobbling
      await vm.Scrobble();

      // then: expected tracks scrobbled
      // we add 1 second to each TimePlayed of the expected because the vm does this too so you can scrobble from yourself...
      for (int e = 0; e < expected.Count; e++)
        expected[e] = expected[e].CloneWithAddedSecond();

      Assert.That(actual.IsEqualScrobble(expected));
    }
  }
}