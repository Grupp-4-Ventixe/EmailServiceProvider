using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using WebApi.Models;

namespace WebApi.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailSendRequest request);
}

public class EmailService : IEmailService
{
    private readonly AzureCommunicationSettings _settings;
    private readonly EmailClient _client;

    public EmailService(IOptions<AzureCommunicationSettings> options)
    {
        _settings = options.Value;
        _client = new EmailClient(_settings.ConnectionString);
    }

    public async Task<bool> SendEmailAsync(EmailSendRequest request)
    {

        var recipients = new List<EmailAddress>();

        foreach (var recipient in request.Recipients)
            recipients.Add(new EmailAddress(recipient));

        var emailMessage = new EmailMessage(
        senderAddress: _settings.SenderAddress,
        content: new EmailContent(request.Subject)
        {
            PlainText = request.PlainText,
            Html = request.Html
        },
        recipients: new EmailRecipients(recipients));

        var result = await _client.SendAsync(Azure.WaitUntil.Completed, emailMessage);
        return result.HasCompleted;

    }

}

