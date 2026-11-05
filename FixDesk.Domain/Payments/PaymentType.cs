namespace FixDesk.Domain.Payments
{
    public enum PaymentType
    {
        // دفعة من العميل
        Payment = 0,

        // قيد عكسي تصحيحي / رد فلوس للعميل
        Reversal = 1,
    }
}
