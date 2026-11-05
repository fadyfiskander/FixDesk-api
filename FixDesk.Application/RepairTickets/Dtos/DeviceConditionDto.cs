namespace FixDesk.Application.RepairTickets.Dtos
{
    // View of the DeviceCondition value object at reception.
    public record DeviceConditionDto(
        string PhysicalStatus,
        string DefectDescription);
}
