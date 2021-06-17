cmake_minimum_required (VERSION 2.6)
project (NativeHost)
set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_definitions(-D_AFXDLL)
set(CMAKE_MFC_FLAG 1)

add_library(NativeHost SHARED
    NativeHost.cpp
    pch.cpp
)
install(TARGETS NativeHost)
