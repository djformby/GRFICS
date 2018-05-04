Option Strict On
'******************************************************************************
'* Omron Base
'*
'* Copyright 2015 Archie Jacobs
'*
'* Reference : Omron W342-E1-15 (W342-E1-15+CS-CJ-CP-NSJ+RefManual.pdf)
'* Revision February 2010
'*
'* The purpose of this class is to implement the common code between the
'* FINS and Host Link protocols
'*
'* This class must be inherited by a class that implements the
'* Omron protocol specifics
'*
'* 17-OCT-15  Created
'* 18-FEB-16 Change threads to tasks for UWP preparation
'***************************************************************************************************

Namespace Omron
    Public MustInherit Class OmronBaseCom
        Inherits System.ComponentModel.Component
        Implements MfgControl.AdvancedHMI.Drivers.IComComponent
        Implements IDisposable

        Public Event DataReceived As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Public Event ComError As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)

        Protected Friend TargetAddress As MfgControl.AdvancedHMI.Drivers.Omron.DeviceAddress
        Protected Friend SourceAddress As MfgControl.AdvancedHMI.Drivers.Omron.DeviceAddress

        Protected Requests(255) As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress
        Protected SavedErrorEventArgs(255) As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs
        Protected waitHandle(255) As System.Threading.EventWaitHandle


        Protected Friend MyDLLInstance As Integer
        Protected Friend EventHandlerDLLInstance As Integer

        Protected MustOverride Sub CreateDLLInstance()
        Protected MustOverride Function GetNextTransactionID(ByVal maxValue As Integer) As Integer
        Public MustOverride Function BeginRead(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As Integer
        Public MustOverride Function BeginWrite(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress, ByVal dataToWrite() As String) As Integer


        Private GroupedSubscriptionReads As New System.Collections.Concurrent.ConcurrentDictionary(Of Integer, MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress)
        Private GroupChangeLock As New Object

        Protected SubscriptionList As New List(Of MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo)

        Private IsDisposed As Boolean '* Without this, it can dispose the DLL completely

        Private Shared ObjectIDs As Int64
        Private MyObjectID As Int64

        Protected QueHoldRelease As System.Threading.EventWaitHandle


#Region "Properties"
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property TargetNodeAddress() As Byte
            Get
                Return TargetAddress.NodeAddress
            End Get
            Set(ByVal value As Byte)
                TargetAddress.NodeAddress = value
            End Set
        End Property


        Protected m_TreatDataAsHex As Boolean
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property TreatDataAsHex() As Boolean
            Get
                Return m_TreatDataAsHex
            End Get
            Set(ByVal value As Boolean)
                m_TreatDataAsHex = value
            End Set
        End Property

        Protected m_PollRateOverride As Integer
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property PollRateOverride() As Integer
            Get
                Return m_PollRateOverride
            End Get
            Set(ByVal value As Integer)
                m_PollRateOverride = value
            End Set
        End Property

        '**************************************************
        '* Its purpose is to fetch
        '* the main form in order to synchronize the
        '* notification thread/event
        '**************************************************
        Protected m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
        Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
            Get
                'If (m_SynchronizingObject Is Nothing) AndAlso Me.DesignMode Then
                If (m_SynchronizingObject Is Nothing) AndAlso AppDomain.CurrentDomain.FriendlyName.IndexOf("DefaultDomain", System.StringComparison.CurrentCultureIgnoreCase) >= 0 Then
                    Dim host1 As System.ComponentModel.Design.IDesignerHost
                    host1 = CType(Me.GetService(GetType(System.ComponentModel.Design.IDesignerHost)), System.ComponentModel.Design.IDesignerHost)
                    If host1 IsNot Nothing Then
                        m_SynchronizingObject = CType(host1.RootComponent, System.ComponentModel.ISynchronizeInvoke)
                    End If
                    '* Windows CE, comment above 5 lines
                End If
                Return m_SynchronizingObject
            End Get

            Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
                If Value IsNot Nothing Then
                    m_SynchronizingObject = Value
                End If
            End Set
        End Property

        '*********************************************************************************
        '* Used to stop subscription updates when not needed to reduce communication load
        '*********************************************************************************
        Private m_DisableSubscriptions As Boolean
        Public Property DisableSubscriptions() As Boolean Implements MfgControl.AdvancedHMI.Drivers.IComComponent.DisableSubscriptions
            Get
                Return m_DisableSubscriptions
            End Get
            Set(ByVal value As Boolean)
                m_DisableSubscriptions = value
            End Set
        End Property

        Private m_Tag As String
        Public Property Tag() As String
            Get
                Return m_Tag
            End Get
            Set(ByVal value As String)
                m_Tag = value
            End Set
        End Property
#End Region

#Region "Constructor"
        Protected Sub New()
            ObjectIDs += 1
            MyObjectID = ObjectIDs

            For index = 0 To 255
                waitHandle(index) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
            Next

            m_PollRateOverride = 500

            QueHoldRelease = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
        End Sub


        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            DisableSubscriptions = True
            '* remove all subscriptions
            For i As Integer = 0 To SubscriptionList.Count - 1
                SubscriptionList(i).MarkForDeletion = True
            Next

            '* Stop the subscription thread
            StopSubscriptions = True
            QueHoldRelease.Set()

            If SubscriptionTask IsNot Nothing AndAlso SubscriptionTask.Status = Threading.Tasks.TaskStatus.Running Then
                'SubscriptionThread.Join(5000)
                SubscriptionTask.Wait(5000)
            End If


            For index = 0 To 255
                waitHandle(index).Dispose()
            Next

            QueHoldRelease.Dispose()


            MyBase.Dispose(disposing)
        End Sub
#End Region

#Region "Subscription"
        '*******************************************************************
        '*******************************************************************

        Private CurrentSubscriptionID As Integer = 1
        Protected SubscriptionListChanged As Boolean

        'Private SubscriptionThread As System.Threading.Thread
        Private SubscriptionTask As System.Threading.Tasks.Task

        Private CurrentID As Integer
        Public Function Subscribe(ByVal plcAddress As String, ByVal numberOfElements As Int16, ByVal pollRate As Integer, ByVal callback As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Subscribe
            ''* If PollRateOverride is other than 0, use that poll rate for this subscription
            'If m_PollRateOverride > 0 Then
            '    pollRate = m_PollRateOverride
            'End If

            ''* Avoid a 0 poll rate
            'If pollRate <= 0 Then
            '    pollRate = 250
            'End If

            '***********************************************
            '* Create an Address object address information
            '***********************************************
            Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(plcAddress)

            '***********************************************************
            '* Check if there was already a subscription made for this
            '***********************************************************
            Dim index As Integer

            While index < SubscriptionList.Count AndAlso _
                (SubscriptionList(index).Address.Address <> plcAddress Or SubscriptionList(index).dlgCallback <> callback)
                index += 1
            End While


            '* If a subscription was already found, then returns it's ID
            If (index < SubscriptionList.Count) Then
                '* Return the subscription that already exists
                Return SubscriptionList(index).ID
            Else
                '* The ID is used as a reference for removing polled addresses
                CurrentID += 1

                Dim tmpPA As New MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo

                tmpPA.PollRate = pollRate

                tmpPA.PollRateDivisor = Convert.ToInt32(pollRate / 100)
                If tmpPA.PollRateDivisor <= 0 Then tmpPA.PollRateDivisor = 1


                tmpPA.dlgCallback = callback
                tmpPA.ID = CurrentID
                tmpPA.Address = address
                tmpPA.Address.Tag = CurrentID
                tmpPA.Address.NumberOfElements = numberOfElements

                '* Add this subscription to the collection and sort
                SubscriptionList.Add(tmpPA)
                ' NewSubscriptionsAdded = True
                '* Move the sort to PollUpdate
                SubscriptionList.Sort(AddressOf SortPolledAddresses)


                If SubscriptionTask Is Nothing OrElse (Not SubscriptionTask.Status = Threading.Tasks.TaskStatus.Created And
                                                    Not SubscriptionTask.Status = Threading.Tasks.TaskStatus.Running And
                                                    Not SubscriptionTask.Status = Threading.Tasks.TaskStatus.WaitingToRun) Then
                    SubscriptionTask = System.Threading.Tasks.Task.Factory.StartNew(AddressOf SubscriptionUpdate)
                End If

                SyncLock GroupChangeLock
                    SubscriptionListChanged = True
                End SyncLock
                Return tmpPA.ID
            End If
        End Function

        '***************************************************************
        '* Used to sort polled addresses by File Type and element
        '* This helps in optimizing reading
        '**************************************************************
        Private Function SortPolledAddresses(ByVal A1 As MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo, ByVal A2 As MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo) As Integer
            If (A1.Address.MemoryAreaCode > A2.Address.MemoryAreaCode) Or _
                (A1.Address.MemoryAreaCode = A2.Address.MemoryAreaCode And (A1.Address.ElementNumber > A2.Address.ElementNumber Or _
                                    (A1.Address.ElementNumber = A2.Address.ElementNumber And A1.Address.BitNumber > A2.Address.BitNumber))) Then
                Return 1
            ElseIf A1.Address.MemoryAreaCode = A2.Address.MemoryAreaCode And A1.Address.ElementNumber = A2.Address.ElementNumber And A1.Address.BitNumber = A2.Address.BitNumber Then
                Return 0
            Else
                Return -1
            End If
        End Function

        Private Sub CreateGroupedReadList()
            SyncLock GroupChangeLock
                SubscriptionListChanged = False
                GroupedSubscriptionReads.Clear()

                Dim i, j, HighestElement, ElementSpan As Integer
                Dim ItemCountToUpdate As Integer = SubscriptionList.Count
                While i < SubscriptionList.Count And i < ItemCountToUpdate
                    '* Is this firing timer at the requested poll rate
                    If i < ItemCountToUpdate Then 'AndAlso (PollCounts / PolledAddressList(i).PollRateDivisor) = Convert.ToInt32(PollCounts / PolledAddressList(i).PollRateDivisor) Then
                        'Dim SavedAsync As Boolean = m_AsyncMode
                        Try
                            SubscriptionList(i).Address.InternalRequest = True
                            'PolledAddressList(i).Address.Tag = PolledAddressList(i).ID

                            j = 0
                            HighestElement = SubscriptionList(i).Address.ElementNumber + SubscriptionList(i).Address.NumberOfElements - 1
                            ElementSpan = HighestElement - SubscriptionList(i).Address.ElementNumber
                            '* V3.99w - Increased group size from 20 to 100 and factored bitsperelement
                            While (i + j + 1) < ItemCountToUpdate AndAlso
                                SubscriptionList(i + j).Address.MemoryAreaCode = SubscriptionList(i + j + 1).Address.MemoryAreaCode AndAlso
                                SubscriptionList(i + j).Address.BitsPerElement = SubscriptionList(i + j + 1).Address.BitsPerElement AndAlso
                                ((SubscriptionList(i + j + 1).Address.ElementNumber + SubscriptionList(i + j + 1).Address.NumberOfElements) - SubscriptionList(i).Address.ElementNumber) < (100 * SubscriptionList(i).Address.BitsPerElement / 16)

                                If (SubscriptionList(i + j + 1).Address.ElementNumber + SubscriptionList(i + j + 1).Address.NumberOfElements - 1) > HighestElement Then
                                    HighestElement = SubscriptionList(i + j + 1).Address.ElementNumber + SubscriptionList(i + j + 1).Address.NumberOfElements - 1
                                End If

                                ElementSpan = HighestElement - SubscriptionList(i).Address.ElementNumber

                                j += 1
                            End While
                        Catch ex1 As Exception
                            Using fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                                fw.WriteLine("1,PollUpdate-" & ex1.Message)
                            End Using
                        End Try

                        Try
                            Dim Address As String
                            Address = SubscriptionList(i).Address.Address
                            If SubscriptionList(i).Address.BitsPerElement = 1 Then
                                '* If it is a bit number, read the word
                                'Address = PolledAddressList(i).Address.Address.Substring(0, PolledAddressList(i).Address.Address.IndexOf("."))
                                'ElementSpan = SubscriptionList(i + j).Address.BitNumber - SubscriptionList(i).Address.BitNumber
                                ElementSpan = (SubscriptionList(i + j).Address.ElementNumber * 16 + SubscriptionList(i + j).Address.BitNumber) - (SubscriptionList(i).Address.ElementNumber * 16 + SubscriptionList(i).Address.BitNumber) + 1
                            End If

                            Dim ReadAddress As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(Address, ElementSpan + 1)
                            ReadAddress.Tag = SubscriptionList(i).ID
                            ReadAddress.InternalRequest = True

                            Try
                                GroupedSubscriptionReads.TryAdd(GroupedSubscriptionReads.Count, ReadAddress)
                            Catch ex1 As Exception
                                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                                fw.WriteLine(String.Format("{0:MM/dd/yyyy hh:mm:ss}", Date.Now) & " -2,PollUpdate-" & ex1.Message)
                                fw.Close()
                            End Try

                        Catch ex As Exception
                            RaiseEvent ComError(Me, New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(-1, ex.Message))
                        End Try
                    End If

                    i += 1 + j
                End While
            End SyncLock
        End Sub


        Private StopSubscriptions As Boolean
        Private ReadTime As New Stopwatch
        Private Sub SubscriptionUpdate()   '(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs)
            System.Threading.Thread.CurrentThread.Name = "OmronSubscriptionUpdate" & MyObjectID

            While Not StopSubscriptions
                'Dim index As Integer
                If Not m_DisableSubscriptions And GroupedSubscriptionReads IsNot Nothing And GroupedSubscriptionReads.Count > 0 Then
                    '* 3-JUN-13 Do not read data until handles are created to avoid exceptions
                    If m_SynchronizingObject Is Nothing OrElse DirectCast(m_SynchronizingObject, Windows.Forms.Control).IsHandleCreated Then
                        Dim DelayBetweenPackets As Integer
                        'Dim response As Integer = 1
                        Dim TransactionNumber As Integer
                        Dim TransactionByte As Integer
                        Dim Signalled As Boolean
                        'Dim T1, T2, T3, T4 As Long
                        'index = 0
                        For Each key In GroupedSubscriptionReads.Keys
                            'While index < GroupedSubscriptionReads.Count And Not StopSubscriptions
                            '* Evenly space out read requests to avoid Send Que Full
                            DelayBetweenPackets = Convert.ToInt32(Math.Max(0, Math.Floor(m_PollRateOverride / GroupedSubscriptionReads.Count)))



                            Try
                                If Not m_DisableSubscriptions And Not StopSubscriptions Then
                                    ReadTime.Reset()
                                    ReadTime.Start()
                                    '* An array or single item
                                    TransactionNumber = Me.BeginRead(GroupedSubscriptionReads(key).Address, GroupedSubscriptionReads(key).NumberOfElements)

                                    TransactionByte = TransactionNumber And 255
                                    Signalled = waitHandle(TransactionByte).WaitOne(3500 + CInt(GroupedSubscriptionReads(key).NumberOfElements / 5))

                                    If Signalled Then
                                        'For ind = 0 To Requests(TransactionByte).Count - 1
                                        'waitHandle(TransactionByte).Reset()
                                        Try
                                            If Requests(TransactionByte).Response IsNot Nothing Then
                                                SendToSubscriptions(Requests(TransactionByte).Response)
                                            Else
                                                Dim dbg = 0
                                            End If
                                        Catch ex As Exception
                                            Throw
                                        End Try
                                        'Next
                                    Else
                                        Dim dbg = 0
                                    End If

                                End If
                            Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
                                Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(ex.ErrorCode, ex.Message)
                                Try
                                    SendToSubscriptions(x)
                                Catch ex1 As Exception
                                    Dim dbg = 0
                                End Try
                                QueHoldRelease.WaitOne(m_PollRateOverride)
                                'Threading.Thread.Sleep(m_PollRateOverride)
                            Catch ex As Exception
                                '* Send this message back to the requesting control
                                Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(-99, ex.Message)
                                SendToSubscriptions(x)
                                QueHoldRelease.WaitOne(m_PollRateOverride)
                            End Try



                            '* Evenly space out the reads to avoid SendQue Full
                            ReadTime.Stop()
                            If Convert.ToInt32(ReadTime.ElapsedMilliseconds) < DelayBetweenPackets Then
                                Threading.Thread.Sleep(DelayBetweenPackets - Convert.ToInt32(ReadTime.ElapsedMilliseconds))
                            End If
                        Next
                    End If
                End If


                If GroupedSubscriptionReads.Count <= 0 Or m_DisableSubscriptions Then
                    QueHoldRelease.WaitOne(m_PollRateOverride)
                End If

                If SubscriptionListChanged Then
                    'SyncLock (CollectionLock)
                    CreateGroupedReadList()
                    'End SyncLock
                End If
            End While

            Dim dbg1 = 0
        End Sub



        'Private SubScribedObjectBeingRemoved As Boolean
        Public Function Unsubscribe(ByVal id As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Unsubscribe
            Dim i As Integer = 0
            While i < SubscriptionList.Count AndAlso SubscriptionList(i).ID <> id
                i += 1
            End While

            If i < SubscriptionList.Count Then
                SyncLock GroupChangeLock
                    SubscriptionListChanged = True
                End SyncLock
                Dim PollRate As Integer = SubscriptionList(i).PollRate
                'SubScribedObjectBeingRemoved = True
                'PolledAddressList.RemoveAt(i)
                SubscriptionList(i).MarkForDeletion = True

                If SubscriptionList.Count = 0 Then
                Else
                End If
            End If
        End Function

        Public Function UnsubscribeAll() As Integer
            Dim i As Integer
            While i < SubscriptionList.Count
                SubscriptionList(i).MarkForDeletion = True
            End While
        End Function

        '* 31-JAN-12
        Public Function IsSubscriptionActive(ByVal id As Integer) As Boolean
            Dim i As Integer = 0
            While i < SubscriptionList.Count AndAlso SubscriptionList(i).ID <> id
                i += 1
            End While

            Return (i < SubscriptionList.Count)
        End Function

        '* 31-JAN-12
        Public Function GetSubscriptionAddress(ByVal id As Integer) As String
            Dim i As Integer = 0
            While i < SubscriptionList.Count AndAlso SubscriptionList(i).ID <> id
                i += 1
            End While

            If i < SubscriptionList.Count Then
                Return SubscriptionList(i).Address.Address
            Else
                Return ""
            End If
        End Function


        Protected Sub SendToSubscriptions(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            Dim i As Integer
            If e.ErrorId = 0 AndAlso e.RawData Is Nothing Then
                Return
            End If
            'Dim Fins As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(New List(Of Byte)(e.RawData))
            Dim sid As Int32 = e.TransactionNumber ' Fins.Header.ServiceId
            While i < SubscriptionList.Count
                '* trap and ignore because subscription may change in the middle of processin
                Try
                    '* 11-MAR-12 V1.20 If a subscription was deleted, then ignore
                    If Not SubscriptionList(i).MarkForDeletion Then
                        '* 06-MAR-12 V1.11 Make sure there are enough values returned (4th condition)
                        If Requests(sid).MemoryAreaCode = SubscriptionList(i).Address.MemoryAreaCode AndAlso
                                                            Requests(sid).ElementNumber <= SubscriptionList(i).Address.ElementNumber AndAlso
                                                           (Requests(sid).ElementNumber + Requests(sid).NumberOfElements) >= (SubscriptionList(i).Address.ElementNumber + SubscriptionList(i).Address.NumberOfElements) AndAlso
                                                           SubscriptionList(i).Address.BitNumber >= Requests(sid).BitNumber Then

                            If e.ErrorId = 0 Then
                                If (SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber + SubscriptionList(i).Address.NumberOfElements) <= Requests(sid).Response.Values.Count Then

                                    Dim f As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(New Byte() {0}, SubscriptionList(i).Address.Address, sid, MyObjectID)
                                    Dim index As Integer = 0
                                    Dim StartElement As Integer = SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber
                                    If Requests(sid).BitsPerElement = 1 Then
                                        StartElement = SubscriptionList(i).Address.BitNumber - Requests(sid).BitNumber
                                        StartElement += (SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber) * 16
                                    End If

                                    If (StartElement + SubscriptionList(i).Address.NumberOfElements) < (Requests(sid).BitNumber + Requests(sid).Response.Values.Count) Then
                                        While index < SubscriptionList(i).Address.NumberOfElements
                                            f.Values.Add(Requests(sid).Response.Values(StartElement + index))
                                            index += 1
                                        End While

                                        Try
                                            '* 11-MAR-12 V1.20
                                            If Not SubscriptionList(i).MarkForDeletion Then
                                                Dim x As Object() = {Me, f}
                                                If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
                                                    m_SynchronizingObject.BeginInvoke(SubscriptionList(i).dlgCallback, x)
                                                Else
                                                    SubscriptionList(i).dlgCallback(Me, f)
                                                End If
                                            End If
                                        Catch ex As Exception
                                            Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                                            fw.WriteLine("1,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
                                            fw.Close()
                                            'Dim debug = 0
                                            '* V1.16 - mark so it can continue
                                            Requests(sid).ErrorReturned = True
                                        End Try
                                    End If
                                End If
                            Else
                                '* Report an error - V3.99w
                                Try
                                    Dim msg As String = e.ErrorMessage
                                    If String.IsNullOrEmpty(msg) Then
                                        msg = "Error code " & e.ErrorId
                                    End If
                                    Dim f As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(e.ErrorId, msg)

                                    If Not SubscriptionList(i).MarkForDeletion Then
                                        Dim x As Object() = {Me, f}
                                        If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
                                            m_SynchronizingObject.BeginInvoke(SubscriptionList(i).dlgCallback, x)
                                        Else
                                            SubscriptionList(i).dlgCallback(Me, f)
                                        End If
                                    End If
                                Catch ex As Exception
                                    Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                                    fw.WriteLine("1,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
                                    fw.Close()
                                    'Dim debug = 0
                                    '* V1.16 - mark so it can continue
                                    Requests(sid).ErrorReturned = True
                                End Try

                            End If
                        End If
                    End If
                Catch ex As Exception
                    Using fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                        fw.WriteLine("2,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
                    End Using
                    'Dim debug = 0
                    '* V1.16 - mark so it can continue
                    Requests(sid).ErrorReturned = True
                End Try
                i += 1
            End While
        End Sub
#End Region

#Region "Public Methods"
        Public Function BeginRead(ByVal startAddress As String, ByVal numberOfElements As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginRead
            Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, numberOfElements)
            Return BeginRead(address)
        End Function


        Public Function BeginRead(ByVal startAddress As String) As Integer  'Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
            Return BeginRead(startAddress, 1)
        End Function

        Public Function Read(ByVal startAddress As String) As String
            Return Read(startAddress, 1)(0)
        End Function

        Public Function Read(ByVal startAddress As String, ByVal numberOfElements As Integer) As String() Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
            Dim TransactionNumber As Integer = BeginRead(startAddress, numberOfElements)

            Dim TransactionByte As Integer = TransactionNumber And 255
            Dim Signalled As Boolean = waitHandle(TransactionByte).WaitOne(3000)

            If Signalled Then
                ' If WaitForResponse(CUShort(TransactionNumber)) = 0 Then
                If Requests(TransactionNumber).Response IsNot Nothing Then
                    Dim tmp(Requests(TransactionNumber).Response.Values.Count - 1) As String
                    For i As Integer = 0 To tmp.Length - 1
                        tmp(i) = Requests(TransactionByte).Response.Values(i)
                    Next
                    Return tmp
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No reponse packet received from PLC.")
                End If
            Else
                If Requests(TransactionByte).ErrorReturned Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error returned " & SavedErrorEventArgs(TransactionNumber).ErrorId)
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No reponse from PLC. Ensure baud rate is correct.")
                End If
            End If
        End Function


        Public Function ReadAsString(ByVal startAddress As String, ByVal numberOfElements As Integer) As String
            Dim values() As String = Read(startAddress, numberOfElements)
            Return MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.WordsToString(values)
        End Function


        '*******************************************************
        Public Function Write(ByVal startAddress As String, ByVal dataToWrite As String) As String Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Write
            Dim DataAsArray() As String = {dataToWrite}
            Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, 1)

            Dim TransactionID As Integer = BeginWrite(address, DataAsArray)
            Return CStr(TransactionID)
        End Function

        Public Function Write(ByVal startAddress As String, ByVal dataToWrite() As String) As Integer
            BeginWrite(startAddress, dataToWrite.Length, dataToWrite)
            '* Commented out 3.99s
            'Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, dataToWrite.Length)
            'Return BeginWrite(address, dataToWrite)
        End Function

        Public Function BeginWrite(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As String) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginWrite
            If dataToWrite.Length = numberOfElements Then
                Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, dataToWrite.Length)
                Return BeginWrite(address, dataToWrite)
            Else
                Dim ElementCount As Integer = Math.Min(numberOfElements, dataToWrite.Length)
                Dim data(ElementCount - 1) As String
                For i = 0 To data.Length - 1
                    data(i) = dataToWrite(i)
                Next
                Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, data.Length)
                Return BeginWrite(address, data)
            End If
        End Function

        Public Sub WriteAsString(ByVal startAddress As String, ByVal stringToWrite As String)
            If Not String.IsNullOrEmpty(stringToWrite) Then
                Dim Values() As Integer = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.StringToWords(stringToWrite)
                Dim ValuesAsString(Values.Length - 1) As String
                For i = 0 To ValuesAsString.Length - 1
                    ValuesAsString(i) = CStr(Values(i))
                Next
                Write(startAddress, ValuesAsString)
            End If
        End Sub
#End Region

#Region "Private Methods"


#Region "Events"
        Protected Overridable Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            If m_SynchronizingObject IsNot Nothing Then
                Dim Parameters() As Object = {Me, e}
                m_SynchronizingObject.BeginInvoke(drsd, Parameters)
            Else
                RaiseEvent DataReceived(Me, e)
            End If
        End Sub

        Protected Overridable Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            If m_SynchronizingObject IsNot Nothing Then
                m_SynchronizingObject.BeginInvoke(errorsd, New Object() {Me, e})
            Else
                RaiseEvent ComError(Me, e)
            End If
        End Sub
        '***********************************************************
        '* Used to synchronize the event back to the calling thread
        '***********************************************************
        Private drsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf DataReceivedSync)
        Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent DataReceived(sender, DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs))
        End Sub

        Private errorsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf ErrorReceivedSync)
        Private Sub ErrorReceivedSync(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent ComError(sender, DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs))
        End Sub
#End Region

        Protected Friend Sub DataLinkLayerComError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            If e.TransactionNumber >= 0 Then
                If e.OwnerObjectID <> MyObjectID Then
                    'If Not MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber.IsMyTNS(e.TransactionNumber, MyObjectID) Then
                    Exit Sub
                End If

                '* Save this for other uses
                SavedErrorEventArgs(e.TransactionNumber And 255) = e

                Requests(e.TransactionNumber And 255).ErrorReturned = True
            End If

            OnComError(e)

            SendToSubscriptions(e)
            waitHandle(e.TransactionNumber And 255).Set()
        End Sub


        ''***************************************
        ''* Extract the returned data
        ''***************************************
        'Private Function ExtractData(ByVal RawData As List(Of Byte), ByVal startByte As Integer, ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As String()
        '    Dim values(address.NumberOfElements - 1) As String

        '    Dim NumberOfBytes As Integer = Convert.ToInt32(Math.Ceiling(address.BitsPerElement / 8))


        '    Dim i As Integer
        '    While i < address.NumberOfElements And (startByte + i) < Math.Floor(RawData.Count / NumberOfBytes)
        '        'Dim HexByte1 As String = Chr(RawData(startByte + i * NumberOfBytes)) & Chr(RawData(startByte + (i * NumberOfBytes) + 1))
        '        If NumberOfBytes > 1 Then
        '            'Dim HexByte2 As String = Chr(RawData(startByte + (i * NumberOfBytes) + 2)) & Chr(RawData(startByte + (i * NumberOfBytes) + 3))
        '            If Not address.IsBCD And Not m_TreatDataAsHex Then
        '                'values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes) * 256 + RawData(startByte + i * NumberOfBytes + 1))
        '                Dim b() As Byte = {RawData(startByte + i * NumberOfBytes + 1), RawData(startByte + i * NumberOfBytes)}
        '                values(i) = Convert.ToString(BitConverter.ToInt16(b, 0))
        '            Else
        '                values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes)) & MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes + 1))
        '            End If
        '        Else
        '            If Not m_TreatDataAsHex Then
        '                values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes))
        '            Else
        '                values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes))
        '            End If
        '        End If

        '        i += 1
        '    End While

        '    Return values
        'End Function
#End Region
    End Class
End Namespace
