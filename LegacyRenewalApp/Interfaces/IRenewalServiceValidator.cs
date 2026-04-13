namespace LegacyRenewalApp.Interfaces;

public interface IRenewalServiceValidator
{
    void Validate(int customerId, string planCode, int seatCount, string paymentMethod);
    void ValidateCustomer(Customer customer);
}