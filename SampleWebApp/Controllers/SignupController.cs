using System.Web.Mvc;
using Formlets.CSharp;
using System;
using SampleWebApp.Models;
using System.Linq;
using SampleWebApp.Formlets;

namespace SampleWebApp.Controllers {
    public class SignupController : Controller {
           
        [HttpGet]
        public ActionResult Index() {
            return View(model: SignupFormlet.Registration().ToString());
        }

        [HttpPost]
        [FormletPost("registration")]
        public ActionResult Index(RegistrationInfo registration) {
            return RedirectToAction("ThankYou", new {name = registration.User.FirstName + " " + registration.User.LastName});
        }

        [HttpPost]
        public ActionResult Register() {
            var result = SignupFormlet.Registration().RunPost(Request);
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