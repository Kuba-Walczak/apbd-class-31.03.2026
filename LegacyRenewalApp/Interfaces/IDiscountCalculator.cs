namespace LegacyRenewalApp.Interfaces;

public interface IDiscountCalculator
{
    public void CalculateDiscount(Customer customer, decimal baseAmount, decimal discountAmount, string notes, int seatCount, SubscriptionPlan plan);
}