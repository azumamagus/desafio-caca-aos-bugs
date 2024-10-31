using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.DateTimes;
using Balta.Domain.SharedContext.DateTimes.Abstractions;

namespace Balta.Domain.AccountContext.ValueObjects;

public class VerificationCode
{
    #region Constants

    private const int MinLength = 6;

    #endregion

    #region Fields

    private readonly IDateTimeProvider _dateTimeProvider;

    #endregion

    #region Constructors

    private VerificationCode(string code, IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        Code = Guid.NewGuid().ToString("N")[..MinLength].ToUpper();
        ExpiresAtUtc = _dateTimeProvider.UtcNow.AddMinutes(5);
        VerifiedAtUtc = null;
    }

    #endregion

    #region Factories

    public static VerificationCode ShouldCreate(IDateTimeProvider? dateTimeProvider)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);
        return new(
            Guid.NewGuid().ToString("N")[..MinLength].ToUpper(),
            dateTimeProvider);
    }

    #endregion

    #region Properties

    public string Code { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public DateTime? VerifiedAtUtc { get; private set; }
    public bool IsActive => VerifiedAtUtc is null && ExpiresAtUtc is not null;
    public bool IsExpired => ExpiresAtUtc < _dateTimeProvider.UtcNow;

    #endregion

    #region Methods

    public void ShouldVerify(string code)
    {
        if (IsExpired == true)
            throw new InvalidVerificationCodeException();

        if (IsActive == false)
            throw new InvalidVerificationCodeException();

        if (string.IsNullOrEmpty(code) || Code != code)
            throw new InvalidVerificationCodeException();

        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidVerificationCodeException();

        if (code.Length != MinLength)
            throw new InvalidVerificationCodeException();

        VerifiedAtUtc = _dateTimeProvider.UtcNow;
        ExpiresAtUtc = null;
    }

    public void SetExpirationDate(DateTime? expirationDate)
    {
        ExpiresAtUtc = expirationDate;
    }

    public void SetVerifiedAt(DateTime? verificationDate)
    {
        VerifiedAtUtc = verificationDate;
    }

    public void SetCode(string code)
    {
        Code = code;
    }

    #endregion

    #region Operators

    public static implicit operator string(VerificationCode verificationCode) => verificationCode.ToString();
    
    #endregion

    #region Others

    public override string ToString() => Code;

    #endregion
}