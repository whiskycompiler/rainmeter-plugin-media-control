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

using MediaControlPlugin.Extensions;
using MediaControlPlugin.NativeInterop;

namespace MediaControlPlugin;

/// <summary>
/// Component to handle cross-measure features.
/// </summary>
public class MeasureManager
{
    private readonly List<Measure> _measures = new();

    /// <summary>
    /// Rainmeter API proxy used as a fallback when no other instance is available (e.g. background task).
    /// </summary>
    public IRainmeterMeasureApiProxy? FallbackApiProxy { get; private set; }

    /// <summary>
    /// Registers a new measure (when loaded by rainmeter).
    /// </summary>
    public void RegisterNewMeasure(Measure measure)
    {
        lock (_measures)
        {
            _measures.Add(measure);

            if (_measures.Count == 0)
            {
                FallbackApiProxy = measure.RainmeterMeasure;
            }
        }
    }

    /// <summary>
    /// Unegisters an existing measure (when unloaded by rainmeter).
    /// </summary>
    public void UnregisterMeasure(Measure measure)
    {
        lock (_measures)
        {
            _measures.Remove(measure);

            // no measures left loaded
            if (_measures.Count == 0)
            {
                Application.DisposeServiceProvider().SafeSyncAwaitAsyncCompletion();
                FallbackApiProxy = null;
                return;
            }

            if (measure.RainmeterMeasure == FallbackApiProxy)
            {
                FallbackApiProxy = _measures.FirstOrDefault()?.RainmeterMeasure;
            }
        }
    }
}