using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IF.Lastfm.Core.Objects;
using System.IO;
using Scrubbler.Interfaces;
using System.Windows;
using Scrubbler.ViewModels.SubViewModels;
using IF.Lastfm.Core.Api.Enums;

namespace Scrubbler.ViewModels.ScrobbleViewModels
{
  /// <summary>
  /// ViewModel for the <see cref="Views.ScrobbleViews.FileScrobbleView"/>.
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

    #endregion Properties

    #region Private Member

    /// <summary>
    /// Supported file formats.
    /// </summary>
    private static readonly string[] SUPPORTEDFILES = new string[] { ".flac", ".mp3", ".m4a", ".wma" };

    /// <summary>
    /// Factory used to create <see cref="ILocalFile"/>s.
    /// </summary>
    private ILocalFileFactory _localFileFactory;

    /// <summary>
    /// FileOperator used to write to disk.
    /// </summary>
    private IFileOperator _fileOperator;

    #endregion Private Member

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
      _localFileFactory = localFileFactory;
      _fileOperator = fileOperator;
    }

    /// <summary>
    /// Shows a dialog to select music files.
    /// Parses the selected files.
    /// </summary>
    public async Task AddFiles()
    {
      EnableControls = false;

      try
      {
        IOpenFileDialog ofd = _windowManager.CreateOpenFileDialog();
        ofd.Multiselect = true;
        ofd.Filter = "Music Files|*.flac;*.mp3;*.m4a;*.wma";

        if (ofd.ShowDialog())
          await ParseFiles(ofd.FileNames);
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while trying to add files: {0}", ex.Message));
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

              newFiles.Add(new LoadedFileViewModel(audioFile));
            }
          }
          catch (Exception ex)
          {
            errors.Add(string.Format("{0} {1}", file, ex.Message));
          }
        }
      });

      Scrobbles = new ObservableCollection<LoadedFileViewModel>(Scrobbles.Concat(newFiles));

      if (errors.Count > 0)
      {
        OnStatusUpdated(string.Format("Finished parsing selected files. {0} files could not be parsed", errors.Count));
        if (_windowManager.MessageBoxService.ShowDialog("Some files could not be parsed. Do you want to save a text file with the files that could not be parsed?",
                                                        "Error parsing files", IMessageBoxServiceButtons.YesNo) == IMessageBoxServiceResult.Yes)
        {
          IFileDialog sfd = _windowManager.CreateSaveFileDialog();
          sfd.Filter = "Text Files|*.txt";
          if (sfd.ShowDialog())
            _fileOperator.WriteAllLines(sfd.FileName, errors.ToArray());
        }
      }
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
        OnStatusUpdated(string.Format("Fatal error while adding dropped files: {0}", ex.Message));
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
      Scrobbles = new ObservableCollection<LoadedFileViewModel>(Scrobbles.Where(i => !i.ToScrobble).ToList());
      NotifyOfPropertyChange(() => CanRemoveFiles);
    }

    /// <summary>
    /// Scrobbles the selected tracks.
    /// </summary>
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
          OnStatusUpdated(string.Format("Error while scrobbling selected tracks: {0}", response.Status));
      }
      catch (Exception ex)
      {
        OnStatusUpdated(string.Format("Fatal error while trying to scrobble the selected tracks: {0}", ex.Message));
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
      foreach (var vm in Scrobbles.Where(i => i.ToScrobble).Reverse())
      {
        scrobbles.Add(new Scrobble(vm.Artist, vm.Album, vm.Track, timePlayed)
        {
          AlbumArtist = vm.AlbumArtist,
          Duration = vm.Duration
        });
        timePlayed = timePlayed.Subtract(vm.Duration);
      }

      return scrobbles;
    }
  }
}