using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApp {
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FormletBindAttribute: Attribute {}
}