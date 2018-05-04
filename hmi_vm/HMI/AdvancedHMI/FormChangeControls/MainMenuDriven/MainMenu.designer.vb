<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainMenu
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.MainMenuButton8 = New MfgControl.AdvancedHMI.MainMenuButton()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(2, 570)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(145, 21)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Exit"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'MainMenuButton8
        '
        Me.MainMenuButton8.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.MainMenuButton8.ComComponent = Nothing
        Me.MainMenuButton8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MainMenuButton8.ForeColor = System.Drawing.Color.Black
        Me.MainMenuButton8.FormToOpen = GetType(MfgControl.AdvancedHMI.MainForm)
        Me.MainMenuButton8.KeypadWidth = 300
        Me.MainMenuButton8.Location = New System.Drawing.Point(2, 12)
        Me.MainMenuButton8.Name = "MainMenuButton8"
        Me.MainMenuButton8.OpenOnStartup = True
        Me.MainMenuButton8.Passcode = Nothing
        Me.MainMenuButton8.PasswordChar = False
        Me.MainMenuButton8.PLCAddressVisible = ""
        Me.MainMenuButton8.Size = New System.Drawing.Size(145, 62)
        Me.MainMenuButton8.TabIndex = 8
        Me.MainMenuButton8.Text = "Startup Form"
        Me.MainMenuButton8.UseVisualStyleBackColor = False
        '
        'MainMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Silver
        Me.ClientSize = New System.Drawing.Size(150, 600)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.MainMenuButton8)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "MainMenu"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "MainMenu"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MainMenuButton10 As MainMenuButton
    Friend WithEvents MainMenuButton9 As MainMenuButton
    Friend WithEvents MainMenuButton8 As MainMenuButton
    Friend WithEvents MainMenuButton7 As MainMenuButton
    Friend WithEvents MainMenuButton6 As MainMenuButton
    Friend WithEvents MainMenuButton5 As MainMenuButton
    Friend WithEvents MainMenuButton4 As MainMenuButton
    Friend WithEvents MainMenuButton3 As MainMenuButton
    Friend WithEvents MainMenuButton2 As MainMenuButton
    Friend WithEvents MainMenuButton1 As MainMenuButton
    Friend WithEvents Button1 As Button
End Class
