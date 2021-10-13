using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IF.Lastfm.Core.Objects;
using System.IO;
using System.Windows;
using IF.Lastfm.Core.Api.Enums;
using Scrubbler.Scrobbling.Data;
using Scrubbler.Helper;
using System.Windows.Input;

namespace Scrubbler.Scrobbling.Scrobbler
{
  /// <summary>
  /// ViewModel for the <see cref="FileScrobbleView"/>.
  /// </summary>
  public class FileScrobbleViewModel : ScrobbleMultipleTimeViewModelBase<LoadedFileViewModel>
  {
    #region Properties

    /// <summary>
    /// Gets if the "Remove Selected Files" button is enabled in the UI.
    /// </summary>
    public bool CanRemoveFiles
    {
      get { return Scrobbles.Any(i => i.ToScrobble); }
    }

    /// <summary>
    /// Command for loading and parsing files.
    /// </summary>
    public ICommand AddFilesCommand { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// Supported file formats.
    /// </summary>
    private static readonly string[] SUPPORTEDFILES = new string[] { ".flac", ".mp3", ".m4a", ".ogg", ".wav", ".wma" };

    /// <summary>
    /// Factory used to create <see cref="ILocalFile"/>s.
    /// </summary>
    private readonly ILocalFileFactory _localFileFactory;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private readonly IFileOperator _fileOperator;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="localFileFactory">Factory used to create <see cref="ILocalFile"/>s.</param>
    /// <param name="fileOperator">FileOperator used to write to disk.</param>
    public FileScrobbleViewModel(IExtendedWindowManager windowManager, ILocalFileFactory localFileFactory, IFileOperator fileOperator)
      : base(windowManager, "File Scrobbler")
    {
      Scrobbles = new ObservableCollection<LoadedFileViewModel>();
      _localFileFactory = localFileFactory ?? throw new ArgumentNullException(nameof(localFileFactory));
      _fileOperator = fileOperator ?? throw new ArgumentNullException(nameof(fileOperator));
      AddFilesCommand = new DelegateCommand((o) => AddFiles().Forget());
    }

    /// <summary>
    /// Shows a dialog to select music files.
    /// Parses the selected files.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task AddFiles()
    {
      EnableControls = false;

      try
      {
        IOpenFileDialog ofd = WindowManager.CreateOpenFileDialog();
        ofd.Multiselect = true;
        ofd.Filter = "Music Files|*.flac;*.mp3;*.m4a;*.ogg;*.wav;*.wma";

        if (ofd.ShowDialog())
          await ParseFiles(ofd.FileNames);
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while trying to add files: {ex.Message}");
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
    private async Task ParseFiles(IEnumerable<string> files)
    {
      OnStatusUpdated("Trying to parse selected files...");
      var errors = new List<string>();

      var newFiles = new List<LoadedFileViewModel>();
      await Task.Run(() =>
      {
        foreach (string file in files)
        {
          try
          {
            if (SUPPORTEDFILES.Contains(Path.GetExtension(file).ToLower()))
            {
              ILocalFile audioFile = _localFileFactory.CreateFile(file);

              if (string.IsNullOrEmpty(audioFile.Artist))
                throw new Exception("No artist name found");
              if (string.IsNullOrEmpty(audioFile.Track))
                throw new Exception("No track name found");

              var vm = new LoadedFileViewModel(audioFile);
              vm.ToScrobbleChanged += Vm_ToScrobbleChanged;
              newFiles.Add(vm);
            }
          }
          catch (Exception ex)
          {
            errors.Add($"{file} {ex.Message}");
          }
        }
      });

      Scrobbles = new ObservableCollection<LoadedFileViewModel>(Scrobbles.Concat(newFiles));

      if (errors.Count > 0)
      {
        OnStatusUpdated($"Finished parsing selected files. {errors.Count} files could not be parsed");
        if (WindowManager.MessageBoxService.ShowDialog("Some files could not be parsed. Do you want to save a text file with the files that could not be parsed?",
                                                       "Error parsing files", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
        {
          IFileDialog sfd = WindowManager.CreateSaveFileDialog();
          sfd.Filter = "Text Files|*.txt";
          if (sfd.ShowDialog())
            _fileOperator.WriteAllLines(sfd.FileName, errors.ToArray());
        }
      }
      else if (Scrobbles.Count == 0)
        OnStatusUpdated("No compatible files");
      else
        OnStatusUpdated("Successfully parsed selected files");
    }

    /// <summary>
    /// Handles files that were dropped onto the GUI.
    /// </summary>
    /// <param name="e">Contains info about the dropped data.</param>
    public async void HandleDrop(DragEventArgs e)
    {
      try
      {
        EnableControls = false;

        if (e == null)
          throw new ArgumentNullException(nameof(e), "No drop data");

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
        OnStatusUpdated($"Fatal error while adding dropped files: {ex.Message}");
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
    private static string[] ReadDroppedFiles(string[] files)
    {
      // todo: this can be way better.
      while (files.Any(i => !Path.HasExtension(i)))
      {
        var tempList1 = files.Where(i => !Path.HasExtension(i)).ToList();
        var tempList2 = new List<string>();

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
      Scrobbles = new ObservableCollection<LoadedFileViewModel>(Scrobbles.Where(i => !i.ToScrobble).ToList());
      NotifyOfPropertyChange(() => CanRemoveFiles);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
    /// <returns>Task.</returns>
    public override async Task Scrobble()
    {
      try
      {
        EnableControls = false;
        OnStatusUpdated("Trying to scrobble selected tracks...");

        var response = await Scrobbler.ScrobbleAsync(CreateScrobbles());
        if (response.Success && response.Status == LastResponseStatus.Successful)
          OnStatusUpdated("Successfully scrobbled selected tracks");
        else
          OnStatusUpdated($"Error while scrobbling selected tracks: {response.Status}");
      }
      catch (Exception ex)
      {
        OnStatusUpdated($"Fatal error while trying to scrobble the selected tracks: {ex.Message}");
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
      DateTime timePlayed = ScrobbleTimeVM.Time;
      List<Scrobble> scrobbles = new List<Scrobble>();
      foreach (var vm in Scrobbles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.ArtistName, vm.AlbumName, vm.TrackName, timePlayed)
        {
          AlbumArtist = vm.AlbumArtist,
          Duration = vm.Duration
        });

        timePlayed = timePlayed.Subtract(vm.Duration ?? TimeSpan.FromSeconds(1));
      }

      return scrobbles;
    }

    /// <summary>
    /// Triggers when the "IsSelected" property of the
    /// <see cref="LoadedFileViewModel"/> changes.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void Vm_ToScrobbleChanged(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => CanRemoveFiles);
    }
  }
}