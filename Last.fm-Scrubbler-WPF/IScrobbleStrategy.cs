using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF
{
	interface IScrobbleStrategy
	{
		event EventHandler<UpdateStatusEventArgs> StatusUpdated;
	}
}
