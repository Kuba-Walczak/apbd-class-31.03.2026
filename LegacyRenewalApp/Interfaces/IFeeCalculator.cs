namespace LegacyRenewalApp.Interfaces;

public interface IFeeCalculator
{
    decimal CalculateSupportFee(string planCode, bool includePremiumSupport);
    decimal CalculatePaymentFee(string paymentMethod, decimal chargeableAmount);
}
