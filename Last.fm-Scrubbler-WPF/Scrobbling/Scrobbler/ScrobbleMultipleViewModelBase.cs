using Scrubbler.Helper;
using Scrubbler.Scrobbling.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

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
      get { return base.CanScrobble && (Scrobbles?.Any(i => i.ToScrobble) ?? false); }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return Scrobbles?.Any(i => i.ToScrobble) ?? false; }
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
    public bool CanCheckAll => Scrobbles?.Any(s => !s.ToScrobble) ?? false;

    /// <summary>
    /// Gets if all scrobbles can currently be unchecked.
    /// </summary>
    public bool CanUncheckAll => Scrobbles?.Any(s => s.ToScrobble) ?? false;

    /// <summary>
    /// Gets if selected scrobbles can be checked.
    /// </summary>
    public bool CanCheckSelected => Scrobbles?.Any(s => s.IsSelected && !s.ToScrobble) ?? false;

    /// <summary>
    /// Gets if selected scrobbles can be unchecked.
    /// </summary>
    public bool CanUncheckSelected => Scrobbles?.Any(s => s.IsSelected && s.ToScrobble) ?? false;

    /// <summary>
    /// Gets the amount of scrobbles that are
    /// marked as "ToScrobble".
    /// </summary>
    public int ToScrobbleCount => Scrobbles?.Where(s => s.ToScrobble).Count() ?? 0;

    /// <summary>
    /// Gets the amount of selected scrobbles.
    /// </summary>
    public int SelectedCount => Scrobbles?.Where(s => s.IsSelected).Count() ?? 0;

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
      SetToScrobbleState(Scrobbles, true);
    }

    /// <summary>
    /// Marks all scrobbles as not "ToScrobble".
    /// </summary>
    public virtual void UncheckAll()
    {
      SetToScrobbleState(Scrobbles, false);
    }

    /// <summary>
    /// Marks all selected scrobbles as "ToScrobble".
    /// </summary>
    public virtual void CheckSelected()
    {
      SetToScrobbleState(Scrobbles.Where(s => s.IsSelected), true);
    }

    /// <summary>
    /// Marks all selected scrobbles as not "ToScrobble".
    /// </summary>
    public virtual void UncheckSelected()
    {
      SetToScrobbleState(Scrobbles.Where(s => s.IsSelected), false);
    }

    /// <summary>
    /// Sets the "ToScrobble" state of the given <paramref name="toSet"/>.
    /// </summary>
    /// <param name="toSet">Items whose "ToScrobble" state to set.</param>
    /// <param name="state">State to set.</param>
    protected void SetToScrobbleState(IEnumerable<T> toSet, bool state)
    {
      Parallel.ForEach(toSet.Take(toSet.Count() - 1), s =>
      {
        s.UpdateToScrobbleSilent(state);
      });

      // set last one manually to trigger the event
      toSet.Last().ToScrobble = state;
      UpdateView();
    }

    /// <summary>
    /// Sets the "IsSelected" state of the given <paramref name="toSet"/>.
    /// </summary>
    /// <param name="toSet">Items whose "IsSelected" state to set.</param>
    /// <param name="state">State to set.</param>
    protected void SetIsSelectedState(IEnumerable<T> toSet, bool state)
    {
      Parallel.ForEach(toSet.Take(toSet.Count() - 1), s =>
      {
        s.UpdateIsSelectedSilent(state);
      });

      // set last one manually to trigger the event
      toSet.Last().ToScrobble = state;
      UpdateView();
    }

    /// <summary>
    /// Notifies that properties have changed.
    /// </summary>
    protected void NotifyCanProperties()
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
    /// Notifies the ui of property changes
    /// and manually refreshes it.
    /// </summary>
    private void UpdateView()
    {
      NotifyCanProperties();
      // todo: this is a workaround for the virtualization problem occurring
      // with huge csv files.
      ICollectionView view = CollectionViewSource.GetDefaultView(Scrobbles);
      view.Refresh();
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