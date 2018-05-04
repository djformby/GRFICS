Public Class ThreeButtons
    Public Property Button1Text() As String
        Get
            Return HandButton.Text
        End Get
        Set(ByVal value As String)
            HandButton.Text = value
        End Set
    End Property

    Public Property Button2Text() As String
        Get
            Return AutoButton.Text
        End Get
        Set(ByVal value As String)
            AutoButton.Text = value
        End Set
    End Property

    Public Property Button3Text() As String
        Get
            Return OffButton.Text
        End Get
        Set(ByVal value As String)
            OffButton.Text = value
        End Set
    End Property

    Public Overrides Property Text() As String
        Get
            Return MyBase.Text
        End Get
        Set(ByVal value As String)
            MyBase.Text = value
        End Set
    End Property

End Class
