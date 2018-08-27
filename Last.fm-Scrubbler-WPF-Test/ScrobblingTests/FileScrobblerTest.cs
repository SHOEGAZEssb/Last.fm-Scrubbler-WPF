using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scrubbler.Scrobbling.Scrobbler;
using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Scrobbling;

namespace Scrubbler.Test.ScrobblingTests
{
  /// <summary>
  /// Tests if scrobbling via the <see cref="FileScrobbleViewModel"/> works.
  /// </summary>
  [TestFixture]
  class FileScrobblerTest
  {
    [Test]
    public async Task ScrobbleTest()
    {
      // given: FileScrobblerViewModel with mocks
      Mock<IOpenFileDialog> openFileDialogMock = new Mock<IOpenFileDialog>();
      openFileDialogMock.Setup(o => o.ShowDialog()).Returns(true);
      string[] files = new[] { "TestFile.mp3", "TestFile2.mp3", "TestFile3.mp3" };
      openFileDialogMock.SetupGet(o => o.FileNames).Returns(files);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>();
      windowManagerMock.Setup(w => w.CreateOpenFileDialog()).Returns(openFileDialogMock.Object);

      Mock<ILocalFile>[] localFileMocks = GetLocalFileMocks(files.Length);

      Mock<ILocalFileFactory> localFileFactoryMock = new Mock<ILocalFileFactory>();
      localFileFactoryMock.Setup(l => l.CreateFile(files[0])).Returns(localFileMocks[0].Object);
      localFileFactoryMock.Setup(l => l.CreateFile(files[1])).Returns(localFileMocks[1].Object);
      localFileFactoryMock.Setup(l => l.CreateFile(files[2])).Returns(localFileMocks[2].Object);

      IEnumerable<Scrobble> actual = null;
      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>(MockBehavior.Strict);
      scrobblerMock.Setup(u => u.ScrobbleAsync(It.IsAny<IEnumerable<Scrobble>>(), false)).Callback<IEnumerable<Scrobble>, bool>((s, c) => actual = s)
                                                                                  .Returns(Task.Run(() => new ScrobbleResponse()));
      scrobblerMock.Setup(u => u.IsAuthenticated).Returns(true);

      Mock<IFileOperator> fileOperatorMock = new Mock<IFileOperator>(MockBehavior.Strict);

      DateTime scrobbleTime = DateTime.Now;
      List<Scrobble> expected = new List<Scrobble>()
      {
        new Scrobble("TestArtist2", "TestAlbum2", "TestTrack2", scrobbleTime) { AlbumArtist = "TestAlbumArtist2", Duration = TimeSpan.FromSeconds(1)},
        new Scrobble("TestArtist1", "TestAlbum1", "TestTrack1", scrobbleTime.Subtract(TimeSpan.FromSeconds(1))) { AlbumArtist = "TestAlbumArtist1", Duration = TimeSpan.FromSeconds(1)},
        new Scrobble("TestArtist0", "TestAlbum0", "TestTrack0", scrobbleTime.Subtract(TimeSpan.FromSeconds(2))) { AlbumArtist = "TestAlbumArtist0", Duration = TimeSpan.FromSeconds(1)}
      };

      FileScrobbleViewModel vm = new FileScrobbleViewModel(windowManagerMock.Object, localFileFactoryMock.Object, fileOperatorMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
      };
      vm.ScrobbleTimeVM.UseCurrentTime = false;
      vm.ScrobbleTimeVM.Time = scrobbleTime;

      await vm.AddFiles();
      vm.CheckAll();

      // when: scrobbling
      await vm.Scrobble();

      // then: actual scrobbles match
      Assert.That(TestHelper.IsEqualScrobble(actual, expected));
    }

    /// <summary>
    /// Helper method for creating <see cref="ILocalFile"/> mocks.
    /// </summary>
    /// <param name="count">Amount of mocks to create.</param>
    /// <returns>ILocalFile mocks.</returns>
    private Mock<ILocalFile>[] GetLocalFileMocks(int count)
    {
      Mock<ILocalFile>[] mocks = new Mock<ILocalFile>[count];
      for(int i = 0; i < count; i++)
      {
        Mock<ILocalFile> m = new Mock<ILocalFile>();
        m.SetupGet(f => f.Artist).Returns("TestArtist" + i);
        m.SetupGet(f => f.Album).Returns("TestAlbum" + i);
        m.SetupGet(f => f.Track).Returns("TestTrack" + i);
        m.SetupGet(f => f.AlbumArtist).Returns("TestAlbumArtist" + i);
        m.SetupGet(f => f.Duration).Returns(TimeSpan.FromSeconds(1));
        m.SetupGet(f => f.TrackNumber).Returns(i);
        mocks[i] = m;
      }

      return mocks;
    }
  }
}