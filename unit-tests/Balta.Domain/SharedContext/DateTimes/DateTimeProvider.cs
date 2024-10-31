using Balta.Domain.SharedContext.DateTimes.Abstractions;

namespace Balta.Domain.SharedContext.DateTimes
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
