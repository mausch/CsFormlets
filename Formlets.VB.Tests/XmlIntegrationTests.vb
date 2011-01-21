Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests
    <Fact()> _
    Public Sub Test()
        Dim input = Formlet.Input()
        Dim inputInt = Formlet.Input().Lift(Function(a) Integer.Parse(a))
        Dim f = _
            Formlet.Yield(L.F(Function(a As String) _
                          L.F(Function(b As Integer) Tuple.Create(a, b)))) _
                .Ap(input) _
                .Ap(<br/>) _
                .Ap(inputInt.WrapWith(<span class="something"/>))
        Console.WriteLine(f.Render())
    End Sub
End Class
