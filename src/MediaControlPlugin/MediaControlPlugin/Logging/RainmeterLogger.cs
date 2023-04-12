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

using MediaControlPlugin.NativeInterop;

using Microsoft.Extensions.Logging;

namespace MediaControlPlugin.Logging;

public class RainmeterLogger : ILogger
{
    private readonly string _categoryName;
    private readonly MeasureManager _measureManager;

    public IExternalScopeProvider? ScopeProvider { get; set; }

    public RainmeterLogger(string categoryName, IExternalScopeProvider? scopeProvider, MeasureManager measureManager)
    {
        _categoryName = categoryName;
        _measureManager = measureManager;
        ScopeProvider = scopeProvider;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // try to get rainmeter API from logging scope
        IRainmeterMeasureApiProxy? rainmeterMeasureApiProxy = null;
        void SetLocalApiProxyIfNotAlreadyDone(object? scope, object? _)
        {
            rainmeterMeasureApiProxy ??= scope as IRainmeterMeasureApiProxy;
        }
        ScopeProvider?.ForEachScope(SetLocalApiProxyIfNotAlreadyDone, (object?)null);

        rainmeterMeasureApiProxy ??= _measureManager.FallbackApiProxy;
        rainmeterMeasureApiProxy?.Log(MapLogLevel(logLevel), $"[{_categoryName}] {formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return ScopeProvider?.Push(state) ?? new EmptyDisposable();
    }

    private static RainmeterLogLevel MapLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Critical or LogLevel.Error => RainmeterLogLevel.Error,
            LogLevel.Warning => RainmeterLogLevel.Warning,
            LogLevel.Information => RainmeterLogLevel.Notice,
            _ => RainmeterLogLevel.Debug,
        };
    }

    private sealed class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}