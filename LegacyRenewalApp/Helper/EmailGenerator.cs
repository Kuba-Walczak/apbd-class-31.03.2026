using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp.Helper;

public class EmailGenerator : IEmailGenerator
{
    public (string Subject, string Body) CreateEmail(Customer customer, string normalizedPlanCode, RenewalInvoice invoice)
    {
        string subject = "Subscription renewal invoice";
        string body =
            $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
            $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

        return (subject, body);
    }
}
