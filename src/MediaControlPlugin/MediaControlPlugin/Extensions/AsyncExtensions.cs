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

using Windows.Foundation;

namespace MediaControlPlugin.Extensions;

internal static class AsyncExtensions
{
    public static T SafeSyncAwaitAsyncResult<T>(this IAsyncOperation<T> task)
    {
        return Task.Run(async () => await task).GetAwaiter().GetResult();
    }

    public static T SafeSyncAwaitAsyncResult<T>(this Task<T> task)
    {
        return Task.Run(async () => await task).GetAwaiter().GetResult();
    }

    public static void SafeSyncAwaitAsyncCompletion(this Task task)
    {
        Task.Run(async () => await task).GetAwaiter().GetResult();
    }
}