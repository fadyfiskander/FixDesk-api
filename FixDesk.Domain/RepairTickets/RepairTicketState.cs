namespace FixDesk.Domain.RepairTickets
{
    public enum RepairTicketState
    {
        Received = 100,
        AwaitingApproval = 150,
        InRepair = 200,
        Ready = 300,
        Delivered = 400,

        Rejected = 170,
        Cancelled = 120
    }
}