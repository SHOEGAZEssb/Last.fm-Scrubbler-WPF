namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for a serializer.
  /// </summary>
  /// <typeparam name="T">Type of the object to serialize.</typeparam>
  public interface ISerializer<T>
  {
    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">Path to serialize the <paramref name="data"/> to.</param>
    void Serialize(T data, string path);

    /// <summary>
    /// Deserializes the file at <paramref name="path"/>.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <returns>Deserialized object.</returns>
    T Deserialize(string path);
  }
}