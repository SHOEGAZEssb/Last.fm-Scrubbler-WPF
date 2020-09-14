using System;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// ViewModel for a last.fm tag.
  /// </summary>
  public class TagViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// Event that is fired when this tag should be opened in the browser.
    /// </summary>
    public event EventHandler RequestOpen;

    /// <summary>
    /// Name of the tag.
    /// </summary>
    public string Name { get; }

    #endregion Properties

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of the tag.</param>
    public TagViewModel(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Empty tag name", nameof(name));

      Name = name;
    }

    #endregion Construction

    /// <summary>
    /// Fires the <see cref="RequestOpen"/> event.
    /// </summary>
    public void Clicked()
    {
      RequestOpen?.Invoke(this, EventArgs.Empty);
    }
  }
}