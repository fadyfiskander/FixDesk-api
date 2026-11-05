using FixDesk.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Categories
{
    public readonly record struct CategoryId(Guid Value)
    {
        public static CategoryId New() => new(Guid.NewGuid());

        public static CategoryId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("CategoryId cannot be empty.", nameof(value));
            return new(value);
        }

        public static CategoryId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("CategoryId cannot be empty.", nameof(value));
            if (!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("CategoryId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }
}