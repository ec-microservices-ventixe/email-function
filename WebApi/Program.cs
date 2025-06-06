using Azure.Communication.Email;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Interfaces;
using WebApi.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS_ConnectionString"]));
builder.Services.AddSingleton<ISendEmailService, SendEmailService>();
builder.Services.AddSingleton<ISendEmailConfirmationLinkService, SendEmailConfirmationLinkService>();

builder.Build().Run();
