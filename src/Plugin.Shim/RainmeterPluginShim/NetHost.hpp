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

constexpr auto NETHOST_SUCCESS = 0;
constexpr auto NETHOST_ERROR_LOAD_HOSTFXR = 1;
constexpr auto NETHOST_ERROR_LOADFUNC = 2;
constexpr auto NETHOST_ERROR_HOSTFXR_RUNTIME_INIT = 3;
constexpr auto NETHOST_ERROR_GET_RUNTIME_DELEGATE = 4;
constexpr auto NETHOST_ERROR_GET_HOSTFXR_PATH = 5;
constexpr auto NETHOST_ERROR_GET_EXPORT = 6;

class NetHost
{
public:
	~NetHost();
	int GetMethodFromAssembly(
		const char_t* binaryPath,
		const char_t* runtimeConfigPath,
		const char_t* dotnetType,
		const char_t* methodName,
		const char_t* delegateName,
		void** methodPointer);

private:
	load_assembly_and_get_function_pointer_fn get_function_pointer_fptr = nullptr;
	hostfxr_initialize_for_runtime_config_fn init_fptr = nullptr;
	hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
	hostfxr_close_fn close_fptr = nullptr;

	int LoadHostFxr();
	[[nodiscard]] bool IsHostFxrLoaded() const;

	int GetAssemblyFunctionLoader(
		const char_t* config_path,
		load_assembly_and_get_function_pointer_fn& loadAssemblyAndGetFunction);
	
	static void* GetExport(HMODULE hLib, const char* name);
};