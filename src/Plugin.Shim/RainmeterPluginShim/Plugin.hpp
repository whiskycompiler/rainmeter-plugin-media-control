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

PLUGIN_EXPORT void Initialize(void** data, void* rm);
PLUGIN_EXPORT double Update(void* data);
PLUGIN_EXPORT void Reload(void* data, void* rm, double* maxValue);
PLUGIN_EXPORT LPCWSTR GetString(void* data);
PLUGIN_EXPORT void ExecuteBang(void* data, LPCWSTR args);
PLUGIN_EXPORT void Finalize(void* data);
PLUGIN_EXPORT LPCWSTR CustomFunc(void* data, int argc, const WCHAR* argv[]);