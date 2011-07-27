using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Xunit;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp.Tests {
    public class Usage {
        [Fact]
        public void Run() {
            var input = Formlet.Input();
            var result = input.Run(new Dictionary<string, string> {{"f0", "something"}});
            Assert.True(result.Value.HasValue());
            Assert.Equal("something", result.Value.Value);
            Console.WriteLine(result.ErrorForm);
            Assert.Equal("<input name=\"f0\" value=\"something\" />", result.ErrorForm.Render());
        }

        [Fact]
        public void Render() {
            var input = Formlet.Input("a value", new Dictionary<string, string> {{"size", "10"}});
            var html = input.ToString();
            Console.WriteLine(html);
            Assert.Equal("<input name=\"f0\" value=\"a value\" size=\"10\" />", html);
        }

        [Fact]
        public void Lift() {
            var input = Formlet.Input();
            var inputInt = input.Select(int.Parse);
            var result = inputInt.Run(new Dictionary<string, string> {{"f0", "15"}});
            Assert.True(result.Value.HasValue());
            Assert.Equal(15, result.Value.Value);
        }

        [Fact]
        public void PureApply() {
            var input = Formlet.Input("a value", new Dictionary<string, string> {{"size", "10"}});
            var inputInt = Formlet.Input().Select(int.Parse);
            var formlet = Formlet.Tuple2<string,int>()
                .Ap(input)
                .Ap("Hello world!")
                .Ap(X.E("span", X.A("class","bla"), "a message"))
                .Ap(inputInt);
            var html = formlet.ToString();
            Console.WriteLine(html);
            Assert.Contains("<input name=\"f0\" value=\"a value\" size=\"10\" />", html);
            Assert.Contains("Hello world!", html);
            Assert.Contains("<input name=\"f1\" value=\"\" />", html);
            var result = formlet.Run(new Dictionary<string, string> {
                {"f0", "bla"},
                {"f1", "20"},
            });
            Assert.Equal("bla", result.Value.Value.Item1);
            Assert.Equal(20, result.Value.Value.Item2);
        }

        [Fact]
        public void Validation() {
            var inputInt = Formlet.Input()
                .Satisfies(s => Regex.IsMatch(s, "[0-9]+"), (s, n) => {
                    var msg = string.Format("'{0}' is not a valid number", s);
                    return n.Append(msg);
                }, v => new string[0])
                .Select(int.Parse);
            var result = inputInt.Run(new Dictionary<string, string> {
                {"f0", "bla"}
            });
            Console.WriteLine(result.ErrorForm);
            Assert.Contains("<input name=\"f0\" value=\"bla\" />'bla' is not a valid number", result.ErrorForm.Render());
            Assert.False(result.Value.HasValue());
        }

        [Fact]
        public void Validation2() {
            var inputInt = Formlet.Input()
                .Satisfies(s => Regex.IsMatch(s, "[0-9]+"),
                           s => string.Format("'{0}' is not a valid number", s))
                .Select(int.Parse);
            var result = inputInt.Run(new Dictionary<string, string> {
                {"f0", "bla"}
            });
            Console.WriteLine(result.ErrorForm);
            Assert.Contains("<input name=\"f0\" value=\"bla\" />", result.ErrorForm.Render());
            Assert.Contains("<span class=\"error\">'bla' is not a valid number</span>", result.ErrorForm.Render());
            Assert.False(result.Value.HasValue());
        }

        [Fact]
        public void Radio() {
            var radio1 = Formlet.Radio("a", new Dictionary<string, string> {
                {"a", "First"},
                {"b", "Second"},
            });
            var radio2 = Formlet.Radio(1, new Dictionary<int, string> {
                {1, "First"},
                {2, "Second"},
            });
            var formlet = Formlet.Tuple2<string,int>()
                .Ap(radio1)
                .Ap(radio2);
            var result = formlet.Run(new Dictionary<string, string> {
                {"f0", "b"},
                {"f1", "2"},
            });
            Assert.Equal("b", result.Value.Value.Item1);
            Assert.Equal(2, result.Value.Value.Item2);
        }

        [Fact]
        public void File() {
            var file = Formlet.File();
            Console.WriteLine(file.ToString());
            var result = file.Run(new Dictionary<string, InputValue> {
                {"f0", InputValue.NewFile(new MockHttpPostedFileBase {MFileName = "test"})}
            });
            Assert.Equal("test", result.Value.Value.Value.FileName);
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase {
            public override string FileName {
                get { return MFileName; }
            }

            public string MFileName { get; set; }
        }

        [Fact]
        public void FormElements() {
            var e = new FormElements();
            var f = e.Int(required: true);
            Assert.Equal("<input required=\"required\" name=\"f0\" value=\"\" />", f.ToString());
            var result = f.Run(new Dictionary<string, string> {
                {"f0", ""},
            });
            var errorForm = result.ErrorForm.Render();
            Assert.Contains("errorinput", errorForm);
        }

        [Fact]
        public void LINQ_formlet_with_validation_error() {
            var e = new FormElements();
            var f = from name in e.Text()
                    join age in e.Int() on 1 equals 1
                    where age == 42
                    select new { name, age };
            var r = f.Run(new Dictionary<string,string> {
                {"f0", "John"},
                {"f1", "44"},
            });
            Assert.False(r.Value.HasValue());
            Assert.Equal(1, r.Errors.Count);
        }

        [Fact]
        public void LINQ_formlet() {
            var e = new FormElements();
            var f = from name in e.Text()
                    join _ in Formlet.Raw(X.E("br")) on 1 equals 1
                    join age in e.Int() on 1 equals 1
                    where age == 42
                    select new { name, age };
            var r = f.Run(new Dictionary<string, string> {
                {"f0", "John"},
                {"f1", "42"},
            });
            Assert.True(r.Value.HasValue());
            Assert.Equal("John", r.Value.Value.name);
            Assert.Equal(42, r.Value.Value.age);
        }

    }
}