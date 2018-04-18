using Scrubbler.Interfaces;

namespace Scrubbler.Models
{
  /// <summary>
  /// Factory creating <see cref="LocalFile"/>s.
  /// </summary>
  class LocalFileFactory : ILocalFileFactory
  {
    /// <summary>
    /// Creates a local file with the given <paramref name="file"/>.
    /// </summary>
    /// <param name="file">Path to the file to create a local file from.</param>
    /// <returns></returns>
    public ILocalFile CreateFile(string file)
    {
      return new LocalFile(file);
    }
  }
}