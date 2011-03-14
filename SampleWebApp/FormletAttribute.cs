using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp
{
    public class FormletAttribute: Attribute, IModelBinder
    {
        private readonly string formletMethodName;



        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }
}