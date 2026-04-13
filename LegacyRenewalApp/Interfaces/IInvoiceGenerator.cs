using LegacyRenewalApp;

namespace LegacyRenewalApp.Interfaces;

public interface IInvoiceGenerator
{
    RenewalInvoice CreateRenewalInvoice(
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
        string notes);
}
