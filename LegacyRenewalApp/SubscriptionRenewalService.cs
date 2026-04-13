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
        private readonly INotesGenerator _notesGenerator;
        private readonly IInvoiceGenerator _invoiceGenerator;
        private readonly IEmailGenerator _emailGenerator;

        public SubscriptionRenewalService() : this(new CustomerRepository(), new SubscriptionPlanRepository(), new RenewalServiceValidator(), new BillingGatewayAdapter(), new DiscountCalculator(), new FeeCalculator(), new TaxCalculator(), new NotesGenerator(), new InvoiceGenerator(), new EmailGenerator()) {}
        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository subscriptionPlanRepository, IRenewalServiceValidator renewalServiceValidator, IBillingGateway billingGateway, IDiscountCalculator discountCalculator, IFeeCalculator feeCalculator, ITaxCalculator taxCalculator, INotesGenerator notesGenerator, IInvoiceGenerator invoiceGenerator, IEmailGenerator emailGenerator)
        {
            _customerRepository = customerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _renewalServiceValidator = renewalServiceValidator;
            _billingGateway = billingGateway;
            _discountCalculator = discountCalculator;
            _feeCalculator = feeCalculator;
            _taxCalculator = taxCalculator;
            _notesGenerator = notesGenerator;
            _invoiceGenerator = invoiceGenerator;
            _emailGenerator = emailGenerator;
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
            decimal discountAmount = _discountCalculator.CalculateDiscount(customer, baseAmount, seatCount, plan, useLoyaltyPoints);
            string notes = _notesGenerator.FormatDiscountNotes(customer, seatCount, plan, useLoyaltyPoints);

            decimal subtotalAfterDiscount = baseAmount - discountAmount;
            bool minimumSubtotalApplied = false;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                minimumSubtotalApplied = true;
            }
            notes += _notesGenerator.FormatMinimumSubtotalNote(minimumSubtotalApplied);

            decimal supportFee = _feeCalculator.CalculateSupportFee(normalizedPlanCode, includePremiumSupport);
            notes += _notesGenerator.FormatSupportFeeNote(normalizedPlanCode, includePremiumSupport);

            decimal paymentFee = _feeCalculator.CalculatePaymentFee(normalizedPaymentMethod, subtotalAfterDiscount + supportFee);
            notes += _notesGenerator.FormatPaymentFeeNote(normalizedPaymentMethod);

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = _taxCalculator.CalculateTax(customer.Country, taxBase);
            decimal finalAmount = taxBase + taxAmount;

            bool minimumInvoiceApplied = false;
            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                minimumInvoiceApplied = true;
            }
            notes += _notesGenerator.FormatMinimumInvoiceNote(minimumInvoiceApplied);

            string invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}";

            var invoice = _invoiceGenerator.CreateRenewalInvoice(invoiceNumber, customer.FullName, normalizedPlanCode, normalizedPaymentMethod, seatCount, baseAmount, discountAmount, supportFee, paymentFee, taxAmount, finalAmount, notes);

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                (string subject, string body) = _emailGenerator.CreateEmail(customer, normalizedPlanCode, invoice);

                _billingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
