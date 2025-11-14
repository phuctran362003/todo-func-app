using Domain.DTOs.EmailDTOs;

namespace Application.Interfaces.Common;

public interface IEmailService
{
    Task SendDatabaseChanges(EmailRequestDto request);
    Task SendDatabaseChangesWithSource(EmailRequestDto request, string functionAppName);
}