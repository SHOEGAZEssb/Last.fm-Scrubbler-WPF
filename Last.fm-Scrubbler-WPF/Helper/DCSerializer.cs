using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Scrubbler.Helper
{
  /// <summary>
  /// Serializer using a <see cref="DataContractSerializer"/>.
  /// </summary>
  class DCSerializer : ISerializer
  {
    #region ISerializer Implementation

    /// <summary>
    /// Deserializes the given file.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize<T>(string path)
    {
      return Deserialize<T>(path, Enumerable.Empty<Type>());
    }

    /// <summary>
    /// Deserializes the given file.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <param name="knownTypes">Known types of the serializer.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize<T>(string path, IEnumerable<Type> knownTypes)
    {
      DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
      using (XmlReader xmlReader = XmlReader.Create(path))
      {
        return (T)serializer.ReadObject(xmlReader);
      }
    }

    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">File to serialize to.</param>
    public void Serialize<T>(T data, string path)
    {
      Serialize(data, path, Enumerable.Empty<Type>());
    }

    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">File to serialize to.</param>
    /// <param name="knownTypes">Known types of the serializer.</param>
    public void Serialize<T>(T data, string path, IEnumerable<Type> knownTypes)
    {
      DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes);
      using (var w = new StreamWriter(path, false, Encoding.UTF8))
      {
        using (var writer = XmlWriter.Create(w, new XmlWriterSettings() { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates, Encoding = Encoding.UTF8 }))
        {
          serializer.WriteObject(writer, data);
        }
      }
    }

    #endregion ISerializer Implementation
  }
}