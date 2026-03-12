using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Utils.Directory.Registrars;
using Soenneker.Utils.File.Registrars;

namespace Soenneker.Python.Utils.File.Registrars;

/// <summary>
/// Python file operations via .NET
/// </summary>
public static class PythonFileUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IPythonFileUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddPythonFileUtilAsSingleton(this IServiceCollection services)
    {
        services.AddDirectoryUtilAsSingleton().AddFileUtilAsSingleton();
        services.TryAddSingleton<IPythonFileUtil, PythonFileUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IPythonFileUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddPythonFileUtilAsScoped(this IServiceCollection services)
    {
        services.AddDirectoryUtilAsScoped().AddFileUtilAsScoped();
        services.TryAddScoped<IPythonFileUtil, PythonFileUtil>();

        return services;
    }
}
