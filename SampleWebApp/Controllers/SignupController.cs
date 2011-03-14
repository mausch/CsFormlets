using System.Web.Mvc;
using Formlets.CSharp;
using System;
using SampleWebApp.Models;
using System.Linq;

namespace SampleWebApp.Controllers {
    public class SignupController : Controller {
        private static readonly FormElements e = new FormElements();

        private static readonly Formlet<string> password =
            Formlet.Tuple2<string, string>()
                .Ap(e.Password(required: true).WithLabelRaw("Password <em>(you'll use this to sign in)</em>"))
                .Ap(e.Password(required: true).WithLabelRaw("Confirm password <em>(for confirmation)</em>"))
                .Satisfies(t => t.Item1 == t.Item2, "Passwords don't match")
                .Select(t => t.Item1);

        private static readonly Formlet<string> account = 
            e.Text(required: true)
            .Satisfies(a => a.Length >= 2, "Two characters minimum")
            .Satisfies(a => string.Format("http://{0}.example.com", a).IsUrl(), "Invalid account");

        private static readonly Formlet<User> user =
            Formlet.Tuple5<string, string, string, string, string>()
                .Ap(e.Text(required: true).WithLabel("First name"))
                .Ap(e.Text(required: true).WithLabel("Last name"))
                .Ap(e.Email(required: true).WithLabel("Email address:"))
                .Ap(password)
                .Ap(account)
                .Select(t => new User(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));

        private static Formlet<DateTime> cardExpiration() {
            var now = DateTime.Now;
            var year = now.Year;
            return Formlet.Tuple2<int,int>()
                .Ap(e.Select(now.Month, Enumerable.Range(1, 12)))
                .Ap(e.Select(year, Enumerable.Range(year, 10)))
                .Select(t => new DateTime(t.Item2, t.Item1, 1).AddMonths(1));
        } 

        private static Formlet<BillingInfo> billing() {
            return Formlet.Tuple4<string, DateTime, string, string>()
                .Ap(e.Text(required: true).Transform(e.Validate.CreditCard).WithLabel("Credit card number"))
                .Ap(cardExpiration())
                .Ap(e.Text())
                .Ap(e.Text())
                .Select(t => new BillingInfo(t.Item1, t.Item2, t.Item3, t.Item4));
        }

        private static Formlet<RegistrationInfo> registration() {
            return Formlet.Tuple2<User, BillingInfo>()
                .Ap(user)
                .Ap(billing())
                .Select(t => new RegistrationInfo(t.Item1, t.Item2));
        }
           
        [HttpGet]
        public ActionResult Index() {
            return View(model: registration().ToString());
        }

        [HttpPost]
        [FormletPost("registration")]
        public ActionResult Index(RegistrationInfo registration) {
            return RedirectToAction("ThankYou", new {name = registration.User.FirstName + " " + registration.User.LastName});
        }

        [HttpPost]
        public ActionResult Register() {
            var result = registration().RunPost(Request);
            if (result.Value.IsNone())
                return View("Index", model: result.ErrorForm.Render());
            var value = result.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        public ActionResult ThankYou(string name) {
            return View(model: name);
        }
    }
}