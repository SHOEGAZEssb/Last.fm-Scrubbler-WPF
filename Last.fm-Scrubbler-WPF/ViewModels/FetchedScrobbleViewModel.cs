using Caliburn.Micro;
using IF.Lastfm.Core.Objects;
using System;

namespace Last.fm_Scrubbler_WPF.ViewModels
{
	/// <summary>
	/// ViewModel for a single fetched scrobble.
	/// Used in the <see cref="FriendScrobbleViewModel"/>.
	/// </summary>
	class FetchedScrobbleViewModel : PropertyChangedBase
	{
		#region Properties

		/// <summary>
		/// Event that triggers when <see cref="ToScrobble"/> changes.
		/// </summary>
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
				NotifyOfPropertyChange(() => IsEnabled);
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

		/// <summary>
		/// Gets if the "Scrobble?" CheckBox is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get { return Scrobble.TimePlayed.Value.LocalDateTime > DateTime.Now.Subtract(TimeSpan.FromDays(14)); }
		}

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