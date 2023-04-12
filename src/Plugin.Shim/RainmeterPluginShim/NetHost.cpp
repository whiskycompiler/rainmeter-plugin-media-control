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

#include <cassert>

#include "NetHost.hpp"

NetHost::~NetHost()
{
	get_function_pointer_fptr = nullptr;
	init_fptr = nullptr;
	get_delegate_fptr = nullptr;
	close_fptr = nullptr;
}

int NetHost::GetMethodFromAssembly(
	const char_t* binaryPath,
	const char_t* runtimeConfigPath,
	const char_t* dotnetType,
	const char_t* methodName,
	const char_t* delegateName,
	void** methodPointer)
{
	// STEP 1: Load HostFxr and get exported hosting functions
	if (!IsHostFxrLoaded())
	{
		const int result = LoadHostFxr();
		if(result != NETHOST_SUCCESS)
		{
			return result;
		}
	}

	// STEP 2: Initialize and start the .NET Core runtime
	load_assembly_and_get_function_pointer_fn loadAssemblyAndGetFunctionPointer = nullptr;
	auto result = GetAssemblyFunctionLoader(runtimeConfigPath, loadAssemblyAndGetFunctionPointer);
	if (result != NETHOST_SUCCESS)
	{
		return result;
	}

	// STEP 3: Load managed assembly and get function pointer to a managed method
	result = loadAssemblyAndGetFunctionPointer(
		binaryPath,
		dotnetType,
		methodName,
		delegateName,
		nullptr /*reserved*/,
		methodPointer);
	if (result != 0 || methodPointer == nullptr)
	{
		return NETHOST_ERROR_LOADFUNC;
	}

	return NETHOST_SUCCESS;
}


// Using the nethost library, discover the location of hostfxr and get exports
int NetHost::LoadHostFxr()
{
	// Pre-allocate a large buffer for the path to hostfxr
	char_t buffer[MAX_PATH];
	size_t buffer_size = sizeof buffer / sizeof(char_t);
	if (get_hostfxr_path(buffer, &buffer_size, nullptr) != 0)
	{
		return NETHOST_ERROR_GET_HOSTFXR_PATH;
	}

	// Load hostfxr and get desired exports
	const HMODULE lib = LoadLibraryW(buffer);
	if(lib == nullptr)
	{
		return NETHOST_ERROR_LOAD_HOSTFXR;
	}

	init_fptr = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(GetExport(lib, "hostfxr_initialize_for_runtime_config"));
	get_delegate_fptr = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetExport(lib, "hostfxr_get_runtime_delegate"));
	close_fptr = reinterpret_cast<hostfxr_close_fn>(GetExport(lib, "hostfxr_close"));

	return IsHostFxrLoaded() ? NETHOST_SUCCESS : NETHOST_ERROR_GET_EXPORT;
}

// Load and initialize .NET Core and get desired function pointer for scenario
int NetHost::GetAssemblyFunctionLoader(
	const char_t* config_path,
	load_assembly_and_get_function_pointer_fn &loadAssemblyAndGetFunction)
{
	if (get_function_pointer_fptr)
	{
		loadAssemblyAndGetFunction = *get_function_pointer_fptr;
		return NETHOST_SUCCESS;
	}

	// Load .NET Core
	hostfxr_handle cxt = nullptr;
	init_fptr(config_path, nullptr, &cxt);
	if (cxt == nullptr) // this is no nullptr only on success
	{
		close_fptr(cxt);
		return NETHOST_ERROR_HOSTFXR_RUNTIME_INIT;
	}

	// Get the load assembly function pointer
	get_delegate_fptr(
		cxt,
		hdt_load_assembly_and_get_function_pointer,
		reinterpret_cast<void**>(&get_function_pointer_fptr));
	close_fptr(cxt);

	if (get_function_pointer_fptr)
	{
		loadAssemblyAndGetFunction = *get_function_pointer_fptr;
		return NETHOST_SUCCESS;
	}

	return NETHOST_ERROR_GET_RUNTIME_DELEGATE;
}

bool NetHost::IsHostFxrLoaded() const
{
	return init_fptr && get_delegate_fptr && close_fptr;
}

void* NetHost::GetExport(const HMODULE hLib, const char* name)
{
	const FARPROC f = GetProcAddress(hLib, name);
	return reinterpret_cast<void*>(f);
}