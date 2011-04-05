using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Formlets.CSharp;
using SampleWebApp.Models;

namespace SampleWebApp.Formlets
{
    static class FormletExtensions
    {
        public static Formlet<T> SatisfiesBr<T>(this Formlet<T> f, Func<T, bool> pred, string error)
        {
            return f.Satisfies(pred, 
                (_, x) => x.Append(X.E("br")).Append(X.E("span", X.A("class","error"), error)),
                _ => new[] { error });
        }
    }

    public class SignupFormlet
    {
        private static readonly FormElements e = new FormElements();

        private static readonly Formlet<string> password =
            Formlet.Tuple2<string, string>()
                .Ap(e.Password(required: true).WithLabelRaw("Password <em>(6 characters or longer)</em>"))
                .Ap(e.Password(required: true).WithLabelRaw("Enter password again <em>(for confirmation)</em>"))
                .SatisfiesBr(t => t.Item1 == t.Item2, "Passwords don't match")
                .Select(t => t.Item1)
                .SatisfiesBr(t => t.Length >= 6, "Password must be 6 characters or longer");

        private static readonly Formlet<string> account =
            Formlet.Single<string>()
                .Ap("http://")
                .Ap(e.Text(attributes: new[] {KV.Create("required","required")}))
                .Ap(".example.com")
                .Ap(X.E("div", "Example: http://", X.E("b", "company"), ".example.com"))
                .Satisfies(a => !string.IsNullOrWhiteSpace(a), "Required field")
                .Satisfies(a => a.Length >= 2, "Two characters minimum")
                .Satisfies(a => string.Format("http://{0}.example.com", a).IsUrl(), "Invalid account")
                .WrapWith(X.E("fieldset"));

        private static readonly Formlet<User> user =
            Formlet.Tuple5<string, string, string, string, string>()
                .Ap(e.Text(required: true).WithLabel("First name"))
                .Ap(e.Text(required: true).WithLabel("Last name"))
                .Ap(e.Email(required: true).WithLabelRaw("Email address <em>(you'll use this to sign in)</em>"))
                .Ap(password)
                .WrapWith(X.E("fieldset"))
                .Ap(X.E("h3", "Profile URL"))
                .Ap(account)
                .Select(t => new User(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));

        private static Formlet<DateTime> cardExpiration()
        {
            var now = DateTime.Now;
            var year = now.Year;
            return Formlet.Tuple2<int, int>()
                .Ap(e.Select(now.Month, Enumerable.Range(1, 12)))
                .Ap(e.Select(year, Enumerable.Range(year, 10)))
                .Select(t => new DateTime(t.Item2, t.Item1, 1).AddMonths(1))
                .WrapWithLabel("Expiration date<br/>");
        }

        private static Formlet<BillingInfo> billing()
        {
            return Formlet.Tuple4<string, DateTime, string, string>()
                .Ap(e.Text(required: true).Transform(e.Validate.CreditCard).WithLabel("Credit card number"))
                .Ap(cardExpiration())
                .Ap(e.Text(required: true).WithLabel("Security code"))
                .Ap(e.Text(required: true).WithLabelRaw("Billing ZIP <em>(postal code if outside the USA)</em>"))
                .Select(t => new BillingInfo(t.Item1, t.Item2, t.Item3, t.Item4))
                .WrapWith(X.E("fieldset"));
        }

        public static Formlet<RegistrationInfo> IndexFormlet()
        {
            return Formlet.Tuple2<User, BillingInfo>()
                .Ap(X.E("h3", "Enter your details"))
                .Ap(user)
                .Ap(X.E("h3", "Billing information"))
                .Ap(billing())
                .Select(t => new RegistrationInfo(t.Item1, t.Item2));
        }

    }
}