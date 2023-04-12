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
/// Extensions for <see cref="string"/>.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Frees the passed manged pointer if necessary and sets it to the newly created allocated memory of the string.
    /// </summary>
    /// <returns>Pointer to the newly allocated memory of the string.</returns>
    public static IntPtr RecyclePointerAndSetAsNewValue(this string stringValue, ref IntPtr pointerToUse)
    {
        pointerToUse.FreeUnmanagedHandle();
        var pointer = Marshal.StringToHGlobalUni(stringValue);
        pointerToUse = pointer;
        return pointer;
    }
}