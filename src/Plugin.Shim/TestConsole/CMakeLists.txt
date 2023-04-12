# -----------------------------------------------------------------------
#    Copyright (C) 2023 whiskycompiler
#
#    This file is part of "Plugin.Shim".
#
#    This program is free software: you can redistribute it and/or
#    modify it under the terms of the GNU General Public License
#    as published by the Free Software Foundation, either version 3
#    of the License, or (at your option) any later version.
#
#    This program is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
#    See the GNU General Public License for more details.
#
#    You should have received a copy of the GNU General Public License
#    along with this program. If not, see <https://www.gnu.org/licenses/>.
# --------------------------------------------------------------------------

cmake_minimum_required (VERSION 3.16)

# Add directories with headers to include
include_directories(
	"${RainmeterPluginSdkDir}/API"
	${NetCoreNativeRuntimePackDir}
	"${RainmeterPluginShim_SOURCE_DIR}"
)

# Add directories with headers to include
include_directories(
	"${RainmeterPluginShim_SOURCE_DIR}"
)

# Add source files for the DLL
add_executable (
	TestConsole
	"Console.cpp"
)

# Declare lib files to link for the DLL
target_link_libraries(
	PluginShim
	"${RainmeterPluginSdkDir}/API/x64/Rainmeter.lib"
	"${NetCoreNativeRuntimePackDir}/nethost.lib"
)

# Declare lib files to link for the DLL
target_link_libraries(TestConsole PluginShim)

set_property(TARGET TestConsole PROPERTY CXX_STANDARD 20)
install(TARGETS PluginShim DESTINATION "TestConsole")
install(TARGETS TestConsole DESTINATION "TestConsole")
install(DIRECTORY "../../../bin/${PLUGIN_NAME}/Debug/" DESTINATION "TestConsole/${PLUGIN_NAME}")
