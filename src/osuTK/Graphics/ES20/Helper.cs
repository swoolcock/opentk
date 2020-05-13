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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace osuTK.Graphics.ES20
{
    /// <summary>
    /// Provides access to OpenGL ES 2.0 methods.
    /// </summary>
    public sealed partial class GL : GLBindings
    {
        private const string Library = "GLESv2";
        private static readonly object sync_root = new object();

        private static IntPtr[] EntryPoints;
        private static byte[] EntryPointNames;
        private static int[] EntryPointNameOffsets;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GL()
        {
            _EntryPointsInstance = EntryPoints;
            _EntryPointNamesInstance = EntryPointNames;
            _EntryPointNameOffsetsInstance = EntryPointNameOffsets;
        }

        /// <summary>
        /// Returns a synchronization token unique for the GL class.
        /// </summary>
        protected override object SyncRoot
        {
            get { return sync_root; }
        }

#pragma warning disable 3019
#pragma warning disable 1591
#pragma warning disable 1572
#pragma warning disable 1573

        // Note: Mono 1.9.1 truncates StringBuilder results (for 'out string' parameters).
        // We work around this issue by doubling the StringBuilder capacity.

        public static void ClearColor(Color color)
        {
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        public static void BlendColor(Color color)
        {
            GL.BlendColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        public static string GetActiveAttrib(int program, int index, out int size, out ActiveAttribType type)
        {
            int length;
            GetProgram(program, GetProgramParameterName.ActiveAttributeMaxLength, out length);
            string str;

            GetActiveAttrib(program, index, length == 0 ? 1 : length * 2, out length, out size, out type, out str);
            return str;
        }

        public static string GetActiveUniform(int program, int uniformIndex, out int size, out ActiveUniformType type)
        {
            int length;
            GetProgram(program, GetProgramParameterName.ActiveUniformMaxLength, out length);

            string str;
            GetActiveUniform(program, uniformIndex, length == 0 ? 1 : length, out length, out size, out type, out str);
            return str;
        }

        public static void ShaderSource(Int32 shader, System.String @string)
        {
            unsafe
            {
                int length = @string.Length;
                GL.ShaderSource((UInt32)shader, 1, new string[] { @string }, &length);
            }
        }

        public static string GetShaderInfoLog(Int32 shader)
        {
            string info;
            GetShaderInfoLog(shader, out info);
            return info;
        }

        public static void GetShaderInfoLog(Int32 shader, out string info)
        {
            unsafe
            {
                int length;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out length);
                if (length == 0)
                {
                    info = String.Empty;
                    return;
                }
                GL.GetShaderInfoLog((UInt32)shader, length * 2, &length, out info);
            }
        }

        public static string GetProgramInfoLog(Int32 program)
        {
            string info;
            GetProgramInfoLog(program, out info);
            return info;
        }

        public static void GetProgramInfoLog(Int32 program, out string info)
        {
            unsafe
            {
                int length;
                GL.GetProgram(program, GetProgramParameterName.InfoLogLength, out length); if (length == 0)
                {
                    info = String.Empty;
                    return;
                }
                GL.GetProgramInfoLog((UInt32)program, length * 2, &length, out info);
            }
        }

        public static void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
        }

        [CLSCompliant(false)]
        public static void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
        }

        public static void DrawElements(BeginMode mode, int count, DrawElementsType type, int offset)
        {
            DrawElements((PrimitiveType)mode, count, type, new IntPtr(offset));
        }

        public static void Viewport(Size size)
        {
            GL.Viewport(0, 0, size.Width, size.Height);
        }
        public static void Viewport(Point location, Size size)
        {
            GL.Viewport(location.X, location.Y, size.Width, size.Height);
        }

        public static void Viewport(Rectangle rectangle)
        {
            GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

#pragma warning restore 3019
#pragma warning restore 1591
#pragma warning restore 1572
#pragma warning restore 1573
    }

    #pragma warning disable 1574 // XML comment cref attribute could not be resolved, compiler bug in Mono 3.4.0

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="source">The <see cref="DebugSource"/> for this debug message.</param>
    /// <param name="type">The <see cref="DebugType"/> for this debug message.</param>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="severity">The <see cref="DebugSeverity"/> for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProc(
        DebugSource source, DebugType type, int id,
        DebugSeverity severity, int length, IntPtr message,
        IntPtr userParam);

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.Khr.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="source">The <see cref="DebugSource"/> for this debug message.</param>
    /// <param name="type">The <see cref="DebugType"/> for this debug message.</param>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="severity">The <see cref="DebugSeverity"/> for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProcKhr(
        DebugSource source, DebugType type, int id,
        DebugSeverity severity, int length, IntPtr message,
        IntPtr userParam);

    #pragma warning restore 1574 // XML comment cref attribute could not be resolved, compiler bug in Mono 3.4.0
}
