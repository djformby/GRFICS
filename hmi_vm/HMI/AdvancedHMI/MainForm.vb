Public Class MainForm
    '*******************************************************************************
    '* Stop polling when the form is not visible in order to reduce communications
    '* Copy this section of code to every new form created
    '*******************************************************************************
    Private NotFirstShow As Boolean

    Private Sub Form_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        '* Do not start comms on first show in case it was set to disable in design mode
        If NotFirstShow Then
            AdvancedHMIDrivers.Utilities.StopComsOnHidden(components, Me)
        Else
            NotFirstShow = True
        End If
    End Sub

    '***************************************************************
    '* .NET does not close hidden forms, so do it here
    '* to make sure forms are disposed and drivers close
    '***************************************************************
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim index As Integer
        While index < My.Application.OpenForms.Count
            If My.Application.OpenForms(index) IsNot Me Then
                My.Application.OpenForms(index).Close()
            End If
            index += 1
        End While
    End Sub

    Private Sub ModbusTCPCom1_DataReceived(sender As Object, e As Drivers.Common.PlcComEventArgs) Handles ModbusTCPCom1.DataReceived

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub PneumaticBallValve1_Click(sender As Object, e As EventArgs) Handles PneumaticBallValve1.Click

    End Sub

    Private Sub Pipe1_Click(sender As Object, e As EventArgs) Handles Pipe1.Click

    End Sub

    Private Sub PneumaticBallValve2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Tank1_Click(sender As Object, e As EventArgs) Handles Tank1.Click

    End Sub

    Private Sub Pipe4_Click(sender As Object, e As EventArgs) Handles Pipe4.Click

    End Sub

    Private Sub BasicLabel1_Click(sender As Object, e As EventArgs) Handles BasicLabel1.Click

    End Sub

    Private Sub MomentaryButton1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub BasicLabel6_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Pipe8_Click(sender As Object, e As EventArgs) Handles Pipe8.Click

    End Sub

    Private Sub DigitalPanelMeter1_Click(sender As Object, e As EventArgs) Handles DigitalPanelMeter1.Click

    End Sub

    Private Sub BasicLabel3_Click(sender As Object, e As EventArgs) Handles BasicLabel3.Click

    End Sub

    Private Sub KeyboardInput1_TextChanged(sender As Object, e As EventArgs) Handles KeyboardInput1.TextChanged

    End Sub

    Private Sub PilotLight1_Click(sender As Object, e As EventArgs) Handles PilotLight1.Click

    End Sub
End Class
