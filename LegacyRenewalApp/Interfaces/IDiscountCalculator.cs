namespace LegacyRenewalApp.Interfaces;

public interface IDiscountCalculator
{
    public (decimal discountAmount, string notes) CalculateDiscount(Customer customer, decimal baseAmount, int seatCount, SubscriptionPlan plan, bool useLoyaltyPoints);
}