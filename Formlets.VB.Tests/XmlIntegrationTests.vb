Imports System.Text.RegularExpressions
Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests

    <Fact()> _
    Public Sub Test()
        Dim input = Formlet.Input()
        Dim inputInt =
            Function(attr As IDictionary(Of String, String)) _
                Formlet.Input(attr).Validate(Function(s) Regex.IsMatch(s, "[0-9]+"),
                                      Function(s) String.Format("{0} is not a valid number")).
                            Lift(Function(a) Integer.Parse(a))
        Dim inputRange =
            Function(min As Integer, max As Integer) _
                inputInt(Nothing).Validate(Function(n) n <= max AndAlso n >= min,
                                      Function(s) String.Format("Value must be between {0} and {1}", min, max))
        Dim inputDate =
            Function()
                Dim isDate = Function(m As Integer, d As Integer, y As Integer)
                                 Try
                                     Dim k = New DateTime(y, m, d)
                                     Return True
                                 Catch ex As Exception
                                     Return False
                                 End Try
                             End Function
                Dim id = Guid.NewGuid.ToString()
                Dim idn = Function(n As Integer) String.Format("d{0}{1}", id, n)
                Dim values = Formlet.Yield(L.F(Function(month As Integer) _
                              L.F(Function(day As Integer) _
                              L.F(Function(year As Integer) tuple.Create(month, day, year))))).
                    Ap(<label for=<%= idn(0) %>>Month: </label>).
                    Ap(inputInt(New Dictionary(Of String, String) From {{"id", idn(0)}})).
                    Ap(<br/>, <label for=<%= idn(1) %>>Day: </label>).
                    Ap(inputInt(New Dictionary(Of String, String) From {{"id", idn(1)}})).
                    Ap(<br/>, <label for=<%= idn(2) %>>Year: </label>).
                    Ap(inputInt(New Dictionary(Of String, String) From {{"id", idn(2)}}))
                Return values.Validate(Function(t) isDate(t.Item1, t.Item2, t.Item3),
                                                          Function(s) "Invalid date").
                            Lift(Function(t) New DateTime(t.Item3, t.Item1, t.Item2))
            End Function

        Dim f =
            Formlet.Yield(L.F(Function(a As String) _
                          L.F(Function(b As Integer) Tuple.Create(a, b)))).
                Ap(input).
                Ap(<br/>, <br/>).
                Ap(inputInt(Nothing).WrapWith(<span class="something"/>)).
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
