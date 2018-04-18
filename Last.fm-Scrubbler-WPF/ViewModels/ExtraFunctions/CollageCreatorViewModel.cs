using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Scrubbler.Interfaces;
using Scrubbler.Views.ExtraFunctions;
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

namespace Scrubbler.ViewModels.ExtraFunctions
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
    /// Create a custom-sized collage.
    /// </summary>
    [Description("Custom")]
    Custom
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
  public class CollageCreatorViewModel : ExtraFunctionViewModelBase
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

        if (SelectedCollageSize == CollageSize.Custom)
          UploadToWeb = false;
      }
    }
    private CollageSize _selectedCollageSize;

    /// <summary>
    /// The size that the custom collage should be.
    /// </summary>
    public int CustomCollageSize
    {
      get { return _customCollageSize; }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof(CustomCollageSize));

        _customCollageSize = value;
        NotifyOfPropertyChange();
      }
    }
    private int _customCollageSize;

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
    /// If enabled, prints the name of the artist or album
    /// onto the image.
    /// </summary>
    public bool ShowNames
    {
      get { return _showNames; }
      set
      {
        _showNames = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _showNames;

    /// <summary>
    /// If enabled, prints the playcount of the
    /// artist or album onto the image.
    /// </summary>
    public bool ShowPlaycounts
    {
      get { return _showPlaycounts; }
      set
      {
        _showPlaycounts = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _showPlaycounts;

    /// <summary>
    /// If enabled, the image gets uploaded to an
    /// image upload service after it is created.
    /// </summary>
    public bool UploadToWeb
    {
      get { return _uploadToWeb; }
      set
      {
        if (value && SelectedCollageSize == CollageSize.Custom)
          throw new InvalidOperationException("A custom-size collage can't be uploaded to the web.");

        _uploadToWeb = value;
        NotifyOfPropertyChange();
      }
    }
    private bool _uploadToWeb;

    /// <summary>
    /// The created collage.
    /// </summary>
    public BitmapSource Collage
    {
      get { return _collage; }
      private set
      {
        _collage = value;
        NotifyOfPropertyChange();
      }
    }
    private BitmapSource _collage;

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

    #region Member

    /// <summary>
    /// WindowManager used to display dialogs.
    /// </summary>
    private IExtendedWindowManager _windowManager;

    /// <summary>
    /// Last.fm user api used to fetch top artists and albums.
    /// </summary>
    private IUserApi _userAPI;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="windowManager">WindowManager used to display dialogs.</param>
    /// <param name="userAPI">Last.fm user api used to fetch top artists and albums.</param>
    public CollageCreatorViewModel(IExtendedWindowManager windowManager, IUserApi userAPI)
      : base("Collage Creator")
    {
      _windowManager = windowManager;
      _userAPI = userAPI;
      TimeSpan = LastStatsTimeSpan.Overall;
      SelectedCollageSize = CollageSize.ThreeByThree;
      CustomCollageSize = 10;
      ShowNames = true;
      ShowPlaycounts = true;
      UploadToWeb = true;
    }

    /// <summary>
    /// Creates and uploads a collage of the top <see cref="SelectedCollageType"/>.
    /// </summary>
    public async void CreateCollage()
    {
      EnableControls = false;

      try
      {
        Collage = null;

        int numCollageItems = 0;
        if (SelectedCollageSize == CollageSize.Custom)
          numCollageItems = CustomCollageSize * CustomCollageSize;
        else
          numCollageItems = (int)SelectedCollageSize * (int)SelectedCollageSize;
        PngBitmapEncoder collage = null;
        if (SelectedCollageType == CollageType.Artists)
        {
          OnStatusUpdated("Fetching top artists...");
          var response = await _userAPI.GetTopArtists(Username, TimeSpan, 1, numCollageItems);
          if (response.Success)
            collage = await StitchImagesTogether(response.Content.Select(a => new Tuple<Uri, string>(a.MainImage.ExtraLarge, CreateArtistText(a))).ToList());
          else
            OnStatusUpdated("Error while fetching top artists");
        }
        else if (SelectedCollageType == CollageType.Albums)
        {
          OnStatusUpdated("Fetching top albums...");
          var response = await _userAPI.GetTopAlbums(Username, TimeSpan, 1, numCollageItems);
          if (response.Success)
            collage = await StitchImagesTogether(response.Content.Select(a => new Tuple<Uri, string>(a.Images.ExtraLarge, CreateAlbumText(a))).ToList());
          else
            OnStatusUpdated("Error while fetching top albums");
        }

        using (MemoryStream ms = new MemoryStream())
        {
          collage.Save(ms);
          ConvertToBitmapImage(ms);

          if (UploadToWeb)
            await UploadImage(ms);
        };

        OnStatusUpdated("Successfully created collage");
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
    /// Shows a dialog to save the <see cref="Collage"/> to file.
    /// </summary>
    public void SaveImage()
    {
      try
      {
        IFileDialog sfd = _windowManager.CreateSaveFileDialog();
        sfd.Filter = "Bitmap Image (.bmp) | *.bmp";
        if (sfd.ShowDialog())
        {
          using (var fileStream = new FileStream(sfd.FileName, FileMode.Create))
          {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Collage));
            encoder.Save(fileStream);
          }
        }
      }
      catch (Exception ex)
      {
        OnStatusUpdated("Fatal error while saving collage to file: " + ex.Message);
      }
    }

    /// <summary>
    /// Converts the collage to a display-able image.
    /// </summary>
    /// <param name="ms">MemoryStream containing the image.</param>
    private void ConvertToBitmapImage(MemoryStream ms)
    {
      var bitmapImage = new BitmapImage();

      ms.Seek(0, SeekOrigin.Begin);
      bitmapImage.BeginInit();
      bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
      bitmapImage.StreamSource = ms;
      bitmapImage.EndInit();

      Collage = bitmapImage;
    }

    /// <summary>
    /// Creates the text for the <see cref="CollageType.Artists"/>.
    /// </summary>
    /// <param name="a">Downloaded artist info.</param>
    /// <returns>String with info text.</returns>
    private string CreateArtistText(LastArtist a)
    {
      if (ShowNames && ShowPlaycounts)
        return string.Format("{0}{1}Plays: {2}", a.Name, Environment.NewLine, a.PlayCount);
      else if (ShowNames)
        return string.Format("{0}", a.Name);
      else if (ShowPlaycounts)
        return string.Format("Plays: {0}", a.PlayCount);
      else
        return string.Empty;
    }

    /// <summary>
    /// Creates the text for the <see cref="CollageType.Albums"/>.
    /// </summary>
    /// <param name="a">Downloaded album info.</param>
    /// <returns>String with info text.</returns>
    private string CreateAlbumText(LastAlbum a)
    {
      if (ShowNames && ShowPlaycounts)
        return string.Format("{0}{1}{2}{3}Plays: {4}", a.ArtistName, Environment.NewLine, a.Name, Environment.NewLine, a.PlayCount);
      else if (ShowNames)
        return string.Format("{0}{1}{2}", a.ArtistName, Environment.NewLine, a.Name);
      else if (ShowPlaycounts)
        return string.Format("Plays: {0}", a.PlayCount);
      else
        return string.Empty;
    }

    /// <summary>
    /// Combines the images to one big image.
    /// </summary>
    /// <param name="infos">Tuple containing the image
    /// and text to stitch together.</param>
    /// <returns>Task.</returns>
    private async Task<PngBitmapEncoder> StitchImagesTogether(List<Tuple<Uri, string>> infos)
    {
      BitmapFrame[] frames = new BitmapFrame[infos.Count];
      for (int i = 0; i < frames.Length; i++)
      {
        frames[i] = BitmapDecoder.Create(infos[i].Item1 ?? (SelectedCollageType == CollageType.Albums ? new Uri("pack://application:,,,/Resources/noalbumimage.png") : new Uri("pack://application:,,,/Resources/noartistimage.png")), BitmapCreateOptions.None, BitmapCacheOption.OnDemand).Frames.First();
      }

      OnStatusUpdated("Downloading images...");
      while (frames.Any(f => f.IsDownloading))
      {
        await Task.Delay(100);
      }

      int imageWidth = frames[0].PixelWidth;
      int imageHeight = frames[0].PixelHeight;

      int collageSize = 0;
      if (SelectedCollageSize == CollageSize.Custom)
        collageSize = CustomCollageSize;
      else
        collageSize = (int)SelectedCollageSize;

      DrawingVisual dv = new DrawingVisual();
      using (DrawingContext dc = dv.RenderOpen())
      {
        int cnt = 0;
        for (int y = 0; y < collageSize; y++)
        {
          for (int x = 0; x < collageSize; x++)
          {
            dc.DrawImage(frames[cnt], new Rect(x * imageWidth, y * imageHeight, imageWidth, imageHeight));

            if (!string.IsNullOrEmpty(infos[cnt].Item2))
            {
              // create text
              FormattedText extraText = new FormattedText(infos[cnt].Item2, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 14, Brushes.Black)
              {
                MaxTextWidth = imageWidth,
                MaxTextHeight = imageHeight
              };

              dc.DrawText(extraText, new Point(x * imageWidth + 1, y * imageHeight + 1));
              extraText.SetForegroundBrush(Brushes.White);
              dc.DrawText(extraText, new Point(x * imageWidth, y * imageHeight));
            }

            cnt++;
          }
        }
      }

      // Converts the Visual (DrawingVisual) into a BitmapSource
      RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * collageSize, imageHeight * collageSize, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(dv);

      // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
      PngBitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(bmp));

      return encoder;
    }

    /// <summary>
    /// Uploads the image to imgur.
    /// </summary>
    /// <param name="ms">MemoryStream containing the image.</param>
    /// <returns>Task.</returns>
    private async Task UploadImage(MemoryStream ms)
    {
      OnStatusUpdated("Uploading image...");
      using (var w = new WebClient())
      {
        w.Proxy = null;
        w.Headers.Add("Authorization", "Client-ID " + "80dfa34b8899ce5");

        var values = new NameValueCollection
          {
            { "image", Convert.ToBase64String(ms.ToArray()) },
          };

        byte[] response = null;
        await Task.Run(() => response = w.UploadValues("https://api.imgur.com/3/upload.xml", values));

        using (MemoryStream xMs = new MemoryStream(response))
        {
          var doc = XDocument.Load(xMs);
          string link = doc.Descendants().Where(i => i.Name == "link").FirstOrDefault().Value;
          Process.Start(link);
        }
      }
    }
  }
}