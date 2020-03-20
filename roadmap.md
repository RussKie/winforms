# WinForms on .NET Roadmap

This roadmap communicates priorities for evolving and extending the scope of WinForms for .NET.

In .NET Core 3.0 and 3.1 our primary focus was to enable the following:

* Achieve WinForms functional and performance parity compared to .NET Framework.
* Publish remaining WinForms components to the repository.
* Publish (and write) more WinForms tests to the repository.

> Note: There are some specific .NET Framework features will not be supported, such as hosting WinForms controls in Internet Explorer.


In .NET 5 and beyond our vision for Windows Forms is to provide a productive way to build Windows-native applications by exposing underlying Win32 API and native capabilities via lightweight managed wrappers, and to serve as an extensibility platform for developers to create their own rich controls.

For general information regarding .NET plans, see [.NET roadmap][core-roadmap].

## Timelines

| Milestone                                         | Date              |
|---                                                |---                |
|Initial launch of WinForms on .NET Core repository |Dec 4, 2018        |
|Windows Forms .NET Core 3.0                        |Q3 2019            |
|Windows Forms .NET Core 3.1 LTS                    |Dec 2019            |
|Windows Forms .NET 5                               |Nov 2020            |
|Designer support in Visual Studio                  | Q2 2020  |

If you'd like to contribute to WinForms, please take a look at our [Contributing
Guide](Documentation/contributing.md).

## Feature Backlog

* Add WinForms Designer support for .NET Core/.NET projects in a Visual Studio 2019 update
* Improve accessibility support for some missing UIA interfaces
* Improve performance of WinForms runtime
* Fix existing scaling bugs in Per Monitor DPI aware applications
* Add a new “clean" way of calculating location/size information in PMA mode.
* Make new projects be per monitor aware
* Add Chromium-based Edge browser control

[comment]: <> (URI Links)

[core-roadmap]: https://github.com/dotnet/core/blob/master/roadmap.md
