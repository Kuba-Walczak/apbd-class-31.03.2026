namespace LegacyRenewalApp.Interfaces;

public interface IFeeCalculator
{
    public (decimal fee, string notes) CalculateSupportFee(string planCode, bool includePremiumSupport);
    public (decimal fee, string notes) CalculatePaymentFee(string paymentMethod, decimal chargeableAmount);
}
