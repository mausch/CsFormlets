using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formlets.CSharp;
using System.Collections.Specialized;
using System.Reflection;

namespace SampleWebApp {
    public class FormletBinder: IModelBinder {
        private readonly Type formletType;
        private readonly string formletMethodName;

        public FormletBinder(Type formletType, string formletMethodName) {
            this.formletType = formletType;
            this.formletMethodName = formletMethodName;
        }

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

        public IFormletResult GetFormletResult(ControllerBase controller, string actionName, HttpRequestBase request) {
            var type = formletType ?? controller.GetType();
            var methodName = formletMethodName ?? (actionName + "Formlet");
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (method == null)
                throw new Exception(string.Format("Formlet method '{0}' not found in '{1}'", methodName, type));
            var formlet = (IFormlet)method.Invoke(controller, null);
            return formlet.Run(GetCollectionBySource(request));
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var actionName = (string)controllerContext.RouteData.Values["action"];
            return GetFormletResult(controllerContext.Controller, actionName, controllerContext.HttpContext.Request);
        }
    }
}