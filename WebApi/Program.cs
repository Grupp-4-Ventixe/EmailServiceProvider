using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using WebApi.Models;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

//1:55:32 publicerar till azure

builder.Services.Configure<AzureCommunicationSettings>(builder.Configuration.GetSection("AzureCommunicationSettings"));
builder.Services.Configure<AzureServiceBusSettings>(builder.Configuration.GetSection("AzureServiceBusSettings"));
builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS:ConnectionString"]));
builder.Services.AddSingleton(x => new ServiceBusClient(builder.Configuration["ASB:ConnectionString"]));

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IQueueService, QueueService>();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var queueService = app.Services.GetRequiredService<IQueueService>();
await queueService.StartAsync();

app.Lifetime.ApplicationStopping.Register(async () =>
{
    await queueService.StopAsync(); //Clean shutdown
});


app.Run();
