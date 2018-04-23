using Scrubbler.Interfaces;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Scrubbler.Models
{
  /// <summary>
  /// Serializer using a <see cref="DataContractSerializer"/>.
  /// </summary>
  /// <typeparam name="T">Type of the object to serialize.</typeparam>
  class DCSerializer<T> : ISerializer<T>
  {
    #region Member

    /// <summary>
    /// The actual serializer.
    /// </summary>
    private DataContractSerializer _serializer;

    #endregion Member

    #region Construction

    public DCSerializer()
    {
      _serializer = new DataContractSerializer(typeof(T));
    }

    #endregion Construction

    #region ISerializer Implementation

    /// <summary>
    /// Deserializes the file at <paramref name="path"/>.
    /// </summary>
    /// <param name="path">Path to the file to deserialize.</param>
    /// <returns>Deserialized object.</returns>
    public T Deserialize(string path)
    {
      using (XmlReader xmlReader = XmlReader.Create(path))
      {
        return (T)_serializer.ReadObject(xmlReader);
      }
    }

    /// <summary>
    /// Serializes the given <paramref name="data"/> to
    /// the given <paramref name="path"/>.
    /// </summary>
    /// <param name="data">Object to serialize.</param>
    /// <param name="path">Path to serialize the <paramref name="data"/> to.</param>
    public void Serialize(T data, string path)
    {
      using (FileStream fs = new FileStream(path, FileMode.Create))
      {
        _serializer.WriteObject(fs, data);
      }
    }

    #endregion ISerializer Implementation
  }
}