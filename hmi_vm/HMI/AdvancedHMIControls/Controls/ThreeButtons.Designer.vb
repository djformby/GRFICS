<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ThreeButtons
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.HandButton = New System.Windows.Forms.Button
        Me.AutoButton = New System.Windows.Forms.Button
        Me.OffButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'HandButton
        '
        Me.HandButton.Location = New System.Drawing.Point(3, 3)
        Me.HandButton.Name = "HandButton"
        Me.HandButton.Size = New System.Drawing.Size(112, 48)
        Me.HandButton.TabIndex = 0
        Me.HandButton.Text = "Hand"
        Me.HandButton.UseVisualStyleBackColor = True
        '
        'AutoButton
        '
        Me.AutoButton.Location = New System.Drawing.Point(3, 57)
        Me.AutoButton.Name = "AutoButton"
        Me.AutoButton.Size = New System.Drawing.Size(112, 48)
        Me.AutoButton.TabIndex = 1
        Me.AutoButton.Text = "Auto"
        Me.AutoButton.UseVisualStyleBackColor = True
        '
        'OffButton
        '
        Me.OffButton.Location = New System.Drawing.Point(3, 111)
        Me.OffButton.Name = "OffButton"
        Me.OffButton.Size = New System.Drawing.Size(112, 48)
        Me.OffButton.TabIndex = 2
        Me.OffButton.Text = "Off"
        Me.OffButton.UseVisualStyleBackColor = True
        '
        'ThreeButtons
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.OffButton)
        Me.Controls.Add(Me.AutoButton)
        Me.Controls.Add(Me.HandButton)
        Me.Name = "ThreeButtons"
        Me.Size = New System.Drawing.Size(150, 165)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents HandButton As System.Windows.Forms.Button
    Friend WithEvents AutoButton As System.Windows.Forms.Button
    Friend WithEvents OffButton As System.Windows.Forms.Button

End Class
