using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebApp {
    public class FormletBindAttribute: CustomModelBinderAttribute {
        public Type FormletType { get; set; }
        public string FormletMethodName { get; set; }

        public override IModelBinder GetBinder() {
            return new FormletBinder(FormletType, FormletMethodName);
        }
    }
}