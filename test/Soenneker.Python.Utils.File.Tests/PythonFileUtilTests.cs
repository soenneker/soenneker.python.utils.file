using System;
using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Python.Utils.File.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class PythonFileUtilTests : HostedUnitTest
{
    private readonly IPythonFileUtil _util;

    public PythonFileUtilTests(Host host) : base(host)
    {
        _util = Resolve<IPythonFileUtil>(true);
    }

    [Test]
    public async Task Nested_packages_use_their_full_package_name()
    {
        string parent = Path.Combine(Path.GetTempPath(), "soenneker-python-file-tests", Guid.NewGuid().ToString("N"));
        string root = Path.Combine(parent, "my_package");
        string featureDirectory = Path.Combine(root, "features");
        string looseDirectory = Path.Combine(root, "scripts");
        string rootModule = Path.Combine(root, "root_module.py");
        string featureModule = Path.Combine(featureDirectory, "feature.py");
        string looseModule = Path.Combine(looseDirectory, "loose.py");

        try
        {
            Directory.CreateDirectory(featureDirectory);
            Directory.CreateDirectory(looseDirectory);
            await System.IO.File.WriteAllTextAsync(Path.Combine(root, "__init__.py"), string.Empty);
            await System.IO.File.WriteAllTextAsync(Path.Combine(featureDirectory, "__init__.py"), string.Empty);
            await System.IO.File.WriteAllTextAsync(rootModule, "from .helpers import parse");
            await System.IO.File.WriteAllTextAsync(featureModule, "from .helpers import parse");
            await System.IO.File.WriteAllTextAsync(looseModule, "from .helpers import parse");

            await _util.ConvertRelativeImports(root);

            (await System.IO.File.ReadAllTextAsync(rootModule)).Should().Contain("from my_package.helpers import parse");
            (await System.IO.File.ReadAllTextAsync(featureModule)).Should().Contain("from my_package.features.helpers import parse");
            (await System.IO.File.ReadAllTextAsync(looseModule)).Should().Contain("from .helpers import parse");
        }
        finally
        {
            if (Directory.Exists(parent))
                Directory.Delete(parent, true);
        }
    }
}
