Public Class ComBridge
    Inherits DataSubscriber

#Region "Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponentTarget As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property ComComponentTarget() As MfgControl.AdvancedHMI.Drivers.IComComponent
        Get
            Return m_ComComponentTarget
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.IComComponent)
            If m_ComComponentTarget IsNot value Then
                m_ComComponentTarget = value
            End If
        End Set
    End Property

    Private m_PLCAddressValueTarget As String
    Public Property PLCAddressValueTarget As String
        Get
            Return m_PLCAddressValueTarget
        End Get
        Set(value As String)
            m_PLCAddressValueTarget = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Overrides Sub OnDataChanged(e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        MyBase.OnDataChanged(e)

        If m_ComComponentTarget IsNot Nothing And Not String.IsNullOrEmpty(m_PLCAddressValueTarget) Then
            If e IsNot Nothing AndAlso e.Values IsNot Nothing AndAlso e.Values.Count > 0 Then
                Try
                    m_ComComponentTarget.Write(m_PLCAddressValueTarget, e.Values(0))
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub
#End Region
End Class
