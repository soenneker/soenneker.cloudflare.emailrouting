[![](https://img.shields.io/nuget/v/soenneker.cloudflare.emailrouting.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.emailrouting/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.emailrouting/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.emailrouting/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.emailrouting.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.emailrouting/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.emailrouting/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.emailrouting/actions/workflows/codeql.yml)

# Soenneker.Cloudflare.EmailRouting

Manages Cloudflare Email Routing destination addresses, forwarding rules, and zone DNS setup.

## Installation

```bash
dotnet add package Soenneker.Cloudflare.EmailRouting
```

## Configuration

```json
{
  "Cloudflare": {
    "ApiKey": "your-api-token"
  }
}
```

Use a scoped API token with the account- and zone-level Email Routing permissions required by the operations your application performs.

## Registration

```csharp
using Soenneker.Cloudflare.EmailRouting.Registrars;

services.AddCloudflareEmailRoutingUtilAsScoped();
```

The scoped utility shares the singleton Cloudflare client utility. Singleton registration is also available.

## Enabling routing and creating an address

```csharp
using Soenneker.Cloudflare.EmailRouting.Abstract;

bool enabled = await emailRouting.SetupEmailRoutingDns(zoneId, cancellationToken);

if (!enabled)
    throw new InvalidOperationException("Cloudflare did not enable Email Routing.");

await emailRouting.CreateCustomAddressWithEmail(
    accountId,
    zoneId,
    "support@example.com",
    "team@example.net",
    cancellationToken);
```

`CreateCustomAddressWithEmail` searches all destination-address pages, creates the destination when it does not exist, and then creates an enabled literal forwarding rule. Cloudflare may require the destination owner to complete verification before mail can be delivered.

Use `CreateCustomAddress` when the destination address has already been added. The forwarding action uses the destination email address; removal methods require Cloudflare's destination-address ID or routing-rule ID, not an email address.

## Listing and removal

```csharp
EmailDestinationAddressesResponseCollection? destinations =
    await emailRouting.ListDestinationAddresses(accountId, cancellationToken);

EmailRulesResponseCollection? rules =
    await emailRouting.ListRoutingRules(zoneId, cancellationToken);
```

Destination listing aggregates every Cloudflare page. Routing-rule listing returns the generated API response directly and may be paginated according to Cloudflare's endpoint defaults.

`DisableEmailRouting` changes zone-level routing/DNS state and should only be used when mail delivery for the zone is intentionally being disabled. The setup/disable boolean methods log failures and return `false`; caller-requested cancellation is propagated. Other API methods return generated nullable responses and allow Kiota/API exceptions to propagate.
