using System;

namespace WebVTT_Ripper
{
    /// <summary>
    /// Utility class to wrap a WPF InteropHelper Handle to a IWin32Window handle
    /// Stolen from: http://stackoverflow.com/a/10296513/721519
    /// </summary>
    public class HandleWrapper : System.Windows.Forms.IWin32Window
    {
        public HandleWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private readonly IntPtr _hwnd;
    }
}
