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

using ExamplePlugin = MediaControlPlugin.NativeInterop.Plugin;

var data = IntPtr.Zero;
ExamplePlugin.Initialize(ref data, IntPtr.Zero);

Console.WriteLine(ExamplePlugin.Update(data));
await Task.Delay(1000);

ExamplePlugin.GetString(data);
await Task.Delay(1000);

Console.ReadLine();

Console.WriteLine(ExamplePlugin.Update(data));
await Task.Delay(1000);

Console.WriteLine(ExamplePlugin.Update(data));
await Task.Delay(1000);

ExamplePlugin.GetString(data);
await Task.Delay(1000);

ExamplePlugin.Finalize(data);