using Last.fm_Scrubbler_WPF.Models;
using System;
using System.Collections.Generic;
using WMPLib;

namespace Last.fm_Scrubbler_WPF
{
  /// <summary>
  /// Wraps around the windows media player to get its info.
  /// </summary>
  class WMP : IDisposable
  {
    /// <summary>
    /// Reference to the windows media player.
    /// </summary>
    private WindowsMediaPlayer _mediaPlayer;

    /// <summary>
    /// Reference to the media collection.
    /// </summary>
    private IWMPMediaCollection _media;

    /// <summary>
    /// Access index of the track name.
    /// </summary>
    private int _titleIndex;

    /// <summary>
    /// Access index of the (album) artist name.
    /// </summary>
    private int _albumArtistIndex;

    /// <summary>
    /// Access index of the artist name.
    /// </summary>
    private int _authorIndex;

    /// <summary>
    /// Access index of the album name.
    /// </summary>
    private int _albumIndex;

    /// <summary>
    /// Access index of the play count.
    /// </summary>
    private int _playCountIndex;

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
            string trackName = GetTitle(mediaItem);
            string artistName = GetArtist(mediaItem);
            string albumName = GetAlbum(mediaItem);
            int playCount = GetPlayCount(mediaItem);

            entries.Add(new MediaDBScrobble(trackName, artistName, albumName, playCount));
          }
          catch (Exception ex)
          {
            // probably missing neccessary info...
          }
        }
      }
      finally
      {
        // make sure we clean up as this is COM
        if (mediaList != null)
        {
          mediaList.clear();
        }
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
      string artist = mediaItem.getItemInfoByAtom(_albumArtistIndex);
      if (string.IsNullOrEmpty(artist) || artist == "VARIOUS ARTISTS")
        artist = mediaItem.getItemInfoByAtom(_authorIndex);

      return artist;
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

    #region IDisposable Members

    /// <summary>
    /// Closes the media player.
    /// </summary>
    public void Dispose()
    {
      if (_mediaPlayer != null)
      {
        _mediaPlayer.close();
      }
    }

    #endregion IDisposable Members
  }
}