using Infrastructure.Interfaces;

namespace Infrastructure.Commons;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime()
    {
        return DateTime.UtcNow.ToUniversalTime();
    }
}