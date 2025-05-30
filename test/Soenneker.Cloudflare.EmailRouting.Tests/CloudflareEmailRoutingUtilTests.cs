using Soenneker.Cloudflare.EmailRouting.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cloudflare.EmailRouting.Tests;

[Collection("Collection")]
public sealed class CloudflareEmailRoutingUtilTests : FixturedUnitTest
{
    private readonly ICloudflareEmailRoutingUtil _util;

    public CloudflareEmailRoutingUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICloudflareEmailRoutingUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
