using System;

namespace osuTK.Graphics
{
    public abstract class GLBindings
    {
        public IntPtr[] _EntryPointsInstance;
        public byte[] _EntryPointNamesInstance;
        public int[] _EntryPointNameOffsetsInstance;

        protected virtual object SyncRoot { get; }
    }
}