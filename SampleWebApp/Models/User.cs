namespace SampleWebApp.Models {
    public class User {
        private readonly string firstName;
        private readonly string lastName;
        private readonly string email;
        private readonly string password;
        private readonly string account;

        public User(string firstName, string lastName, string email, string password, string account) {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.password = password;
            this.account = account;
        }

        public string FirstName {
            get { return firstName; }
        }

        public string LastName {
            get { return lastName; }
        }

        public string Email {
            get { return email; }
        }

        public string Password {
            get { return password; }
        }

        public string Account {
            get { return account; }
        }
    }
}