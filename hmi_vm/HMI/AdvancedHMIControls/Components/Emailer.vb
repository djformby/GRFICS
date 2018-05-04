Option Strict On
'*****************************************************************************
'* Emailer
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 09-JUN-16
'* http://www.advancedhmi.com
'*
'* This component is used to send emails
'*
'*****************************************************************************
Imports System.ComponentModel

Public Class Emailer
    Inherits MfgControl.AdvancedHMI.Controls.EmailerEx

    Implements System.ComponentModel.IComponent
    Implements ISupportInitialize

    Public Event ComError As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event SuccessfulSubscription As EventHandler

    Protected m_synchronizationContext As System.Threading.SynchronizationContext

#Region "Basic Properties"
    Public Overrides Property Site() As ISite
        Get
            Return MyBase.Site
        End Get
        Set(value As ISite)
            MyBase.Site = value

            If (value IsNot Nothing) And m_ComComponent Is Nothing Then
                If MyBase.Site.DesignMode Then
                    '********************************************************
                    '* Search for AdvancedHMIDrivers.IComComponent component in parent form
                    '* If one exists, set the client of this component to it
                    '********************************************************
                    Dim i As Integer = 0
                    While m_ComComponent Is Nothing And i < Me.Site.Container.Components.Count
                        If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then m_ComComponent = DirectCast(Me.Site.Container.Components(i), MfgControl.AdvancedHMI.Drivers.IComComponent)
                        i += 1
                    End While

                    '************************************************
                    '* If no comm component was found, then add one and
                    '* point the ComComponent property to it
                    '*********************************************
                    If m_ComComponent Is Nothing Then
                        m_ComComponent = New AdvancedHMIDrivers.EthernetIPforCLXCom(Me.Container)
                    End If
                End If
            End If
        End Set
    End Property

    Private m_ErrorMessageDisplayLabel As Label
    Public Property ErrorMessageDisplayLabel As Label
        Get
            Return m_ErrorMessageDisplayLabel
        End Get
        Set(value As Label)
            m_ErrorMessageDisplayLabel = value
        End Set
    End Property

#End Region

#Region "Constructor/Destructor"
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        If (container IsNot Nothing) Then
            container.Add(Me)
        End If
    End Sub

    Public Sub New()
        MyBase.New()

        m_synchronizationContext = System.Windows.Forms.WindowsFormsSynchronizationContext.Current
    End Sub

    '****************************************************************
    '* Component overrides dispose to clean up the component list.
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
                End If

                m_ComComponent = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

    'Private _PollRate As Integer
    'Public Property PollRate() As Integer
    '    Get
    '        Return _PollRate
    '    End Get
    '    Set(ByVal value As Integer)
    '        _PollRate = value
    '    End Set
    'End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValue As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressValue() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressValue
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressValue IsNot value Then
                m_PLCAddressValue = value

                '* When address is changed, re-subscribe to new address
                If Not Initializing Then
                    SubscribeToComDriver()
                End If
            End If
        End Set
    End Property

    Private SubscribedValueList As New System.Collections.Concurrent.ConcurrentDictionary(Of String, String)
    '* Do this to hide it from the Property Window
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property SubscribedValues As System.Collections.Concurrent.ConcurrentDictionary(Of String, String)
        Get
            Return SubscribedValueList
        End Get
    End Property
#End Region

#Region "Events"
    Private Initializing As Boolean
    Public Sub BeginInit() Implements ISupportInitialize.BeginInit
        Initializing = True
    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit
        Initializing = False

        If m_ComComponent IsNot Nothing Then
            SubscribeToComDriver()
        End If
    End Sub


    Protected Overrides Sub OnValueChanged(e As EventArgs)
        If m_synchronizationContext IsNot Nothing Then
            m_synchronizationContext.Post(AddressOf ValueChangedSync, e)
        Else
            MyBase.OnValueChanged(e)
        End If
    End Sub

    Private Sub ValueChangedSync(ByVal e As Object)
        Try
            Dim e1 As EventArgs = DirectCast(e, EventArgs)
            MyBase.OnValueChanged(e1)
        Catch ex As Exception
            OnSendingError(New MfgControl.AdvancedHMI.Controls.Common.BaseEventArgs(-201, ex.Message))
            'Dim dbg = 0
        End Try
    End Sub

    Protected Overrides Sub OnSendingError(e As MfgControl.AdvancedHMI.Controls.Common.BaseEventArgs)
        MyBase.OnSendingError(e)

        If m_synchronizationContext IsNot Nothing Then
            m_synchronizationContext.Post(AddressOf SendingErrorSync, e)
            'Else
            '    MyBase.OnValueChanged(e)
        End If
    End Sub


    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    'Dim dcsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataChangedSync
    Private Sub SendingErrorSync(ByVal e As Object)
        Try
            Dim e1 As MfgControl.AdvancedHMI.Controls.Common.BaseEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Controls.Common.BaseEventArgs)
            m_ErrorMessageDisplayLabel.Text = e1.ErrorMessage
        Catch ex As Exception
            'Dim dbg = 0
        End Try
    End Sub


    Protected Overridable Sub OnSuccessfulSubscription(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent SuccessfulSubscription(Me, e)
    End Sub

    Protected Overridable Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent ComError(Me, e)
    End Sub
#End Region

#Region "Subscribing and PLC data receiving"
    Private InvertValue As Boolean
    Private SubScriptions As SubscriptionHandler
    '**************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '**************************************************
    Protected Sub SubscribeToComDriver()
        If Not DesignMode Then
            'If (m_SynchronizingObject Is Nothing OrElse DirectCast(m_SynchronizingObject, Control).IsHandleCreated) Then
            '* Create a subscription handler object
            If SubScriptions Is Nothing Then
                SubScriptions = New SubscriptionHandler

                SubScriptions.Parent = Me
                AddHandler SubScriptions.DisplayError, AddressOf DisplaySubscribeError
            End If
            SubScriptions.ComComponent = m_ComComponent

            If m_PLCAddressValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(m_PLCAddressValue.PLCAddress) Then
                Dim AddressList() As String
                AddressList = m_PLCAddressValue.PLCAddress.Split(","c)

                For i = 0 To AddressList.Count - 1
                    If Not SubscribedValueList.ContainsKey(AddressList(i)) Then
                        '* We must pass the address as a property name so the subscriptionHandler doesn't confuse the next address as a change for the same property
                        SubScriptions.SubscribeTo(AddressList(i), 1, AddressOf PolledDataReturned, AddressList(i), 1, 0)

                        SubscribedValueList.TryAdd(AddressList(i), "")

                        Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(0, "")
                        x.PlcAddress = AddressList(i)
                        OnSuccessfulSubscription(x)
                    End If
                Next
            End If
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As SubscriptionHandlerEventArgs)
        If e.PLCComEventArgs.ErrorId = 0 Then
            Try
                If String.IsNullOrEmpty(e.SubscriptionDetail.PropertyNameToSet) Or String.Compare(e.SubscriptionDetail.PropertyNameToSet, e.PLCComEventArgs.PlcAddress, True) = 0 Then
                    'If String.Compare(e.SubscriptionDetail.PropertyNameToSet, "value", True) = 0 Then
                    If Value <> e.PLCComEventArgs.Values(0) Then
                        Value = e.PLCComEventArgs.Values(0)
                    End If
                    'End If
                    '    PolledDataReturnedValue(sender, e.PLCComEventArgs)
                    'ElseIf e.SubscriptionDetail.PropertyNameToSet = "Value" Then
                    '    PolledDataReturnedValue(sender, e.PLCComEventArgs)
                Else
                    '* Write the value to the property that came from the end of the PLCAddress... property name
                    Try
                        '* Write the value to the property that came from the end of the PLCAddress... property name
                        Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet). _
                                    SetValue(Me, Utilities.DynamicConverter(e.PLCComEventArgs.Values(0), _
                                                Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet).PropertyType), Nothing)
                    Catch ex As Exception
                        DisplayError("INVALID VALUE RETURNED!" & e.PLCComEventArgs.Values(0))
                    End Try
                End If
            Catch ex As Exception
                DisplayError("INVALID VALUE!" & ex.Message)
            End Try
        Else
            DisplayError("Com Error " & e.PLCComEventArgs.ErrorId & "." & e.PLCComEventArgs.ErrorMessage)
        End If
    End Sub


    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
    End Sub
#End Region

#Region "Error Display"
    '********************************************************
    '* Show an error via the text property for a short time
    '********************************************************
    Private WithEvents ErrorDisplayTime As System.Windows.Forms.Timer
    Private Sub DisplayError(ByVal ErrorMessage As String)
        If ErrorDisplayTime Is Nothing Then
            ErrorDisplayTime = New System.Windows.Forms.Timer
            AddHandler ErrorDisplayTime.Tick, AddressOf ErrorDisplay_Tick
            ErrorDisplayTime.Interval = 5000
        End If

        '* Save the text to return to
        If Not ErrorDisplayTime.Enabled Then
            ' OriginalText = Me.Text
        End If

        OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(1, ErrorMessage))

        ErrorDisplayTime.Enabled = True

        If m_ErrorMessageDisplayLabel IsNot Nothing Then
            m_ErrorMessageDisplayLabel.Text = ErrorMessage
        End If
    End Sub


    '**************************************************************************************
    '* Return the text back to its original after displaying the error for a few seconds.
    '**************************************************************************************
    Private Sub ErrorDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If ErrorDisplayTime IsNot Nothing Then
            ErrorDisplayTime.Enabled = False
            ErrorDisplayTime.Dispose()
            ErrorDisplayTime = Nothing
        End If
    End Sub
#End Region

End Class
