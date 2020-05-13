//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Diagnostics;

namespace osuTK.Graphics
{
    /// <summary>
    /// Implements BindingsBase for the osuTK.Graphics namespace (OpenGL and OpenGL|ES).
    /// </summary>
    public class GraphicsBindingsBase : BindingsBase
    {
        internal IntPtr[] _EntryPointsInstance;
        internal byte[] _EntryPointNamesInstance;
        internal int[] _EntryPointNameOffsetsInstance;

        private readonly GLBindings glBindings;

        protected GraphicsBindingsBase()
        {
        }

        public GraphicsBindingsBase(GLBindings glBindings)
        {
            this.glBindings = glBindings;
        }

        /// <summary>
        /// Retrieves an unmanaged function pointer to the specified function.
        /// </summary>
        /// <param name="funcname">
        /// A <see cref="System.String"/> that defines the name of the function.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/> that contains the address of funcname or IntPtr.Zero,
        /// if the function is not supported by the drivers.
        /// </returns>
        /// <remarks>
        /// Note: some drivers are known to return non-zero values for unsupported functions.
        /// Typical values include 1 and 2 - inheritors are advised to check for and ignore these
        /// values.
        /// </remarks>
        protected override IntPtr GetAddress(string funcname)
        {
            var context = GraphicsContext.CurrentContext as IGraphicsContextInternal;
            if (context == null)
            {
                throw new GraphicsContextMissingException();
            }
            return context != null ? context.GetAddress(funcname) : IntPtr.Zero;
        }

        protected override object SyncRoot { get; }

        // Loads all available entry points for the current API.
        // Note: we prefer IGraphicsContextInternal.GetAddress over
        // this.GetAddress to improve loading performance (less
        // validation necessary.)
        internal override void LoadEntryPoints()
        {
            Debug.Print("Loading entry points for {0}", GetType().FullName);

            IGraphicsContext context = GraphicsContext.CurrentContext;
            if (context == null)
            {
                throw new GraphicsContextMissingException();
            }

            IGraphicsContextInternal context_internal = context as IGraphicsContextInternal;
            unsafe
            {
                fixed (byte* name = glBindings?._EntryPointNamesInstance ?? _EntryPointNamesInstance)
                {
                    var length = glBindings?._EntryPointsInstance.Length ?? _EntryPointsInstance.Length;

                    for (int i = 0; i < length; i++)
                    {
                        var offset = glBindings?._EntryPointNameOffsetsInstance[i] ?? _EntryPointNameOffsetsInstance[i];
                        var address = context_internal.GetAddress(new IntPtr(name + offset));

                        if (glBindings != null)
                            glBindings._EntryPointsInstance[i] = address;
                        else
                            _EntryPointsInstance[i] = address;
                    }
                }
            }
        }
    }
}
