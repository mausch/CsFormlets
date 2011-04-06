Imports System.Text.RegularExpressions
Imports Formlets.CSharp
Imports Xunit

Public Class XmlIntegrationTests

    <Fact()> _
    Public Sub Test()
        Dim e = New FormElements()
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
                Dim values =
                    Formlet.Tuple3(Of Integer, Integer, Integer)().
                        Ap(e.Int().WithLabel("Month: ")).
                        Ap(e.Int().WithLabel("Day: ")).
                        Ap(e.Int().WithLabel("Year: "))

                Return From v In values
                        Where isDate(v.Item1, v.Item2, v.Item3)
                        Select New DateTime(v.Item3, v.Item1, v.Item2)
            End Function

        Console.WriteLine(inputDate().ToString())

        Dim f =
            Formlet.Tuple2(Of String, Integer)().
                Ap(e.Text()).
                Ap(<br/>, <br/>).
                Ap(e.Int().WrapWith(<span class="something"/>)).
                Ap(<input type="submit" value="Send!"/>, <br/>)
        Console.WriteLine(f.ToString())
        Dim result = f.Run(New Dictionary(Of String, String) From
                           {
                               {"f0", "something"},
                               {"f1", "else"}
                           })
        Assert.False(FSharpOptionExtensions.HasValue(result.Value))
        Console.WriteLine()
        Console.WriteLine("Error form:")
        Console.WriteLine(result.ErrorForm.Render())
    End Sub
End Class
