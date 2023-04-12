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
/// Test proxy that logs all rainmeter API calls to the console.
/// Useful when testing stuff with e.g. a C++/C# console app.
/// </summary>
public class MeasureApiTestProxy : IRainmeterMeasureApiProxy
{
    /// <inheritdoc/>
    public void Log(RainmeterLogLevel logLevel, string message)
    {
        Console.WriteLine($"Called Log(\n\tlogLevel: '{logLevel}',\n\tmessage: '{message}')");
    }

    /// <inheritdoc/>
    public string ReadString(string optionName, string defaultValue, bool replaceMeasures = true)
    {
        Console.WriteLine(
            $"Called ReadString(\n\toptionName: '{optionName}',\n\tdefaultValue: '{defaultValue}',\n\treplaceMeasures: '{replaceMeasures}')");
        return "Thumbnail";
    }

    public string ReadPath(string optionName, string defaultValue)
    {
        Console.WriteLine(
            $"Called ReadPath(\n\toptionName: '{optionName}',\n\tdefaultValue: '{defaultValue}')");
        return defaultValue;
    }

    public double ReadDouble(string optionName, double defaultValue)
    {
        Console.WriteLine(
            $"Called ReadDouble(\n\toptionName: '{optionName}',\n\tdefaultValue: '{defaultValue}')");
        return defaultValue;
    }

    public int ReadInt(string optionName, int defaultValue)
    {
        Console.WriteLine(
            $"Called ReadInt(\n\toptionName: '{optionName}',\n\tdefaultValue: '{defaultValue}')");
        return defaultValue;
    }

    public string ReplaceVariables(string str)
    {
        Console.WriteLine($"Called ReplaceVariables(\n\tstr: '{str}')");
        return string.Empty;
    }

    public string GetMeasureName()
    {
        Console.WriteLine("Called GetMeasureName()");
        return string.Empty;
    }

    public IntPtr GetSkin()
    {
        Console.WriteLine("Called GetSkin()");
        return IntPtr.Zero;
    }

    public string GetSkinName()
    {
        Console.WriteLine("Called GetSkinName()");
        return string.Empty;
    }

    public IntPtr GetSkinWindow()
    {
        Console.WriteLine("Called GetSkinWindow()");
        return IntPtr.Zero;
    }

    public string GetSettingsFile()
    {
        Console.WriteLine("Called GetSettingsFile()");
        return string.Empty;
    }

    public void Execute(string command)
    {
        Console.WriteLine($"Called ReadString(\n\rcommand: '{command}')");
    }
}