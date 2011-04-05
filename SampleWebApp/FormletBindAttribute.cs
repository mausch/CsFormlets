using System;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Formlets.CSharp;
using System.Collections.Specialized;
using System.Web;

namespace SampleWebApp {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FormletBindAttribute : ActionFilterAttribute {
        private readonly string formletMethodName;
        private readonly Type formletType;

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        /// <param name="formletMethodName"></param>
        public FormletBindAttribute(Type formletType, string formletMethodName) {
            this.formletType = formletType;
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of type <paramref name="formletType"/> to get the formlet to be used
        /// </summary>
        /// <param name="formletType"></param>
        public FormletBindAttribute(Type formletType) {
            this.formletType = formletType;
        }

        /// <summary>
        /// Uses method <paramref name="formletMethodName"/> of current controller to get the formlet to be used
        /// </summary>
        /// <param name="formletMethodName"></param>
        public FormletBindAttribute(string formletMethodName) {
            this.formletMethodName = formletMethodName;
        }

        /// <summary>
        /// Uses method [action]Formlet of current controller to get the formlet to be used
        /// </summary>
        public FormletBindAttribute() {}

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

        public NameValueCollection GetCollectionBySource(HttpRequestBase request) {
            if (Source == Source.QueryString)
                return request.QueryString;
            if (Source == Source.Form)
                return request.Form;
            return request.Params;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var type = formletType ?? filterContext.Controller.GetType();
            var methodName = formletMethodName ?? (filterContext.ActionDescriptor.ActionName + "Formlet");
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method == null)
                throw new Exception(string.Format("Formlet method '{0}' not found in '{1}'", methodName, type));
            dynamic formlet = method.Invoke(filterContext.Controller, null);
            dynamic result = formlet.Run(GetCollectionBySource(filterContext.HttpContext.Request));
            if (result.Value == null) {
                IEnumerable<XNode> errorNodes = result.ErrorForm;
                string errorForm = errorNodes.Render();
                filterContext.Result = new ViewResult {
                    ViewName = ViewName,
                    ViewData = new ViewDataDictionary(errorForm)
                };
            } else {
                var value = result.Value.Value;
                object option = result.Value;
                var valueType = option.GetType().GetGenericArguments()[0];
                var actionParams = filterContext.ActionDescriptor.GetParameters();
                var boundParam = actionParams.FirstOrDefault(d => d.IsDefined(typeof(FormletParameterAttribute), true));
                if (boundParam == null)
                    boundParam = actionParams.FirstOrDefault(d => d.ParameterType == valueType);
                if (boundParam == null)
                    throw new Exception(string.Format("Could not find any action parameter to bind formlet '{0}.{1}'. No action parameter of type '{2}' found and no action parameter was marked with [FormletParameter]", type, methodName, valueType));

                filterContext.ActionParameters[boundParam.ParameterName] = value;
            }
        }
    }
}