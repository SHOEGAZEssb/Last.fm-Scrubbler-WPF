using System;
using System.Runtime.InteropServices;

namespace Scrubbler
{
  /// <summary>
  /// Static class for native methods.
  /// </summary>
  static class NativeMethods
  {
    #region Remove 'X'

    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    /// <summary>
    /// Removes the 'X' from a window.
    /// </summary>
    /// <param name="hwnd">Handle of the window.</param>
    public static int RemoveXFromWindow(IntPtr hwnd)
    {
      return SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }

    #endregion Remove 'X'
  }
}