using Soenneker.Python.Utils.File.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Python.Utils.File.Tests;

[Collection("Collection")]
public class PythonFileUtilTests : FixturedUnitTest
{
    private readonly IPythonFileUtil _util;

    public PythonFileUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IPythonFileUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
