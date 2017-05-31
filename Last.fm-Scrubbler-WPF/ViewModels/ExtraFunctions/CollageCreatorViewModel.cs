using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Last.fm_Scrubbler_WPF.Views.ExtraFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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
  }

  /// <summary>
  /// Which type of top data to get.
  /// </summary>
  public enum CollageType
  {
    /// <summary>
    /// Get the top artists.
    /// </summary>
    Artists,

    /// <summary>
    /// Get the top albums.
    /// </summary>
    Albums
  }

  /// <summary>
  /// ViewModel for the <see cref="CollageCreatorView"/>
  /// </summary>
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
    /// Selected type of the collage to be created.
    /// </summary>
    public CollageType SelectedCollageType
    {
      get { return _selectedCollageType; }
      set
      {
        _selectedCollageType = value;
        NotifyOfPropertyChange(() => SelectedCollageType);
      }
    }
    private CollageType _selectedCollageType;

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

    /// <summary>
    /// Constructor.
    /// </summary>
    public CollageCreatorViewModel()
    {
      Username = "";
      TimeSpan = LastStatsTimeSpan.Overall;
      SelectedCollageSize = CollageSize.ThreeByThree;
    }

    /// <summary>
    /// Creates and uploads a collage of the top <see cref="SelectedCollageType"/>.
    /// </summary>
    public async void CreateCollage()
    {
      EnableControls = false;

      try
      {
        int numCollageItems = (int)SelectedCollageSize * (int)SelectedCollageSize;

        if (SelectedCollageType == CollageType.Artists)
        {
          OnStatusUpdated("Fetching top artists...");
          var response = await MainViewModel.Client.User.GetTopArtists(Username, TimeSpan, 1, numCollageItems);
          if (response.Success)
          {
            OnStatusUpdated("Getting artist images...");

            await StitchImagesTogether(response.Content.Select(a => new Tuple<Uri, string>(a.MainImage.ExtraLarge,
              string.Format("{0}{1}Plays: {2}", a.Name, Environment.NewLine, a.PlayCount))).ToList());

            OnStatusUpdated("Successfully created collage");
          }
          else
            OnStatusUpdated("Error while fetching top artists");
        }
        else if(SelectedCollageType == CollageType.Albums)
        {
          OnStatusUpdated("Fetching top albums...");
          var response = await MainViewModel.Client.User.GetTopAlbums(Username, TimeSpan, 1, numCollageItems);
          if(response.Success)
          {
            OnStatusUpdated("Getting album images...");

            await StitchImagesTogether(response.Content.Select(a => new Tuple<Uri, string>(a.Images.ExtraLarge,
              string.Format("{0}{1}{2}{3}Plays: {4}", a.ArtistName, Environment.NewLine, a.Name, Environment.NewLine, a.PlayCount))).ToList());

            OnStatusUpdated("Successfully created collage");
          }
          else
            OnStatusUpdated("Error while fetching top albums");
        }
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

    /// <summary>
    /// Combines the images to one big image.
    /// </summary>
    /// <param name="infos">Tuple containing the image
    /// and text to stitch together.</param>
    /// <returns>Task.</returns>
    private async Task StitchImagesTogether(List<Tuple<Uri, string>> infos)
    {
      BitmapFrame[] frames = new BitmapFrame[infos.Count];
      for (int i = 0; i < frames.Length; i++)
      {
        frames[i] = BitmapDecoder.Create(infos[i].Item1 != null ? infos[i].Item1 :
          (SelectedCollageType == CollageType.Albums ? new Uri("pack://application:,,,/Resources/noalbumimage.png") : new Uri("pack://application:,,,/Resources/noartistimage.png"))
          , BitmapCreateOptions.None, BitmapCacheOption.OnDemand).Frames.First();
      }

      OnStatusUpdated("Downloading images...");
      while (frames.Any(f => f.IsDownloading))
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
        for (int y = 0; y < col; y++)
        {
          for (int x = 0; x < col; x++)
          {
            dc.DrawImage(frames[cnt], new Rect(x * imageWidth, y * imageHeight, imageWidth, imageHeight));

            // create artist text
            FormattedText extraText = new FormattedText(infos[cnt].Item2, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.Black)
            {
              MaxTextWidth = imageWidth,
              MaxTextHeight = imageHeight
            };

            dc.DrawText(extraText, new Point(x * imageWidth + 1, y * imageHeight + 1));
            extraText.SetForegroundBrush(Brushes.White);
            dc.DrawText(extraText, new Point(x * imageWidth, y * imageHeight));
            cnt++;
          }
        }
      }

      // Converts the Visual (DrawingVisual) into a BitmapSource
      RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * col, imageHeight * col, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(dv);

      // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
      PngBitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(bmp));

      await UploadImage(encoder);
    }

    /// <summary>
    /// Uploads the image to imgur.
    /// </summary>
    /// <param name="encoder">Encoder containing the stitched image.</param>
    /// <returns>Task.</returns>
    private async Task UploadImage(PngBitmapEncoder encoder)
    {
      OnStatusUpdated("Uploading image...");
      using (var w = new WebClient())
      {

        w.Proxy = null;
        w.Headers.Add("Authorization", "Client-ID " + "80dfa34b8899ce5");

        using (MemoryStream ms = new MemoryStream())
        {
          encoder.Save(ms);
          var values = new NameValueCollection
          {
            { "image", Convert.ToBase64String(ms.ToArray()) },
          };

          byte[] response = null;
          await Task.Run(() => response = w.UploadValues("https://api.imgur.com/3/upload.xml", values));

          var doc = XDocument.Load(new MemoryStream(response));

          string link = doc.Descendants().Where(i => i.Name == "link").FirstOrDefault().Value;

          Process.Start(link);
        }
      }
    }
  }
}