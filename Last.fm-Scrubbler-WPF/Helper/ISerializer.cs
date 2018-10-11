using System;
using System.Collections.Generic;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Interface for a serializer.
  /// </summary>
  public interface ISerializer
  {
    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">File to serialize to.</param>
    void Serialize<T>(T data, string path);

    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">File to serialize to.</param>
    /// <param name="knownTypes">Known types of the serializer.</param>
    void Serialize<T>(T data, string path, IEnumerable<Type> knownTypes);

    /// <summary>
    /// Deserializes the given file.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    T Deserialize<T>(string path);

    /// <summary>
    /// Deserializes the given file.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <param name="knownTypes">Known types of the serializer.</param>
    /// <returns>The deserialized object.</returns>
    T Deserialize<T>(string path, IEnumerable<Type> knownTypes);
  }
}