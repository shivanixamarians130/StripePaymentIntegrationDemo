using Stripe;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentGatwayPage : ContentPage
    {
        string ApiKey = "sk_test_51HWNy9LQmBCg8QnwKa9FQL9Qf2Iz7ZkE8PRzR3pOWoCx63VUzYzf5ZrSlqFK8dtyOSRNLll5r3XQJxxGfH5uIbcY007K67Tylk";
        public PaymentGatwayPage()
        {
            InitializeComponent();
        }
        public string CreateToken(string cardNumber, long cardExpMonth, long cardExpYear, string cardCVC)
        {
            try
            {
                StripeConfiguration.ApiKey = ApiKey;
              
                var tokenOptions = new TokenCreateOptions()
                {
                    Card = new TokenCardOptions()
                    {
                        Number = cardNumber,
                        ExpYear = cardExpYear,
                        ExpMonth = cardExpMonth,
                        Cvc = cardCVC
                    }
                };
                var tokenService = new TokenService();
                Token stripeToken = tokenService.Create(tokenOptions);
                return stripeToken.Id;
            }
           catch(Exception exc)
            {
                DisplayAlert("Alert", "Token creation failed", "Ok");
                return "";
            }
        }

        private void Btn_Clicked(object sender, EventArgs e)
        {
                string cardNumber = entryCardNumber.Text;
                long month = Convert.ToInt32(entryMonth.Text);
                long year = Convert.ToInt32(entryYear.Text);
                string cvv = entryCvv.Text;
                string tokenId = CreateToken(cardNumber, month, year, cvv);  //Get token
                if (!string.IsNullOrEmpty(tokenId))
                {
                    MakePayment(tokenId);
                }
        }
        private void MakePayment(string tokenId)
        {
            try
            {
                //create customer info
                CustomerCreateOptions customer = new CustomerCreateOptions
                {
                    Name = "Test Stripe",
                    Email = "customer@test.com",
                    Description = "test",
                    Address = new AddressOptions { City = "Kolkata", Country = "India", Line1 = "Sample Address", Line2 = "Sample Address 2", PostalCode = "700030", State = "WB" }
                };
                var customerService = new CustomerService();
                var cust = customerService.Create(customer);


                //create source options
                var option = new SourceCreateOptions
                {
                    Type = SourceType.Card,
                    Currency = "INR", //  the currency you are dealing with
                    Token = tokenId,
                };
                var sourceService = new SourceService();
                Source source = sourceService.Create(option);

                long amt = 0;
                if (!string.IsNullOrEmpty(entryPrice.Text))
                    amt = Convert.ToInt64(entryPrice.Text);

                var chargeoption = new ChargeCreateOptions
                {
                    Amount = amt * 100,
                    Currency = "INR",
                    ReceiptEmail = "sender@test.com",
                    Customer = cust.Id,
                    Source = source.Id,
                    Description = "test"
                };

                var chargeService = new ChargeService();
                Charge charge = chargeService.Create(chargeoption);

                if (charge.Status == "succeeded")
                {
                    DisplayAlert("Info", "Your Payment is successfully done", "Ok");
                    // success
                }
                else
                {
                    DisplayAlert("Alert", "Your Payment is not done", "Ok");
                    // failed
                }
            }
         catch(Exception exc)
            {
                DisplayAlert("Alert", exc.Message, "Ok");
            }
        }
    }
}