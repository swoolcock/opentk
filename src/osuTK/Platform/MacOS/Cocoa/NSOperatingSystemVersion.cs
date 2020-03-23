using System.Runtime.InteropServices;

namespace osuTK.Platform.MacOS
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NSOperatingSystemVersion
    {
        public long MajorVersion;
        public long MinorVersion;
        public long PatchVersion;
        
        public NSOperatingSystemVersion(long majorVersion, long minorVersion, long patchVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            PatchVersion = patchVersion;
        }
    }
}