using System.Collections.ObjectModel;

namespace Scrubbler.Interfaces
{
  /// <summary>
  /// Interface for an object that can
  /// scrobble multiple scrobbles.
  /// </summary>
  /// <typeparam name="T">Type of the scrobbable object.</typeparam>
  interface ICanSelectScrobbles<T> where T : IScrobbableObjectViewModel
  {
    /// <summary>
    /// Possible objects to scrobble.
    /// </summary>
    ObservableCollection<T> Scrobbles { get; }

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    void CheckAll();

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    void UncheckAll();

    /// <summary>
    /// Gets if all scrobbles can currently be selected.
    /// </summary>
    bool CanCheckAll { get; }

    /// <summary>
    /// Gets if all scrobbles can currently be unselected.
    /// </summary>
    bool CanUncheckAll { get; }

    /// <summary>
    /// Gets the amount of scrobbles that are
    /// marked as "ToScrobble".
    /// </summary>
    int ToScrobbleCount { get; }
  }
}