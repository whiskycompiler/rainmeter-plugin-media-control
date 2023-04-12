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
	"${RainmeterPluginSdkDir}/.."
	"${NetCoreNativeRuntimePackDir}"
)

# Add source files for the DLL
add_library (
	PluginShim SHARED
	"Plugin.cpp"
	"NetHost.cpp"
	"MeasureShim.cpp"
)

add_compile_definitions(
	PLUGIN_NAME=${PLUGIN_NAME}
	PLUGIN_VERSION=${PLUGIN_VERSION}
	COPYRIGHT=${COPYRIGHT})

# Declare resoure files
target_sources(PluginShim PRIVATE "Plugin.rc")

# Declare lib files to link for the DLL
target_link_libraries(
	PluginShim
	"${RainmeterPluginSdkDir}/Rainmeter.lib"
	"${NetCoreNativeRuntimePackDir}/nethost.lib"
)

set_target_properties(PluginShim PROPERTIES OUTPUT_NAME ${PLUGIN_NAME})
set_property(TARGET PluginShim PROPERTY CXX_STANDARD 20)

install(TARGETS PluginShim DESTINATION "PluginShim")
install(FILES "${NetCoreNativeRuntimePackDir}/nethost.dll" DESTINATION "PluginShim")
