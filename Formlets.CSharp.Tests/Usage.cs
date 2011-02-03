using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Xunit;

namespace Formlets.CSharp.Tests {
    public class Usage {
        [Fact]
        public void Run() {
            var input = Formlet.Input();
            var result = input.Run(new Dictionary<string, string> {{"input_0", "something"}});
            Assert.True(result.Value.IsSome());
            Assert.Equal("something", result.Value.Value);
            Console.WriteLine(result.ErrorForm);
            Assert.Equal("<input name=\"input_0\" value=\"something\" />", result.ErrorForm.ToString());
        }

        [Fact]
        public void Render() {
            var input = Formlet.Input("a value", new Dictionary<string, string> {{"size", "10"}});
            var html = input.ToString();
            Console.WriteLine(html);
            Assert.Equal("<input name=\"input_0\" value=\"a value\" size=\"10\" />", html);
        }

        [Fact]
        public void Lift() {
            var input = Formlet.Input();
            var inputInt = input.Select(int.Parse);
            var result = inputInt.Run(new Dictionary<string, string> {{"input_0", "15"}});
            Assert.True(result.Value.IsSome());
            Assert.Equal(15, result.Value.Value);
        }

        [Fact]
        public void PureApply() {
            var input = Formlet.Input("a value", new Dictionary<string, string> {{"size", "10"}});
            var inputInt = Formlet.Input().Select(int.Parse);
            var formlet = Formlet.Yield(L.F((string a) => L.F((int b) => Tuple.Create(a, b))))
                .Ap(input)
                .Ap("Hello world!")
                .Ap(X.E("span", X.A("class","bla"), "a message"))
                .Ap(inputInt);
            var html = formlet.ToString();
            Console.WriteLine(html);
            Assert.Contains("<input name=\"input_0\" value=\"a value\" size=\"10\" />", html);
            Assert.Contains("Hello world!", html);
            Assert.Contains("<input name=\"input_1\" value=\"\" />", html);
            var result = formlet.Run(new Dictionary<string, string> {
                {"input_0", "bla"},
                {"input_1", "20"},
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
                {"input_0", "bla"}
            });
            Console.WriteLine(result.ErrorForm);
            Assert.Contains("<input name=\"input_0\" value=\"bla\" />'bla' is not a valid number", result.ErrorForm.ToString());
            Assert.True(result.Value.IsNone());
        }

        [Fact]
        public void Validation2() {
            var inputInt = Formlet.Input()
                .Satisfies(s => Regex.IsMatch(s, "[0-9]+"),
                           s => string.Format("'{0}' is not a valid number", s))
                .Select(int.Parse);
            var result = inputInt.Run(new Dictionary<string, string> {
                {"input_0", "bla"}
            });
            Console.WriteLine(result.ErrorForm);
            Assert.Contains("<input name=\"input_0\" value=\"bla\" />", result.ErrorForm.ToString());
            Assert.Contains("<span class=\"error\">'bla' is not a valid number</span>", result.ErrorForm.ToString());
            Assert.True(result.Value.IsNone());
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
            var formlet = Formlet.Yield(L.F((string a) => L.F((int b) => Tuple.Create(a, b))))
                .Ap(radio1)
                .Ap(radio2);
            var result = formlet.Run(new Dictionary<string, string> {
                {"input_0", "b"},
                {"input_1", "2"},
            });
            Assert.Equal("b", result.Value.Value.Item1);
            Assert.Equal(2, result.Value.Value.Item2);
        }

        [Fact]
        public void File() {
            var file = Formlet.File();
            Console.WriteLine(file.ToString());
            var result = file.Run(new Dictionary<string, InputValue> {
                {"input_0", InputValue.NewFile(new MockHttpPostedFileBase {MFileName = "test"})}
            });
            Assert.Equal("test", result.Value.Value.Value.FileName);
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase {
            public override string FileName {
                get { return MFileName; }
            }

            public string MFileName { get; set; }
        }
    }
}