using System;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Formlets.CSharp;
using System.Collections.Specialized;
using System.Web;
using Microsoft.FSharp.Core;

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

        public IFormletResult GetFormletResult(ActionExecutingContext filterContext) {
            var type = formletType ?? filterContext.Controller.GetType();
            var methodName = formletMethodName ?? (filterContext.ActionDescriptor.ActionName + "Formlet");
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method == null)
                throw new Exception(string.Format("Formlet method '{0}' not found in '{1}'", methodName, type));
            var formlet = (IFormlet)method.Invoke(filterContext.Controller, null);
            return formlet.Run(GetCollectionBySource(filterContext.HttpContext.Request));
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var result = GetFormletResult(filterContext);
            var valueType = result.ValueType;
            var getValue = typeof(FSharpOption<>).MakeGenericType(valueType).GetProperty("Value");
            var actionParams = filterContext.ActionDescriptor.GetParameters();
            var resultType = typeof(FormletResult<>).MakeGenericType(valueType);
            var boundParam = actionParams.FirstOrDefault(p => p.ParameterType == resultType);
            if (boundParam != null) {
                filterContext.ActionParameters[boundParam.ParameterName] = result;
                return;
            }
            if (result.Value == null) {
                var errorNodes = result.ErrorForm;
                string errorForm = errorNodes.Render();
                filterContext.Result = new ViewResult {
                    ViewName = ViewName,
                    ViewData = new ViewDataDictionary(errorForm)
                };
            } else {
                var value = getValue.GetValue(result.Value, null);
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