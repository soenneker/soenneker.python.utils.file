using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Python.Utils.File.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class PythonFileUtilTests : HostedUnitTest
{
    private readonly IPythonFileUtil _util;

    public PythonFileUtilTests(Host host) : base(host)
    {
        _util = Resolve<IPythonFileUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
