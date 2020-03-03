using System;

namespace Scrubbler
{
  /// <summary>
  /// Base ViewModel for a ViewModel that is displayed in a TabControl.
  /// </summary>
  public abstract class TabViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// Header to display.
    /// </summary>
    public string Header { get; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="header">Header to display.</param>
    protected TabViewModel(string header)
    {
      Header = header ?? throw new ArgumentNullException(nameof(header));
    }

    #endregion Construction
  }
}