using FixDesk.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.Brands
{
    public readonly record struct BrandId(Guid Value)
    {
        public static BrandId New() => new(Guid.NewGuid());

        public static BrandId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("BrandId cannot be empty.", nameof(value));
            return new(value);
        }

        public static BrandId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("BrandId cannot be empty.", nameof(value));
            if (!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("BrandId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }
}