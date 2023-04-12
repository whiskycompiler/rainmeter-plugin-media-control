/* -----------------------------------------------------------------------
	Copyright (C) 2023 whiskycompiler

	This file is part of "Plugin.Shim".

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

#pragma once
#include "include.hpp"
#include "NetHost.hpp"

typedef void (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_initialize_fn)(void** data, void* rainmeter);
typedef double (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_update_fn)(void* data);
typedef void (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_reload_fn)(void* data, void* rainmeter, double* maxValue);
typedef LPCWSTR (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_get_string_fn)(void* data);
typedef void (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_exec_bang_fn)(void* data, LPCWSTR args);
typedef LPCWSTR (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_custom_func_fn)(void* data, int argc, const WCHAR* argv[]);
typedef void (CORECLR_DELEGATE_CALLTYPE* dotnet_plugin_finalize_fn)(void* data);

class Measure
{
public:
	Measure(string_t binaryPath, string_t runtimeConfigPath, string_t dotnetPluginType);
	~Measure();

	void Initialize(void* rm);
	double Update();
	void Reload(void* rm, double* maxValue);
	LPCWSTR GetString();
	void ExecuteBang(LPCWSTR args);
	void Finalize();
	LPCWSTR CutomFunc(int argc, const WCHAR* argv[]);

private:
	// Pointer to the rainmeter measure
	void* rainmeter = nullptr;

	// Data of the dotnet plugin that is held by the shim which is held by rainmeter
	void* data = nullptr;

	// Initialize method of the dotnet plugin
	dotnet_plugin_initialize_fn initialize = nullptr;

	// Update method of the dotnet plugin
	dotnet_plugin_update_fn update = nullptr;

	// Reload method of the dotnet plugin
	dotnet_plugin_reload_fn reload = nullptr;

	// GetString method of the dotnet plugin
	dotnet_plugin_get_string_fn getString = nullptr;

	// ExecuteBang method of the dotnet plugin
	dotnet_plugin_exec_bang_fn executeBang = nullptr;

	// CustomFunc method of the dotnet plugin
	dotnet_plugin_custom_func_fn customFunc = nullptr;

	// Finalize method of the dotnet plugin
	dotnet_plugin_finalize_fn finalize = nullptr;

	// Path to the dotnet DLL
	string_t binaryPath;

	// Path to the dotnet DLL runtimeconfig json
	string_t runtimeConfigPath;

	// Type of the dotnet plugin
	string_t dotnetPluginType;

	// Dotnet runtime host
	NetHost* netHost;

	// Ensures that the pointer to the dotnet method is initialized - returns false on error
	bool EnsureInitializedNetMethodPointer(const wchar_t* entryPointName, const wchar_t* delegateName, void** methodPointer) const;

	// Transforms the name of the delegate into a qualified name that includes the dotnet plugin type details
	void GetFullDelegateName(const wchar_t* delegateName, string_t** fullDelegateName) const;
};
