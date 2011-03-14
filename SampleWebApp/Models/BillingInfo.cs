using System;

namespace SampleWebApp.Models {
    public class BillingInfo {
        private readonly string cardNumber;
        private readonly DateTime expDate;
        private readonly string cvv;
        private readonly string billingZip;

        public BillingInfo(string cardNumber, DateTime expDate, string cvv, string billingZip) {
            this.cardNumber = cardNumber;
            this.expDate = expDate;
            this.cvv = cvv;
            this.billingZip = billingZip;
        }

        public string CardNumber {
            get { return cardNumber; }
        }

        public DateTime ExpDate {
            get { return expDate; }
        }

        public string Cvv {
            get { return cvv; }
        }

        public string BillingZip {
            get { return billingZip; }
        }
    }
}