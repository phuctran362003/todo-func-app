using System.Security.Claims;
using Infrastructure.Interfaces;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Commons;

public class ClaimsService : IClaimsService
{
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // Lấy ClaimsIdentity
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

        var extractedId = AuthenTools.GetCurrentUserId(identity);
        if (Guid.TryParse(extractedId, out var parsedId))
            CurrentUserId = parsedId;
        else
            CurrentUserId = Guid.Empty;

        IpAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public Guid CurrentUserId { get; }

    public string? IpAddress { get; }
}