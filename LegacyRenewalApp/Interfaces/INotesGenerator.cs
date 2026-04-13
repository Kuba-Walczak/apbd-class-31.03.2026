using LegacyRenewalApp;

namespace LegacyRenewalApp.Interfaces;

public interface INotesGenerator
{
    string FormatDiscountNotes(Customer customer, int seatCount, SubscriptionPlan plan, bool useLoyaltyPoints);
    string FormatSupportFeeNote(string planCode, bool includePremiumSupport);
    string FormatPaymentFeeNote(string paymentMethod);
    string FormatMinimumSubtotalNote(bool minimumSubtotalApplied);
    string FormatMinimumInvoiceNote(bool minimumInvoiceApplied);
}
