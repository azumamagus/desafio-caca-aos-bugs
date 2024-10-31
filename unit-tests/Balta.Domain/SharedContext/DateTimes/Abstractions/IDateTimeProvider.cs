namespace Balta.Domain.SharedContext.DateTimes.Abstractions;

public interface IDateTimeProvider
{
    #region Properties

    DateTime UtcNow { get; }

    #endregion
}