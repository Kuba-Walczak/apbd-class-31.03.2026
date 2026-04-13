using LegacyRenewalApp;

namespace LegacyRenewalApp.Interfaces;

public interface IDiscountCalculator
{
    decimal CalculateDiscount(Customer customer, decimal baseAmount, int seatCount, SubscriptionPlan plan, bool useLoyaltyPoints);
}
