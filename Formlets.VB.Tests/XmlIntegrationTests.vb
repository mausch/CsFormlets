Imports System.Text.RegularExpressions
Imports Formlets.CSharp
Imports Xunit
Imports FSharpx.FSharpOption

Public Class XmlIntegrationTests

    Private Shared Function IsValidDate(month As Integer, day As Integer, year As Integer) As Boolean
        ' Sadly, DateTime.TryCreate is internal
        Try
            Dim k = New DateTime(year, month, day)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <Fact()> _
    Public Sub Test()
        Dim e = New Formlets.CSharp.FormElements()
        Dim inputDate =
            Function()
                Dim values =
                    Formlet.Tuple3(Of Integer, Integer, Integer)().
                        Ap(e.Int().WithLabel("Month: ")).
                        Ap(e.Int().WithLabel("Day: ")).
                        Ap(e.Int().WithLabel("Year: "))

                Return From v In values
                        Where IsValidDate(month:=v.Item1, day:=v.Item2, year:=v.Item3)
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
        Assert.False(result.Value.HasValue)
        Console.WriteLine()
        Console.WriteLine("Error form:")
        Console.WriteLine(result.Form.Render())
    End Sub
End Class
