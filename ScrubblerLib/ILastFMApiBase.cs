﻿using IF.Lastfm.Core.Api;
using System;
using System.Net.Http;

namespace ScrubblerLib
{
  /// <summary>
  /// Interface for basic last.fm api functions.
  /// </summary>
  public interface ILastFMApiBase : IDisposable
  {
    /// <summary>
    /// Authentication.
    /// </summary>
    ILastAuth Auth { get; }

    /// <summary>
    /// HttpClient used for requests.
    /// </summary>
    HttpClient HttpClient { get; }
  }
}