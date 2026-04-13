using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper;

public class FeeCalculator : IFeeCalculator
{
    public decimal CalculateSupportFee(string planCode, bool includePremiumSupport)
    {
        if (!includePremiumSupport)
        {
            return 0m;
        }

        if (planCode == "START")
        {
            return 250m;
        }
        else if (planCode == "PRO")
        {
            return 400m;
        }
        else if (planCode == "ENTERPRISE")
        {
            return 700m;
        }

        return 0m;
    }

    public decimal CalculatePaymentFee(string paymentMethod, decimal chargeableAmount)
    {
        if (paymentMethod == "CARD")
        {
            return chargeableAmount * 0.02m;
        }
        else if (paymentMethod == "BANK_TRANSFER")
        {
            return chargeableAmount * 0.01m;
        }
        else if (paymentMethod == "PAYPAL")
        {
            return chargeableAmount * 0.035m;
        }
        else if (paymentMethod == "INVOICE")
        {
            return 0m;
        }

        throw new ArgumentException("Unsupported payment method");
    }
}
