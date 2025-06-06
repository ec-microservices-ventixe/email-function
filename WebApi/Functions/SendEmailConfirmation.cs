
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Functions;

public class SendEmailConfirmation(ISendEmailConfirmationLinkService sendConfirmationService, IConfiguration config)
{
    private readonly ISendEmailConfirmationLinkService _sendConfirmationService = sendConfirmationService;
    private readonly IConfiguration _config = config;

    [Function("SendEmailConfirmation")]
    public void Run(
        [ServiceBusTrigger("validate-email-queue", Connection = "ACS_ConnectionString")] 
        ServiceBusReceivedMessage message)
    {
        var deserializedMsg = message.Body.ToObjectFromJson<ValidateEmailMessage>();
        if (deserializedMsg is not null)
        {
            _sendConfirmationService.SendConfirmationLink(deserializedMsg.Email, deserializedMsg.Token);
        }
    }
}
