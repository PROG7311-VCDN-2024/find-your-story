using System.ComponentModel.DataAnnotations;

namespace FindYourStoryApp.Models
{
    //MockPayment class represents a mock payment that requires the following fields:
    //card number, expiry date, cvv digits, and the amount.
    public class MockPayment
    {
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        public string CVVDigits { get; set; }
        [Required]
        public int Amount { get; set; }

        //ValidatePayment() method takes in the necessary parameters to process a payment and returns true
        //as this is a mock payment.
        public bool ValidatePayment(decimal amount, string cardNumber, string expiryDate, string cvv)
        {
            return true;
        }
    }
}
