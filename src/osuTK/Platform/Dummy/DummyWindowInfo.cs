using System;

namespace osuTK.Platform.Dummy
{
    internal class DummyWindowInfo : IWindowInfo
    {
        public void Dispose()
        {
        }

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }
    }
}
