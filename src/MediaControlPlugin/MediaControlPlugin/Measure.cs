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

using System.Globalization;

using MediaControlPlugin.Extensions;
using MediaControlPlugin.NativeInterop;
using MediaControlPlugin.Services;

using Microsoft.Extensions.Logging;

namespace MediaControlPlugin;

/// TODO: fix logging scopes (BG invocations have no invoking measure need a fallback)
/// TODO: remove all data when a session is stopped
/// TODO: thumbnail path is only working on the first invocation
/// 
/// <remarks>
/// Sealed to use simple dispose pattern.
/// </remarks>
public sealed class Measure : IDisposable
{
    private readonly ILogger<Measure> _logger;
    private readonly MediaControlService _mediaControlService;

    private IntPtr _getStringBufferIntPtr;
    private string _getStringValue = " ";
    private MeasureType _measureType;

    public IRainmeterMeasureApiProxy RainmeterMeasure { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> class.
    /// </summary>
    public Measure(
        IRainmeterMeasureApiProxy rainmeterMeasure,
        ILogger<Measure> logger,
        MediaControlService mediaControlService)
    {
        RainmeterMeasure = rainmeterMeasure;
        _logger = logger;
        _mediaControlService = mediaControlService;

        _getStringValue.RecyclePointerAndSetAsNewValue(ref _getStringBufferIntPtr);
    }

    /// <inheritdoc cref="Plugin.Reload"/>
    public void Reload()
    {
        var type = RainmeterMeasure.ReadString("Type", string.Empty);
        if (!Enum.TryParse(type, true, out MeasureType measureType))
        {
            _logger.LogError("Invalid value of option 'Type': {Type}", type);
            return;
        }

        _measureType = measureType;
    }

    /// <inheritdoc cref="Plugin.Update"/>
    public double Update()
    {
        string? TransformNumber(int number)
        {
            return number == 0 ? null : number.ToString(CultureInfo.InvariantCulture);
        }

        var stringValue = _measureType switch
        {
            MeasureType.Artist => _mediaControlService.MediaInfo.Artist,
            MeasureType.Title => _mediaControlService.MediaInfo.Title,
            MeasureType.AlbumTitle => _mediaControlService.MediaInfo.AlbumTitle,
            MeasureType.AlbumArtist => _mediaControlService.MediaInfo.AlbumArtist,
            MeasureType.TrackCount => TransformNumber(_mediaControlService.MediaInfo.TrackCount),
            MeasureType.TrackNumber => TransformNumber(_mediaControlService.MediaInfo.TrackNumber),
            MeasureType.Genres => _mediaControlService.MediaInfo.Genres,
            MeasureType.PlaybackType => _mediaControlService.MediaInfo.PlaybackType,
            MeasureType.Subtitle => _mediaControlService.MediaInfo.Subtitle,
            MeasureType.Thumbnail => _mediaControlService.MediaInfo.ThumbnailPath,
            MeasureType.ProcessName => _mediaControlService.MediaInfo.ProcessName,
            _ => null,
        };

        if (string.IsNullOrEmpty(stringValue))
        {
            stringValue = " ";
        }

        if (stringValue != _getStringValue)
        {
            _getStringValue = stringValue;
            stringValue.RecyclePointerAndSetAsNewValue(ref _getStringBufferIntPtr);
        }

        return 0d;
    }

    /// <inheritdoc cref="Plugin.GetString"/>
    public IntPtr GetString()
    {
        return _getStringBufferIntPtr;
    }

    /// <inheritdoc cref="Plugin.ExecuteBang"/>
    public void ExecuteBang()
    {
        _logger.LogError("No ExecuteBang handling implemented!");
    }

    /// <inheritdoc cref="Plugin.CustomFunc"/>
    public IntPtr CustomFunc(string[] arguments)
    {
        _logger.LogError("No CustomFunc handling implemented!");
        return IntPtr.Zero;
    }

    /// <inheritdoc cref="Plugin.Finalize"/>
    public void Dispose()
    {
        _getStringBufferIntPtr.FreeUnmanagedHandle();
    }
}