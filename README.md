[![](https://img.shields.io/nuget/v/soenneker.python.utils.file.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.python.utils.file/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.python.utils.file/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.python.utils.file/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.python.utils.file.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.python.utils.file/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.python.utils.file/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.python.utils.file/actions/workflows/codeql.yml)

# Soenneker.Python.Utils.File

Python file operations via .NET.

## Install

```bash
dotnet add package Soenneker.Python.Utils.File
```

## Quick start

```csharp
using Soenneker.Python.Utils.File.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddPythonFileUtilAsSingleton();
```

Adds `IPythonFileUtil` as a singleton service.

## What you get

- `IPythonFileUtil` — Python file operations via .NET.
- `PythonFileUtilRegistrar` — Python file operations via .NET.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IPythonFileUtil.ConvertRelativeImports(directory, cancellationToken)` | Converts all relative imports to absolute imports in Python scripts within the specified directory. | A task that completes when the convert relative imports operation is complete. |
| `PythonFileUtilRegistrar.AddPythonFileUtilAsSingleton(services)` | Adds `IPythonFileUtil` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `PythonFileUtilRegistrar.AddPythonFileUtilAsScoped(services)` | Adds `IPythonFileUtil` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
