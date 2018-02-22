using Last.fm_Scrubbler_WPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Last.fm_Scrubbler_WPF.ViewModels.ScrobbleViewModels
{
  interface INeedScrobbler
  {
    IAuthScrobbler Scrobbler { get; set; }
  }
}