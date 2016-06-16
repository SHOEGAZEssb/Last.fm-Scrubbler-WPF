using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using TagLib;
using IF.Lastfm.Core.Objects;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  class FileScrobbleViewModel : PropertyChangedBase
  {
    #region Properties

    /// <summary>
    /// Event that triggers an update of the status text.
    /// </summary>
    public event EventHandler<UpdateStatusEventArgs> StatusUpdated;

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
    public bool EnableControls
    {
      get { return _enableControls; }
      private set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyOfPropertyChange(() => CanScrobble);
        NotifyOfPropertyChange(() => CanSelectAll);
        NotifyOfPropertyChange(() => CanSelectNone);
      }
    }
    private bool _enableControls;

    /// <summary>
    /// Gets if the scrobble button on the ui is enabled.
    /// </summary>
    public bool CanScrobble
    {
      get { return MainViewModel.Client.Auth.Authenticated && LoadedFiles.Any(i => i.ToScrobble) && EnableControls; }
    }

    public bool CanSelectAll
    {
      get { return !LoadedFiles.All(i => i.ToScrobble); }
    }

    public bool CanSelectNone
    {
      get { return LoadedFiles.Any(i => i.ToScrobble); }
    }

    #endregion

    private Dispatcher _dispatcher;

    /// <summary>
    /// Constructor.
    /// </summary>
    public FileScrobbleViewModel()
    {
      LoadedFiles = new ObservableCollection<LoadedFileViewModel>();
      MainViewModel.ClientAuthChanged += MainViewModel_ClientAuthChanged;
      _dispatcher = Dispatcher.CurrentDispatcher;
    }

    /// <summary>
    /// Triggers when the client auth of the MainViewModel changes.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void MainViewModel_ClientAuthChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
    }

    public async void AddFiles()
    {
      EnableControls = false;

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Multiselect = true;
      ofd.Filter = "MP3 Files|*.mp3";

      if (ofd.ShowDialog() == DialogResult.OK)
      {
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to parse selected files..."));
        List<string> errors = new List<string>();

        await Task.Run(() =>
        {
          foreach (string file in ofd.FileNames)
          {
            try
            {
              File audioFile = File.Create(file);
              LoadedFileViewModel vm = new LoadedFileViewModel(audioFile);
              vm.ToScrobbleChanged += ToScrobbleChanged;
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
          StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Partially parsed selected files. " + errors.Count + " files could not be parsed"));
          if (MessageBox.Show("Some rows could not be parsed. Do you want to save a text file with the rows that could not be parsed?", "Error parsing rows", MessageBoxButtons.YesNo) == DialogResult.Yes)
          {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
              System.IO.File.WriteAllLines(sfd.FileName, errors.ToArray());
          }
        }
        else
          StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully parsed selected files. Parsed " + LoadedFiles.Count + " files"));
      }

      EnableControls = true;
    }

    public async void Scrobble()
    {
      EnableControls = false;
      StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Trying to scrobble selected tracks"));

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
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Successfully scrobbled!"));
      else
        StatusUpdated?.Invoke(this, new UpdateStatusEventArgs("Error while scrobbling!"));
    }

    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
    }

    public void SelectAll()
    {
      foreach (var vm in LoadedFiles)
      {
        vm.ToScrobble = true;
      }
    }

    public void SelectNone()
    {
      foreach (var vm in LoadedFiles)
      {
        vm.ToScrobble = false;
      }
    }
  }
}