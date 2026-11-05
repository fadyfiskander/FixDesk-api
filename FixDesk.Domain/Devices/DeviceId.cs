using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Devices
{
    public readonly record struct DeviceId(Guid Value)
    {
        public static DeviceId New() => new(Guid.NewGuid());

        public static DeviceId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("DeviceId cannot be empty.", nameof(value));
            return new(value);
        }

        public static DeviceId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("DeviceId cannot be empty.", nameof(value));
            if (!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("DeviceId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }
}