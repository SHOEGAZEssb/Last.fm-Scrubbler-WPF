using Moq;
using NUnit.Framework;
using Octokit;
using Scrubbler.Configuration;
using Scrubbler.Helper;
using Scrubbler.Test.MockData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrubbler.Test.ConfigurationTest
{
  /// <summary>
  /// Tests for the <see cref="GeneralSettingsViewModel"/>.
  /// </summary>
  [TestFixture]
  class GeneralSettingsViewModelTest
  {
    /// <summary>
    /// Tests if the automatic update check at startup works
    /// when a new version is found.
    /// </summary>
    [Test]
    public void AutomaticUpdateCheckNewVersionTest()
    {
      // given: mocks
      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<NewVersionViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);

      Mock<IGitHubClient> gitHubClientMock = new Mock<IGitHubClient>(MockBehavior.Strict);
      Mock<IRepositoriesClient> repositoriesClientMock = new Mock<IRepositoriesClient>(MockBehavior.Strict);
      Mock<IReleasesClient> releasesClientMock = new Mock<IReleasesClient>(MockBehavior.Strict);
      ReleaseAssetMock releaseAssetMock = new ReleaseAssetMock("TestDownloadUrl");
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", releaseAssetMock, "2.0");
      releasesClientMock.Setup(r => r.GetAll("coczero", "Last.fm-Scrubbler-WPF")).Returns(Task.FromResult((IReadOnlyList<Release>)new List<Release>(new[] { releaseMock })));
      repositoriesClientMock.Setup(r => r.Release).Returns(releasesClientMock.Object);
      gitHubClientMock.Setup(g => g.Repository).Returns(repositoriesClientMock.Object);

      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);

      // when: creating the vm and automatically searching for updates
      var vm = new GeneralSettingsViewModel(windowManagerMock.Object, gitHubClientMock.Object, processManagerMock.Object);

      // then: got repo version and NewVersionViewModel was shown
      Assert.That(() => releasesClientMock.Verify(r => r.GetAll("coczero", "Last.fm-Scrubbler-WPF"), Times.Once), Throws.Nothing);
      Assert.That(() => windowManagerMock.Verify(w => w.ShowDialog(It.IsAny<NewVersionViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>()), Times.Once),
                                                 Throws.Nothing);
    }

    /// <summary>
    /// Tests if the automatic update check at startup works
    /// when no new version is found.
    /// </summary>
    [Test]
    public void AutomaticUpdateCheckNoNewVersionTest()
    {
      // given: vm with mocks
      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<NewVersionViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);

      Mock<IGitHubClient> gitHubClientMock = new Mock<IGitHubClient>(MockBehavior.Strict);
      Mock<IRepositoriesClient> repositoriesClientMock = new Mock<IRepositoriesClient>(MockBehavior.Strict);
      Mock<IReleasesClient> releasesClientMock = new Mock<IReleasesClient>(MockBehavior.Strict);
      ReleaseAssetMock releaseAssetMock = new ReleaseAssetMock("TestDownloadUrl");
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", releaseAssetMock, "B1.0");
      releasesClientMock.Setup(r => r.GetAll("coczero", "Last.fm-Scrubbler-WPF")).Returns(Task.FromResult((IReadOnlyList<Release>)new List<Release>(new[] { releaseMock })));
      repositoriesClientMock.Setup(r => r.Release).Returns(releasesClientMock.Object);
      gitHubClientMock.Setup(g => g.Repository).Returns(repositoriesClientMock.Object);

      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);

      // when: creating the vm and automatically searching for updates
      var vm = new GeneralSettingsViewModel(windowManagerMock.Object, gitHubClientMock.Object, processManagerMock.Object);

      // then: got repo version and NewVersionViewModel was shown
      Assert.That(() => releasesClientMock.Verify(r => r.GetAll("coczero", "Last.fm-Scrubbler-WPF"), Times.Once), Throws.Nothing);
      Assert.That(() => windowManagerMock.Verify(w => w.ShowDialog(It.IsAny<NewVersionViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>()), Times.Never),
                                                 Throws.Nothing);
    }

    /// <summary>
    /// Tests if exceptions while checking for updates
    /// are handled correctly.
    /// </summary>
    [Test]
    public void UpdateCheckExceptionTest()
    {
      // given: mocks
      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);
      Mock<IMessageBoxService> messageBoxServiceMock = new Mock<IMessageBoxService>(MockBehavior.Strict);
      messageBoxServiceMock.Setup(m => m.ShowDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageBoxServiceButtons>(), It.IsAny<IMessageBoxServiceIcon>()))
                           .Returns(IMessageBoxServiceResult.OK);
      windowManagerMock.Setup(w => w.MessageBoxService).Returns(messageBoxServiceMock.Object);
      windowManagerMock.Setup(w => w.ShowDialog(It.IsAny<NewVersionViewModel>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(true);

      Mock<IGitHubClient> gitHubClientMock = new Mock<IGitHubClient>(MockBehavior.Strict);
      gitHubClientMock.Setup(g => g.Repository).Throws<Exception>();

      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);

      // when: creating the vm and automatically searching for updates
      var vm = new GeneralSettingsViewModel(windowManagerMock.Object, gitHubClientMock.Object, processManagerMock.Object);

      // then: no NewVersionViewModel was shown
      Assert.That(() => messageBoxServiceMock.Verify(m => m.ShowDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessageBoxServiceButtons>(), It.IsAny<IMessageBoxServiceIcon>()),
                                                     Times.Once), Throws.Nothing);

    }
  }
}