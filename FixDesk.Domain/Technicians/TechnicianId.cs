using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Technicians
{
    public readonly record struct TechnicianId(Guid Value)
    {
        public static TechnicianId New() => new(Guid.NewGuid());

        public static TechnicianId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("TechnicianId cannot be empty.", nameof(value));
            return new(value);
        }

        public override string ToString() => Value.ToString();
    }
}
