using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Identity
{
    public readonly record struct UserId(Guid Value)
    {
        public static UserId New() => new(Guid.NewGuid());

        public static UserId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(value));
            return new(value);
        }

        public override string ToString() => Value.ToString();
    }
}
