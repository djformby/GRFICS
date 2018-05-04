Public Class MainMenu
    '***************************************************************************************
    '* Close all forms when the exit button is clicked in order to close all communications
    '***************************************************************************************
    Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim index As Integer = 0
        While index < My.Application.OpenForms.Count
            If My.Application.OpenForms(index) IsNot Me Then
                My.Application.OpenForms(index).Close()
            End If
            index += 1
        End While

        Me.Close()
    End Sub

    '***************************************************************************************
    '* Open an initial form as designated by the OpenOnStartup property of the button
    '***************************************************************************************
    Private Sub MainMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim index As Integer
        While index < Me.Controls.Count
            If TypeOf Me.Controls(index) Is MainMenuButton Then
                If DirectCast(Me.Controls(index), MainMenuButton).OpenOnStartup Then
                    DirectCast(Me.Controls(index), MainMenuButton).PerformClick()
                    Exit While
                End If
            End If
            index += 1
        End While
    End Sub
End Class