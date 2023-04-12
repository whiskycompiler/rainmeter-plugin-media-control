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

namespace MediaControlPlugin.NativeInterop;

/// <summary>
/// Exception for errors when working with native components.
/// </summary>
public class NativeInteropException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInteropException"/> class.
    /// </summary>
    public NativeInteropException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInteropException"/> class.
    /// </summary>
    public NativeInteropException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInteropException"/> class.
    /// </summary>
    public NativeInteropException(string message, Exception inner)
        : base(message, inner)
    {
    }
}