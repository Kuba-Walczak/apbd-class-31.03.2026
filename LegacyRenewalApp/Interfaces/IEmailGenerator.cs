using LegacyRenewalApp;

namespace LegacyRenewalApp.Interfaces;

public interface IEmailGenerator
{
    (string Subject, string Body) CreateEmail(Customer customer, string normalizedPlanCode, RenewalInvoice invoice);
}
