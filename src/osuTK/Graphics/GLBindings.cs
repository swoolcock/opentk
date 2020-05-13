using System;

namespace osuTK.Graphics
{
    public abstract class GLBindings
    {
        internal IntPtr[] _EntryPointsInstance;
        internal byte[] _EntryPointNamesInstance;
        internal int[] _EntryPointNameOffsetsInstance;

        protected virtual object SyncRoot { get; }
    }
}