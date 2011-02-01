Imports Formlets.CSharp
Imports System.Runtime.CompilerServices

Module FormletExtensions
    <Extension()> _
    Public Function Validate(Of T)(ByVal formlet As Formlet(Of T), ByVal isValid As Func(Of T, Boolean), ByVal msg As Func(Of T, String)) As Formlet(Of T)
        Return formlet.
           Satisfies(isValid,
                     Function(s, x) x.
                       Append(<span class="errorMsg">'<%= msg(s) %>'</span>).
                       WrapWith(<span class="error"/>))
    End Function

End Module
