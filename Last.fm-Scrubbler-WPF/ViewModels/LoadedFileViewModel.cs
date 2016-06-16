using Caliburn.Micro;
using System;
using TagLib;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class LoadedFileViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers when <see cref="ToScrobble"/> changed.
    /// </summary>
    public event EventHandler ToScrobbleChanged;

    /// <summary>
    /// Event that triggers when <see cref="IsSelected"/> changes.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    /// <summary>
    /// The loaded file.
    /// </summary>
    public File LoadedFile
    {
      get { return _loadedFile; }
      private set
      {
        _loadedFile = value;
        NotifyOfPropertyChange(() => LoadedFile);
      }
    }
    private File _loadedFile;

    /// <summary>
    /// Indicates if this file should be scrobbled.
    /// </summary>
    public bool ToScrobble
    {
      get { return _toScrobble; }
      set
      {
        _toScrobble = value;
        NotifyOfPropertyChange(() => ToScrobble);
        ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _toScrobble;

    /// <summary>
    /// Gets if this item is selected in the UI.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
        IsSelectedChanged?.Invoke(this, EventArgs.Empty);
      }
    }
    private bool _isSelected;

    #endregion Properties

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="file">The loaded file.</param>
    public LoadedFileViewModel(File file)
    {
      LoadedFile = file;
    }
  }
}