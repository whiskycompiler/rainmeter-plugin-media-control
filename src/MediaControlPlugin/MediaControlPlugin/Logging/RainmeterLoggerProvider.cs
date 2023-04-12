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

using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace MediaControlPlugin.Logging;

public class RainmeterLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly MeasureManager _measureManager;
    private IExternalScopeProvider? _scopeProvider;
    private readonly ConcurrentDictionary<string, RainmeterLogger> _loggers;

    public RainmeterLoggerProvider(MeasureManager measureManager)
    {
        _measureManager = measureManager;
        _loggers = new ConcurrentDictionary<string, RainmeterLogger>();
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.TryGetValue(categoryName, out var logger)
            ? logger
            : _loggers.GetOrAdd(
                categoryName,
                new RainmeterLogger(categoryName, _scopeProvider, _measureManager));
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (var logger in _loggers)
        {
            logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}