Public NotInheritable Class Utilities
    Private Sub New()

    End Sub

    Public Shared Sub StopComsOnHidden(ByVal components As System.ComponentModel.IContainer, ByVal form As System.Windows.Forms.Control)
        '* V3.97d - moved this to a shared sub to reduce code in the form
        If components IsNot Nothing Then
            Dim drv As MfgControl.AdvancedHMI.Drivers.IComComponent
            '*****************************
            '* Search for comm components
            '*****************************
            For i As Integer = 0 To components.Components.Count - 1
                If components.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then
                    drv = DirectCast(components.Components.Item(i), MfgControl.AdvancedHMI.Drivers.IComComponent)
                    '* Stop/Start polling based on form visibility
                    drv.DisableSubscriptions = Not form.Visible
                End If
            Next
        End If
    End Sub

    Public Shared Sub SetPropertiesByIniFile(ByVal targetObject As Object, ByVal iniFileName As String, ByVal iniFileSection As String)
        If Not String.IsNullOrEmpty(iniFileName) Then
            Dim p As New IniParser(iniFileName)
            Dim settings() As String = p.EnumSection(iniFileSection)
            For index = 0 To settings.Length - 1
                '* Write the value to the property that came from the end of the PLCAddress... property name
                Dim pi As System.Reflection.PropertyInfo
                pi = targetObject.GetType().GetProperty(settings(index), Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                If pi IsNot Nothing Then
                    Dim value As Object
                    value = Convert.ChangeType(p.GetSetting(iniFileSection, settings(index)), targetObject.GetType().GetProperty(pi.Name).PropertyType, Globalization.CultureInfo.InvariantCulture)
                    pi.SetValue(targetObject, value, Nothing)
                Else
                    System.Windows.Forms.MessageBox.Show("Ini File Error - " & settings(index) & " is not a valid property.")
                End If
            Next
            'Dim x = p.GetSetting(iniFileSection, settings(0))
        End If
    End Sub


    Public Shared Function IsImplemented(objectType As Type, interfaceType As Type) As Boolean
        For Each thisInterface As Type In objectType.GetInterfaces
            If thisInterface Is interfaceType Then
                Return True
            End If
        Next

        Return False
    End Function
End Class
