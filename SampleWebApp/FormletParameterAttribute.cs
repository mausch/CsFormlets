using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApp {
    /// <summary>
    /// Marks an action parameter to be bound by formlet
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FormletParameterAttribute: Attribute {}
}