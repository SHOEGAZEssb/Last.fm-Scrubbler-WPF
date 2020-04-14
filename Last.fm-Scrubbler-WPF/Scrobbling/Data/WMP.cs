using System;
using System.Collections.Generic;
using WMPLib;

namespace Scrubbler.Scrobbling.Data
{
  /// <summary>
  /// Wraps around the windows media player to get its info.
  /// </summary>
  class WMP : IDisposable
  {
    #region Member

    /// <summary>
    /// Reference to the windows media player.
    /// </summary>
    private readonly WindowsMediaPlayer _mediaPlayer;

    /// <summary>
    /// Reference to the media collection.
    /// </summary>
    private readonly IWMPMediaCollection _media;

    /// <summary>
    /// Access index of the track name.
    /// </summary>
    private readonly int _titleIndex;

    /// <summary>
    /// Access index of the (album) artist name.
    /// </summary>
    private readonly int _albumArtistIndex;

    /// <summary>
    /// Access index of the artist name.
    /// </summary>
    private readonly int _authorIndex;

    /// <summary>
    /// Access index of the album name.
    /// </summary>
    private readonly int _albumIndex;

    /// <summary>
    /// Access index of the play count.
    /// </summary>
    private readonly int _playCountIndex;

    /// <summary>
    /// Access index of the last played date.
    /// </summary>
    private readonly int _userLastPlayedTimeIndex;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    public WMP()
    {
      _mediaPlayer = new WindowsMediaPlayer();
      _media = _mediaPlayer.mediaCollection;

      _titleIndex = _media.getMediaAtom("Title");
      _albumArtistIndex = _media.getMediaAtom("WM/AlbumArtist");
      _authorIndex = _media.getMediaAtom("Author");
      _albumIndex = _media.getMediaAtom("Album");
      _playCountIndex = _media.getMediaAtom("PlayCount");
      _userLastPlayedTimeIndex = _media.getMediaAtom("UserLastPlayedTime");
    }

    /// <summary>
    /// Parses the windows media player database.
    /// </summary>
    /// <returns>List with tracks that can be scrobbled.</returns>
    public List<MediaDBScrobble> GetMusicLibrary()
    {
      List<MediaDBScrobble> entries;
      IWMPPlaylist mediaList = null;
      IWMPMedia mediaItem;

      try
      {
        // get the full audio media list
        mediaList = _media.getByAttribute("MediaType", "Audio");
        entries = new List<MediaDBScrobble>(mediaList.count);

        for (int i = 0; i < mediaList.count; i++)
        {
          try
          {
            mediaItem = mediaList.get_Item(i);

            // create the new entry and populate its properties
            int playCount = GetPlayCount(mediaItem);
            if (playCount == 0)
              continue;
            string trackName = GetTitle(mediaItem);
            string artistName = GetArtist(mediaItem);
            string albumName = GetAlbum(mediaItem);
            string albumArtist = GetAlbumArtist(mediaItem);
            TimeSpan duration = TimeSpan.FromSeconds(mediaItem.duration);
            DateTime lastPlayed = GetUserLastPlayedTime(mediaItem);

            entries.Add(new MediaDBScrobble(playCount, lastPlayed, trackName, artistName, albumName, albumArtist, duration));
          }
          catch (Exception)
          {
            // probably missing necessary info...
          }
        }
      }
      finally
      {
        // make sure we clean up as this is COM
        if (mediaList != null)
          mediaList.clear();
      }

      return entries;
    }

    /// <summary>
    /// Gets the artist name.
    /// </summary>
    /// <param name="mediaItem">Item to get artist name for.</param>
    /// <returns>The artist name.</returns>
    private string GetArtist(IWMPMedia mediaItem)
    {
      return mediaItem.getItemInfoByAtom(_authorIndex);
    }

    /// <summary>
    /// Gets the album artist name.
    /// </summary>
    /// <param name="mediaItem">Item to get album artist name for.</param>
    /// <returns>The album artist name.</returns>
    private string GetAlbumArtist(IWMPMedia mediaItem)
    {
      return mediaItem.getItemInfoByAtom(_albumArtistIndex);
    }

    /// <summary>
    /// Gets the track name.
    /// </summary>
    /// <param name="mediaItem">Item to get track title for.</param>
    /// <returns>The track name.</returns>
    private string GetTitle(IWMPMedia mediaItem)
    {
      string title = mediaItem.getItemInfoByAtom(_titleIndex);
      if (string.IsNullOrEmpty(title))
        title = mediaItem.name;

      return title;
    }

    /// <summary>
    /// Gets the album string.
    /// </summary>
    /// <param name="mediaItem">Item to get album for.</param>
    /// <returns>The album name.</returns>
    private string GetAlbum(IWMPMedia mediaItem)
    {
      return mediaItem.getItemInfoByAtom(_albumIndex);
    }

    /// <summary>
    /// Gets the play count.
    /// </summary>
    /// <param name="mediaItem">Item to get play count for.</param>
    /// <returns>Play count.</returns>
    private int GetPlayCount(IWMPMedia mediaItem)
    {
      return int.Parse(mediaItem.getItemInfoByAtom(_playCountIndex));
    }

    private DateTime GetUserLastPlayedTime(IWMPMedia mediaItem)
    {
      return DateTime.Parse(mediaItem.getItemInfoByAtom(_userLastPlayedTimeIndex));
    }

    #region IDisposable Members

    /// <summary>
    /// Closes the media player.
    /// </summary>
    public void Dispose()
    {
      if (_mediaPlayer != null)
        _mediaPlayer.close();
    }

    #endregion IDisposable Members
  }
}