using System;

namespace ScrubblerLib.Data
{
  public class AppleMusicInfo : IEquatable<AppleMusicInfo>
  {
    public string SongName;
    public string SongAlbumArtist;
    public string SongAlbum;
    public string SongArtist;
    public int SongDuration;

    public AppleMusicInfo(string songName, string songAlbumArtist, string songAlbum, string songArtist)
    {
      SongName = songName;
      SongAlbumArtist = songAlbumArtist;
      SongAlbum = songAlbum;
      SongArtist = songArtist;
    }

    public bool Equals(AppleMusicInfo other)
    {
      return other != null && other.SongName == SongName && other.SongArtist == SongArtist && other.SongAlbumArtist == SongAlbumArtist && other.SongDuration == SongDuration;
    }

    public override bool Equals(object obj) => Equals(obj as AppleMusicInfo);

    public static bool operator ==(AppleMusicInfo a1, AppleMusicInfo a2)
    {
      if (a1 is null && a2 is null)
      {
        return true;
      }
      else if (a1 is null || a2 is null)
      {
        return false;
      }
      else
      {
        return a1.Equals(a2);
      }
    }

    public static bool operator !=(AppleMusicInfo a1, AppleMusicInfo a2)
    {
      return !(a1 == a2);
    }

    public override int GetHashCode()
    {
      return SongName.GetHashCode() ^ SongArtist.GetHashCode() ^ SongAlbumArtist.GetHashCode();
    }
  }
}
