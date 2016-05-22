using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	class FetchedScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		public event EventHandler ToScrobbleChanged;

		/// <summary>
		/// Gets the fetched scrobble.
		/// </summary>
		public LastTrack Scrobble
		{
			get { return _scrobble; }
			private set
			{
				_scrobble = value;
				NotifyOfPropertyChange(() => Scrobble);
			}
		}
		private LastTrack _scrobble;

		/// <summary>
		/// Gets/sets if this scrobble should be scrobbled.
		/// </summary>
		public bool ToScrobble
		{
			get { return _toScrobble; }
			set
			{
				_toScrobble = value;
				NotifyOfPropertyChange(() => ToScrobble);
				ToScrobbleChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		private bool _toScrobble;

		#endregion Properties

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="scrobble">The scrobbled track.</param>
		public FetchedScrobbleViewModel(LastTrack scrobble)
		{
			Scrobble = scrobble;
		}
	}
}