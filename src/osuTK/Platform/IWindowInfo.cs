/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the osuTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */

using System;

namespace osuTK.Platform
{
    /// <summary>Describes an OS window.</summary>
    public interface IWindowInfo : IDisposable
    {
        /// <summary>
        /// Retrieves a platform-specific handle to this window.
        /// </summary>
        IntPtr Handle { get; }
    }
}
