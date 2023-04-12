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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MediaControlPlugin.NativeInterop;

/// <inheritdoc cref="IRainmeterMeasureApiProxy"/>
public class RainmeterMeasureApiProxy : IRainmeterMeasureApiProxy
{
    private readonly IntPtr _rainmeterMeasurePointer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RainmeterMeasureApiProxy"/> class.
    /// </summary>
    public RainmeterMeasureApiProxy(IntPtr rainmeterMeasurePointer)
    {
        _rainmeterMeasurePointer = rainmeterMeasurePointer;
    }

    /// <inheritdoc/>
    public void Log(RainmeterLogLevel logLevel, string message)
    {
        RmLog(_rainmeterMeasurePointer, logLevel, message);
    }

    /// <inheritdoc/>
    public string ReadString(string optionName, string defaultValue, bool replaceMeasures = true)
    {
        return PointerToNonNullUnicodeString(
            RmReadString(_rainmeterMeasurePointer, optionName, defaultValue, replaceMeasures));
    }

    /// <inheritdoc/>
    public string ReadPath(string optionName, string defaultValue)
    {
        return PointerToNonNullUnicodeString(
            RmPathToAbsolute(_rainmeterMeasurePointer, ReadString(optionName, defaultValue)));
    }

    /// <inheritdoc/>
    public double ReadDouble(string optionName, double defaultValue)
    {
        return RmReadFormula(_rainmeterMeasurePointer, optionName, defaultValue);
    }

    /// <inheritdoc/>
    public int ReadInt(string optionName, int defaultValue)
    {
        return Convert.ToInt32(RmReadFormula(_rainmeterMeasurePointer, optionName, defaultValue));
    }

    /// <inheritdoc/>
    public string ReplaceVariables(string str)
    {
        return PointerToNonNullUnicodeString(RmReplaceVariables(_rainmeterMeasurePointer, str));
    }

    /// <inheritdoc/>
    public string GetMeasureName()
    {
        return PointerToNonNullUnicodeString(RmGet(_rainmeterMeasurePointer, RmGetType.MeasureName));
    }

    /// <inheritdoc/>
    public IntPtr GetSkin()
    {
        return RmGet(_rainmeterMeasurePointer, RmGetType.Skin);
    }

    /// <inheritdoc/>
    public string GetSkinName()
    {
        return PointerToNonNullUnicodeString(RmGet(_rainmeterMeasurePointer, RmGetType.SkinName));
    }

    /// <inheritdoc/>
    public IntPtr GetSkinWindow()
    {
        return RmGet(_rainmeterMeasurePointer, RmGetType.SkinWindowHandle);
    }

    /// <inheritdoc/>
    public string GetSettingsFile()
    {
        return PointerToNonNullUnicodeString(RmGet(IntPtr.Zero, RmGetType.SettingsFile));
    }

    /// <inheritdoc/>
    public void Execute(string command)
    {
        RmExecute(GetSkin(), command);
    }

    private string PointerToNonNullUnicodeString(IntPtr pointer, [CallerMemberName] string caller = null!)
    {
        var result = Marshal.PtrToStringUni(pointer);
        if (result == null)
        {
            var message = $"Marshalling pointer to unicode non-null string failed in '{caller}'!";
            Log(RainmeterLogLevel.Error, message);
            throw new NativeInteropException(message);
        }

        return result;
    }

    private enum RmGetType
    {
        MeasureName = 0,
        Skin = 1,
        SettingsFile = 2,
        SkinName = 3,
        SkinWindowHandle = 4,
    }

    #region DllImports

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    private static extern void RmLog(IntPtr rm, RainmeterLogLevel level, string message);

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr RmReadString(IntPtr rm, string option, string defValue, bool replaceMeasures);

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode)]
    private static extern double RmReadFormula(IntPtr rm, string option, double defValue);

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr RmReplaceVariables(IntPtr rm, string str);

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr RmPathToAbsolute(IntPtr rm, string? relativePath);

    [DllImport("Rainmeter.dll")]
    private static extern IntPtr RmGet(IntPtr rm, RmGetType type);

    [DllImport("Rainmeter.dll", CharSet = CharSet.Unicode)]
    private static extern void RmExecute(IntPtr skin, string command);

    #endregion
}