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
Public Class DataSubscriber2
    Inherits System.ComponentModel.Component
    Implements ISupportInitialize

    Public Event DataReturned As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event DataChanged As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event ComError As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event SuccessfulSubscription As EventHandler

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
                        Dim z = Me.Site.Container.Components(i).GetType.GetInterfaces()
                        Dim x = Me.Site.Container.Components  '(i).GetType.GetInterface("IComComponent")
                        ' If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then
                        If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then
                            m_ComComponent = DirectCast(Me.Site.Container.Components(i), MfgControl.AdvancedHMI.Drivers.IComComponent)
                        End If
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

    '**************************************************
    '* Its purpose is to fetch
    '* the main form in order to synchronize the
    '* notification thread/event
    '**************************************************
    Protected m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
    '* do not let this property show up in the property window
    ' <System.ComponentModel.Browsable(False)> _
    Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        Get
            Dim host1 As Design.IDesignerHost
            If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
                host1 = CType(Me.GetService(GetType(Design.IDesignerHost)), Design.IDesignerHost)
                If host1 IsNot Nothing Then
                    m_SynchronizingObject = CType(host1.RootComponent, System.ComponentModel.ISynchronizeInvoke)
                End If
            End If

            Return m_SynchronizingObject
        End Get

        Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
            If Not Value Is Nothing Then
                m_SynchronizingObject = Value
            End If
        End Set
    End Property

    Protected m_Value As String
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
    End Sub

    '****************************************************************
    '* Component overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If SubScriptions IsNot Nothing Then
                    SubScriptions.UnsubscribeAll()
                    SubScriptions.dispose()
                End If
                RemoveHandler m_PLCAddressValueItems.CollectionChanged, AddressOf SubscribedItemsChanged
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
                    SubScriptions.ComComponent = m_ComComponent
                End If

                m_ComComponent = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private _PollRate As Integer
    Public Property PollRate() As Integer
        Get
            Return _PollRate
        End Get
        Set(ByVal value As Integer)
            _PollRate = value
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValueItems As New System.Collections.ObjectModel.ObservableCollection(Of MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
    <System.ComponentModel.Category("PLC Properties")> _
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)> _
    Public ReadOnly Property PLCAddressValueItems() As System.Collections.ObjectModel.ObservableCollection(Of MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Get
            Return m_PLCAddressValueItems
        End Get
        'Set(ByVal value As String)
        '    If m_PLCAddressValue <> value Then
        '        m_PLCAddressValue = value

        '        '* When address is changed, re-subscribe to new address
        '        If Not Initializing Then
        '            SubscribeToComDriver()
        '        End If
        '    End If
        'End Set
    End Property

    'Private SubscribedValueList As New System.Collections.Concurrent.ConcurrentDictionary(Of String, String)
    'Public ReadOnly Property SubscribedValues As System.Collections.Concurrent.ConcurrentDictionary(Of String, String)
    '    Get
    '        Return SubscribedValueList
    '    End Get
    'End Property
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

        AddHandler m_PLCAddressValueItems.CollectionChanged, AddressOf SubscribedItemsChanged
    End Sub


    Protected Overridable Sub OnDataReturned(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If m_SynchronizingObject Is Nothing Then
            RaiseEvent DataReturned(Me, e)
        Else
            If DirectCast(m_SynchronizingObject, Control).IsHandleCreated Then
                Dim Parameters() As Object = {Me, e}
                SynchronizingObject.BeginInvoke(drsd, Parameters)
            End If
        End If
    End Sub

    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Dim drsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataReturnedSync
    Private Sub DataReturnedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataReturned(Me, e)
    End Sub


    Protected Overridable Sub OnDataChanged(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If m_SynchronizingObject Is Nothing Then
            RaiseEvent DataChanged(Me, e)
        Else
            If DirectCast(m_SynchronizingObject, Control).IsHandleCreated Then
                Dim Parameters() As Object = {Me, e}
                SynchronizingObject.BeginInvoke(dcsd, Parameters)
            End If
        End If
    End Sub

    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Dim dcsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataChangedSync
    Private Sub DataChangedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataChanged(Me, e)
    End Sub


    Protected Overridable Sub OnSuccessfulSubscription(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent SuccessfulSubscription(Me, e)
    End Sub

    Protected Overridable Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent ComError(Me, e)
    End Sub

    Protected Overridable Sub SubscribedItemsChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
        '* Ver 3.99b - Added Not Initializing
        If Not Me.DesignMode And Not Initializing Then
            If SubScriptions IsNot Nothing Then
                SubScriptions.UnsubscribeAll()
            End If
            SubscribeToComDriver()
        End If
    End Sub
#End Region

#Region "Subscribing and PLC data receiving"
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

            For index = 0 To m_PLCAddressValueItems.Count - 1
                If Not String.IsNullOrEmpty(m_PLCAddressValueItems(index).PLCAddress) Then
                    'Dim AddressList() As String
                    'AddressList = m_PLCAddressValue.Split(","c)

                    'For i = 0 To AddressList.Count - 1
                    'If Not SubscribedValueList.ContainsKey(AddressList(i)) Then
                    '* We must pass the address as a property name so the subscriptionHandler doesn't confuse the next address as a change for the same property
                    SubScriptions.SubscribeTo(m_PLCAddressValueItems(index).PLCAddress, m_PLCAddressValueItems(index).NumberOfElements, AddressOf PolledDataReturned, m_PLCAddressValueItems(index).PLCAddress, 1, 0)

                    'SubscribedValueList.TryAdd(m_PLCAddressValueItems(index), "")

                    Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(0, "")
                    x.PlcAddress = m_PLCAddressValueItems(index).PLCAddress
                    OnSuccessfulSubscription(x)
                    'End If
                    'Next
                End If
            Next

            ''* Check through the properties looking for PLCAddress***, then see if the suffix matches an existing property
            'Dim p() As Reflection.PropertyInfo = Me.GetType().GetProperties

            'For i As Integer = 0 To p.Length - 1
            '    '* Does this property start with "PLCAddress"?
            '    If p(i).Name.IndexOf("PLCAddress", StringComparison.CurrentCultureIgnoreCase) = 0 Then
            '        '* Get the property value
            '        Dim PLCAddress As String = Convert.ToString(p(i).GetValue(Me, Nothing))
            '        If PLCAddress IsNot Nothing AndAlso (String.Compare(PLCAddress, "") <> 0) Then
            '            '* Get the text in the name after PLCAddress
            '            Dim PropertyToWrite As String = p(i).Name.Substring(10)
            '            Dim j As Integer = 0
            '            '* See if there is a corresponding property with the extracted name
            '            While j < p.Length AndAlso p(j).Name <> PropertyToWrite
            '                j += 1
            '            End While

            '            '* If the proprty was found, then subscribe to the PLC Address
            '            If j < p.Length Then
            '                SubScriptions.SubscribeTo(PLCAddress, AddressOf PolledDataReturned, PropertyToWrite)

            '                Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(0, "")
            '                x.PlcAddress = PLCAddress
            '                OnSuccessfulSubscription(x)
            '            End If
            '        End If
            '    End If
            'Next
            'End If
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

            For index = 0 To m_PLCAddressValueItems.Count - 1
                If String.Compare(e.PlcAddress, m_PLCAddressValueItems(index).PLCAddress, True) = 0 Then
                    If e.Values(0) <> m_PLCAddressValueItems(index).LastValue Then
                        '* Save this value so we know if it changed without comparing the invert
                        'SubscribedValueList(e.PlcAddress) = e.Values(0)
                        m_PLCAddressValueItems(index).LastValue = e.Values(0)

                        'If InvertValue Then
                        '    m_Value = Convert.ToString(Not CBool(e.Values(0)))
                        'Else
                        '    m_Value = e.Values(0)
                        'End If

                        '* This event is only fired when the returned data has changed
                        OnDataChanged(e)
                    End If
                End If
            Next

        Catch
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

#Region "Public Methods"
    Public Function GetValueByName(ByVal name As String) As String
        Dim index As Integer
        While index < m_PLCAddressValueItems.Count
            If String.Compare(m_PLCAddressValueItems(index).Name, name, True) = 0 Then
                Return m_PLCAddressValueItems(index).LastValue
            End If
            index += 1
        End While

        Return ""
    End Function
#End Region

End Class
