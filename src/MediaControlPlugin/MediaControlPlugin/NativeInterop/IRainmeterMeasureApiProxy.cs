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

// ReSharper disable UnusedMemberInSuper.Global | reflects complete native API

namespace MediaControlPlugin.NativeInterop;

/// <summary>
/// Proxy to access rainmeter API as a measure.
/// </summary>
public interface IRainmeterMeasureApiProxy
{
    /// <summary>
    ///     Sends a message to the Rainmeter log.
    /// </summary>
    /// <remarks>
    ///     <see cref="RainmeterLogLevel.Debug"/> messages are logged only when Rainmeter is in debug mode.
    /// </remarks>
    /// <param name="logLevel">Severity level of the logged message.</param>
    /// <param name="message">Message to be logged.</param>
    public void Log(RainmeterLogLevel logLevel, string message);

    /// <summary>
    ///     Retrieves the option defined in the skin file.
    /// </summary>
    /// <param name="optionName">Option name to be read from the skin.</param>
    /// <param name="defaultValue">Default value for the option if it is not found or invalid.</param>
    /// <param name="replaceMeasures">If true, replaces section variables in the returned string.</param>
    /// <returns>Returns the option value as a string.</returns>
    public string ReadString(string optionName, string defaultValue, bool replaceMeasures = true);

    /// <summary>
    ///     Retrieves the option defined in the skin file and converts a relative path to an absolute path.
    /// </summary>
    /// <param name="optionName">Option name to be read from the skin.</param>
    /// <param name="defaultValue">Default value for the option if it is not found or invalid.</param>
    /// <returns>Returns the absolute path of the option value as a string.</returns>
    public string ReadPath(string optionName, string defaultValue);

    /// <summary>
    ///     Retrieves the option defined in the skin file and converts it to a double.
    /// </summary>
    /// <remarks>
    ///     If the option is a formula, the returned value will be the result of the parsed formula.
    /// </remarks>
    /// <param name="optionName">Option name to read from the skin.</param>
    /// <param name="defaultValue">
    ///     Default value for the option if it is not found, invalid, or a formula could not be parsed.
    /// </param>
    /// <returns>Returns the option value as a double.</returns>
    public double ReadDouble(string optionName, double defaultValue);

    /// <summary>
    ///     Retrieves the option defined in the skin file and converts it to an integer.
    /// </summary>
    /// <remarks>
    ///     If the option is a formula, the returned value will be the result of the parsed formula.
    /// </remarks>
    /// <param name="optionName">Option name to read from the skin.</param>
    /// <param name="defaultValue">
    ///     Default value for the option if it is not found, invalid, or a formula could not be parsed.
    /// </param>
    /// <returns>Returns the option value as an integer.</returns>
    public int ReadInt(string optionName, int defaultValue);

    /// <summary>
    /// Replaces all rainmeter variables in the string and returns the result.
    /// </summary>
    /// <param name="str">String containing unresolved variables.</param>
    public string ReplaceVariables(string str);

    /// <summary>
    /// Gets the name of the measure.
    /// </summary>
    /// <remarks>Call in the Initialize function and store the results for later use.</remarks>
    /// <returns>The name of the measure as a string.</returns>
    public string GetMeasureName();

    /// <summary>
    /// Gets an internal pointer to the current skin.
    /// </summary>
    /// <remarks>Call in the Initialize function and store the results for later use.</remarks>
    /// <returns>A Pointer to the current skin.</returns>
    public IntPtr GetSkin();

    /// <summary>
    /// Gets the full path of the currrent skin.
    /// </summary>
    /// <remarks>Call in the Initialize function and store the results for later use.</remarks>
    /// <returns>Path of the skin as a string,</returns>
    public string GetSkinName();

    /// <summary>
    /// Gets the pointer of the skin window (HWND).
    /// </summary>
    /// <remarks>Call in the Initialize function and store the results for later use.</remarks>
    /// <returns>Handle to the skin window as an IntPtr.</returns>
    public IntPtr GetSkinWindow();

    /// <summary>
    /// Get the path to the Rainmeter data file (Rainmeter.data)
    /// </summary>
    /// <remarks>Call in the Initialize function and store the results for later use.</remarks>
    /// <returns>Path of the Rainmeter data file as a string</returns>
    public string GetSettingsFile();

    /// <summary>
    /// Execute a command on the current skin.
    /// </summary>
    /// <param name="command">Bang to execute</param>
    public void Execute(string command);
}