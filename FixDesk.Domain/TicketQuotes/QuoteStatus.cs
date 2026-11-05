using System;
using System.Collections.Generic;
using System.Text;

namespace FixDesk.Domain.TicketQuotes
{
    public enum QuoteStatus
    {
        Pending,
        Approved,
        Rejected,
        Superseded
    }
}