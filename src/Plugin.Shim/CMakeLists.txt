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

cmake_minimum_required (VERSION 3.8)

SET(PLUGIN_NAME "TestPlugin" CACHE STRING "Name of the dotnet plugin folder and assembly")
SET(PLUGIN_VERSION "1.0.0.0" CACHE STRING "Version of the plugin")
SET(COPYRIGHT "@ 2023 - whiskycompiler" CACHE STRING "Copyright notice of the plugin")

project ("RainmeterPluginShim")

IF("${TARGET_ARCHITECTURE}" MATCHES "x64")
	file(TO_CMAKE_PATH "$ENV{NetCoreNativeRuntimeX64PackDir}" NetCoreNativeRuntimePackDir)
	file(TO_CMAKE_PATH "$ENV{RainmeterPluginSdkDir}/API/x64" RainmeterPluginSdkDir)
ELSEIF("${TARGET_ARCHITECTURE}" MATCHES "x86")
	file(TO_CMAKE_PATH "$ENV{NetCoreNativeRuntimeX86PackDir}" NetCoreNativeRuntimePackDir)
	file(TO_CMAKE_PATH "$ENV{RainmeterPluginSdkDir}/API/x32" RainmeterPluginSdkDir)
ELSE()
	file(TO_CMAKE_PATH "$ENV{NetCoreNativeRuntimePackDir}" NetCoreNativeRuntimePackDir)
	file(TO_CMAKE_PATH "$ENV{RainmeterPluginSdkDir}/API/x64" RainmeterPluginSdkDir)
ENDIF()

# Include sub-projects
add_subdirectory ("RainmeterPluginShim")

IF(CMAKE_BUILD_TYPE MATCHES Debug)
   add_subdirectory ("TestConsole")
ENDIF()