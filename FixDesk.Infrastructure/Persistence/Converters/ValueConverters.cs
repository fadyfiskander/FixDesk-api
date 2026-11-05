using System.Text.Json;
using FixDesk.Domain.RepairTickets;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FixDesk.Infrastructure.Persistence.Converters
{
    public sealed class UtcDateTimeTicksConverter() : ValueConverter<DateTime, long>(
        dt => dt.ToUniversalTime().Ticks,
        ticks => new DateTime(ticks, DateTimeKind.Utc));

    public sealed class NullableUtcDateTimeTicksConverter() : ValueConverter<DateTime?, long?>(
        dt => dt.HasValue ? dt.Value.ToUniversalTime().Ticks : null,
        ticks => ticks.HasValue ? new DateTime(ticks.Value, DateTimeKind.Utc) : null);

    internal sealed record DeviceConditionDto(string PhysicalStatus, string DefectDescription);

    internal static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }

    public sealed class DeviceConditionJsonConverter() : ValueConverter<DeviceCondition, string>(
        c => SerializeCondition(c),
        s => DeserializeCondition(s))
    {
        private static string SerializeCondition(DeviceCondition condition)
            => JsonSerializer.Serialize(
                new DeviceConditionDto(condition.PhysicalStatus, condition.DefectDescription),
                JsonOptions.Default);

        private static DeviceCondition DeserializeCondition(string json)
        {
            var dto = JsonSerializer.Deserialize<DeviceConditionDto>(json, JsonOptions.Default);
            return dto is null
                ? throw new InvalidOperationException("Empty device condition JSON.")
                : DeviceCondition.Create(dto.PhysicalStatus, dto.DefectDescription);
        }
    }

    public sealed class NullableUserIdConverter() : ValueConverter<Domain.Identity.UserId?, Guid?>(
        id => id.HasValue ? id.Value.Value : null,
        value => value.HasValue ? new Domain.Identity.UserId(value.Value) : null);

    public sealed class DeviceAttachmentsJsonConverter() : ValueConverter<IReadOnlyCollection<DeviceAttachment>, string>(
        list => JsonSerializer.Serialize(list, JsonOptions.Default),
        s => JsonSerializer.Deserialize<List<DeviceAttachment>>(s, JsonOptions.Default)
            ?? new List<DeviceAttachment>());

    public sealed class DeviceConditionComparer() : ValueComparer<DeviceCondition>(
        (l, r) => (l == null && r == null)
            || (l != null && r != null
                && l.PhysicalStatus == r.PhysicalStatus
                && l.DefectDescription == r.DefectDescription),
        v => v == null ? 0 : HashCode.Combine(v.PhysicalStatus, v.DefectDescription),
        v => v);

    public sealed class DeviceAttachmentsComparer() : ValueComparer<IReadOnlyCollection<DeviceAttachment>>(
        (l, r) => SerializeAttachments(l) == SerializeAttachments(r),
        v => SerializeAttachments(v).GetHashCode(),
        v => v)
    {
        private static string SerializeAttachments(IReadOnlyCollection<DeviceAttachment>? list)
            => JsonSerializer.Serialize(list, JsonOptions.Default);
    }
}
