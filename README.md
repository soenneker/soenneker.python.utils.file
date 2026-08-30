[![](https://img.shields.io/nuget/v/soenneker.python.utils.file.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.python.utils.file/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.python.utils.file/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.python.utils.file/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.python.utils.file/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.python.utils.file/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.python.utils.file.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.python.utils.file/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.python.utils.file/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.python.utils.file/actions/workflows/codeql.yml)

# Soenneker.Python.Utils.File

Rewrites safe single-dot relative imports in a Python package to absolute, package-qualified imports.

## Installation

```bash
dotnet add package Soenneker.Python.Utils.File
```

## Registration and usage

```csharp
using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Python.Utils.File.Registrars;

services.AddPythonFileUtilAsScoped();

IPythonFileUtil pythonFiles =
    serviceProvider.GetRequiredService<IPythonFileUtil>();

await pythonFiles.ConvertRelativeImports(
    @"C:\src\my_package",
    cancellationToken);
```

The root must contain `__init__.py`, and its directory name must be a valid Python identifier.

## Rewrites

For `my_package/module.py`:

```python
from .helpers import parse
# becomes: from my_package.helpers import parse
```

For `my_package/features/module.py`, when `features/__init__.py` exists:

```python
from .helpers import parse
# becomes: from my_package.features.helpers import parse
```

Files beneath directories without `__init__.py` are skipped because they are not part of the package chain.

The converter intentionally leaves these unchanged:

- Parent-relative imports such as `from ..common import value`.
- Multiline imports and imports using line continuations.
- Ordinary absolute imports.

The operation edits every eligible `.py` file beneath the package. Commit or back up source files first. Cancellation and file errors propagate; a failed run may have updated earlier files, so review the working tree before retrying.
