using Scrubbler.Scrobbling.Data;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for a factory creating <see cref="ILocalFile"/>s.
  /// </summary>
  public interface ILocalFileFactory
  {
    /// <summary>
    /// Creates a local file from the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">Path of the file.</param>
    /// <returns>Newly created <see cref="ILocalFile"/>.</returns>
    ILocalFile CreateFile(string file);
  }
}