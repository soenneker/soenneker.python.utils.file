using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Python.Utils.File.Abstract;

/// <summary>
/// Python file operations via .NET
/// </summary>
public interface IPythonFileUtil
{
    /// <summary>
    /// Converts all relative imports to absolute imports in Python scripts within the specified directory.
    /// </summary>
    /// <param name="directory">Directory to read from or write to.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the convert relative imports operation is complete.</returns>
    ValueTask ConvertRelativeImports(string directory, CancellationToken cancellationToken = default);
}
