Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests
    <Fact()> _
    Public Sub Test()
        Dim input = Formlet.Input()
        Dim inputInt = Formlet.Input().Lift(Function(a) Integer.Parse(a))
        Dim extXml = <div class="adiv"><%= input.RenderToXml() %></div>
        Dim flet = Formlet.Xml(extXml)
        Dim f = _
            Formlet.Yield(L.F(Function(a As String) _
                          L.F(Function(b As Integer) Tuple.Create(a, b)))) _
                .Ap(input) _
                .Ap(<br/>) _
                .Ap(inputInt)
        Console.WriteLine(extXml)
    End Sub
End Class
