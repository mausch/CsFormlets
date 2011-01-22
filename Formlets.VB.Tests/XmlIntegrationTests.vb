Imports System.Text.RegularExpressions
Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests
    <Fact()> _
    Public Sub Test()
        Dim input = Formlet.Input()
        Dim inputInt = Formlet.Input() _
                .Satisfies(Function(s) Regex.IsMatch(s, "[0-9]+"), _
                           Function(s, x) x.Append(<span class="error">'<%= s %>' is not a valid number</span>)) _
                .Lift(Function(a) Integer.Parse(a))
        Dim f = _
            Formlet.Yield(L.F(Function(a As String) _
                          L.F(Function(b As Integer) Tuple.Create(a, b)))) _
                .Ap(input) _
                .Ap(<br/>, <br/>) _
                .Ap(inputInt.WrapWith(<span class="something"/>)) _
                .Ap(<input type="submit" value="Send!"/>, <br/>)
        Console.WriteLine(f.Render())
    End Sub
End Class
