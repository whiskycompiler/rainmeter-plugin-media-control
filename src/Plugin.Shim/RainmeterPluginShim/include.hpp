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
#include <Windows.h>
#include <string>
#include <nethost.h>
#include <hostfxr.h>
#include <coreclr_delegates.h>
#include <RainmeterAPI.h>

#define CONCAT2(X, Y) X##Y
#define CONCAT(X, Y) CONCAT2(X, Y)
#define STRINGIFY2(X) #X
#define STRINGIFY(X) STRINGIFY2(X)
#define WIDEN(X) CONCAT(L, STRINGIFY(X))
// Thanks C/C++ ...

#ifndef PLUGIN_NAME
	#define PLUGIN_NAME TestPlugin
#endif
#define PLUGIN_NAME_STRING WIDEN(PLUGIN_NAME)

using string_t = std::basic_string<char_t>;