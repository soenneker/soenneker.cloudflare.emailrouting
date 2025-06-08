using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient.Models;

namespace Soenneker.Cloudflare.EmailRouting.Abstract;

/// <summary>
/// Abstraction for managing Cloudflare email routing and addresses.
/// </summary>
public interface ICloudflareEmailRoutingUtil
{
    /// <summary>
    /// Adds a new destination address for email routing.
    /// </summary>
    ValueTask<Email_destination_address_response_single> AddDestinationAddress(string accountIdentifier, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a destination address from email routing.
    /// </summary>
    ValueTask<Email_destination_address_response_single> RemoveDestinationAddress(string accountIdentifier, string destinationAddressId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all destination addresses.
    /// </summary>
    ValueTask<Email_destination_addresses_response_collection> ListDestinationAddresses(string accountIdentifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a custom email address with routing rules.
    /// </summary>
    ValueTask<Email_rule_response_single> CreateCustomAddress(string zoneIdentifier, string customEmail, string destinationEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a custom email address and its routing rules.
    /// </summary>
    ValueTask<Email_rule_response_single> RemoveCustomAddress(string zoneIdentifier, string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all routing rules.
    /// </summary>
    ValueTask<Email_rules_response_collection> ListRoutingRules(string zoneIdentifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a destination address ID by matching a given email address.
    /// </summary>
    /// <param name="accountIdentifier">Cloudflare account identifier.</param>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The destination address ID if found; otherwise null.</returns>
    ValueTask<string?> GetDestinationAddressIdByEmail(string accountIdentifier, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a custom email routing rule using a destination email. If the destination doesn't exist, it will be created.
    /// </summary>
    /// <param name="accountIdentifier">Cloudflare account identifier.</param>
    /// <param name="zoneIdentifier">Cloudflare zone identifier.</param>
    /// <param name="customEmail">The custom email address to create.</param>
    /// <param name="destinationEmail">The destination email to route to.</param>
    /// <returns>The response from the rule creation.</returns>
    ValueTask<Email_rule_response_single> CreateCustomAddressWithEmail(string accountIdentifier, string zoneIdentifier, string customEmail, string destinationEmail, CancellationToken cancellationToken = default);
}
