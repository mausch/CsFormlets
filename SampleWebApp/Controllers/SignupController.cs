﻿using System;
using System.Web.Mvc;
using FSharpx;
using Formlets;
using Formlets.CSharp;
using SampleWebApp.Models;
using SampleWebApp.Formlets;
using Microsoft.FSharp.Core;

namespace SampleWebApp.Controllers {
    public class SignupController : Controller {
           
        [HttpGet]
        public ActionResult Index() {
            return View(model: SignupFormlet.IndexFormlet().ToString());
        }

        [HttpPost]
        [FormletFilter(typeof(SignupFormlet))]
        public ActionResult Index1(RegistrationInfo registration) {
            return RedirectToAction("ThankYou", new {name = registration.User.FirstName + " " + registration.User.LastName});
        }

        [HttpPost]
        public ActionResult Index2() {
            var result = SignupFormlet.IndexFormlet().RunPost(Request);
            if (!result.Value.HasValue())
                return View(model: result.Form.Render());
            var value = result.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        [HttpPost]
        public ActionResult Index(FormCollection form) {
            var result = SignupFormlet.IndexFormlet().Run(form);
            if (!result.Value.HasValue())
                return View(model: result.Form.Render());
            var value = result.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        [HttpPost]
        public ActionResult Index4(FormCollection form) {
            var result = SignupFormlet.IndexFormlet().Run(form);
            return Signup(result);
        }

        [NonAction]
        public ActionResult Signup(FormletResult<RegistrationInfo> registration) {
            if (!registration.Value.HasValue())
                return View(model: registration.Form.Render());
            return Signup(registration.Value.Value);
        }

        [NonAction]
        public ActionResult Signup(RegistrationInfo registration) {
            return RedirectToAction("ThankYou", new { name = registration.User.FirstName + " " + registration.User.LastName });
        }

        [HttpPost]
        [FormletFilter(typeof(SignupFormlet))]
        public ActionResult Index5(FormletResult<RegistrationInfo> registration) {
            if (!registration.Value.HasValue())
                return View(model: registration.Form.Render());
            var value = registration.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        [HttpPost]
        public ActionResult Index6([FormletBind(FormletType = typeof(SignupFormlet))] FormletResult<RegistrationInfo> registration) {
            if (!registration.Value.HasValue())
                return View(model: registration.Form.Render());
            var value = registration.Value.Value;
            return RedirectToAction("ThankYou", new { name = value.User.FirstName + " " + value.User.LastName });
        }

        public ActionResult ThankYou(string name) {
            return View(model: name);
        }
    }
}