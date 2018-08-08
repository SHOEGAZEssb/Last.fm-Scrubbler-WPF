using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// Base class for ViewModels that have the ability
  /// to scrobble multiple scrobbles with a
  /// "Starting" or "Finishing" time.
  /// </summary>
  /// <typeparam name="T">Type of the scrobble ViewModel.</typeparam>
  public abstract class ScrobbleMultipleViewModelBase<T> : ScrobbleViewModelBase, ICanSelectScrobbles<T> where T : IScrobbableObjectViewModel
  {
    #region Properties

    /// <summary>
    /// Gets if the scrobble button on the ui is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && Scrobbles.Any(i => i.ToScrobble); }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return Scrobbles.Any(i => i.ToScrobble); }
    }

    /// <summary>
    /// The collection of scrobbles.
    /// </summary>
    public ObservableCollection<T> Scrobbles
    {
      get { return _scrobbles; }
      protected set
      {
        if (Scrobbles != null)
        {
          Scrobbles.CollectionChanged -= Scrobbles_CollectionChanged;
          DisconnectExistingToScrobbleEvent();
        }

        _scrobbles = value;
        _scrobbles.CollectionChanged += Scrobbles_CollectionChanged;
        ConnectExistingToScrobbleEvent();

        NotifyOfPropertyChange();
        NotifyCanProperties();
      }
    }
    private ObservableCollection<T> _scrobbles;

    /// <summary>
    /// Gets if all scrobbles can currently be selected.
    /// </summary>
    public bool CanCheckAll => Scrobbles.Any(s => !s.ToScrobble);

    /// <summary>
    /// Gets if all scrobbles can currently be unchecked.
    /// </summary>
    public bool CanUncheckAll => Scrobbles.Any(s => s.ToScrobble);

    /// <summary>
    /// Gets if selected scrobbles can be checked.
    /// </summary>
    public bool CanCheckSelected => Scrobbles.Any(s => s.IsSelected && !s.ToScrobble);

    /// <summary>
    /// Gets if selected scrobbles can be unchecked.
    /// </summary>
    public bool CanUncheckSelected => Scrobbles.Any(s => s.IsSelected && s.ToScrobble);

    /// <summary>
    /// Gets the amount of scrobbles that are
    /// marked as "ToScrobble".
    /// </summary>
    public int ToScrobbleCount => Scrobbles.Where(s => s.ToScrobble).Count();

    /// <summary>
    /// Gets the amount of selected scrobbles.
    /// </summary>
    public int SelectedCount => Scrobbles.Where(s => s.IsSelected).Count();

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="displayName">Display name.</param>
    public ScrobbleMultipleViewModelBase(IExtendedWindowManager windowManager, string displayName)
      : base(windowManager, displayName)
    { }

    #endregion Construction

    /// <summary>
    /// Marks all scrobbles as "ToScrobble".
    /// </summary>
    public virtual void CheckAll()
    {
      foreach (var s in Scrobbles)
      {
        s.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public virtual void UncheckAll()
    {
      foreach (var s in Scrobbles)
      {
        s.ToScrobble = false;
      }
    }

    /// <summary>
    /// Marks all selected scrobbles as "ToScrobble".
    /// </summary>
    public virtual void CheckSelected()
    {
      foreach(var s in Scrobbles.Where(s => s.IsSelected))
      {
        s.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all selected scrobbles as not "ToScrobble".
    /// </summary>
    public virtual void UncheckSelected()
    {
      foreach (var s in Scrobbles.Where(s => s.IsSelected))
      {
        s.ToScrobble = false;
      }
    }

    /// <summary>
    /// Connects events when <see cref="Scrobbles"/> change.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">EventArgs.</param>
    private void Scrobbles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (T scrobble in e.NewItems)
        {
          scrobble.ToScrobbleChanged += Scrobble_StateChanged;
          scrobble.IsSelectedChanged += Scrobble_StateChanged;
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (T scrobble in e.OldItems)
        {
          scrobble.ToScrobbleChanged -= Scrobble_StateChanged;
          scrobble.IsSelectedChanged -= Scrobble_StateChanged;
        }
      }

      NotifyCanProperties();
    }

    /// <summary>
    /// Notifies that properties have changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void Scrobble_StateChanged(object sender, EventArgs e)
    {
      NotifyCanProperties();
    }

    /// <summary>
    /// Notifies that properties have changed.
    /// </summary>
    private void NotifyCanProperties()
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
      NotifyOfPropertyChange(() => CanCheckAll);
      NotifyOfPropertyChange(() => CanUncheckAll);
      NotifyOfPropertyChange(() => CanCheckSelected);
      NotifyOfPropertyChange(() => CanUncheckSelected);
      NotifyOfPropertyChange(() => ToScrobbleCount);
      NotifyOfPropertyChange(() => SelectedCount);
    }

    /// <summary>
    /// Connects the ToScrobbleChanged event
    /// for all scrobbles.
    /// </summary>
    private void ConnectExistingToScrobbleEvent()
    {
      foreach (T scrobble in Scrobbles)
      {
        scrobble.ToScrobbleChanged += Scrobble_StateChanged;
        scrobble.IsSelectedChanged += Scrobble_StateChanged;
      }
    }

    /// <summary>
    /// Disconnects the ToScrobbleChanged event
    /// for all scrobbles.
    /// </summary>
    private void DisconnectExistingToScrobbleEvent()
    {
      foreach (T scrobble in Scrobbles)
      {
        scrobble.ToScrobbleChanged -= Scrobble_StateChanged;
        scrobble.IsSelectedChanged -= Scrobble_StateChanged;
      }
    }
  }
}