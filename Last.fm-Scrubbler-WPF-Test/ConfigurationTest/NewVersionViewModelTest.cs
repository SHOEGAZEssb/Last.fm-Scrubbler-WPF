using Moq;
using NUnit.Framework;
using Scrubbler.Configuration;
using Scrubbler.Helper;
using Scrubbler.Test.MockData;
using System.Diagnostics;

namespace Scrubbler.Test.ConfigurationTest
{
  /// <summary>
  /// Tests for the <see cref="NewVersionViewModel"/>.
  /// </summary>
  [TestFixture]
  class NewVersionViewModelTest
  {
    /// <summary>
    /// Tests the construction of a new <see cref="NewVersionViewModel"/>.
    /// </summary>
    [Test]
    public void ConstructionTest()
    {
      // given: mocks
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", new ReleaseAssetMock("TestBrowserDownloadUrl"), "TestTagName");
      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);

      // when: creating the vm
      var vm = new NewVersionViewModel(releaseMock, processManagerMock.Object);

      // then: properties have been read out
      Assert.That(vm.VersionName, Is.SameAs(releaseMock.Name));
      Assert.That(vm.Description, Is.SameAs(releaseMock.Body));
    }

    /// <summary>
    /// Tests that constructing a <see cref="NewVersionViewModel"/>
    /// without a given <see cref="Octokit.Release"/> fails
    /// </summary>
    [Test]
    public void NoReleaseConstructionTest()
    {
      // given: mocks
      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);

      // when: creating the vm
      Assert.That(() => new NewVersionViewModel(null, processManagerMock.Object), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests that constructing a <see cref="NewVersionViewModel"/>
    /// without a given <see cref="Octokit.Release"/> fails
    /// </summary>
    [Test]
    public void NoIProcessManagerConstructionTest()
    {
      // given: mocks
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", new ReleaseAssetMock("TestBrowserDownloadUrl"), "TestTagName");

      // when: creating the vm
      Assert.That(() => new NewVersionViewModel(releaseMock, null), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests if opening the release page works.
    /// </summary>
    [Test]
    public void OpenReleasePageTest()
    {
      // given: vm with mocks
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", new ReleaseAssetMock("TestBrowserDownloadUrl"), "TestTagName");
      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);
      processManagerMock.Setup(p => p.Start(releaseMock.HtmlUrl)).Returns<Process>(null);

      var vm = new NewVersionViewModel(releaseMock, processManagerMock.Object);

      // when: trying to open the release page
      vm.OpenReleasePage();

      // then: process was started
      Assert.That(() => processManagerMock.Verify(p => p.Start(releaseMock.HtmlUrl), Times.Once), Throws.Nothing);
    }

    /// <summary>
    /// Tests if downloading the release works.
    /// </summary>
    [Test]
    public void DownloadReleaseTest()
    {
      // given: vm with mocks
      ReleaseMock releaseMock = new ReleaseMock("TestName", "TestBody", "TestHtmlUrl", new ReleaseAssetMock("TestBrowserDownloadUrl"), "TestTagName");
      Mock<IProcessManager> processManagerMock = new Mock<IProcessManager>(MockBehavior.Strict);
      processManagerMock.Setup(p => p.Start(releaseMock.Assets[0].BrowserDownloadUrl)).Returns<Process>(null);

      var vm = new NewVersionViewModel(releaseMock, processManagerMock.Object);

      // when: trying to download the release
      vm.DownloadRelease();

      // then: process was started
      Assert.That(() => processManagerMock.Verify(p => p.Start(releaseMock.Assets[0].BrowserDownloadUrl), Times.Once), Throws.Nothing);
    }
  }
}
