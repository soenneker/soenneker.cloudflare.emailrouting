using Soenneker.Cloudflare.EmailRouting.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cloudflare.EmailRouting.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloudflareEmailRoutingUtilTests : HostedUnitTest
{
    private readonly ICloudflareEmailRoutingUtil _util;

    public CloudflareEmailRoutingUtilTests(Host host) : base(host)
    {
        _util = Resolve<ICloudflareEmailRoutingUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
