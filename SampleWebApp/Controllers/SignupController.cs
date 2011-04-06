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
            return View(model: SignupFormlet.IndexFormlet().ToString());
        }

        [HttpPost]
        [FormletBind(typeof(SignupFormlet))]
        public ActionResult Index(RegistrationInfo registration) {
            return RedirectToAction("ThankYou", new {name = registration.User.FirstName + " " + registration.User.LastName});
        }

        /// <summary>
        /// Alternative to action above. Explicitly calls formlet and handles its result.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register() {
            var result = SignupFormlet.IndexFormlet().RunPost(Request);
            if (!result.Value.HasValue())
                return View("Index", model: result.ErrorForm.Render());
            var value = result.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        public ActionResult ThankYou(string name) {
            return View(model: name);
        }
    }
}