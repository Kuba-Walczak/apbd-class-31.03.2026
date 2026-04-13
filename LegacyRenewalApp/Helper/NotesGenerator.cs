using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper;

public class NotesGenerator : INotesGenerator
{
    public string FormatDiscountNotes(Customer customer, int seatCount, SubscriptionPlan plan, bool useLoyaltyPoints)
    {
        string notes = string.Empty;

        if (customer.Segment == "Silver")
        {
            notes += "silver discount; ";
        }
        else if (customer.Segment == "Gold")
        {
            notes += "gold discount; ";
        }
        else if (customer.Segment == "Platinum")
        {
            notes += "platinum discount; ";
        }
        else if (customer.Segment == "Education" && plan.IsEducationEligible)
        {
            notes += "education discount; ";
        }

        if (customer.YearsWithCompany >= 5)
        {
            notes += "long-term loyalty discount; ";
        }
        else if (customer.YearsWithCompany >= 2)
        {
            notes += "basic loyalty discount; ";
        }

        if (seatCount >= 50)
        {
            notes += "large team discount; ";
        }
        else if (seatCount >= 20)
        {
            notes += "medium team discount; ";
        }
        else if (seatCount >= 10)
        {
            notes += "small team discount; ";
        }

        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            notes += $"loyalty points used: {pointsToUse}; ";
        }

        return notes;
    }

    public string FormatSupportFeeNote(string planCode, bool includePremiumSupport)
    {
        if (!includePremiumSupport)
        {
            return string.Empty;
        }

        if (planCode == "START" || planCode == "PRO" || planCode == "ENTERPRISE")
        {
            return "premium support included; ";
        }

        return string.Empty;
    }

    public string FormatPaymentFeeNote(string paymentMethod)
    {
        if (paymentMethod == "CARD")
        {
            return "card payment fee; ";
        }
        else if (paymentMethod == "BANK_TRANSFER")
        {
            return "bank transfer fee; ";
        }
        else if (paymentMethod == "PAYPAL")
        {
            return "paypal fee; ";
        }
        else if (paymentMethod == "INVOICE")
        {
            return "invoice payment; ";
        }

        throw new ArgumentException("Unsupported payment method");
    }

    public string FormatMinimumSubtotalNote(bool minimumSubtotalApplied)
    {
        return minimumSubtotalApplied ? "minimum discounted subtotal applied; " : string.Empty;
    }

    public string FormatMinimumInvoiceNote(bool minimumInvoiceApplied)
    {
        return minimumInvoiceApplied ? "minimum invoice amount applied; " : string.Empty;
    }
}
