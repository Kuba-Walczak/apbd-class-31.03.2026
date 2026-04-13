namespace LegacyRenewalApp.Interfaces;

public interface ITaxCalculator
{
    public decimal CalculateTax(string country, decimal taxBase);
}
