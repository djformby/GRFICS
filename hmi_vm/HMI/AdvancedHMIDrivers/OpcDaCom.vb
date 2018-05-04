Option Strict On
'**********************************************************************************************
'* AdvancedHMI Driver
'* http://www.advancedhmi.com
'* OPC Comm
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 20-MAY-11
'*
'*
'* Copyright 2011 Archie Jacobs
'*
'*
'*******************************************************************************************************
Imports System.ComponentModel.Design

Public Class OpcDaCom
    Inherits System.ComponentModel.Component
    Implements MfgControl.AdvancedHMI.Drivers.IComComponent

    '* Create a common instance to share so multiple driver instances can be used in a project
    Private DLL As Opc.Da.Server
    Private fact As New OpcCom.Factory

    Private SubscribedCollection As New List(Of PolledAddressInfo)

    
    Public Event DataReceived As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)

    '*********************************************************************************
    '* This is used for linking a notification.
    '* An object can request a continuous poll and get a callback when value updated
    '*********************************************************************************
    'Delegate Sub ReturnValues(ByVal Values As String)
    Private Structure PolledAddressInfo
        Dim OPCItem As Opc.Da.Item
        Dim dlgCallBack As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Dim PollRate As Integer
        Dim ID As Integer
        Dim ElementsToRead As Integer
    End Structure

    Private CurrentID As Integer = 1
    Private TransactionID As Integer

    'Private PolledAddressList As New List(Of PolledAddressInfo)

#Region "Constructor/Destructor"
    Private components As System.ComponentModel.IContainer
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub


    '* Component overrides dispose to clean up the component list.
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        '* The handle linked to the DataLink Layer has to be removed, otherwise it causes a problem when a form is closed
        'RemoveHandler DLL(MyDLLInstance).DataReceived, Dr
        If DLL IsNot Nothing Then
            If DLL.IsConnected Then
                Try
                    DLL.Disconnect()
                Catch
                End Try
            End If
            DLL.Dispose()
            fact.Dispose()
        End If

        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
#End Region


    '***************************************************************
    '* Create the Data Link Layer Instances
    '* if the IP Address is the same, then resuse a common instance
    '***************************************************************
    Private Sub CreateDLLInstance()
        If DLL Is Nothing Then
            DLL = New Opc.Da.Server(fact, Nothing)
            DLL.Url = New Opc.URL(m_OPCServerPath & "/" & m_OPCServer)
            'DLL.Connect(DLL.Url, New Opc.ConnectData(New System.Net.NetworkCredential()))
            DLL.Connect()

            '* Give time to startup
            Threading.Thread.Sleep(2000)
        End If
    End Sub


#Region "Properties"
    Private m_OPCServerPath As String = "opcda://localhost"
    Public Property OPCServerPath As String
        Get
            Return m_OPCServerPath
        End Get
        Set(value As String)
            m_OPCServerPath = value.TrimEnd

            '* Strip off the path separator because it is added in CreateDLLInstance
            If m_OPCServerPath.LastIndexOf("/") = (m_OPCServerPath.Length - 1) Or _
               m_OPCServerPath.LastIndexOf("\") = (m_OPCServerPath.Length - 1) Then
                m_OPCServerPath = m_OPCServerPath.Substring(0, m_OPCServerPath.Length - 1)
            End If
        End Set
    End Property

    Private m_OPCServer As String = "OPC.IwSCP"
    Public Property OPCServer() As String
        Get
            Return m_OPCServer
        End Get
        Set(ByVal value As String)
            m_OPCServer = value
        End Set
    End Property

    Private m_OPCGroup As String = ""
    Public Property OPCGroup() As String
        Get
            Return m_OPCGroup
        End Get
        Set(ByVal value As String)
            m_OPCGroup = value
        End Set
    End Property

    Private m_OPCTopic As String
    Public Property OPCTopic() As String
        Get
            Return m_OPCTopic
        End Get
        Set(ByVal value As String)
            m_OPCTopic = value
        End Set
    End Property


    '**************************************************************
    '* Stop the polling of subscribed data
    '**************************************************************
    Private m_DisableSubscriptions As Boolean
    Public Property DisableSubscriptions() As Boolean Implements MfgControl.AdvancedHMI.Drivers.IComComponent.DisableSubscriptions
        Get
            Return m_DisableSubscriptions
        End Get
        Set(ByVal value As Boolean)
            If m_DisableSubscriptions <> value Then
                m_DisableSubscriptions = value
                If SubscriptionState IsNot Nothing Then
                    SubscriptionState.Active = Not m_DisableSubscriptions
                End If
            End If
        End Set
    End Property

    Private m_PollRateOverride As Integer = 500
    <System.ComponentModel.Category("Communication Settings")>
    Public Property PollRateOverride() As Integer
        Get
            Return m_PollRateOverride
        End Get
        Set(ByVal value As Integer)
            If value >= 0 Then
                m_PollRateOverride = value
            End If
        End Set
    End Property


    '**************************************************
    '* Its purpose is to fetch
    '* the main form in order to synchronize the
    '* notification thread/event
    '**************************************************
    Private m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
    '* do not let this property show up in the property window
    ' <System.ComponentModel.Browsable(False)> _
    Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        Get
            'If Me.Site.DesignMode Then

            Dim host1 As IDesignerHost
            Dim obj1 As Object
            If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
                host1 = CType(Me.GetService(GetType(IDesignerHost)), IDesignerHost)
                If host1 IsNot Nothing Then
                    obj1 = host1.RootComponent
                    m_SynchronizingObject = CType(obj1, System.ComponentModel.ISynchronizeInvoke)
                End If
            End If
            'End If
            Return m_SynchronizingObject

        End Get

        Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
            If Not Value Is Nothing Then
                m_SynchronizingObject = Value
            End If
        End Set
    End Property
#End Region

#Region "Read/Write Interface to Driver"
    Private ReadSubscription As Opc.Da.ISubscription
    Private ReadSubscriptionState As Opc.Da.SubscriptionState

    Public Function BeginRead(ByVal startAddress As String, ByVal numberOfElements As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginRead
        If DLL Is Nothing Then CreateDLLInstance()

        '*********************************************************************
        '* If Async Mode, then return immediately and return value on event
        '*********************************************************************
        If TransactionID < 32767 Then
            TransactionID += 1
        Else
            TransactionID = 0
        End If


        If ReadSubscriptionState Is Nothing Then
            ReadSubscriptionState = New Opc.Da.SubscriptionState
            ReadSubscriptionState.Name = "AsyncReadGroup"

            ReadSubscription = DLL.CreateSubscription(ReadSubscriptionState)
        End If


        Dim Items(0) As Opc.Da.Item
        Items(0) = New Opc.Da.Item

        If (m_OPCTopic IsNot Nothing) AndAlso (String.Compare(m_OPCTopic, "") <> 0) Then
            Items(0).ItemName = "[" & m_OPCTopic & "]"
        Else
            Items(0).ItemName = ""
        End If

        Items(0).ItemName &= startAddress

        If numberOfElements > 1 Then
            Items(0).ItemName &= ",L" & numberOfElements
        End If

        Items(0).SamplingRate = 250
        Items(0).ClientHandle = TransactionID

        Dim ItemRes() As Opc.Da.ItemResult
        ItemRes = ReadSubscription.AddItems(Items)

        For i As Integer = 0 To ItemRes.Length - 1
            Items(i).ServerHandle = ItemRes(i).ServerHandle
        Next


        Dim req As Opc.IRequest = Nothing
        ReadSubscription.Read(Items, TransactionID, New Opc.Da.ReadCompleteEventHandler(AddressOf AsyncReadCompleteCallback), req)

        Return TransactionID
    End Function

    Public Function Read(ByVal startAddress As String, ByVal numberOfElements As Integer) As String() Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
        If DLL Is Nothing Then CreateDLLInstance()

        Dim items(0) As Opc.Da.Item
        items(0) = New Opc.Da.Item
        If m_OPCTopic IsNot Nothing AndAlso (String.Compare(OPCTopic, "") <> 0) Then
            items(0).ItemName = "[" & m_OPCTopic & "]"
        Else
            items(0).ItemName = ""
        End If

        items(0).ItemName &= startAddress

        If numberOfElements > 1 Then
            items(0).ItemName &= ",L" & numberOfElements
        End If

        items(0).SamplingRate = 50
        items(0).Active = True

        Dim values() As Opc.Da.ItemValue
        values = DLL.Read(items)

        Dim ArraySize As Integer = 0
        If TypeOf (values(0).Value) Is System.Array Then
            ArraySize = DirectCast(values(0).Value, System.Array).Length - 1
        End If

        Dim ReturnValues(ArraySize) As String
        If ArraySize > 0 Then
            For i As Integer = 0 To DirectCast(values(0).Value, System.Array).Length - 1
                ReturnValues(i) = Convert.ToString(values(0).Value)
            Next
        Else
            ReturnValues(0) = Convert.ToString(values(0).Value)
        End If

        Return ReturnValues
    End Function


    '*************************************************************
    '* Overloaded method of ReadAny - that reads only one element
    '*************************************************************
    Public Function BeginRead(ByVal startAddress As String) As Integer
        Return BeginRead(startAddress, 1)
    End Function

    '*************************************
    '* Return values from an Async Read
    '*************************************
    Private Sub AsyncReadCompleteCallback(ByVal clientHandle As Object, ByVal values() As Opc.Da.ItemValueResult)
        Dim ArraySize As Integer = 0
        If TypeOf (values(0).Value) Is System.Array Then
            ArraySize = DirectCast(values(0).Value, System.Array).Length - 1
        End If

        'Dim x As String =values(0).
        Dim ReturnValues(ArraySize) As String
        If ArraySize > 0 Then
            Dim ar As System.Array = DirectCast(values(0).Value, System.Array)
            Dim ari As Integer() = DirectCast(values(0).Value, Integer())
            For i As Integer = 0 To ar.Length - 1
                ReturnValues(i) = Convert.ToString(values(0).Value)
                ReturnValues(i) = Convert.ToString(ari(i))
            Next
        Else
            ReturnValues(0) = Convert.ToString(values(0).Value)
        End If

        Dim PLCAddress As String = values(0).ItemName
        '* If there is a Topic, strip it back off before sending back to subscriber
        If m_OPCTopic IsNot Nothing AndAlso Not String.IsNullOrEmpty(m_OPCTopic) Then
            PLCAddress = PLCAddress.Substring(m_OPCTopic.Length + 2)
        End If

        Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(ReturnValues, PLCAddress, CUShort(clientHandle))
        OnDataReceived(x)

        ReadSubscription.SetEnabled(False)
    End Sub

#Region "AsyncReturn"
    '* This is needed so the handler can be removed
    Private drsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataReceivedSync
    Protected Overridable Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
            m_SynchronizingObject.BeginInvoke(drsd, New Object() {e})
            'm_SynchronizingObject.BeginInvoke(New EventHandler(Of Common.PlcComEventArgs)(AddressOf DataReceivedSync), New Object() {e})
        Else
            RaiseEvent DataReceived(Me, e)
        End If
    End Sub
    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataReceived(Me, e)
    End Sub
#End Region

    '*****************************************************************
    '* Write Section
    '*
    '* Address is in the form of <file type><file Number>:<offset>
    '* examples  N7:0, B3:0,
    '******************************************************************
    Public Function BeginWrite(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As String) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginWrite
        Return Write(startAddress, numberOfElements, dataToWrite)
    End Function

    Public Function Write(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As String) As Integer
        If DLL Is Nothing Then CreateDLLInstance()

        Dim items(0) As Opc.Da.Item
        items(0) = New Opc.Da.Item

        '* If there is a topic then add it to the Item Name
        If m_OPCTopic IsNot Nothing AndAlso (String.Compare(OPCTopic, "") <> 0) Then
            items(0).ItemName = "[" & m_OPCTopic & "]"
        Else
            items(0).ItemName = ""
        End If
        items(0).ItemName &= startAddress

        '* If Writing multiple elements, add the length to the Item Nam
        If numberOfElements > 1 Then items(0).ItemName &= ",L" & numberOfElements

        '* Create an array OPC ItemValue
        Dim Values(numberOfElements - 1) As Opc.Da.ItemValue
        For i As Integer = 0 To numberOfElements - 1
            Values(i) = New Opc.Da.ItemValue((items(i)))
            Values(i).Value = Convert.ToString(dataToWrite(i))
        Next

        Dim x() As Opc.IdentifiedResult = DLL.Write(Values)


        If x(0).ResultID = Opc.ResultID.S_OK Then
            Return 0
        Else
            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Failed to Write to " & items(0).ItemName)
        End If
    End Function

    Public Function Write(ByVal startAddress As String, ByVal dataToWrite As String) As String Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Write
        Dim temp(0) As String
        temp(0) = dataToWrite
        Return Write(startAddress, 1, temp).ToString
    End Function
#End Region

#Region "Subscribing"
    '*********************************************************************
    Private SubscriptionOPC As Opc.Da.Subscription
    Private SubscriptionState As New Opc.Da.SubscriptionState
    Private SubscribedItems() As Opc.Da.Item


    Public Function Subscribe(ByVal PLCAddress As String, ByVal numberOfElements As Int16, ByVal updateRate As Integer, ByVal CallBack As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Subscribe
        If DLL Is Nothing Then CreateDLLInstance()

        If SubscriptionOPC Is Nothing Then
            SubscriptionState.Name = "SubscribedGroup"
            SubscriptionState.UpdateRate = m_PollRateOverride
            Try
                SubscriptionOPC = DirectCast(DLL.CreateSubscription(SubscriptionState), Opc.Da.Subscription)
                AddHandler SubscriptionOPC.DataChanged, AddressOf DataChangedCallBack
            Catch ex As Exception
                Dim dbg = 0
            End Try
        End If

        '* See if this subscription already exists
        Dim index As Integer
        While index < SubscriptionOPC.Items.Length AndAlso _
                (DirectCast(SubscriptionOPC.Items(index).ClientHandle, PolledAddressInfo).dlgCallBack <> CallBack Or _
                 SubscriptionOPC.Items(index).ItemName <> PLCAddress)
            index += 1
        End While

        If index >= SubscriptionOPC.Items.Length Then
            Dim OPCSubscriptionItem(0) As Opc.Da.Item
            OPCSubscriptionItem(0) = New Opc.Da.Item
            OPCSubscriptionItem(0).ItemName = PLCAddress
            If m_OPCTopic IsNot Nothing AndAlso (String.Compare(m_OPCTopic, "") <> 0) Then
                OPCSubscriptionItem(0).ItemName = "[" & m_OPCTopic & "]" & OPCSubscriptionItem(0).ItemName   '*& ",L" & numberOfElements
            End If
            OPCSubscriptionItem(0).SamplingRate = updateRate
            'OPCSubscriptionItem(0).SamplingRate = 1



            '* Create a PollAddressInfo to associate a callback address with subscribed item
            Dim tmpPA As New PolledAddressInfo
            tmpPA.OPCItem = OPCSubscriptionItem(0)
            tmpPA.dlgCallBack = CallBack

            '* The ID is used as a reference for removing polled addresses
            CurrentID += 1
            tmpPA.ID = CurrentID




            OPCSubscriptionItem(0).ClientHandle = tmpPA
            ItemResult = SubscriptionOPC.AddItems(OPCSubscriptionItem)

            OPCSubscriptionItem(0).ServerHandle = ItemResult(0).ServerHandle
            If ItemResult.Length > 0 AndAlso ItemResult(0).ResultID.Code <> 0 Then
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Failed OPC Subscription Start (OpcDaCom)")
            Else
                'TODO:
                'If Subscription.Items.Count < 800 Or Subscription.Items.Count / 50 = Convert.ToInt32(Subscription.Items.Count / 50) Then
                Dim req As Opc.IRequest = Nothing
                'Dim result() As Opc.IdentifiedResult
                Try
                    '* Perform an initial read since the event only fires on data change
                    'result = Subscription.Read(OPCSubscriptionItem, tmpPA, New Opc.Da.ReadCompleteEventHandler(AddressOf ReadCompleteCallback), req)
                    Dim values() As Opc.Da.ItemValueResult = DLL.Read(OPCSubscriptionItem)

                    If values.Length <= 0 OrElse values(0).ResultID.Code <> 0 Then
                        Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Failed OPC READ (OpcDaCom)")
                    Else
                        Dim ReturnedValues() As String = {Convert.ToString(values(0).Value)}

                        Dim PLCAddressR As String = values(0).ItemName
                        '* If there is a Topic, strip it back off before sending back to subscriber
                        If m_OPCTopic IsNot Nothing AndAlso Not String.IsNullOrEmpty(m_OPCTopic) Then
                            PLCAddressR = PLCAddressR.Substring(m_OPCTopic.Length + 2)
                        End If

                        Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(ReturnedValues, PLCAddressR, 0)
                        Dim z() As Object = {Me, x}
                        m_SynchronizingObject.BeginInvoke(tmpPA.dlgCallBack, z)
                    End If

                Catch ex As Exception
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException(ex.Message)
                End Try

                ' If result.Length > 0 AndAlso result(0).ResultID.Code <> 0 Then
                ' Throw New Common.PLCDriverException("Failed OPC READ (OpcDaCom)")
                'End If
                'End If
            End If

            'End If
            'result = Subscription.Read(t, tmpPA.ID, New Opc.Da.ReadCompleteEventHandler(AddressOf ReadCompleteCallback), req)
            'Subscription.Read(SubscribedItems)

            SubscribedCollection.Add(tmpPA)
            Return tmpPA.ID
        Else
            'Return SubscribedCollection(index).ID
            Return DirectCast(SubscriptionOPC.Items(index).ClientHandle, PolledAddressInfo).ID
        End If
    End Function

    'Private ReadCompleted As Boolean
    Private Sub ReadCompleteCallback(ByVal clientHandle As Object, ByVal values() As Opc.Da.ItemValueResult)
        'ReadCompleted = True
        Dim x = 0
    End Sub
    Private Sub DataChangedCallBack(ByVal clientHandle As Object, ByVal requestHandle As Object, ByVal values() As Opc.Da.ItemValueResult)
        For i = 0 To values.Length - 1
            Dim ReturnedValues() As String = {Convert.ToString(values(i).Value)}
            Dim PolledAddress As PolledAddressInfo
            Try
                PolledAddress = DirectCast(values(i).ClientHandle, PolledAddressInfo)
            Catch ex As Exception
                Exit Sub
            End Try

            'For j = 0 To Subscription.Items.Count - 1
            'If PolledAddress.OPCItem.ItemName = DirectCast(Subscription.Items(j).ClientHandle, PolledAddressInfo).OPCItem.ItemName Then

            Dim PLCAddress As String = values(i).ItemName
            '* If there is a Topic, strip it back off before sending back to subscriber
            If m_OPCTopic IsNot Nothing AndAlso Not String.IsNullOrEmpty(m_OPCTopic) Then
                PLCAddress = PLCAddress.Substring(m_OPCTopic.Length + 2)
            End If

            Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(ReturnedValues, PLCAddress, 0)
            x.SubscriptionID = PolledAddress.ID
            Dim z() As Object = {Me, x}
            m_SynchronizingObject.BeginInvoke(PolledAddress.dlgCallBack, z)
            'End If
            'Next
        Next
    End Sub

    Private ItemResult() As Opc.Da.ItemResult

    Private Sub UpdateSubscribedItems()
        '****************************************************************************
        If DLL Is Nothing Then CreateDLLInstance()

        If DLL.IsConnected Then
            If SubscriptionOPC Is Nothing Then
                'SubscriptionState.Name = _OPCGroup
                SubscriptionState.Name = "SubscribedGroup"

                SubscriptionOPC = DirectCast(DLL.CreateSubscription(SubscriptionState), Opc.Da.Subscription)
                AddHandler SubscriptionOPC.DataChanged, AddressOf DataChangedCallBack
            End If


            If SubscribedItems IsNot Nothing AndAlso SubscriptionOPC.Items.Length > 0 Then SubscriptionOPC.RemoveItems(SubscribedItems)

            ReDim SubscribedItems(SubscribedCollection.Count - 1)
            For i As Integer = 0 To SubscribedCollection.Count - 1
                SubscribedItems(i) = New Opc.Da.Item
                SubscribedItems(i).ItemName = SubscribedCollection(i).OPCItem.ItemName
                SubscribedItems(i).SamplingRate = 200
                'SubscribedItems(i).Active = True
                SubscribedItems(i).ClientHandle = i
            Next
            ItemResult = SubscriptionOPC.AddItems(SubscribedItems)

            For i As Integer = 0 To ItemResult.Length - 1
                SubscribedItems(i).ServerHandle = ItemResult(i).ServerHandle
            Next
        End If
    End Sub

    Public Function UnSubscribe(ByVal ID As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Unsubscribe
        Dim i As Integer = 0
        While i < SubscribedCollection.Count AndAlso SubscribedCollection(i).ID <> ID
            i += 1
        End While

        If i < SubscribedCollection.Count Then
            SubscribedCollection.RemoveAt(i)
        End If

        UpdateSubscribedItems()
    End Function

#End Region




End Class






