using System;

namespace osuTK.Platform.MacOS
{
    internal static class NSProcessInfo
    {
        private static readonly IntPtr classProcessInfo = Class.Get("NSProcessInfo");
        private static readonly IntPtr selProcessInfo = Selector.Get("processInfo");
        private static readonly IntPtr selIsOperatingSystemAtLeastVersion = Selector.Get("isOperatingSystemAtLeastVersion:");

        public static bool IsOperatingSystemAtLeastVersion(NSOperatingSystemVersion version)
        {
            var processInfo = Cocoa.SendIntPtr(classProcessInfo, selProcessInfo);
            return Cocoa.SendBool(processInfo, selIsOperatingSystemAtLeastVersion, version);
        }
    }
}