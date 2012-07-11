using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Formlets;
using Formlets.CSharp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace SampleWebApp {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FormletFilterAttribute : ActionFilterAttribute {
        private readonly string formletMethodName;
        private readonly Type formletType;

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        /// <param name="formletMethodName"></param>
        public FormletFilterAttribute(Type formletType, string formletMethodName) {
            this.formletType = formletType;
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        public FormletFilterAttribute(Type formletType) {
            this.formletType = formletType;
        }

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of current controller to get the formlet to be used
        /// </summary>
        /// <param name="formletMethodName"></param>
        public FormletFilterAttribute(string formletMethodName) {
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of current controller to get the formlet to be used
        /// </summary>
        public FormletFilterAttribute() {}

        /// <summary>
        /// View to show in case there's a binding error. 
        /// By default null, which means the default view for the current action.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// HTTP request collection to use as source for formlet binding. 
        /// By default Request.Params
        /// </summary>
        public Source Source { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var binder = new FormletBinder(formletType, formletMethodName) { Source = Source };
            var result = binder.GetFormletResult(filterContext.Controller, filterContext.ActionDescriptor.ActionName, filterContext.HttpContext.Request);
            var valueType = formletType.GetGenericArguments()[0];
            var optionValue = typeof(FSharpOption<>).MakeGenericType(valueType).GetProperty("Value");
            var actionParams = filterContext.ActionDescriptor.GetParameters();
            var resultType = typeof(FormletResult<>).MakeGenericType(valueType);
            var getFormletValue = resultType.GetProperty("Value");
            var boundParam = actionParams.FirstOrDefault(p => p.ParameterType == resultType);
            if (boundParam != null) {
                filterContext.ActionParameters[boundParam.ParameterName] = result;
                return;
            }
            var collectedValue = getFormletValue.GetValue(result, null);
            if (collectedValue == null) {
                var getFormletForm = resultType.GetProperty("Form");
                var errorNodes = (FSharpList<XNode>) getFormletForm.GetValue(result, null);
                string errorForm = errorNodes.Render();
                filterContext.Result = new ViewResult {
                    ViewName = ViewName,
                    ViewData = new ViewDataDictionary(errorForm)
                };
            } else {
                var value = optionValue.GetValue(collectedValue, null);
                boundParam = actionParams.FirstOrDefault(d => d.IsDefined(typeof(FormletParameterAttribute), true));
                if (boundParam == null)
                    boundParam = actionParams.FirstOrDefault(d => d.ParameterType == valueType);
                if (boundParam == null)
                    throw new Exception(string.Format("Could not find any action parameter to bind formlet. No action parameter of type '{0}' or FormletResult<{0}> found and no action parameter was marked with [FormletParameter]", valueType));

                filterContext.ActionParameters[boundParam.ParameterName] = value;
            }
        }
    }
}