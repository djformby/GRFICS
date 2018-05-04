'*********************************************************************************
'* Ref: http://stackoverflow.com/questions/416897/how-do-i-rotate-a-label-in-c
'*********************************************************************************

Public Class OrientedTextLabel
    Inherits System.Windows.Forms.Label

#Region "Enum Orientation/Direction"

    'Orientation of the text
    Public Enum Orientation
        Circle
        Arc
        Rotate
    End Enum

    Public Enum Direction
        Clockwise
        AntiClockwise
        Mirrored
    End Enum

#End Region

#Region "Variables"

    Private m_textOrientation As Orientation = Orientation.Rotate
    Private m_textDirection As Direction = Direction.Clockwise

#End Region

#Region "Properties"

    Private m_rotationAngle As Double
    <System.ComponentModel.Description("Rotation Angle"), System.ComponentModel.Category("Appearance")>
    Public Property RotationAngle() As Double
        Get
            Return Me.m_rotationAngle
        End Get
        Set(value As Double)
            Me.m_rotationAngle = value
            Me.Invalidate()
        End Set
    End Property

    <System.ComponentModel.Description("Kind of Text Orientation"), System.ComponentModel.Category("Appearance")>
    Public Property TextOrientation() As Orientation
        Get
            Return Me.m_textOrientation
        End Get
        Set(value As Orientation)
            Me.m_textOrientation = value
            If Me.m_textOrientation = Orientation.Arc OrElse Me.m_textOrientation = Orientation.Circle Then
                Me.OrientedTextLabel_Resize(Me, Nothing)
            End If
            Me.Invalidate()
        End Set
    End Property

    <System.ComponentModel.Description("Direction of the Text"), System.ComponentModel.Category("Appearance")>
    Public Property TextDirection() As Direction
        Get
            Return Me.m_textDirection
        End Get
        Set(value As Direction)
            Me.m_textDirection = value
            If Me.m_textOrientation = Orientation.Arc OrElse Me.m_textOrientation = Orientation.Circle Then
                Me.OrientedTextLabel_Resize(Me, Nothing)
            End If
            Me.Invalidate()
        End Set
    End Property

#End Region

#Region "Method"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim graphics As Graphics = e.Graphics

        Dim stringFormat As New StringFormat()
        stringFormat.Alignment = StringAlignment.Center
        stringFormat.Trimming = StringTrimming.None

        Dim textBrush As Brush = New SolidBrush(Me.ForeColor)

        'Getting the width and height of the text, which we are going to write
        Dim width As Single = graphics.MeasureString(Text, Me.Font).Width
        Dim height As Single = graphics.MeasureString(Text, Me.Font).Height

        'The radius is set to 0.9 of the width or height, b'cos not to
        'hide and part of the text at any stage
        Dim radius As Single
        If ClientRectangle.Width < ClientRectangle.Height Then
            radius = ClientRectangle.Height / 2
        Else
            radius = ClientRectangle.Width / 2
        End If

        'Setting the text according to the selection
        Select Case m_textOrientation
            Case Orientation.Arc
                If True Then
                    If ClientRectangle.Width < ClientRectangle.Height Then
                        Me.Width = Me.Height
                    Else
                        Me.Height = Me.Width
                    End If
                    'Arc angle must be get from the length of the text.
                    Dim arcAngle As Single = (2 * width / radius) / Text.Length
                    If m_textDirection = Direction.Clockwise Then
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(radius * (1 - Math.Cos(arcAngle * i + m_rotationAngle / 180 * Math.PI))), CSng(radius * (1 - Math.Sin(arcAngle * i + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform(-90 + CSng(m_rotationAngle) + 180 * arcAngle * i / CSng(Math.PI))
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    ElseIf m_textDirection = Direction.Mirrored Then
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(radius * (1 - Math.Cos(arcAngle * i + m_rotationAngle / 180 * Math.PI))), CSng(radius * (1 + Math.Sin(arcAngle * i + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform((-90 - CSng(m_rotationAngle) - 180 * arcAngle * i / CSng(Math.PI)))
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    Else
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(height + (radius - height) * (1 - Math.Cos(arcAngle * i - Math.PI + m_rotationAngle / 180 * Math.PI))), CSng(height + (radius - height) * (1 + Math.Sin(arcAngle * i - Math.PI + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform(-90 - CSng(m_rotationAngle) - 180 * arcAngle * i / CSng(Math.PI))
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    End If
                    Exit Select
                End If

            Case Orientation.Circle
                If True Then
                    If ClientRectangle.Width < ClientRectangle.Height Then
                        Me.Width = Me.Height
                    Else
                        Me.Height = Me.Width
                    End If
                    If m_textDirection = Direction.Clockwise Then
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(radius * (1 - Math.Cos((2 * Math.PI / Text.Length) * i + m_rotationAngle / 180 * Math.PI))), CSng(radius * (1 - Math.Sin((2 * Math.PI / Text.Length) * i + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform(-90 + CSng(m_rotationAngle) + (360 / Text.Length) * i)
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    ElseIf m_textDirection = Direction.Mirrored Then
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(radius * (1 - Math.Cos((2 * Math.PI / Text.Length) * i + m_rotationAngle / 180 * Math.PI))), CSng(radius * (1 + Math.Sin((2 * Math.PI / Text.Length) * i + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform(-90 - CSng(m_rotationAngle) - (360 / Text.Length) * i)
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    Else
                        For i As Integer = 0 To Text.Length - 1
                            graphics.TranslateTransform(CSng(height + (radius - height) * (1 - Math.Cos((2 * Math.PI / Text.Length) * i - Math.PI + m_rotationAngle / 180 * Math.PI))), CSng(height + (radius - height) * (1 + Math.Sin((2 * Math.PI / Text.Length) * i - Math.PI + m_rotationAngle / 180 * Math.PI))))
                            graphics.RotateTransform(-90 - CSng(m_rotationAngle) - (360 / Text.Length) * i)
                            graphics.DrawString(Text(i).ToString(), Me.Font, textBrush, 0, 0)
                            graphics.ResetTransform()
                        Next
                    End If
                    Exit Select
                End If

            Case Orientation.Rotate
                If True Then
                    'For rotation, who about rotation?
                    Dim angle As Double = (m_rotationAngle / 180) * Math.PI
                    graphics.TranslateTransform((ClientRectangle.Width + CSng(height * Math.Sin(angle)) - CSng(width * Math.Cos(angle))) / 2, (ClientRectangle.Height - CSng(height * Math.Cos(angle)) - CSng(width * Math.Sin(angle))) / 2)
                    graphics.RotateTransform(CSng(m_rotationAngle))
                    graphics.DrawString(Text, Me.Font, textBrush, 0, 0)
                    graphics.ResetTransform()
                    Exit Select
                End If
        End Select
    End Sub

    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        MyBase.AutoSize = False
    End Sub

    Private Sub OrientedTextLabel_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.m_textOrientation = Orientation.Arc OrElse Me.m_textOrientation = Orientation.Circle Then
            Me.Height = Me.Width
        End If
    End Sub

#End Region

End Class

