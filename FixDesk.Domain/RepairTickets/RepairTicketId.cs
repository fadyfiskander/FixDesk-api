using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.RepairTickets
{
    public readonly record struct RepairTicketId(Guid Value)
    {
        public static RepairTicketId New() => new(Guid.NewGuid());

        public static RepairTicketId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("RepairTicketId cannot be empty.", nameof(value));
            return new(value);
        }

        public static RepairTicketId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("RepairTicketId cannot be empty.", nameof(value));
            if (!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("RepairTicketId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }

}
