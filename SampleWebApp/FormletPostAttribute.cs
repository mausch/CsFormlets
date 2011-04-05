using System;
using System.Web.Mvc;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Formlets.CSharp;

namespace SampleWebApp {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FormletPostAttribute : ActionFilterAttribute {
        private readonly string formletMethodName;
        private readonly Type formletType;

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        /// <param name="formletMethodName"></param>
        public FormletPostAttribute(Type formletType, string formletMethodName) {
            this.formletType = formletType;
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        public FormletPostAttribute(Type formletType) {
            this.formletType = formletType;
        }

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of current controller to get the formlet to be used
        /// </summary>
        /// <param name="formletMethodName"></param>
        public FormletPostAttribute(string formletMethodName) {
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of current controller to get the formlet to be used
        /// </summary>
        public FormletPostAttribute() {}

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var type = formletType ?? filterContext.Controller.GetType();
            var methodName = formletMethodName ?? (filterContext.ActionDescriptor.ActionName + "Formlet");
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method == null)
                throw new Exception(string.Format("Formlet method '{0}' not found in '{1}'", formletMethodName, type));
            dynamic formlet = method.Invoke(filterContext.Controller, null);
            dynamic result = formlet.RunPost(filterContext.HttpContext.Request);
            if (result.Value == null) {
                IEnumerable<XNode> errorNodes = result.ErrorForm;
                string errorForm = errorNodes.Render();
                filterContext.Result = new ViewResult {
                    ViewData = new ViewDataDictionary(errorForm)
                };
            } else {
                filterContext.ActionParameters[formletMethodName] = result.Value.Value;
            }
        }
    }
}