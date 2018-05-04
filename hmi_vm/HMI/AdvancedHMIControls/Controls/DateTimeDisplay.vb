Public Class DateTimeDisplay
    Inherits Label

    Private UpdateTimer As Windows.Forms.Timer

#Region "Properties"
    Private m_DisplayFormat As String = "MM/dd/yyyy hh:mm:ss"
    Public Property DisplayFormat As String
        Get
            Return m_DisplayFormat
        End Get
        Set(value As String)
            m_DisplayFormat = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()

        If (MyBase.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText) Or ForeColor = Color.FromArgb(0, 0, 0)) Then
            ForeColor = System.Drawing.Color.WhiteSmoke
        End If

        UpdateTimer = New Windows.Forms.Timer
        UpdateTimer.Interval = 1000
        AddHandler UpdateTimer.Tick, AddressOf UpdateTick

        UpdateTick(Me, System.EventArgs.Empty)
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If UpdateTimer IsNot Nothing Then
                UpdateTimer.Enabled = False
                RemoveHandler UpdateTimer.Tick, AddressOf UpdateTick
            End If
        End If
    End Sub
#End Region

#Region "Events"
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        If Me.DesignMode Then
            UpdateTick(Me, System.EventArgs.Empty)
        Else
            UpdateTimer.Enabled = True
        End If
    End Sub
    Private Sub UpdateTick(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrEmpty(m_DisplayFormat) Then
            Me.Text = Date.Now
        Else
            Me.Text = Date.Now.ToString(m_DisplayFormat)
        End If
    End Sub
#End Region
End Class
