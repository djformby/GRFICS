'*****************************************************************************
'* Simple Sound Player
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 26-JAN-15
'* http://www.advancedhmi.com
'*
'* This component subscribes to a value in the PLC through a com driver
'* and monitors it for a change. When it changes, it will play a WAV file
'*
'* 26-JAN-15 Created
'*****************************************************************************
Public Class SoundPlayer
    Inherits DataSubscriber

 
#Region "Properties"
    Private m_FileFolder As String = "C:\Windows\Media\"
    <System.ComponentModel.BrowsableAttribute(True), System.ComponentModel.EditorAttribute(GetType(FileFolderEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    Public Property FileFolder As String
        Get
            Return m_FileFolder
        End Get
        Set(value As String)
            If value.Length > 0 Then
                '* Remove the last back slash if it is there
                If value.Substring(value.Length - 1, 1) = "\" Then value = value.Substring(0, value.Length - 1)
                m_FileFolder = value
            End If
        End Set
    End Property

    Private m_SoundFileName As String = "tada.wav"
    Public Property SoundFileName As String
        Get
            Return m_SoundFileName
        End Get
        Set(value As String)
            If m_SoundFileName <> value Then
                m_SoundFileName = value
            End If
        End Set
    End Property

    Public Enum TriggerTypeOptions
        PositiveChange
        NegativeChange
        AnyChange
    End Enum
    Private m_TriggerType As TriggerTypeOptions = TriggerTypeOptions.AnyChange
    Public Property TriggerType As TriggerTypeOptions
        Get
            Return m_TriggerType
        End Get
        Set(value As TriggerTypeOptions)
            m_TriggerType = value
        End Set
    End Property

    Private m_Enabled As Boolean = True
    Public Property Enabled As Boolean
        Get
            Return m_Enabled
        End Get
        Set(value As Boolean)
            m_Enabled = value
        End Set
    End Property
#End Region

#Region "Constructor/Destructor"
    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub
#End Region

#Region "Events"
    Private LastValue As Boolean
    Protected Overrides Sub onDataChanged(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        MyBase.OnDataChanged(e)

        If e.Values IsNot Nothing AndAlso e.Values.Count > 0 Then
            If m_TriggerType = TriggerTypeOptions.AnyChange Then
                If m_Enabled Then PlaySound()
            Else
                Dim NewValue As Boolean
                Try
                    '* convert the value to Boolean so we can look for rising/falling edges
                    NewValue = Utilities.DynamicConverter(e.Values(0), GetType(Boolean))
                    If (m_TriggerType = TriggerTypeOptions.PositiveChange And NewValue And Not LastValue) Or _
                        (m_TriggerType = TriggerTypeOptions.NegativeChange And Not NewValue And LastValue) Then
                        If m_Enabled Then PlaySound()
                    End If
                    LastValue = NewValue
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show("Failed to convert " & e.Values(0) & " to Boolean")
                End Try
            End If

            m_Value = e.Values(0)
        End If
    End Sub

    '* When the subscription with the PLC succeeded
    Protected Overrides Sub OnSuccessfulSubscription(e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        MyBase.OnSuccessfulSubscription(e)
    End Sub

    Private player As System.Media.SoundPlayer
    Private Sub PlaySound()
        Try
            If player Is Nothing Then
                player = New System.Media.SoundPlayer(m_FileFolder & "\" & m_SoundFileName)
            End If
            player.Play()
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message)
        End Try
    End Sub
#End Region
End Class

