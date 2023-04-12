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

#include <iostream>

#include "RainmeterPluginShim/Plugin.hpp"

int main()
{
	void* data;
	Initialize(&data, nullptr);

	Sleep(1000);

	std::wcout << Update(data) << std::endl;

	Sleep(1000);

	constexpr int argc = 2;
	const WCHAR* argv[argc] = { const_cast<WCHAR*>(L"Hello"), const_cast<WCHAR*>(L"Custom Function") };
	std::wcout << CustomFunc(data, argc, argv) << std::endl;
	std::wcout << GetString(data) << std::endl;

	Sleep(1000);

	std::wcout << Update(data) << std::endl;

	Sleep(1000);

	std::wcout << Update(data) << std::endl;
	std::wcout << GetString(data) << std::endl;


	Finalize(data);
}
