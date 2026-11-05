namespace FixDesk.Application.Settings.Dtos
{
    // Single-row settings view. Amounts stay in cents (INTEGER financial rule).
    public record ShopSettingsDto(
        string ShopName,
        string? PhoneNumber,
        string? Address,
        bool InspectionFeeEnabled,
        long? DefaultInspectionFeeInCents,
        DateTime UpdatedAtUtc);
}
