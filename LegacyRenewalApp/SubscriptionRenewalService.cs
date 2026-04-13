using System;
using LegacyRenewalApp.Helper;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IRenewalServiceValidator _renewalServiceValidator;
        private readonly IBillingGateway _billingGateway;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly IFeeCalculator _feeCalculator;
        private readonly ITaxCalculator _taxCalculator;

        public SubscriptionRenewalService() : this(new CustomerRepository(), new SubscriptionPlanRepository(), new RenewalServiceValidator(), new BillingGatewayAdapter(), new DiscountCalculator(), new FeeCalculator(), new TaxCalculator()) {}
        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository subscriptionPlanRepository, IRenewalServiceValidator renewalServiceValidator, IBillingGateway billingGateway, IDiscountCalculator discountCalculator, IFeeCalculator feeCalculator, ITaxCalculator taxCalculator)
        {
            _customerRepository = customerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _renewalServiceValidator = renewalServiceValidator;
            _billingGateway = billingGateway;
            _discountCalculator = discountCalculator;
            _feeCalculator = feeCalculator;
            _taxCalculator = taxCalculator;
        }
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _renewalServiceValidator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _customerRepository.GetById(customerId);
            var plan = _subscriptionPlanRepository.GetByCode(normalizedPlanCode);

            _renewalServiceValidator.ValidateCustomer(customer);

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
            string notes = string.Empty;

            var (discountAmount, discountNotes) = _discountCalculator.CalculateDiscount(customer, notes, baseAmount, seatCount, plan, useLoyaltyPoints);
            notes += discountNotes;
            
            decimal subtotalAfterDiscount = baseAmount - discountAmount;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            var (supportFee, supportNotes) = _feeCalculator.CalculateSupportFee(normalizedPlanCode, includePremiumSupport);
            notes += supportNotes;

            var (paymentFee, paymentNotes) = _feeCalculator.CalculatePaymentFee(normalizedPaymentMethod, subtotalAfterDiscount + supportFee);
            notes += paymentNotes;

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = _taxCalculator.CalculateTax(customer.Country, taxBase);
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
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

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                _billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
