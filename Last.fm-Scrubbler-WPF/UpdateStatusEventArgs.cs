namespace Last.fm_Scrubbler_WPF
{
	public class UpdateStatusEventArgs
	{
		public string NewStatus
		{
			get { return _newStatus; }
			private set { _newStatus = value; }
		}
		private string _newStatus;

		public UpdateStatusEventArgs(string newStatus)
		{
			NewStatus = newStatus;
		}
	}
}