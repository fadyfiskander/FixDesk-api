using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Customers
{
    public readonly record struct CustomerId(Guid Value)
    {
        public static CustomerId New() => new(Guid.NewGuid());

        public static CustomerId FromGuid(Guid value)
        {
            if(value == Guid.Empty)
                throw new ArgumentException("CustomerId cannot be empty.", nameof(value));
            return new(value);
        }

        public static CustomerId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("CustomerId cannot be empty.", nameof(value));
            if(!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("CustomerId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }
}