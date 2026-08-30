using Microsoft.Extensions.Logging;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Utils.Directory.Abstract;
using Soenneker.Utils.File.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Python.Utils.File;

public sealed class PythonFileUtil : IPythonFileUtil
{
    private readonly ILogger<PythonFileUtil> _logger;
    private readonly IFileUtil _fileUtil;
    private readonly IDirectoryUtil _directoryUtil;

    // Matches single-line "from .foo import bar" and "from . import bar"
    // Captures: indent, dots, module(optional), imported(rest), comment(optional)
    private static readonly Regex _fromImportRegex = new(
        @"^(?<indent>\s*)from\s+(?<dots>\.+)(?<module>[A-Za-z_][A-Za-z0-9_\.]*)?\s+import\s+(?<imported>.*?)(?<comment>\s*#.*)?\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex _pythonIdentifierRegex = new(
        "^[A-Za-z_][A-Za-z0-9_]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public PythonFileUtil(ILogger<PythonFileUtil> logger, IFileUtil fileUtil, IDirectoryUtil directoryUtil)
    {
        _logger = logger;
        _fileUtil = fileUtil;
        _directoryUtil = directoryUtil;
    }

    public async ValueTask ConvertRelativeImports(string directory, CancellationToken cancellationToken = default)
    {
        if (!(await _directoryUtil.Exists(directory, cancellationToken)))
        {
            _logger.LogError("Directory {Directory} does not exist.", directory);
            throw new DirectoryNotFoundException($"Directory {directory} does not exist.");
        }

        // Only operate on actual package folders
        string initPath = Path.Combine(directory, "__init__.py");

        if (!await _fileUtil.Exists(initPath, cancellationToken).NoSync())
        {
            _logger.LogWarning("Skipping {Directory} (no __init__.py found).", directory);
            return;
        }

        string root = Path.GetFullPath(directory);
        string rootPackageName = new DirectoryInfo(root).Name;

        if (!_pythonIdentifierRegex.IsMatch(rootPackageName))
            throw new InvalidOperationException($"Package directory '{rootPackageName}' is not a valid Python identifier.");

        List<string> pythonFiles = await _directoryUtil.GetFilesByExtension(root, "py", true, cancellationToken);

        foreach (string scriptPath in pythonFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string? packageName = ResolveContainingPackage(root, rootPackageName, scriptPath);
            if (packageName is null)
            {
                _logger.LogDebug("Skipping {ScriptPath} because it is not inside a Python package.", scriptPath);
                continue;
            }

            List<string> lines = await _fileUtil.ReadAsLines(scriptPath, true, cancellationToken).NoSync();
            var modified = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string originalLine = lines[i];

                if (!TryRewriteRelativeImportLine(originalLine, packageName, out string rewritten) || rewritten.Equals(originalLine, StringComparison.Ordinal))
                    continue;

                lines[i] = rewritten;
                modified = true;
            }

            if (modified)
                await _fileUtil.WriteAllLines(scriptPath, lines, true, cancellationToken).NoSync();
        }
    }

    private static string? ResolveContainingPackage(string root, string rootPackageName, string scriptPath)
    {
        string fullScriptPath = Path.GetFullPath(scriptPath);
        string? scriptDirectory = Path.GetDirectoryName(fullScriptPath);

        if (scriptDirectory is null)
            throw new InvalidOperationException($"Could not determine the containing directory for '{scriptPath}'.");

        string relativeDirectory = Path.GetRelativePath(root, scriptDirectory);

        if (Path.IsPathRooted(relativeDirectory) || relativeDirectory.StartsWith("..", StringComparison.Ordinal))
            throw new InvalidOperationException($"Refusing to modify a Python file outside '{root}'.");

        if (relativeDirectory == ".")
            return rootPackageName;

        string[] segments = relativeDirectory.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
        string currentDirectory = root;
        var packageName = rootPackageName;

        foreach (string segment in segments)
        {
            if (!_pythonIdentifierRegex.IsMatch(segment))
                return null;

            currentDirectory = Path.Combine(currentDirectory, segment);
            if (!System.IO.File.Exists(Path.Combine(currentDirectory, "__init__.py")))
                return null;

            packageName += $".{segment}";
        }

        return packageName;
    }

    private static bool TryRewriteRelativeImportLine(string line, string packageName, out string rewritten)
    {
        rewritten = line;

        // Skip obvious multi-line/continuation patterns (don’t risk corrupting them)
        string trimmed = line.TrimStart();
        if (!trimmed.StartsWith("from .", StringComparison.Ordinal))
            return false;

        if (line.Contains("\\") || line.Contains("import (") || trimmed.EndsWith("(", StringComparison.Ordinal))
            return false;

        var m = _fromImportRegex.Match(line);
        if (!m.Success)
            return false;

        string indent = m.Groups["indent"].Value;
        string dots = m.Groups["dots"].Value;          // ".", "..", "..."
        string module = m.Groups["module"].Value;      // may be empty for "from . import X"
        string imported = m.Groups["imported"].Value.TrimEnd();
        string comment = m.Groups["comment"].Success ? m.Groups["comment"].Value : "";

        if (imported.IsNullOrEmpty())
            return false;

        // We can safely rewrite ONLY single-dot relative imports.
        // Multi-dot relative imports depend on parent package context and are not safe to auto-flatten.
        if (dots.Length != 1)
            return false;

        // Preserve any trailing comment already captured in "comment" group.
        // Remove comment from imported payload if regex captured it as part of imported (paranoia)
        // (regex is non-greedy for imported, but keep it defensive)
        if (comment.Length > 0 && imported.EndsWith(comment, StringComparison.Ordinal))
            imported = imported[..^comment.Length].TrimEnd();

        // from . import X        -> from <pkg> import X
        // from .foo import X     -> from <pkg>.foo import X
        string fullModule = module.IsNullOrEmpty()
            ? packageName
            : $"{packageName}.{module}";

        rewritten = $"{indent}from {fullModule} import {imported}{comment}";
        return true;
    }
}
