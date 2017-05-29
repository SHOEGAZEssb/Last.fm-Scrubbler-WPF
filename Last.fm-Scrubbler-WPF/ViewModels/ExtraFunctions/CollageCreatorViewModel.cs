using IF.Lastfm.Core.Api.Enums;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Last.fm_Scrubbler_WPF.ViewModels.ExtraFunctions
{
  /// <summary>
  /// Available collage sizes.
  /// </summary>
  public enum CollageSize
  {
    /// <summary>
    /// Create a 3x3 collage.
    /// </summary>
    [Description("3x3")]
    ThreeByThree = 3,

    /// <summary>
    /// Create a 4x4 collage.
    /// </summary>
    [Description("4x4")]
    FourByFour = 4,

    /// <summary>
    /// Create a 5x5 collage.
    /// </summary>
    [Description("5x5")]
    FiveByFive = 5,

    /// <summary>
    /// Create a 10x10 collage.
    /// </summary>
    [Description("10x10")]
    TenByTen = 10
  }

  class CollageCreatorViewModel : ViewModelBase
  {
    #region Properties

    /// <summary>
    /// Name of the user whose top artists to fetch.
    /// </summary>
    public string Username
    {
      get { return _username; }
      set
      {
        _username = value;
        NotifyOfPropertyChange(() => Username);
      }
    }
    private string _username;

    /// <summary>
    /// The timespan from when to get the top artists.
    /// </summary>
    public LastStatsTimeSpan TimeSpan
    {
      get { return _timeSpan; }
      set
      {
        _timeSpan = value;
        NotifyOfPropertyChange(() => TimeSpan);
      }
    }
    private LastStatsTimeSpan _timeSpan;

    /// <summary>
    /// Selected size of the collage to be created.
    /// </summary>
    public CollageSize SelectedCollageSize
    {
      get { return _selectedCollageSize; }
      set
      {
        _selectedCollageSize = value;
        NotifyOfPropertyChange(() => SelectedCollageSize);
      }
    }
    private CollageSize _selectedCollageSize;

    /// <summary>
    /// Gets if certain controls on the ui are enabled.
    /// </summary>
    public override bool EnableControls
    {
      get { return _enableControls; }
      protected set
      {
        _enableControls = value;
        NotifyOfPropertyChange(() => EnableControls);
      }
    }

    #endregion Properties

    public CollageCreatorViewModel()
    {
      Username = "";
      TimeSpan = LastStatsTimeSpan.Overall;
      SelectedCollageSize = CollageSize.ThreeByThree;
    }

    public async void CreateCollage()
    {
      EnableControls = false;

      try
      {
        OnStatusUpdated("Fetching top artists...");

        int numCollageItems = (int)SelectedCollageSize * (int)SelectedCollageSize;
        var response = await MainViewModel.Client.User.GetTopArtists(Username, TimeSpan, 1, numCollageItems);
        if (response.Success)
        {
          OnStatusUpdated("Getting artist images...");

          // extract images
          Uri[] imageUris = new Uri[numCollageItems];
          for (int i = 0; i < numCollageItems; i++)
          {
            imageUris[i] = response.Content[i].MainImage.ExtraLarge;
          }


          OnStatusUpdated("Stitching images together...");
          await StitchImagesTogether(imageUris);

        }
        else
          OnStatusUpdated("Error while fetching top artists");
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while creating collage: " + ex.Message);
      }
      finally
      {
        EnableControls = true;
      }
    }

    private async Task StitchImagesTogether(Uri[] uris)
    {
      BitmapFrame[] frames = new BitmapFrame[uris.Length];
      for(int i = 0; i < frames.Length; i++)
      {
        frames[i] = BitmapDecoder.Create(uris[i], BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
      }

      while(frames.Any(f => f.IsDownloading))
      {
        await Task.Delay(100);
      }

      int imageWidth = frames[0].PixelWidth;
      int imageHeight = frames[0].PixelHeight;

      int col = (int)SelectedCollageSize;
      DrawingVisual dv = new DrawingVisual();
      using (DrawingContext dc = dv.RenderOpen())
      {
        int cnt = 0;
        for(int y = 0; y < col; y++)
        {
          for(int x = 0; x < col; x++)
          {
            dc.DrawImage(frames[cnt++], new Rect(x * imageWidth, y * imageHeight, imageWidth, imageHeight));
          }
        }
      }

      // Converts the Visual (DrawingVisual) into a BitmapSource
      RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * col, imageHeight * col, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(dv);

      // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
      PngBitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(bmp));

      // Saves the image into a file using the encoder
      using (Stream stream = File.Create("tmpImg.bmp"))
        encoder.Save(stream);
    }
  }
}