Public NotInheritable Class Utilities
    Public Sub New()
    End Sub


    Public Shared Function DynamicConverter(ByVal value As String, ByVal t As Type) As Object
        If t = GetType(Boolean) Then
            Dim boolValue As Boolean
            If (Boolean.TryParse(value, boolValue)) Then
                Return boolValue
            Else
                Dim intValue As Integer
                If (Integer.TryParse(value, intValue)) Then
                    Return System.Convert.ChangeType(intValue, t)
                Else
                    Throw New Exception("Invalid Conversion of " & value)
                End If
            End If
        Else
            Return Convert.ChangeType(value, t)
        End If
    End Function
End Class
