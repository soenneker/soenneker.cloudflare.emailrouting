using Microsoft.Extensions.Logging;
using Soenneker.Cloudflare.EmailRouting.Abstract;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.OpenApiClient;

namespace Soenneker.Cloudflare.EmailRouting;

///<inheritdoc cref="ICloudflareEmailRoutingUtil"/>
public sealed class CloudflareEmailRoutingUtil : ICloudflareEmailRoutingUtil
{
    private readonly ICloudflareClientUtil _clientUtil;
    private readonly ILogger<CloudflareEmailRoutingUtil> _logger;

    public CloudflareEmailRoutingUtil(ICloudflareClientUtil clientUtil, ILogger<CloudflareEmailRoutingUtil> logger)
    {
        _clientUtil = clientUtil;
        _logger = logger;
    }

    public async ValueTask<string?> GetDestinationAddressIdByEmail(string accountIdentifier, string email, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting destination address ID for email '{Email}' on account '{Account}'", email, accountIdentifier);
        Email_destination_addresses_response_collection? addresses = await ListDestinationAddresses(accountIdentifier, cancellationToken).NoSync();
        Email_addresses? address = addresses?.Result?.FirstOrDefault(a => a.Email?.Value?.Equals(email, StringComparison.OrdinalIgnoreCase) == true);
        return address?.Id?.ToString();
    }

    public async ValueTask<Email_destination_address_response_single> AddDestinationAddress(string accountIdentifier, string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding destination address '{Email}' to account '{Account}'", email, accountIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_create_destination_address_properties
        {
            Email = new Email_email { Value = email }
        };
        return await client.Accounts[accountIdentifier].Email.Routing.Addresses.PostAsync(body, null, cancellationToken).NoSync();
    }

    public async ValueTask<Email_destination_address_response_single> RemoveDestinationAddress(string accountIdentifier, string destinationAddressId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing destination address ID '{Id}' from account '{Account}'", destinationAddressId, accountIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_routing_destination_addresses_delete_destination_address_RequestBody_application_json();
        return await client.Accounts[accountIdentifier].Email.Routing.Addresses[destinationAddressId].DeleteAsync(body, null, cancellationToken).NoSync();
    }

    public async ValueTask<Email_destination_addresses_response_collection> ListDestinationAddresses(string accountIdentifier, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing destination addresses for account '{Account}'", accountIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_routing_destination_addresses_list_destination_addresses_RequestBody_application_json();
        return await client.Accounts[accountIdentifier].Email.Routing.Addresses.GetAsync(body, null, cancellationToken).NoSync();
    }

    public async ValueTask<Email_rule_response_single> CreateCustomAddressWithEmail(string accountIdentifier, string zoneIdentifier, string customEmail, string destinationEmail, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating custom address '{Custom}' -> '{Dest}' in zone '{Zone}'", customEmail, destinationEmail, zoneIdentifier);
        string? destinationAddressId = await GetDestinationAddressIdByEmail(accountIdentifier, destinationEmail, cancellationToken).NoSync();

        if (destinationAddressId == null)
        {
            _logger.LogDebug("Destination email '{Dest}' not found, creating it...", destinationEmail);
            Email_destination_address_response_single newDestination = await AddDestinationAddress(accountIdentifier, destinationEmail, cancellationToken).NoSync();
            destinationAddressId = newDestination.Result?.Id?.ToString();

            if (destinationAddressId == null)
            {
                _logger.LogError("Failed to create destination address for '{Dest}'", destinationEmail);
                throw new InvalidOperationException($"Failed to create destination address for {destinationEmail}");
            }
        }

        return await CreateCustomAddress(zoneIdentifier, customEmail, destinationAddressId, cancellationToken).NoSync();
    }

    public async ValueTask<Email_rule_response_single> CreateCustomAddress(string zoneIdentifier, string customEmail, string destinationAddressId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating custom routing rule for '{Custom}' to destination ID '{Dest}' in zone '{Zone}'", customEmail, destinationAddressId, zoneIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_create_rule_properties
        {
            Name = new Email_rule_name { Value = customEmail },
            Enabled = new Email_rule_enabled { Value = true },
            Actions =
            [
                new Email_rule_action
                {
                    Type = Email_rule_action_type.Forward,
                    Value = [destinationAddressId]
                }
            ],
            Matchers =
            [
                new Email_rule_matcher
                {
                    Type = Email_rule_matcher_type.Literal,
                    Field = Email_rule_matcher_field.To,
                    Value = customEmail
                }
            ]
        };
        return await client.Zones[zoneIdentifier].Email.Routing.Rules.PostAsync(body, null, cancellationToken).NoSync();
    }

    public async ValueTask<Email_rule_response_single> RemoveCustomAddress(string zoneIdentifier, string ruleId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing routing rule ID '{RuleId}' from zone '{Zone}'", ruleId, zoneIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_routing_routing_rules_delete_routing_rule_RequestBody_application_json();
        return await client.Zones[zoneIdentifier].Email.Routing.Rules[ruleId].DeleteAsync(body, null, cancellationToken).NoSync();
    }

    public async ValueTask<Email_rules_response_collection> ListRoutingRules(string zoneIdentifier, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing routing rules for zone '{Zone}'", zoneIdentifier);
        CloudflareOpenApiClient client = await _clientUtil.Get(cancellationToken).NoSync();
        var body = new Email_routing_routing_rules_list_routing_rules_RequestBody_application_json();
        return await client.Zones[zoneIdentifier].Email.Routing.Rules.GetAsync(body, null, cancellationToken).NoSync();
    }
}
