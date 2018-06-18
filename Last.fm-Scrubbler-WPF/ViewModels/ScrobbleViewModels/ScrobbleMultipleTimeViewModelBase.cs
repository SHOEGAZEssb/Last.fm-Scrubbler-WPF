using Scrubbler.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// Base class for ViewModels that have the ability
  /// to scrobble multiple scrobbles with a
  /// "Starting" or "Finishing" time.
  /// </summary>
  /// <typeparam name="T">Type of the scrobble ViewModel.</typeparam>
  public abstract class ScrobbleMultipleTimeViewModelBase<T> : ScrobbleTimeViewModelBase, ICanSelectScrobbles<T> where T : IScrobbableObject
  {
    #region Construction.

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    public ScrobbleMultipleTimeViewModelBase(IExtendedWindowManager windowManager, string displayName)
      : base(windowManager, displayName)
    { }

    #endregion Construction

    /// <summary>
    /// The collection of scrobbles.
    /// </summary>
    public abstract ObservableCollection<T> Scrobbles { get; protected set; }

    /// <summary>
    /// Gets if all scrobbles can currently be selected.
    /// </summary>
    public bool CanSelectAll => Scrobbles.Any(s => !s.ToScrobble);

    /// <summary>
    /// Gets if no scrobbles can currently be selected.
    /// </summary>
    public bool CanSelectNone => Scrobbles.Any(s => s.ToScrobble);

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    public virtual void SelectAll()
    {
      foreach (var s in Scrobbles)
      {
        s.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public virtual void SelectNone()
    {
      foreach (var s in Scrobbles)
      {
        s.ToScrobble = false;
      }
    }
  }
}