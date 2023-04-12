/* -----------------------------------------------------------------------
    Copyright (C) 2023 whiskycompiler

    This file is part of "MediaControlPlugin".

    This program is free software: you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation, either version 3
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see <https://www.gnu.org/licenses/>.
--------------------------------------------------------------------------*/

using System.Runtime.InteropServices;

namespace MediaControlPlugin.Extensions;

/// <summary>
/// Extensions for <see cref="IntPtr"/>.
/// </summary>
internal static class IntPtrExtensions
{
    /// <summary>
    /// Casts the <see cref="IntPtr"/> into the specified type.
    /// </summary>
    /// <returns>Resolved instance of the given type or <see langword="null"/> if the pointer is zero.</returns>
    public static T? Resolve<T>(this IntPtr pointer) where T : class
    {
        return pointer == IntPtr.Zero
            ? null
            : (T?)GCHandle.FromIntPtr(pointer).Target;
    }

    /// <summary>
    /// Casts the <see cref="IntPtr"/> into the specified type.
    /// Will throw if the pointer is zero.
    /// </summary>
    /// <exception cref="NullReferenceException">When pointer is zero.</exception>
    public static T ResolveOrThrow<T>(this IntPtr pointer) where T : class
    {
        return Resolve<T>(pointer)
               ?? throw new NullReferenceException("Attempted to resolve nullptr!");
    }

    /// <summary>
    /// Frees the GC allocated memory at the pointer location and sets the pointer to zero.
    /// Zero pointers are ignored.
    /// </summary>
    /// <exception cref="InvalidOperationException">When the pointer was not managed.</exception>
    public static void FreeManagedHandle(this ref IntPtr pointer)
    {
        if (pointer != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(pointer).Free();
            pointer = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Frees the unmanaged memory at the pointer location and sets the pointer to zero.
    /// Zero pointers are ignored.
    /// </summary>
    public static void FreeUnmanagedHandle(this ref IntPtr pointer)
    {
        if (pointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(pointer);
            pointer = IntPtr.Zero;
        }
    }
}