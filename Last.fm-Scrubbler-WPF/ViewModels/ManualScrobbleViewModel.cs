using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	class ManualScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		public string Artist
		{
			get { return _artist; }
			set
			{
				_artist = value;
				NotifyOfPropertyChange(() => Artist);
			}
		}
		private string _artist;

		public string Track
		{
			get { return _track; }
			set
			{
				_track = value;
				NotifyOfPropertyChange(() => Track);
			}
		}
		private string _track;

		public string Album
		{
			get { return _album; }
			set
			{
				_album = value;
				NotifyOfPropertyChange(() => Album);
			}
		}
		private string _album;

		public DateTime TimePlayed
		{
			get { return _timePlayed; }
			set
			{
				_timePlayed = value;
				NotifyOfPropertyChange(() => TimePlayed);
			}
		}
		private DateTime _timePlayed;

		public bool CurrentDateTime
		{
			get { return _currentDateTime; }
			set
			{
				_currentDateTime = value;
				if(value)
					TimePlayed = DateTime.Now;

				NotifyOfPropertyChange(() => CurrentDateTime);
			}
		}
		private bool _currentDateTime;

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		public ManualScrobbleViewModel()
		{
			CurrentDateTime = true;
		}

		public void Scrobble()
		{

		}
	}
}