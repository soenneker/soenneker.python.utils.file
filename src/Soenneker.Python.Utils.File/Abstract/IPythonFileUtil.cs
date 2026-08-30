using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Python.Utils.File.Abstract;

/// <summary>
/// Rewrites safe, single-dot relative imports in Python packages.
/// </summary>
public interface IPythonFileUtil
{
    /// <summary>
    /// Converts single-line, single-dot relative imports to package-qualified imports in Python package files beneath the directory.
    /// </summary>
    /// <param name="directory">Directory to read from or write to.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after every eligible file has been updated.</returns>
    ValueTask ConvertRelativeImports(string directory, CancellationToken cancellationToken = default);
}
