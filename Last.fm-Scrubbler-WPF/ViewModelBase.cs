using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scrubbler
{
  /// <summary>
  /// Base class for all ViewModels.
  /// </summary>
  public abstract class ViewModelBase : Screen, INotifyPropertyChanged
  {
    #region Properties

    /// <summary>
    /// Event that is fired when a property changed.
    /// </summary>
    //public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Event that triggers when the status should be changed.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

    /// <summary>
    /// Indicates if controls on the UI
    /// are enabled.
    /// </summary>
    public virtual bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange();
      }
    }
    /// <summary>
    /// Indicates if controls on the UI
    /// are enabled.
    /// </summary>
    protected bool _enableControls;

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    public ViewModelBase()
    {
      EnableControls = true;
    }

    #endregion Construction

    /// <summary>
    /// Fires the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    //protected void NotifyOfPropertyChange([CallerMemberName] string propertyName = "")
    //{
    //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    /// <summary>
    /// Updates the status.
    /// </summary>
    /// <param name="newStatus">New status.</param>
    protected void OnStatusUpdated(string newStatus)
    {
      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs(newStatus));
    }
  }
}