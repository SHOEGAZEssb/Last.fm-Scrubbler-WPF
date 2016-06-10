using System;

namespace Last.fm_Scrubbler_WPF.Models
{
	class CSVScrobble
	{
		#region Properties

		public string Artist
		{
			get { return _artist; }
			private set { _artist = value; }
		}
		private string _artist;

		public string Album
		{
			get { return _album; }
			private set { _album = value; }
		}
		private string _album;

		public string Track
		{
			get { return _track; }
			private set { _track = value; }
		}
		private string _track;

		public DateTime DateTime
		{
			get { return _dateTime; }
			private set { _dateTime = value; }
		}
		private DateTime _dateTime;

		#endregion Properties

		public CSVScrobble(string artist, string album, string track, DateTime dateTime)
		{
			Artist = artist;
			Album = album;
			Track = track;
			DateTime = dateTime;
		}
	}
}
