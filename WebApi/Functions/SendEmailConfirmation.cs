
using Azure.Messaging.ServiceBus;
using Google.Protobuf.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Functions;

public class SendEmailConfirmation(ISendEmailConfirmationLinkService sendConfirmationService, IConfiguration config)
{
    private readonly ISendEmailConfirmationLinkService _sendConfirmationService = sendConfirmationService;
    private readonly IConfiguration _config = config;
    private readonly ILogger<SendEmailConfirmation> _logger;

    [Function("SendEmailConfirmation")]
    public async Task Run(
        [ServiceBusTrigger("validate-email-queue", Connection = "ACS_ConnectionString")] 
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body.ToString());
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        await messageActions.CompleteMessageAsync(message);
        var deserializedMsg = message.Body.ToObjectFromJson<ValidateEmailMessage>();
        if (deserializedMsg is not null)
        {
            _sendConfirmationService.SendConfirmationLink(deserializedMsg.Email, deserializedMsg.Token);
        }
    }
}
