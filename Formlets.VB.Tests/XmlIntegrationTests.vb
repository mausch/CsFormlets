Imports System.Text.RegularExpressions
Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests
    <Fact()> _
    Public Sub Test()
        Dim input = Formlet.Input()
        Dim inputInt = Formlet.Input().
                        Satisfies(Function(s) Regex.IsMatch(s, "[0-9]+"),
                                   Function(s, x) x.Append(<span class="error">'<%= s %>' is not a valid number</span>)).
                        Lift(Function(a) Integer.Parse(a))
        Dim f =
            Formlet.Yield(L.F(Function(a As String) _
                          L.F(Function(b As Integer) Tuple.Create(a, b)))).
                Ap(input).
                Ap(<br/>, <br/>).
                Ap(inputInt.WrapWith(<span class="something"/>)).
                Ap(<input type="submit" value="Send!"/>, <br/>)
        Console.WriteLine(f.Render())
        Dim result = f.Run(New Dictionary(Of String, String) From
                           {
                               {"input_0", "something"},
                               {"input_1", "else"}
                           })
        Assert.True(FSharpOptionExtensions.IsNone(result.Value))
        Console.WriteLine()
        Console.WriteLine("Error form:")
        Console.WriteLine(result.ErrorForm)
    End Sub
End Class
