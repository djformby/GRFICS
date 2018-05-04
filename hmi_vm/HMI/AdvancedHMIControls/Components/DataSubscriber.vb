Option Strict On
'*****************************************************************************
'* Data Subscriber
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* 03-MAR-13
'* http://www.advancedhmi.com
'*
'* This component is used to simplify the creation of subscriptions
'*
'* 03-MAR-13 Created
'*****************************************************************************
Imports System.ComponentModel

<DefaultEvent("DataChanged")> _
Public Class DataSubscriber
    Inherits System.ComponentModel.Component
    Implements ISupportInitialize

    Public Event DataReturned As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event DataChanged As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
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

    ''**************************************************
    ''* Its purpose is to fetch
    ''* the main form in order to synchronize the
    ''* notification thread/event
    ''**************************************************
    'Protected m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
    ''* do not let this property show up in the property window
    '' <System.ComponentModel.Browsable(False)> _
    'Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
    '    Get
    '        Dim host1 As Design.IDesignerHost
    '        If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
    '            host1 = CType(Me.GetService(GetType(Design.IDesignerHost)), Design.IDesignerHost)
    '            If host1 IsNot Nothing Then
    '                m_SynchronizingObject = CType(host1.RootComponent, System.ComponentModel.ISynchronizeInvoke)
    '            End If
    '        End If

    '        Return m_SynchronizingObject
    '    End Get

    '    Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
    '        If Not Value Is Nothing Then
    '            m_SynchronizingObject = Value
    '        End If
    '    End Set
    'End Property

    Protected m_Value As String
    <System.ComponentModel.Browsable(False)> _
    Public Property Value As String
        Get
            Return m_Value
        End Get
        Set(value As String)
            m_Value = value
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


    Protected Overridable Sub OnDataReturned(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If m_synchronizationContext IsNot Nothing Then
            m_synchronizationContext.Post(AddressOf DataReceivedSync, e)
        Else
            OnDataReceived(e)
        End If

        'If m_SynchronizingObject Is Nothing Then
        '    RaiseEvent DataReturned(Me, e)
        'Else
        '    If DirectCast(m_SynchronizingObject, Control).IsHandleCreated Then
        '        Dim Parameters() As Object = {Me, e}
        '        SynchronizingObject.BeginInvoke(drsd, Parameters)
        '    End If
        'End If
    End Sub

    Private Sub DataReceivedSync(ByVal e As Object)
        Try
            Dim e1 As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            OnDataReceived(e1)
        Catch ex As Exception
            'Dim dbg = 0
        End Try
    End Sub

    Protected Overridable Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataReturned(Me, e)
    End Sub


    ''****************************************************************************
    ''* This is required to sync the event back to the parent form's main thread
    ''****************************************************************************
    ''Dim drsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataReturnedSync
    'Private Sub DataReturnedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    '    RaiseEvent DataReturned(Me, e)
    'End Sub


    Protected Overridable Sub OnDataChanged(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataChanged(Me, e)


        'If m_synchronizationContext IsNot Nothing Then
        '    m_synchronizationContext.Post(AddressOf DataChangedSync, e)
        'Else
        '    OnDataChanged(e)
        'End If

        'If m_SynchronizingObject Is Nothing Then
        '    RaiseEvent DataChanged(Me, e)
        'Else
        '    If DirectCast(m_SynchronizingObject, Control).IsHandleCreated Then
        '        Dim Parameters() As Object = {Me, e}
        '        SynchronizingObject.BeginInvoke(dcsd, Parameters)
        '    End If
        'End If
    End Sub

    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    'Dim dcsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataChangedSync
    Private Sub DataChangedSync(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            Dim e1 As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            OnDataChanged(e1)
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
                    PolledDataReturnedValue(sender, e.PLCComEventArgs)
                ElseIf e.SubscriptionDetail.PropertyNameToSet = "Value" Then
                    PolledDataReturnedValue(sender, e.PLCComEventArgs)
                Else
                    '* Write the value to the property that came from the end of the PLCAddress... property name
                    Try
                        '* Write the value to the property that came from the end of the PLCAddress... property name
                        Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet). _
                                    SetValue(Me, Utilities.DynamicConverter(e.PLCComEventArgs.Values(0), _
                                                Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet).PropertyType), Nothing)
                    Catch ex As Exception
                        'OnDisplayError("INVALID VALUE RETURNED!" & a.PLCComEventArgs.Values(0))
                    End Try

                    'Dim pi As System.Reflection.PropertyInfo
                    'pi = Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet)
                    'Dim value As Object
                    'value = Convert.ChangeType(e.PLCComEventArgs.Values(0), Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet).PropertyType)
                    'pi.SetValue(Me, value, Nothing)
                    ''* Write the value to the property that came from the end of the PLCAddress... property name
                    ''Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet). _
                    '' SetValue(Me, Utilities.DynamicConverter(e.PLCComEventArgs.Values(0), _
                    '' Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet).PropertyType), Nothing)
                End If
            Catch ex As Exception
                DisplayError("INVALID VALUE!" & ex.Message)
            End Try
        Else
            DisplayError("Com Error " & e.PLCComEventArgs.ErrorId & "." & e.PLCComEventArgs.ErrorMessage)
        End If
    End Sub


    '***************************************
    '* Call backs for returned data
    '***************************************
    Private LastValue As String
    Private Sub PolledDataReturnedValue(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            '* Fire this event every time data is returned
            OnDataReturned(e)

            '* Case may be switched so find key based on that
            Dim TargetKey As String = ""
            For Each key In SubscribedValueList.Keys
                If String.Compare(key, e.PlcAddress, True) = 0 Then
                    TargetKey = key
                End If
            Next


            If e.Values(0) <> SubscribedValueList(TargetKey) Then
                '* Save this value so we know if it changed without comparing the invert
                SubscribedValueList(e.PlcAddress) = e.Values(0)

                If InvertValue Then
                    m_Value = Convert.ToString(Not CBool(e.Values(0)))
                Else
                    m_Value = e.Values(0)
                End If

                '* This event is only fired when the returned data has changed
                OnDataChanged(e)
            End If

        Catch ex As Exception
            DisplayError("INVALID VALUE RETURNED!")
        End Try
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
