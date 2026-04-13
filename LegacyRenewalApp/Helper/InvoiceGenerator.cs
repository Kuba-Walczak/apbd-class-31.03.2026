using System;
using LegacyRenewalApp;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper;

public class InvoiceGenerator : IInvoiceGenerator
{
    public RenewalInvoice CreateRenewalInvoice(
        string invoiceNumber,
        string customerName,
        string planCode,
        string paymentMethod,
        int seatCount,
        decimal baseAmount,
        decimal discountAmount,
        decimal supportFee,
        decimal paymentFee,
        decimal taxAmount,
        decimal finalAmount,
        string notes)
    {
        return new RenewalInvoice
        {
            InvoiceNumber = invoiceNumber,
            CustomerName = customerName,
            PlanCode = planCode,
            PaymentMethod = paymentMethod,
            SeatCount = seatCount,
            BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
            DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
            SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
            PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
            TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
            FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
            Notes = notes.Trim(),
            GeneratedAt = DateTime.UtcNow
        };
    }
}
