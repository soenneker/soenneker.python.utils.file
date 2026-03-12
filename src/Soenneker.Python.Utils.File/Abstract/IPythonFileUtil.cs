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
    /// <param name="directory">The root directory containing Python scripts.</param>
    /// <param name="cancellationToken"></param>
    ValueTask ConvertRelativeImports(string directory, CancellationToken cancellationToken = default);
}
