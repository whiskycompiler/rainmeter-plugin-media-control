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

using System.Runtime.InteropServices;

using MediaControlPlugin.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaControlPlugin.NativeInterop;

// ReSharper disable UnusedMember.Global | members in this class are used by native callers
// ReSharper disable RedundantAssignment | false positive? refs are used to modify pointers in native callers
#pragma warning disable IDE0060 // unused paramter check | parameters are necessary for native interop

/// <summary>
/// This is the plugin class containing methods invoked by the native shim via hostfxr.<br/>
/// You probably do not need to change much in here if at all.<br/>
/// I explained some cases in the README.MD.
/// </summary>
public static class Plugin
{
    #region Delegates for native callers used by hostfxr

    public delegate void InitializeDelegate(ref IntPtr measureData, IntPtr rainmeter);

    public delegate double UpdateDelegate(IntPtr measureData);

    public delegate void ReloadDelegate(IntPtr measureData, IntPtr rainmeter, ref double maxValue);

    public delegate void FinalizeDelegate(IntPtr measureData);

    public delegate IntPtr GetStringDelegate(IntPtr measureData);

    public delegate void ExecuteBangDelegate(IntPtr measureData, string args);

    public delegate IntPtr CustomFuncDelegate(
        IntPtr measurePointer,
        int argc,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)]
        string[] arguments);

    #endregion

    /// <summary>
    /// Method that is called when the plugin is loaded (e.g. skin load or refresh)
    /// and is responsible for setting up your measure.
    /// </summary>
    /// <param name="measurePointer">
    ///     Reference to a pointer to the data containing your measure.
    /// </param>
    /// <param name="measureApiPointer">
    ///     Pointer to interact with rainmeter in the name of your measure.
    /// </param>
    public static void Initialize(ref IntPtr measurePointer, IntPtr measureApiPointer)
    {
        IRainmeterMeasureApiProxy? measureApiProxy = measureApiPointer == IntPtr.Zero
            ? null
            : new RainmeterMeasureApiProxy(measureApiPointer);

#if DEBUG
        measureApiProxy ??= new MeasureApiTestProxy();
#else
        if (measureApiProxy == null)
        {
            throw new ArgumentNullException(nameof(measureApiPointer));
        }
#endif

        try
        {
            var serviceProvider = Application.GetServiceProvider().SafeSyncAwaitAsyncResult();
            var logger = serviceProvider.GetRequiredService<ILogger<Measure>>();
            using var _ = logger.BeginScope(measureApiProxy);

            var measureManager = serviceProvider.GetRequiredService<MeasureManager>();

            var measure = ActivatorUtilities.CreateInstance<Measure>(
                serviceProvider,
                measureApiProxy);

            measurePointer = GCHandle.ToIntPtr(GCHandle.Alloc(measure));

            measureManager.RegisterNewMeasure(measure);
        }
        catch (Exception e)
        {
            measureApiProxy.Log(RainmeterLogLevel.Error, $"Failed to initialize Measure! {e}");
        }
    }

    /// <summary>
    /// Method that is called to clean up all your data when the measure is cleaned up.
    /// </summary>
    /// <param name="measurePointer">Pointer to the data of your measure.</param>
    public static void Finalize(IntPtr measurePointer)
    {
        var measure = measurePointer.Resolve<Measure>();
        if (measure != null)
        {
            using var _ = BeginLoggingScopeForMeasure(measure);

            try
            {
                var serviceProvider = Application.GetServiceProvider().SafeSyncAwaitAsyncResult();
                var measureManager = serviceProvider.GetRequiredService<MeasureManager>();
                measureManager.UnregisterMeasure(measure);
            }
            catch
            {
                measure.Dispose();
                measurePointer.FreeManagedHandle();
                Environment.Exit(666); // kill application to prevent any after-free calls
                return;
            }

            measure.Dispose();
        }

        measurePointer.FreeManagedHandle();
    }

    /// <summary>
    /// Method that is called to instruct your measure to update its settings.<br/>
    /// This can happen right after the <see cref="Initialize"/> call and<br/>
    /// before every <see cref="Update"/> call if "DynamicVariables=1".
    /// </summary>
    /// <param name="measurePointer">
    ///     Pointer to the data of your measure.
    /// </param>
    /// <param name="measureApiPointer">
    ///     Pointer to interact with rainmeter in the name of your measure.
    /// </param>
    /// <param name="maxValue">
    ///     Reference to a double that can be set to the max value of your measure.<br/>
    ///     Do not set this unless necessary. When set to 0d will set this automatically<br/>
    ///     base on the highest value returned by the <see cref="Update"/> method.
    /// </param>
    public static void Reload(IntPtr measurePointer, IntPtr measureApiPointer, ref double maxValue)
    {
        var measure = measurePointer.ResolveOrThrow<Measure>();
        using var _ = BeginLoggingScopeForMeasure(measure);
        measure.Reload();
    }

    /// <summary>
    /// Method that is called to update the value of your measure (i.e. on each update cycle).<br/>
    /// </summary>
    /// <param name="measurePointer">Pointer to the data of your measure.</param>
    /// <returns>The updated number value of your measure.</returns>
    public static double Update(IntPtr measurePointer)
    {
        var measure = measurePointer.ResolveOrThrow<Measure>();
        using var _ = BeginLoggingScopeForMeasure(measure);
        return measure.Update();
    }

    /// <summary>
    /// Optional method that is called to get the string value of your measure.<br/>
    /// Keep the processing to the minimum because this method can get called quite often.
    /// </summary>
    /// <param name="measurePointer">Pointer to the data of your measure.</param>
    /// <returns>The current string value of your measure.</returns>
    public static IntPtr GetString(IntPtr measurePointer)
    {
        var measure = measurePointer.ResolveOrThrow<Measure>();
        using var _ = BeginLoggingScopeForMeasure(measure);
        return measure.GetString();
    }

    /// <summary>
    /// Optional method that is called when a custom bang is invoked for your measure.
    /// </summary>
    /// <param name="measurePointer">Pointer to the data of your measure.</param>
    /// <param name="args">Arguments for your custom bang logic.</param>
    public static void ExecuteBang(IntPtr measurePointer, [MarshalAs(UnmanagedType.LPWStr)] string args)
    {
        var measure = measurePointer.ResolveOrThrow<Measure>();
        using var _ = BeginLoggingScopeForMeasure(measure);
        measure.ExecuteBang();
    }

    /// <summary>
    /// Optional custom function that can be invoked in section variables.
    /// You can implement any number of them but you'll need to adjust some more code for it (see README.MD).
    /// Alternatively you can just proxy all your funtion through this one and distinguish them by arguments.
    /// </summary>
    /// <param name="measurePointer">Pointer to the data of your measure.</param>
    /// <param name="argc">Count of elements in the arguements array only used for marshalling.</param>
    /// <param name="arguments">Array of string arguments for the function invocation.</param>
    /// <returns>Pointer to the string that will replace the section variable or null to let it remain unchanged.</returns>
    public static IntPtr CustomFunc(
        IntPtr measurePointer,
        int argc,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)]
        string[] arguments)
    {
        var measure = measurePointer.ResolveOrThrow<Measure>();
        using var _ = BeginLoggingScopeForMeasure(measure);
        return measure.CustomFunc(arguments);
    }

    private static IDisposable? BeginLoggingScopeForMeasure(Measure measure)
    {
        var serviceProvider = Application.GetServiceProvider().SafeSyncAwaitAsyncResult();
        var logger = serviceProvider.GetRequiredService<ILogger<Measure>>();
        return logger.BeginScope(measure.RainmeterMeasure);
    }
}