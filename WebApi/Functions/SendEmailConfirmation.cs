
using Azure.Messaging.ServiceBus;
using Google.Protobuf.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Functions;

public class SendEmailConfirmation(ISendEmailConfirmationLinkService sendConfirmationService, IConfiguration config, ILogger<SendEmailConfirmation> logger)
{
    private readonly ISendEmailConfirmationLinkService _sendConfirmationService = sendConfirmationService;
    private readonly IConfiguration _config = config;
    private readonly ILogger<SendEmailConfirmation> _logger = logger;

    [Function("SendEmailConfirmation")]
    public async Task Run(
        [ServiceBusTrigger("validate-email-queue", Connection = "ASB_ConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {MessageId}", message.MessageId);
        _logger.LogInformation("Message Body: {Body}", message.Body.ToString());

        try
        {
            var body = message.Body.ToString();
            var deserializedMsg = JsonSerializer.Deserialize<ValidateEmailMessage>(body);

            if (deserializedMsg is not null)
            {
                _sendConfirmationService.SendConfirmationLink(deserializedMsg.Email, deserializedMsg.Token);
                _logger.LogInformation("Confirmation link sent to {Email}", deserializedMsg.Email);
            }
            else
            {
                _logger.LogWarning("Failed to deserialize message body.");
            }

            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Service Bus message.");
        }
    }
}
