using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper;

public class FeeCalculator : IFeeCalculator
{
    public (decimal fee, string notes) CalculateSupportFee(string planCode, bool includePremiumSupport)
    {
        if (!includePremiumSupport)
        {
            return (0m, string.Empty);
        }

        if (planCode == "START")
        {
            return (250m, "premium support included; ");
        }
        else if (planCode == "PRO")
        {
            return (400m, "premium support included; ");
        }
        else if (planCode == "ENTERPRISE")
        {
            return (700m, "premium support included; ");
        }

        return (0m, string.Empty);
    }

    public (decimal fee, string notes) CalculatePaymentFee(string paymentMethod, decimal chargeableAmount)
    {
        if (paymentMethod == "CARD")
        {
            return (chargeableAmount * 0.02m, "card payment fee; ");
        }
        else if (paymentMethod == "BANK_TRANSFER")
        {
            return (chargeableAmount * 0.01m, "bank transfer fee; ");
        }
        else if (paymentMethod == "PAYPAL")
        {
            return (chargeableAmount * 0.035m, "paypal fee; ");
        }
        else if (paymentMethod == "INVOICE")
        {
            return (0m, "invoice payment; ");
        }

        throw new ArgumentException("Unsupported payment method");
    }
}
