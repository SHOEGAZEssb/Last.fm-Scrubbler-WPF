using System;
using System.Windows.Input;

namespace Scrubbler
{
  /// <summary>
  /// Command that can be executed via the UI.
  /// </summary>
  class DelegateCommand : ICommand
  {
    #region ICommand Implementation

    /// <summary>
    /// Event that is fired when the can execute state
    /// of the command might have changed.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Checks if the command can be executed.
    /// </summary>
    /// <param name="parameter">Parameter for the check.</param>
    /// <returns>true if the command can be executed, otherwise false.</returns>
    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute(parameter);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Parameter for the command.</param>
    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    #endregion ICommand Implementation

    #region Member

    /// <summary>
    /// Delegate to execute.
    /// </summary>
    private readonly Predicate<object> _canExecute;

    /// <summary>
    /// Predicate to check if the
    /// command can be executed.
    /// </summary>
    private readonly Action<object> _execute;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="execute">Delegate to execute.</param>
    /// <param name="canExecute">Predicate to check if the
    /// command can be executed.</param>
    public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    #endregion Construction

    /// <summary>
    /// Fires the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
