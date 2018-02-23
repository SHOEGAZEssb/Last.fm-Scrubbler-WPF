using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views;
using Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels;
using System.IO;
using Caliburn.Micro;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="FileScrobbleView"/>.
  /// </summary>
  public class FileScrobbleViewModel : ScrobbleTimeViewModelBase
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
    /// Gets if certain controls should be enabled on the UI.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
        NotifyCanProperties();
      }
    }

    /// <summary>
    /// Gets if the scrobble button on the ui is enabled.
    /// </summary>
    public override bool CanScrobble
    {
      get { return base.CanScrobble && LoadedFiles.Any(i => i.ToScrobble) && EnableControls; }
    }

    /// <summary>
    /// Gets if the preview button is enabled.
    /// </summary>
    public override bool CanPreview
    {
      get { return LoadedFiles.Any(i => i.ToScrobble); }
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
      get { return LoadedFiles.Any(i => i.ToScrobble) && EnableControls; }
    }

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Current dispatcher.
    /// </summary>
    private Dispatcher _dispatcher;

    /// <summary>
    /// Supported file formats.
    /// </summary>
    private static readonly string[] SUPPORTEDFILES = new string[] { ".flac", ".mp3", ".m4a", ".wma" };

    #endregion Private Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    public FileScrobbleViewModel(IWindowManager windowManager)
      : base(windowManager, "File Scrobbler")
    {
      LoadedFiles = new ObservableCollection<LoadedFileViewModel>();
      _dispatcher = Dispatcher.CurrentDispatcher;
      UseCurrentTime = true;
    }

    /// <summary>
    /// Shows a dialog to select music files.
    /// Parses the selected files.
    /// </summary>
    public async void AddFiles()
    {
      EnableControls = false;

      try
      {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
          ofd.Multiselect = true;
          ofd.Filter = "Music Files|*.flac;*.mp3;*.m4a;*.wma";

          if (ofd.ShowDialog() == DialogResult.OK)
            await ParseFiles(ofd.FileNames);
        }
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while trying to add files: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Parses the metadata of the given <paramref name="files"/>.
    /// </summary>
    /// <param name="files">Files whose metadata to parse.</param>
    /// <returns>Task.</returns>
    private async Task ParseFiles(string[] files)
    {
      OnStatusUpdated("Trying to parse selected files...");
      List<string> errors = new List<string>();

      await Task.Run(() =>
      {
        foreach (string file in files)
        {
          try
          {
            if (SUPPORTEDFILES.Contains(Path.GetExtension(file).ToLower()))
            {
              TagLib.File audioFile = TagLib.File.Create(file);

              if (audioFile.Tag.FirstPerformer == null || audioFile.Tag.Title == null)
                throw new Exception("No artist name or track title found!");

              LoadedFileViewModel vm = new LoadedFileViewModel(audioFile);
              vm.ToScrobbleChanged += ToScrobbleChanged;
              _dispatcher.Invoke(() => LoadedFiles.Add(vm));
            }
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
          SaveFileDialog sfd = new SaveFileDialog()
          {
            Filter = "Text Files|*.txt"
          };

          if (sfd.ShowDialog() == DialogResult.OK)
            File.WriteAllLines(sfd.FileName, errors.ToArray());
        }
      }
      else
        OnStatusUpdated("Successfully parsed selected files. Parsed " + LoadedFiles.Count + " files");
    }

    /// <summary>
    /// Handles files that were dropped onto the GUI.
    /// </summary>
    /// <param name="e">Contains info about the dropped data.</param>
    public async void HandleDrop(System.Windows.DragEventArgs e)
    {
      EnableControls = false;

      try
      {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
          string[] files = ReadDroppedFiles((string[])e.Data.GetData(DataFormats.FileDrop));

          if (files.Length > 0)
            await ParseFiles(files);
          else
            OnStatusUpdated("No suitable files found");
        }
        else
          OnStatusUpdated("Incorrect drop operation");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while adding dropped files: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Reads out all dropped files.
    /// Resolves folders and subfolders.
    /// </summary>
    /// <param name="files">Files to "extract".</param>
    /// <returns>Array containing all files.</returns>
    private string[] ReadDroppedFiles(string[] files)
    {
      // todo: this can be way better.
      while (files.Any(i => !Path.HasExtension(i)))
      {
        var tempList1 = files.Where(i => !Path.HasExtension(i)).ToList();
        List<string> tempList2 = new List<string>();

        foreach (var ex in tempList1)
        {
          try
          {
            var newFiles = Directory.GetFiles(ex, "*.*", SearchOption.AllDirectories);
            foreach (string file in newFiles)
            {
              tempList2.Add(file);
            }
          }
          catch (IOException)
          {
            // swallow, probably a file without extension.
          }
        }

        files = files.Where(i => Path.HasExtension(i)).Concat(tempList2).ToArray();
      }

      return files;
    }

    /// <summary>
    /// Removes the selected files.
    /// </summary>
    public void RemoveFiles()
    {
      LoadedFiles = new ObservableCollection<LoadedFileViewModel>(LoadedFiles.Where(i => !i.ToScrobble).ToList());
      NotifyCanProperties();
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    public override async Task Scrobble()
    {
      EnableControls = false;

      try
      {
        OnStatusUpdated("Trying to scrobble selected tracks...");

        var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success)
          OnStatusUpdated("Successfully scrobbled!");
        else
          OnStatusUpdated("Error while scrobbling!");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("An error occurred while trying to scrobble the selected tracks. Error: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    /// <summary>
    /// Creates the list of the tracks that will be scrobbled.
    /// </summary>
    /// <returns>List with scrobbles.</returns>
    protected override IEnumerable<Scrobble> CreateScrobbles()
    {
      DateTime timePlayed = Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (var vm in LoadedFiles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.LoadedFile.Tag.FirstPerformer, vm.LoadedFile.Tag.Album, vm.LoadedFile.Tag.Title, timePlayed)
                          { AlbumArtist = vm.LoadedFile.Tag.FirstAlbumArtist, Duration = vm.LoadedFile.Properties.Duration });
        timePlayed = timePlayed.Subtract(vm.LoadedFile.Properties.Duration);
      }

      return scrobbles;
    }

    /// <summary>
    /// Notifies the UI that the ToScrobble property of a
    /// <see cref="LoadedFileViewModel"/> has changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyCanProperties();
    }

    /// <summary>
    /// Notifies the UI that something of the collection changed.
    /// </summary>
    private void NotifyCanProperties()
    {
      NotifyOfPropertyChange(() => CanScrobble);
      NotifyOfPropertyChange(() => CanPreview);
      NotifyOfPropertyChange(() => CanSelectAll);
      NotifyOfPropertyChange(() => CanSelectNone);
      NotifyOfPropertyChange(() => CanRemoveFiles);
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