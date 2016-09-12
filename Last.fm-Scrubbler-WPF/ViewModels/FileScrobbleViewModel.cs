using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using TagLib;
using IF.Lastfm.Core.Objects;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class FileScrobbleViewModel : ScrobbleViewModelBase
  {
    #region Properties

    /// <summary>
    /// List of loaded files.
    /// </summary>
    public ObservableCollection<LoadedFileViewModel> LoadedFiles
    {
      get { return _loadedFiles; }
      private set
      {
        _loadedFiles = value;
        NotifyOfPropertyChange(() => LoadedFiles);
      }
    }
    private ObservableCollection<LoadedFileViewModel> _loadedFiles;

    /// <summary>
    /// Time to reverse when scrobbling.
    /// </summary>
    public DateTime FinishingTime
    {
      get { return _finishingTime; }
      set
      {
        _finishingTime = value;
        NotifyOfPropertyChange(() => FinishingTime);
      }
    }
    private DateTime _finishingTime;

    /// <summary>
    /// Gets if the current date time should be used for the <see cref="FinishingTime"/>
    /// </summary>
    public bool CurrentDateTime
    {
      get { return _currentDateTime; }
      set
      {
        _currentDateTime = value;
        if (value)
          FinishingTime = DateTime.Now;

        NotifyOfPropertyChange(() => CurrentDateTime);
      }
    }
    private bool _currentDateTime;

    /// <summary>
    /// Gets if certain controls should be enabled on the UI.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
        NotifyOfPropertyChange(() => CanRemoveFiles);
      }
    }

    /// <summary>
    /// Gets if the scrobble button on the ui is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && LoadedFiles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the "Select All" button is enabled in the UI.
    /// </summary>
    public bool CanSelectAll
    {
      get { return !LoadedFiles.All(i => i.ToScrobble); }
    }

    /// <summary>
    /// Gets if the "Select None" button is enabled in the UI.
    /// </summary>
    public bool CanSelectNone
    {
      get { return LoadedFiles.Any(i => i.ToScrobble); }
    }

    /// <summary>
    /// Gets if the "Remove Selected Files" button is enabled in the UI.
    /// </summary>
    public bool CanRemoveFiles
    {
      get { return LoadedFiles.Any(i => i.IsSelected) && EnableControls; }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Current dispatcher.
    /// </summary>
    private Dispatcher _dispatcher;

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public FileScrobbleViewModel()
    {
      LoadedFiles = new ObservableCollection<LoadedFileViewModel>();
      _dispatcher = Dispatcher.CurrentDispatcher;
      CurrentDateTime = true;
    }

    /// <summary>
    /// Shows a dialog to select music files.
    /// Parses the selected files.
    /// </summary>
    public async void AddFiles()
    {
      EnableControls = false;

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Multiselect = true;
      ofd.Filter = "MP3 Files|*.mp3";

      if (ofd.ShowDialog() == DialogResult.OK)
      {
        OnStatusUpdated("Trying to parse selected files...");
        List<string> errors = new List<string>();

        await Task.Run(() =>
        {
          foreach (string file in ofd.FileNames)
          {
            try
            {
              File audioFile = File.Create(file);

              if (audioFile.Tag.FirstPerformer == null || audioFile.Tag.Title == null)
                throw new Exception("No artist name or track title found!");

              LoadedFileViewModel vm = new LoadedFileViewModel(audioFile);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              vm.IsSelectedChanged += IsSelectedChanged;
              _dispatcher.Invoke(() => LoadedFiles.Add(vm));
            }
            catch (Exception ex)
            {
              errors.Add(file + " " + ex.Message);
            }
          }
        });

        if (errors.Count > 0)
        {
          OnStatusUpdated("Finished parsing selected files. " + errors.Count + " files could not be parsed");
          if (MessageBox.Show("Some files could not be parsed. Do you want to save a text file with the files that could not be parsed?", "Error parsing files", MessageBoxButtons.YesNo) == DialogResult.Yes)
          {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
              System.IO.File.WriteAllLines(sfd.FileName, errors.ToArray());
          }
        }
        else
          OnStatusUpdated("Successfully parsed selected files. Parsed " + LoadedFiles.Count + " files");
      }

      EnableControls = true;
    }

    /// <summary>
    /// Notifies the UI that the IsSelected property of a
    /// <see cref="LoadedFileViewModel"/> has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void IsSelectedChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanRemoveFiles);
    }

    /// <summary>
    /// Removes the selected files.
    /// </summary>
    public void RemoveFiles()
    {
      List<LoadedFileViewModel> toRemove = LoadedFiles.Where(i => i.IsSelected).ToList();
      for(int i = 0; i < toRemove.Count; i++)
      {
        LoadedFiles.RemoveAt(LoadedFiles.IndexOf(toRemove[i]));
      }

      NotifyOfPropertyChange(() => CanRemoveFiles);
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    public override async Task Scrobble()
    {
      EnableControls = false;
      OnStatusUpdated("Trying to scrobble selected tracks...");

      CurrentDateTime = CurrentDateTime;

      DateTime timePlayed = FinishingTime;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach(var vm in LoadedFiles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.LoadedFile.Tag.FirstPerformer, vm.LoadedFile.Tag.Album, vm.LoadedFile.Tag.Title, timePlayed));
        timePlayed = timePlayed.Subtract(vm.LoadedFile.Properties.Duration);
      }

      var response = await MainViewModel.Scrobbler.ScrobbleAsync(scrobbles);
      if (response.Success)
        OnStatusUpdated("Successfully scrobbled!");
      else
        OnStatusUpdated("Error while scrobbling!");
    }

    /// <summary>
    /// Notifies the UI that the ToScrobble property of a
    /// <see cref="LoadedFileViewModel"/> has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    /// <summary>
    /// Marks all tracks as "ToScrobble".
    /// </summary>
    public void SelectAll()
    {
      foreach (var vm in LoadedFiles)
      {
        vm.ToScrobble = true;
      }
    }

    /// <summary>
    /// Marks all tracks as not "ToScrobble".
    /// </summary>
    public void SelectNone()
    {
      foreach (var vm in LoadedFiles)
      {
        vm.ToScrobble = false;
      }
    }
  }
}