using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.TicketQuotes
{
    public readonly record struct TicketQuoteId(Guid Value)
    {
        public static TicketQuoteId New() => new(Guid.NewGuid());

        public static TicketQuoteId FromGuid(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("TicketQuoteId cannot be empty.", nameof(value));
            return new(value);
        }

        public static TicketQuoteId FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("TicketQuoteId cannot be empty.", nameof(value));
            if (!Guid.TryParse(value, out Guid guidValue))
                throw new ArgumentException("TicketQuoteId must be a valid GUID.", nameof(value));
            return new(guidValue);
        }

        public override string ToString() => Value.ToString();
    }

}
