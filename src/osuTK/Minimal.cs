#if !MINIMAL

using System.Drawing;

#else

using System;
using System.Drawing;

// Override a number of System.* classes when compiling for
// minimal targets (e.g. MonoTouch).
// Note: the "overriden" classes must not be fully qualified for this to work!

namespace Microsoft.Win32
{
    internal class RegistryKey
    {
        public RegistryKey OpenSubKey(string name)
        {
            return new RegistryKey();
        }

        public object GetValue(string name)
        {
            return "";
        }
    }

    internal class Registry
    {
        public static readonly RegistryKey LocalMachine = new RegistryKey();
    }

    internal sealed class SystemEvents
    {
        public static event EventHandler DisplaySettingsChanged;
    }
}

namespace OpenTK
{
    public sealed class Icon : IDisposable
    {
        private IntPtr handle;

        public Icon(Icon icon, int width, int height)
        {
            handle = icon.Handle;
            Width = width;
            Height = height;
        }

        public IntPtr Handle { get { return handle; } set { handle = value; } }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Bitmap ToBitmap()
        {
            return new Bitmap(Width, Height);
        }

        public void Dispose()
        { }

        public static Icon ExtractAssociatedIcon (string location)
        {
            return null;
        }
    }

    public abstract class Image : IDisposable
    {
        public void Dispose() { }

        internal void Save(System.IO.Stream s, ImageFormat fomat)
        {
        }
    }

    public sealed class Bitmap : Image
    {
        private int width;
        private int height;

        public Bitmap() { }

        public Bitmap(int width, int height)
        {
            // TODO: Complete member initialization
            this.width = width;
            this.height = height;
        }

        internal Bitmap(int width, int height, int stride, PixelFormat format, IntPtr pixels)
        {
            // TODO: Complete member initialization
            this.width = width;
            this.height = height;
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        internal void UnlockBits(BitmapData data)
        {
        }

        internal BitmapData LockBits(Rectangle rectangle, ImageLockMode imageLockMode, PixelFormat pixelFormat)
        {
            return new BitmapData(Width, Height, 0);
        }

        internal static int GetPixelFormatSize(PixelFormat format) => 32;
    }

    internal sealed class BitmapData
    {
        internal BitmapData(int width, int height, int stride)
        {
            Width = width;
            Height = height;
            Stride = stride;
        }

        public IntPtr Scan0 { get { return IntPtr.Zero; } }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
    }

    internal enum ImageLockMode
    {
        ReadOnly,
        WriteOnly,
        ReadWrite,
        UserInputBuffer
    }

    internal enum PixelFormat
    {
        Format32bppArgb = 2498570
    }

    internal enum ImageFormat
    {
        Png
    }
}

// Need a different namespace to avoid clash with OpenTK.Graphics.
namespace OpenTK.Minimal
{
    using System.Drawing;

    internal sealed class Graphics : IDisposable
    {
        public static Graphics FromImage(Image img)
        {
            return new Graphics();
        }

        public void Dispose()
        { }

        internal void DrawImage(Bitmap bitmap, int p, int p_2, int p_3, int p_4)
        {
        }
    }
}

#endif
