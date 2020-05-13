//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2015 Stefanos Apostolopoulos for the Open Toolkit Library
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
using System.Text;
using System.Runtime.InteropServices;


namespace osuTK.Graphics.OpenGL
{
    /// <summary>
    /// OpenGL bindings for .NET, implementing the full OpenGL API, including extensions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains all OpenGL enums and functions defined in the latest OpenGL specification.
    /// The official .spec files can be found at: http://opengl.org/registry/.
    /// </para>
    /// <para> A valid OpenGL context must be created before calling any OpenGL function.</para>
    /// <para>
    /// Use the GL.Load and GL.LoadAll methods to prepare function entry points prior to use. To maintain
    /// cross-platform compatibility, this must be done for both core and extension functions. The GameWindow
    /// and the GLControl class will take care of this automatically.
    /// </para>
    /// <para>
    /// You can use the GL.SupportsExtension method to check whether any given category of extension functions
    /// exists in the current OpenGL context. Keep in mind that different OpenGL contexts may support different
    /// extensions, and under different entry points. Always check if all required extensions are still supported
    /// when changing visuals or pixel formats.
    /// </para>
    /// <para>
    /// You may retrieve the entry point for an OpenGL function using the GL.GetDelegate method.
    /// </para>
    /// </remarks>
    /// <see href="http://opengl.org/registry/"/>
    public sealed partial class GL : GLBindings
    {
        internal const string Library = "opengl32.dll";

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

        // Note: Mono 1.9.1 truncates StringBuilder results (for 'out string' parameters).
        // We work around this issue by doubling the StringBuilder capacity.

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Set the RGB values of the current color.
        /// </summary>
        /// <param name="color">The color to set.</param>
        public static void Color3(Color color)
        {
            GL.Color3(color.R, color.G, color.B);
        }

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Set the current color.
        /// </summary>
        /// <param name="color">The color to set.</param>
        public static void Color4(Color color)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// [requires: v1.0]
        /// Specify clear values for the color buffers.
        /// </summary>
        /// <param name="color">The color to set as the clear value.</param>
        public static void ClearColor(Color color)
        {
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        /// <summary>
        /// [requires: v1.4 or ARB_imaging|VERSION_1_4]
        /// Set the blend color.
        /// </summary>
        /// <param name="color">The blend color to set.</param>
        public static void BlendColor(Color color)
        {
            GL.BlendColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        /// <summary>
        /// [requires: v2.0]
        /// Returns information about an active attribute variable for the specified program object.
        /// </summary>
        /// <param name="program">
        /// Specifies the program object to be queried.
        /// </param>
        /// <param name="index">
        /// Specifies the index of the attribute variable to be queried.
        /// </param>
        /// <param name="size">
        /// Returns the size of the attribute variable.
        /// </param>
        /// <param name="type">
        /// Returns the data type of the attribute variable.
        /// </param>
        /// <returns>
        /// The name of the attribute variable.
        /// </returns>
        public static string GetActiveAttrib(int program, int index, out int size, out ActiveAttribType type)
        {
            int length;
            GetProgram(program, osuTK.Graphics.OpenGL.GetProgramParameterName.ActiveAttributeMaxLength, out length);
            string str;

            GetActiveAttrib(program, index, length == 0 ? 1 : length * 2, out length, out size, out type, out str);
            return str;
        }

        /// <summary>
        /// [requires: v2.0]
        /// Returns information about an active uniform variable for the specified program object.
        /// </summary>
        /// <param name="program">
        /// Specifies the program object to be queried.
        /// </param>
        /// <param name="uniformIndex">
        /// Specifies the index of the uniform variable to be queried.
        /// </param>
        /// <param name="size">[length: 1]
        /// Returns the size of the uniform variable.
        /// </param>
        /// <param name="type">[length: 1]
        /// Returns the data type of the uniform variable.
        /// </param>
        /// <returns>[length: bufSize]
        /// The name of the uniform variable.
        /// </returns>
        public static string GetActiveUniform(int program, int uniformIndex, out int size, out ActiveUniformType type)
        {
            int length;
            GetProgram(program, osuTK.Graphics.OpenGL.GetProgramParameterName.ActiveUniformMaxLength, out length);

            string str;
            GetActiveUniform(program, uniformIndex, length == 0 ? 1 : length, out length, out size, out type, out str);
            return str;
        }

        /// <summary>
        /// [requires: v3.1 or ARB_uniform_buffer_object|VERSION_3_1]
        /// Query the name of an active uniform.
        /// </summary>
        /// <param name="program">
        /// Specifies the program containing the active uniform index uniformIndex.
        /// </param>
        /// <param name="uniformIndex">
        /// Specifies the index of the active uniform whose name to query.
        /// </param>
        /// <returns>
        /// The name of the active uniform at uniformIndex within program.
        /// </returns>
        public static string GetActiveUniformName(int program, int uniformIndex)
        {
            int length;
            GetProgram(program, osuTK.Graphics.OpenGL.GetProgramParameterName.ActiveUniformMaxLength, out length);
            string str;

            GetActiveUniformName(program, uniformIndex, length == 0 ? 1 : length * 2, out length, out str);
            return str;
        }

        /// <summary>
        /// [requires: v3.1 or ARB_uniform_buffer_object|VERSION_3_1]
        /// Retrieve the name of an active uniform block.
        /// </summary>
        /// <param name="program">
        /// Specifies the name of a program containing the uniform block.
        /// </param>
        /// <param name="uniformIndex">
        /// Specifies the index of the uniform block within program.
        /// </param>
        /// <returns>
        /// The name of the uniform block at uniformIndex.
        /// </returns>
        public static string GetActiveUniformBlockName(int program, int uniformIndex)
        {
            int length;
            GetProgram(program, osuTK.Graphics.OpenGL.GetProgramParameterName.ActiveUniformBlockMaxNameLength, out length);
            string str;

            GetActiveUniformBlockName(program, uniformIndex, length == 0 ? 1 : length * 2, out length, out str);
            return str;
        }

        /// <summary>
        /// [requires: v2.0]
        /// Replaces the source code in a shader object.
        /// </summary>
        /// <param name="shader">
        /// Specifies the handle of the shader object whose source code is to be replaced.
        /// </param>
        /// <param name="string">
        /// Specifies a string containing the source code to be loaded into the shader.
        /// </param>
        public static void ShaderSource(Int32 shader, System.String @string)
        {
            unsafe
            {
                int length = @string.Length;
                GL.ShaderSource((UInt32)shader, 1, new string[] { @string }, &length);
            }
        }

        /// <summary>
        /// [requires: v2.0]
        /// Returns the information log for a shader object.
        /// </summary>
        /// <param name="shader">
        /// Specifies the shader object whose information log is to be queried.
        /// </param>
        /// <returns>
        /// The information log.
        /// </returns>
        public static string GetShaderInfoLog(Int32 shader)
        {
            string info;
            GetShaderInfoLog(shader, out info);
            return info;
        }

        /// <summary>
        /// [requires: v2.0]
        /// Returns the information log for a shader object.
        /// </summary>
        /// <param name="shader">
        /// Specifies the shader object whose information log is to be queried.
        /// </param>
        /// <param name="info">[length: bufSize]
        /// Specifies a string that is used to return the information log.
        /// </param>
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

        /// <summary>
        /// [requires: v2.0]
        /// Returns the information log for a program object.
        /// </summary>
        /// <param name="program">
        /// Specifies the program object whose information log is to be queried.
        /// </param>
        /// <returns>
        /// The information log.
        /// </returns>
        public static string GetProgramInfoLog(Int32 program)
        {
            string info;
            GetProgramInfoLog(program, out info);
            return info;
        }

        /// <summary>
        /// [requires: v2.0]
        /// Returns the information log for a program object.
        /// </summary>
        /// <param name="program">
        /// Specifies the program object whose information log is to be queried.
        /// </param>
        /// <param name="info">[length: bufSize]
        /// Specifies a string that is used to return the information log.
        /// </param>
        public static void GetProgramInfoLog(Int32 program, out string info)
        {
            unsafe
            {
                int length;
                GL.GetProgram(program, osuTK.Graphics.OpenGL.GetProgramParameterName.InfoLogLength, out length); if (length == 0)
                {
                    info = String.Empty;
                    return;
                }
                GL.GetProgramInfoLog((UInt32)program, length * 2, &length, out info);
            }
        }

        /// <summary>
        /// Helper function that defines the coordinate origin of the Point Sprite.
        /// </summary>
        /// <param name="param">
        /// A osuTK.Graphics.OpenGL.GL.PointSpriteCoordOriginParameter token,
        /// denoting the origin of the Point Sprite.
        /// </param>
        public static void PointParameter(PointSpriteCoordOriginParameter param)
        {
            GL.PointParameter(PointParameterName.PointSpriteCoordOrigin, (int)param);
        }

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rect">
        /// Specifies the vertices of the rectangle.
        /// </param>
        [CLSCompliant(false)]
        public static void Rect(RectangleF rect)
        {
            GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rect">
        /// Specifies the vertices of the rectangle.
        /// </param>
        [CLSCompliant(false)]
        public static void Rect(Rectangle rect)
        {
            GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rect">
        /// Specifies the vertices of the rectangle.
        /// </param>
        [CLSCompliant(false)]
        public static void Rect(ref RectangleF rect)
        {
            GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// [requires: v1.0][deprecated: v3.2]
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rect">
        /// Specifies the vertices of the rectangle.
        /// </param>
        [CLSCompliant(false)]
        public static void Rect(ref Rectangle rect)
        {
            GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// [requires: v1.1][deprecated: v3.2]
        /// Define an array of vertex data.
        /// </summary>
        /// <param name="size">
        /// Specifies the number of coordinates per vertex. Must be 2, 3, or 4. The initial value is 4.
        /// </param>
        /// <param name="type">
        /// Specifies the data type of each coordinate in the array. Symbolic constants Short, Int, Float, or Double
        /// are accepted. The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive vertices. If stride is 0, the vertices are understood to
        /// be tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first coordinate of the first vertex in the array. The initial value is 0.
        /// </param>
        public static void VertexPointer(int size, VertexPointerType type, int stride, int offset)
        {
            VertexPointer(size, type, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.1][deprecated: v3.2]
        /// Define an array of normals.
        /// </summary>
        /// <param name="type">
        /// Specifies the data type of each coordinate in the array. Symbolic constants Byte, Short, Int, Float, and
        /// Double are accepted. The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive normals. If stride is 0, the normals are understood to be
        /// tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">[length: type,stride]
        /// Specifies the first coordinate of the first normal in the array. The initial value is 0.
        /// </param>
        public static void NormalPointer(NormalPointerType type, int stride, int offset)
        {
            NormalPointer(type, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.1][deprecated: v3.2]
        /// Define an array of color indexes.
        /// </summary>
        /// <param name="type">
        /// Specifies the data type of each color index in the array. Symbolic constants UnsignedByte, Short, Int,
        /// Float, and Double are accepted. The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive color indexes. If stride is 0, the color indexes are
        /// understood to be tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first index in the array. The initial value is 0.
        /// </param>
        public static void IndexPointer(IndexPointerType type, int stride, int offset)
        {
            IndexPointer(type, stride, (IntPtr)offset);
        }

        /// <summary>
        ///[requires: v1.1][deprecated: v3.2]
        /// Define an array of colors.
        /// </summary>
        /// <param name="size">
        /// Specifies the number of components per color. Must be 3 or 4. The initial value is 4.
        /// </param>
        /// <param name="type">
        /// Specifies the data type of each color component in the array. Symbolic constants Byte, UnsignedByte, Short,
        /// UnsignedShort, Int, UnsignedInt, Float, and Double are accepted. The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive colors. If stride is 0, the colors are understood to be
        /// tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first component of the first color element in the array. The initial value is 0.
        /// </param>
        public static void ColorPointer(int size, ColorPointerType type, int stride, int offset)
        {
            ColorPointer(size, type, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.4][deprecated: v3.2]
        /// Define an array of fog coordinates.
        /// </summary>
        /// <param name="type">
        /// Specifies the data type of each fog coordinate. Symbolic constants Float, or Double are accepted.
        /// The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive fog coordinates. If stride is 0, the array elements are
        /// understood to be tightly packed. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first coordinate of the first fog coordinate in the array. The initial value is 0.
        /// </param>
        public static void FogCoordPointer(FogPointerType type, int stride, int offset)
        {
            FogCoordPointer(type, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.1][deprecated: v3.2]
        /// Define an array of edge flags.
        /// </summary>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive edge flags. If stride is 0, the edge flags are understood to
        ///  be tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first edge flag in the array. The initial value is 0.
        /// </param>
        public static void EdgeFlagPointer(int stride, int offset)
        {
            EdgeFlagPointer(stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.1][deprecated: v3.2]
        /// Define an array of texture coordinates.
        /// </summary>
        /// <param name="size">
        /// Specifies the number of coordinates per array element. Must be 1, 2, 3, or 4. The initial value is 4.
        /// </param>
        /// <param name="type">
        /// Specifies the data type of each texture coordinate. Symbolic constants Short, Int, Float, or Double are
        /// accepted. The initial value is Float.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive texture coordinate sets. If stride is 0, the array
        /// elements are understood to be tightly packed. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first coordinate of the first texture coordinate set in the array. The initial value is 0.
        /// </param>
        public static void TexCoordPointer(int size, TexCoordPointerType type, int stride, int offset)
        {
            TexCoordPointer(size, type, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v2.0]
        /// Define an array of generic vertex attribute data.
        /// </summary>
        /// <param name="index">
        /// Specifies the index of the generic vertex attribute to be modified.
        /// </param>
        /// <param name="size">
        /// Specifies the number of components per generic vertex attribute. Must be 1, 2, 3, 4. Additionally, the
        /// symbolic constant Bgra is accepted by glVertexAttribPointer. The initial value is 4.
        /// </param>
        /// <param name="type">
        /// Specifies the data type of each component in the array. The symbolic constants Byte, UnsignedByte, Short,
        /// UnsignedShort, Int, and UnsignedInt are accepted by glVertexAttribPointer and glVertexAttribIPointer.
        /// Additionally HalfFloat, Float, Double, Fixed, Int2101010Rev, UnsignedInt2101010Rev and
        /// UnsignedInt10F11F11FRev are accepted by glVertexAttribPointer. Double is also accepted by
        /// glVertexAttribLPointer and is the only token accepted by the type parameter for that function.
        /// The initial value is Float.
        /// </param>
        /// <param name="normalized">
        /// For glVertexAttribPointer, specifies whether fixed-point data values should be normalized (True) or
        /// converted directly as fixed-point values (False) when they are accessed.
        /// </param>
        /// <param name="stride">
        /// Specifies the byte offset between consecutive generic vertex attributes. If stride is 0, the generic vertex
        /// attributes are understood to be tightly packed in the array. The initial value is 0.
        /// </param>
        /// <param name="offset">
        /// Specifies the first component of the first generic vertex attribute in the array in the data store of the
        /// buffer currently bound to the ArrayBuffer target. The initial value is 0.
        /// </param>
        public static void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
        }

        /// <summary>
        /// [requires: v1.0]
        /// Set the viewport. This function assumes a lower left corner of (0, 0).
        /// </summary>
        /// <param name="size">
        /// Specifies the width and height of the viewport. When a GL context is first attached to a window,
        /// width and height are set to the dimensions of that window.
        /// </param>
        public static void Viewport(Size size)
        {
            GL.Viewport(0, 0, size.Width, size.Height);
        }

        /// <summary>
        /// [requires: v1.0]
        /// Set the viewport.
        /// </summary>
        /// <param name="location">
        /// Specifies the lower left corner of the viewport.
        /// </param>
        /// <param name="size">
        /// Specifies the width and height of the viewport. When a GL context is first attached to a window,
        /// width and height are set to the dimensions of that window.
        /// </param>
        public static void Viewport(Point location, Size size)
        {
            GL.Viewport(location.X, location.Y, size.Width, size.Height);
        }

        /// <summary>
        /// [requires: v1.0]
        /// Set the viewport.
        /// </summary>
        /// <param name="rectangle">
        /// Specifies the lower left corner, as well as the width and height of the viewport. When a GL context is
        /// first attached to a window, width and height are set to the dimensions of that window.
        /// </param>
        public static void Viewport(Rectangle rectangle)
        {
            GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }

    #pragma warning disable 1574 // XML comment cref attribute could not be resolved, compiler bug in Mono 3.4.0

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.Amd.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="category">The <see cref="AmdDebugOutput"/> category for this debug message.</param>
    /// <param name="severity">The <see cref="AmdDebugOutput"/> severity for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProcAmd(int id,
        AmdDebugOutput category, AmdDebugOutput severity,
        int length, IntPtr message, IntPtr userParam);

    /// <summary>
    /// Defines the signature of a debug callback for
    /// <see cref="GL.Arb.DebugMessageCallback"/>.
    /// </summary>
    /// <param name="source">The <see cref="DebugSource"/> for this debug message.</param>
    /// <param name="type">The <see cref="DebugType"/> for this debug message.</param>
    /// <param name="id">The id of this debug message.</param>
    /// <param name="severity">The <see cref="DebugSeverity"/> for this debug message.</param>
    /// <param name="length">The length of this debug message.</param>
    /// <param name="message">A pointer to a null-terminated ASCII C string, representing the content of this debug message.</param>
    /// <param name="userParam">A pointer to a user-specified parameter.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DebugProcArb(
        DebugSource source, DebugType type, int id,
        DebugSeverity severity, int length, IntPtr message,
        IntPtr userParam);

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
