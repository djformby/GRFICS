Public Class KeyboardInput
    Inherits MfgControl.AdvancedHMI.Controls.KeyboardInput

    Public Event ValueChanged As EventHandler


#Region "Constructor"
    Public Sub New()
        MyBase.New()

        Me.ClearAfterEnterKey = True
    End Sub

    '****************************************************************
    '* UserControl overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If SubScriptions IsNot Nothing Then
                    SubScriptions.dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
#End Region

#Region "PLC Related Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property ComComponent() As MfgControl.AdvancedHMI.Drivers.IComComponent
        Get
            Return m_ComComponent
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.IComComponent)
            If m_ComComponent IsNot value Then
                If SubScriptions IsNot Nothing Then
                    SubScriptions.UnsubscribeAll()
                    SubScriptions.ComComponent = value
                End If

                m_ComComponent = value

                SubscribeToComDriver()
            End If
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressWriteValue As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressWriteValue() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressWriteValue
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressWriteValue IsNot value Then
                m_PLCAddressWriteValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private m_PLCAddressGetFocusValue As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressGetFocusValue() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressGetFocusValue
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressGetFocusValue IsNot value Then
                m_PLCAddressGetFocusValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressVisible As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressVisible() As String
        Get
            Return m_PLCAddressVisible
        End Get
        Set(ByVal value As String)
            If m_PLCAddressVisible <> value Then
                m_PLCAddressVisible = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property
#End Region

#Region "Events"
    '********************************************************************
    '* When an instance is added to the form, set the comm component
    '* property. If a comm component does not exist, add one to the form
    '********************************************************************
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        If Me.DesignMode Then
            '********************************************************
            '* Search for AdvancedHMIDrivers.IComComponent component in parent form
            '* If one exists, set the client of this component to it
            '********************************************************
            Dim i As Integer = 0
            While m_ComComponent Is Nothing And i < Me.Site.Container.Components.Count
                If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then m_ComComponent = Me.Site.Container.Components(i)
                i += 1
            End While

            '************************************************
            '* If no com component was found, then add one and
            '* point the ComComponent property to it
            '*********************************************
            If m_ComComponent Is Nothing Then
                m_ComComponent = New AdvancedHMIDrivers.EthernetIPforCLXCom(Me.Site.Container)
            End If
        Else
            SubscribeToComDriver()
        End If
    End Sub

    Protected Overrides Sub OnEnterKeyPressed(e As EventArgs)
        MyBase.OnEnterKeyPressed(e)

        Try
            If m_PLCAddressWriteValue IsNot Nothing Then
                If m_PLCAddressWriteValue.ScaleFactor = 1 Or m_PLCAddressWriteValue.ScaleFactor = 0 Then
                    m_ComComponent.Write(m_PLCAddressWriteValue.PLCAddress, Me.Text)
                Else
                    m_ComComponent.Write(m_PLCAddressWriteValue.PLCAddress, CDbl(Me.Text) / m_PLCAddressWriteValue.ScaleFactor)
                End If
            Else
                System.Windows.Forms.MessageBox.Show("PLCAddressWriteValue not set to anything")
            End If
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show("Failed to write value - " & ex.Message)
        End Try

    End Sub
#End Region

#Region "Subscribing and PLC data receiving"
    Private SubScriptions As SubscriptionHandler
    '**************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '**************************************************
    Private Sub SubscribeToComDriver()
        If Not DesignMode And IsHandleCreated Then
            '* Create a subscription handler object
            If SubScriptions Is Nothing Then
                SubScriptions = New SubscriptionHandler
                SubScriptions.Parent = Me
                'AddHandler SubScriptions.DisplayError, AddressOf DisplaySubscribeError
            End If
            SubScriptions.ComComponent = m_ComComponent

            SubScriptions.SubscribeAutoProperties()
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As SubscriptionHandlerEventArgs)
    End Sub
#End Region

End Class
