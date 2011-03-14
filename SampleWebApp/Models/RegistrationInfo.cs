namespace SampleWebApp.Models {
    public class RegistrationInfo {
        private readonly User user;
        private readonly BillingInfo billing;

        public RegistrationInfo() {}

        public RegistrationInfo(User user, BillingInfo billing) {
            this.user = user;
            this.billing = billing;
        }

        public User User {
            get { return user; }
        }

        public BillingInfo Billing {
            get { return billing; }
        }
    }
}