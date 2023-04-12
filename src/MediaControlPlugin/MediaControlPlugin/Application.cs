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

using MediaControlPlugin.Logging;
using MediaControlPlugin.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MediaControlPlugin;

public static class Application
{
    private static readonly SemaphoreSlim _serviceProviderCreationLock = new(1);

    // manually cache task result because we can't use ValueTask with synchronicity forced by native interop
    private static Task<IServiceProvider>? _cachedServiceProviderResultTask;

    public static Task<IServiceProvider> GetServiceProvider()
    {
        if (_cachedServiceProviderResultTask is { IsFaulted: false, IsCanceled: false })
        {
            return _cachedServiceProviderResultTask;
        }

        return _cachedServiceProviderResultTask = GetServiceProviderInternal();
    }

    public static async Task DisposeServiceProvider()
    {
        var cachedServiceProviderResultTask = _cachedServiceProviderResultTask;
        if (cachedServiceProviderResultTask != null)
        {
            _cachedServiceProviderResultTask = null;

            var serviceProvider = await cachedServiceProviderResultTask;
            if (serviceProvider is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if(serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    private static async Task<IServiceProvider> GetServiceProviderInternal()
    {
        await _serviceProviderCreationLock.WaitAsync();

        if (_cachedServiceProviderResultTask is { IsFaulted: false, IsCanceled: false })
        {
            return await _cachedServiceProviderResultTask;
        }

        try
        {
            var serviceProvider = CreateServiceProvider();
            await WarmupNecessaryServices(serviceProvider);
            return serviceProvider;
        }
        finally
        {
            _serviceProviderCreationLock.Release();
        }
    }

    private static async Task WarmupNecessaryServices(IServiceProvider serviceProvider)
    {
        await serviceProvider.GetRequiredService<MediaControlService>().Initialize();
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging(builder =>
        {
#if DEBUG
            builder.AddConsole();
#endif
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, RainmeterLoggerProvider>());
        });

        serviceCollection
            .AddSingleton<MediaControlService>()
            .AddSingleton<MeasureManager>()
            .AddSingleton<MediaThumbnailService>();

        var serviceProvider = serviceCollection.BuildServiceProvider(
#if DEBUG
            new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            }
#endif
        );

        return serviceProvider;
    }
}