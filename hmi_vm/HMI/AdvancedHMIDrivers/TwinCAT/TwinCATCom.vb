'*************************************************************************************
'* AdvancedHMI Driver
'* http://www.advancedhmi.com
'* ADS/AMS for TwinCAT
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'*
'*
'* Copyright 2013, 2015 Archie Jacobs
'*
'* This class implements the TwinCAT ADS/AMS protocol.
'*
'* Distributed under the GNU General Public License (www.gnu.org)
'*
'* This program is free software; you can redistribute it and/or
'* as published by the Free Software Foundation; either version 2
'* of the License, or (at your option) any later version.
'*
'* This program is distributed in the hope that it will be useful,
'* but WITHOUT ANY WARRANTY; without even the implied warranty of
'* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'* GNU General Public License for more details.

'* You should have received a copy of the GNU General Public License
'* along with this program; if not, write to the Free Software
'* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
'*************************************************************************************

Imports System.ComponentModel

Public Class TwinCATCom
    Inherits System.ComponentModel.Component
    Implements MfgControl.AdvancedHMI.Drivers.IComComponent
    Implements ISupportInitialize
    Implements IDisposable


    Public Event ComError As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event DataReceived As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)

    Private Requests(255) As MfgControl.AdvancedHMI.Drivers.TwinCATAMSRequest
    Public ReadOnly UsedSymbols As New System.Collections.ObjectModel.Collection(Of MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo)

    Private Shared ObjectIDs As Int64
    Protected MyObjectID As Int64


    Private Shared DLL As System.Collections.Concurrent.ConcurrentDictionary(Of Integer, MfgControl.AdvancedHMI.Drivers.ADSforTwinCATEx)
    Private Shared NextDLLInstance As Integer
    Private MyDLLInstance As Integer
    Private EventHandlerDLLInstance As Integer

    ' Private SubscriptionThread As System.Threading.Thread
    Private SubscriptionTask As System.Threading.Tasks.Task
    Private StopSubscriptions As Boolean
    Private ThreadStarted As Boolean
    Private waitHandle(255) As System.Threading.EventWaitHandle
    Protected SubscriptionHoldRelease As New System.Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)
    Protected IsDisposed As Boolean

    Private Structure SubscriptionInfo
        Private sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
        Public Property Symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
            Get
                Return sym
            End Get
            Set(value As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo)
                sym = value
            End Set
        End Property

        Friend dlgCallBack As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Friend PollRate As Integer
        Friend ID As Integer
        Friend NumberOfElements As Int32
        'Friend NumberOfBytes As Int32
    End Structure

    Private NewSubscriptions As New List(Of SubscriptionInfo)
    '* A copy made to Create Group List without holding up Subscribe function
    Private WorkingSubscriptionList As New List(Of SubscriptionInfo)
    Private GroupedSubscriptionReads As New System.Collections.ObjectModel.Collection(Of System.Collections.ObjectModel.Collection(Of MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite))
    'Private SubscriptionListChanges As Integer


#Region "Properties"
    Private m_IniFileName As String = ""
    Public Property IniFileName As String
        Get
            Return m_IniFileName
        End Get
        Set(value As String)
            m_IniFileName = value
        End Set
    End Property

    Private m_IniFileSection As String
    Public Property IniFileSection As String
        Get
            Return m_IniFileSection
        End Get
        Set(value As String)
            m_IniFileSection = value
        End Set
    End Property


    Private m_TargetIPAddress As String = "192.168.2.6"   '* this is a default value
    Public Property TargetIPAddress() As String
        Get
            Return m_TargetIPAddress
        End Get
        Set(ByVal value As String)
            Dim NewIP As String = ""
            NewIP = value
            'End If

            '* Do we have a new IP Address?
            If String.Compare(NewIP, m_TargetIPAddress, True) <> 0 Then
                '* If this been attached to a DLL, then remove first
                If EventHandlerDLLInstance = (MyDLLInstance + 1) Then
                    RemoveDLLConnection(MyDLLInstance)
                End If

                m_TargetIPAddress = NewIP

                '* If a new instance needs to be created, such as a different AMS Address
                If Not Initializing Then CreateDLLInstance()
            End If
        End Set
    End Property

    '* USing a small "a" ensures the designer will set this property first
    '* If anything else is set before, a new instance will not get created
    Private m_TargetAMSNetID As String = "0.0.0.0.0.0"
    Private m_AMSNetIDIniFile As String = ""
    Public Property TargetAMSNetID() As String
        Get
            If Not String.IsNullOrEmpty(m_AMSNetIDIniFile) Then
                Return m_AMSNetIDIniFile
            Else
                Return m_TargetAMSNetID
            End If
        End Get
        Set(ByVal value As String)
            'If m_TargetAMSNetID <> value And m_AMSNetIDIniFile <> value Then
            Dim NewAMS As String = ""
            'If value.IndexOf(".ini", 0, StringComparison.CurrentCultureIgnoreCase) > 0 Then
            '    Try
            '        If Not Me.DesignMode Then
            '            Dim p As New IniParser(value)
            '            NewAMS = p.GetSetting("TARGETAMSNETID")
            '        End If
            '        m_AMSNetIDIniFile = value
            '        'm_TargetAMSNetID = "0.0.0.0.0.0"
            '    Catch ex As Exception
            '        Exit Property
            '    End Try
            'Else
            NewAMS = String.Copy(value)
            m_AMSNetIDIniFile = ""
            'End If

            '* Did the AMS Net ID change?
            If String.Compare(m_TargetAMSNetID, NewAMS, True) <> 0 Then
                '* If this been attached to a DLL, then remove first
                If EventHandlerDLLInstance = (MyDLLInstance + 1) Then
                    RemoveDLLConnection(MyDLLInstance)
                End If

                m_TargetAMSNetID = String.Copy(NewAMS)


                '* If a new instance needs to be created, such as a different AMS Address
                If Not Initializing Then CreateDLLInstance()
            End If
        End Set
    End Property

    Private m_TargetAMSPort As UInt16 = 801
    Public Property TargetAMSPort() As UInt16
        Get
            Return m_TargetAMSPort
        End Get
        Set(ByVal value As UInt16)
            m_TargetAMSPort = value

            '* If a new instance needs to be created, such as a different AMS Address
            If Not Initializing Then CreateDLLInstance()
        End Set
    End Property

    '*****************************************************************
    '* The username and password used to register with the AMS router
    Private m_UserName As String = "Administrator"
    Public Property UserName() As String
        Get
            Return m_UserName
        End Get
        Set(ByVal value As String)
            m_UserName = String.Copy(value)

            '* If a new instance needs to be created, such as a different AMS Address
            If Not Initializing Then CreateDLLInstance()


            If DLL.ContainsKey(MyDLLInstance) AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                DLL(MyDLLInstance).UserName = String.Copy(value)
            End If

        End Set
    End Property

    Private m_Password As String = "1"
    Public Property Password() As String
        Get
            Return m_Password
        End Get
        Set(ByVal value As String)
            m_Password = String.Copy(value)

            '* If a new instance needs to be created, such as a different AMS Address
            If Not Initializing Then CreateDLLInstance()


            If DLL.ContainsKey(MyDLLInstance) AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                DLL(MyDLLInstance).Password = String.Copy(value)
            End If
        End Set
    End Property

    Private m_UseStaticRoute As Boolean = True
    Public Property UseStaticRoute() As Boolean
        Get
            Return m_UseStaticRoute
        End Get
        Set(ByVal value As Boolean)
            m_UseStaticRoute = value

            '* If a new instance needs to be created, such as a different AMS Address
            If Not Initializing Then CreateDLLInstance()


            If DLL.ContainsKey(MyDLLInstance) AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                DLL(MyDLLInstance).UseStaticRoute = value
            End If
        End Set
    End Property

    Private m_PollRateOverride As Integer = 500
    <System.ComponentModel.Category("Communication Settings")> _
    <System.ComponentModel.DefaultValue(0)> _
    Public Property PollRateOverride() As Integer
        Get
            Return m_PollRateOverride
        End Get
        Set(ByVal value As Integer)
            '* Poll rate are in increments of 100
            m_PollRateOverride = Convert.ToInt32(Math.Ceiling(value / 10)) * 10
        End Set
    End Property


    '**************************************************
    '* Its purpose is to fetch
    '* the main form in order to synchronize the
    '* notification thread/event
    '**************************************************
    Protected m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke ' System.ComponentModel.ISynchronizeInvoke
    '* do not let this property show up in the property window
    ' <System.ComponentModel.Browsable(False)> _
    Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        Get
            'If Me.Site.DesignMode Then

            Dim host1 As Design.IDesignerHost
            Dim obj1 As Object
            If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
                host1 = CType(Me.GetService(GetType(Design.IDesignerHost)), Design.IDesignerHost)
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

    Private m_DisableSubscriptions As Boolean
    Public Property DisableSubScriptions() As Boolean Implements MfgControl.AdvancedHMI.Drivers.IComComponent.DisableSubscriptions
        Get
            Return m_DisableSubscriptions
        End Get
        Set(ByVal value As Boolean)
            m_DisableSubscriptions = value

            'If value Then
            '    '* Stop the poll timers
            '    For i = 0 To tmrPollList.Count - 1
            '        tmrPollList(i).Enabled = False
            '    Next
            'Else
            '    '* Start the poll timers
            '    For i = 0 To tmrPollList.Count - 1
            '        tmrPollList(i).Enabled = True
            '    Next
            'End If
        End Set
    End Property
#End Region

#Region "ConstructorDestructor"
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support for controls and components
        container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        If DLL Is Nothing Then
            DLL = New System.Collections.Concurrent.ConcurrentDictionary(Of Integer, MfgControl.AdvancedHMI.Drivers.ADSforTwinCATEx)
        End If

        For index = 0 To 255
            waitHandle(index) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.ManualReset)
        Next

        ObjectIDs += 1
        MyObjectID = ObjectIDs
    End Sub


    'Component overrides dispose to clean up the component list.
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        '* The handle linked to the DataLink Layer has to be removed, otherwise it causes a problem when a form is closed
        If disposing Then
            Try
                RemoveDLLConnection(MyDLLInstance)
                'Catch
            Finally
                IsDisposed = True
                '* Stop the subscription thread
                StopSubscriptions = True
                SubscriptionHoldRelease.Set()


                If SubscriptionTask IsNot Nothing AndAlso SubscriptionTask.Status = Threading.Tasks.TaskStatus.Running Then
                    SubscriptionTask.Wait(5000)
                End If

                For i = 0 To 255
                    If waitHandle(i) IsNot Nothing Then
                        waitHandle(i).Dispose()
                    End If
                Next

                MyBase.Dispose(disposing)
            End Try
        End If
        'MyBase.Dispose(disposing)
    End Sub



    '***************************************************************
    '* Create the Data Link Layer Instances
    '* if the AMS NET Address is the same, then resuse a common instance
    '***************************************************************
    Private CreateDLLLockObject As New Object
    Private Sub CreateDLLInstance()
        '*** For Windows CE port, this checks designmode and works in full .NET also***
        If AppDomain.CurrentDomain.FriendlyName.IndexOf("DefaultDomain", System.StringComparison.CurrentCultureIgnoreCase) >= 0 Then
            Exit Sub
        End If

        '* Still default, so ignore
        If String.Compare(m_TargetAMSNetID, "0.0.0.0.0.0") = 0 Then Exit Sub

        SyncLock (CreateDLLLockObject)
            '* Check to see if it has the same IP address and Port
            '* if so, reuse the instance, otherwise create a new one
            Dim KeyFound As Boolean
            For Each d In DLL
                If d.Value IsNot Nothing Then
                    If (d.Value.ServerNetID = m_TargetAMSNetID) Then
                        MyDLLInstance = d.Key
                        KeyFound = True
                        Exit For
                    End If
                End If
            Next

            If Not KeyFound Then
                NextDLLInstance += 1
                MyDLLInstance = NextDLLInstance
            End If

            If (Not DLL.ContainsKey(MyDLLInstance) OrElse (DLL(MyDLLInstance) Is Nothing)) Then
                DLL(MyDLLInstance) = New MfgControl.AdvancedHMI.Drivers.ADSforTwinCATEx

                '* Try to resolve the DNS name
                Dim ip As System.Net.IPAddress = Nothing
                If System.Net.IPAddress.TryParse(m_TargetIPAddress, ip) Then
                    DLL(MyDLLInstance).TargetIPAddress = m_TargetIPAddress
                Else
                    Dim ipe As System.Net.IPHostEntry
                    Try
                        'Using ping As New System.Net.NetworkInformation.Ping
                        'If ping.Send(m_TargetIPAddress).Status = System.Net.NetworkInformation.IPStatus.Success Then
                        ipe = System.Net.Dns.GetHostEntry(m_TargetIPAddress)
                        'Else
                        'Dim e As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(11, m_TargetIPAddress & " not found.")
                        'OnComError(e)
                        'Exit Sub
                        'End If
                        'End Using
                    Catch ex As Exception
                        Dim e As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(11, m_TargetIPAddress & " not found.")
                        OnComError(e)
                        Exit Sub
                    End Try
                    DLL(MyDLLInstance).TargetIPAddress = ipe.AddressList(0).ToString
                End If

                Try
                    DLL(MyDLLInstance).ServerNetID = m_TargetAMSNetID
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show("Invalid AMS Address")
                End Try
                DLL(MyDLLInstance).ServerPort = m_TargetAMSPort
                DLL(MyDLLInstance).UserName = m_UserName
                DLL(MyDLLInstance).Password = m_Password
                DLL(MyDLLInstance).UseStaticRoute = m_UseStaticRoute
                ' DLL(MyDLLInstance) = NewDLL
            End If


            '* Have we already attached event handler to this data link layer?
            If EventHandlerDLLInstance <> (MyDLLInstance + 1) Then
                '* If event handler to another layer has been created, remove them
                If EventHandlerDLLInstance > 0 Then
                    If DLL.ContainsKey(EventHandlerDLLInstance - 1) Then
                        RemoveDLLConnection(EventHandlerDLLInstance - 1)
                    End If
                End If


                AddHandler DLL(MyDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
                AddHandler DLL(MyDLLInstance).ComError, AddressOf DataLinkLayerComError
                DLL(MyDLLInstance).ConnectionCount += 1
                EventHandlerDLLInstance = MyDLLInstance + 1
            End If
        End SyncLock
    End Sub

    Private Sub RemoveDLLConnection(ByVal instance As Integer)
        '* The handle linked to the DataLink Layer has to be removed, otherwise it causes a problem when a form is closed
        If DLL.ContainsKey(instance) AndAlso DLL(instance) IsNot Nothing Then
            RemoveHandler DLL(instance).DataReceived, AddressOf DataLinkLayerDataReceived
            RemoveHandler DLL(instance).ComError, AddressOf DataLinkLayerComError
            EventHandlerDLLInstance = 0

            DLL(instance).ConnectionCount -= 1

            If DLL(instance).ConnectionCount <= 0 Then
                DLL(instance).Dispose()
                DLL(instance) = Nothing
                Dim x As MfgControl.AdvancedHMI.Drivers.ADSforTwinCATEx = Nothing
                DLL.TryRemove(instance, x)
            End If
        End If
    End Sub
#End Region

#Region "Subscribing"
    '*******************************************************************
    '*******************************************************************
    Private CurrentID As Integer
    Private SubscribeLock As New Object
    Public Function Subscribe(ByVal plcAddress As String, ByVal numberOfElements As Int16, ByVal pollRate As Integer, ByVal callback As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Subscribe
        'Dim Sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo

        SyncLock (SubscribeLock)
            Dim SubExists As Boolean
            Dim k As Integer

            '* This if statement would allow multiple subscriptions
            'If UsedSymbols.Count > 0 Then
            For k = 0 To NewSubscriptions.Count - 1
                If String.Compare(NewSubscriptions(k).Symbol.Name, plcAddress, True) = 0 And NewSubscriptions(k).dlgCallBack = callback Then
                    SubExists = True
                    Exit For
                End If
            Next
            'End If


            If (Not SubExists) Then
                '* The ID is used as a reference for removing polled addresses
                CurrentID += 1

                Dim tmpPA As New SubscriptionInfo

                tmpPA.PollRate = pollRate
                tmpPA.dlgCallBack = callback
                tmpPA.ID = CurrentID
                tmpPA.Symbol = New MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
                tmpPA.Symbol.Name = plcAddress
                'tmpPA.Symbol = Sym
                tmpPA.NumberOfElements = Convert.ToInt32(numberOfElements)

                NewSubscriptions.Add(tmpPA)


                ''* Flag this so it will run the optimizer after the first read
                'SubscriptionListChanges += 1

                If SubscriptionTask Is Nothing OrElse (SubscriptionTask.Status <> Threading.Tasks.TaskStatus.Created And
                                                           SubscriptionTask.Status <> Threading.Tasks.TaskStatus.Running And
                                                           SubscriptionTask.Status <> Threading.Tasks.TaskStatus.WaitingToRun) Then
                    If SubscriptionTask IsNot Nothing Then
                    End If
                    SubscriptionTask = System.Threading.Tasks.Task.Factory.StartNew(AddressOf SubscriptionUpdate, System.Threading.Tasks.TaskCreationOptions.LongRunning)
                End If

                ''* Start the subscription updater if not already running
                'If SubscriptionThread Is Nothing Then
                '    SubscriptionThread = New System.Threading.Thread(AddressOf SubscriptionUpdate)
                '    SubscriptionThread.Name = "SubscriptionUpdater"
                '    '* IF this is False, then hidden secondary forms will not close when the main form is closed
                '    '* https://msdn.microsoft.com/en-us/library/system.threading.thread.isbackground(v=vs.110).aspx
                '    SubscriptionThread.IsBackground = True
                '    '* Prevent 2 threads
                '    If Not SubscriptionThread.IsAlive And Not ThreadStarted Then SubscriptionThread.Start()
                'End If
            End If

            ' Return tmpPA.ID
            'Else
            '* Return the subscription that already exists
            Return NewSubscriptions(k).ID
        End SyncLock
        'End If
        'Return -1
    End Function

    '***************************************************************
    '* Used to sort polled addresses by File Number and element
    '* This helps in optimizing reading
    '**************************************************************
    Private Function SortPolledAddresses(ByVal A1 As SubscriptionInfo, ByVal A2 As SubscriptionInfo) As Integer
        If A1.Symbol.IndexGroup = A2.Symbol.IndexGroup Then
            If A1.Symbol.IndexOffset > A2.Symbol.IndexOffset Then
                Return 1
            ElseIf A1.Symbol.IndexOffset = A2.Symbol.IndexOffset Then
                Return 0
            Else
                Return -1
            End If
        Else
            If A1.Symbol.IndexGroup > A2.Symbol.IndexGroup Then
                Return 1
            Else
                Return -1
            End If
        End If
    End Function

    '**************************************************************
    '* Perform the reads for the variables added for notification
    '* Attempt to optimize by grouping reads
    '**************************************************************
    Private FirstReadCompleted As Boolean
    Private Sub SubscriptionUpdate()
        ThreadStarted = True
        System.Threading.Thread.CurrentThread.Name = "TCATSubscriptionUpdate" & MyObjectID

        Dim ReadTime As New Stopwatch
        Dim i As Integer = 0
        Dim TransactionNumber As Integer
        Dim DelayBetweenPackets As Integer
        Dim Signalled As Boolean
        While Not StopSubscriptions
            If Not m_DisableSubscriptions Then
                i = 0
                While i < GroupedSubscriptionReads.Count
                    DelayBetweenPackets = Convert.ToInt32(Math.Max(1, Math.Floor(m_PollRateOverride / GroupedSubscriptionReads.Count)))
                    ReadTime.Reset()
                    ReadTime.Start()

                    Try
                        If GroupedSubscriptionReads(i).Count > 0 Then
                            TransactionNumber = ReadGroup(GroupedSubscriptionReads(i))

                            Dim TIDByte As Integer = TransactionNumber And 255
                            'Dim response As Integer
                            If Not FirstReadCompleted Then
                                Signalled = waitHandle(TIDByte).WaitOne(35000)
                                'response = WaitForResponse(TIDByte, 25000)
                            Else
                                Signalled = waitHandle(TIDByte).WaitOne(5000)
                                'response = WaitForResponse(TIDByte, 5000)
                            End If
                            Try
                                If Signalled Then
                                    FirstReadCompleted = True
                                    SendToSubscriptions(Requests(TIDByte).Response)
                                Else
                                    'Dim dbg = 0
                                End If
                            Catch ex As Exception
                                'Dim sts = 0
                            End Try
                        Else
                            ' Dim dbg = 0
                        End If
                    Catch ex As Exception
                        'Dim y = 0
                    End Try

                    ReadTime.Stop()

                    '* Evenly space out the reads to avoid SendQue Full
                    If Convert.ToInt32(ReadTime.ElapsedMilliseconds) < DelayBetweenPackets Then
                        'Threading.Thread.Sleep(DelayBetweenPackets - Convert.ToInt32(ReadTime.ElapsedMilliseconds))
                        SubscriptionHoldRelease.WaitOne(DelayBetweenPackets - Convert.ToInt32(ReadTime.ElapsedMilliseconds))
                        'SubscriptionHoldRelease.Reset()
                    End If

                    i += 1
                End While
            End If

            If NewSubscriptions.Count > 0 Then
                CreateGroupedReadList()
            ElseIf (GroupedSubscriptionReads.Count <= 0 Or m_DisableSubscriptions) And Not StopSubscriptions Then
                'Threading.Thread.Sleep(m_PollRateOverride)
                SubscriptionHoldRelease.WaitOne(CInt(Math.Max(m_PollRateOverride, 250)))
                SubscriptionHoldRelease.Reset()
            End If
            ' Threading.Thread.Sleep(m_PollRateOverride)
            'End If
        End While

        ThreadStarted = False
    End Sub

    '****************************************************************************
    '* Group reads together to optimize communications
    '****************************************************************************
    Private Sub CreateGroupedReadList()
        'If SubscriptionListChanges > 0 Then SubscriptionListChanges -= 1

        Dim CombinedSubscriptionReads As New List(Of SubscriptionInfo)
        'GroupedSubscriptionReads.Clear()
        '***************************************************************
        '* Read multiple values when they are close together in memory
        '***************************************************************
        Dim SubIndex, NumberToRead As Integer
        Dim BytesToRead As Int32
        Dim Gr As SubscriptionInfo
        Dim FailedToGetSymbol As Boolean


        SyncLock (SubscribeLock)
            'WorkingSubscriptionList.Clear()
            For index = 0 To NewSubscriptions.Count - 1
                WorkingSubscriptionList.Add(NewSubscriptions(index))
            Next
            NewSubscriptions.Clear()
        End SyncLock

        '* Get the symbol info for any new variables
        While SubIndex < WorkingSubscriptionList.Count
            Try
                If WorkingSubscriptionList(SubIndex).Symbol.IndexGroup <= 0 Then
                    Dim sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo = GetSymbolInfo(WorkingSubscriptionList(SubIndex).Symbol.Name)

                    If sym IsNot Nothing Then
                        Dim tmpSI As New SubscriptionInfo

                        tmpSI.PollRate = WorkingSubscriptionList(SubIndex).PollRate
                        tmpSI.dlgCallBack = WorkingSubscriptionList(SubIndex).dlgCallBack
                        tmpSI.ID = WorkingSubscriptionList(SubIndex).ID
                        tmpSI.Symbol = sym
                        tmpSI.NumberOfElements = Convert.ToInt32(WorkingSubscriptionList(SubIndex).NumberOfElements)

                        WorkingSubscriptionList.RemoveAt(SubIndex)
                        WorkingSubscriptionList.Insert(SubIndex, tmpSI)
                    Else
                        FailedToGetSymbol = True
                    End If
                End If
            Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
                Dim x As Object() = {Me, New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(ex.ErrorCode, ex.Message)}
                m_SynchronizingObject.BeginInvoke(WorkingSubscriptionList(SubIndex).dlgCallBack, x)
                FailedToGetSymbol = True
               Catch ex As Exception
                FailedToGetSymbol = True
            End Try


            If FailedToGetSymbol Then
                SyncLock (SubscribeLock)
                    NewSubscriptions.Add(WorkingSubscriptionList(SubIndex))
                End SyncLock
                WorkingSubscriptionList.RemoveAt(SubIndex)
                FailedToGetSymbol = False
            End If


            SubIndex += 1
        End While

        WorkingSubscriptionList.Sort(AddressOf SortPolledAddresses)


        SubIndex = 0
        While SubIndex < WorkingSubscriptionList.Count
            NumberToRead = 1
            Try
                While (SubIndex + NumberToRead) < WorkingSubscriptionList.Count AndAlso _
                    (WorkingSubscriptionList(SubIndex).Symbol.IndexGroup < &HF000) AndAlso _
                      (WorkingSubscriptionList(SubIndex).Symbol.IndexGroup = WorkingSubscriptionList(SubIndex + NumberToRead).Symbol.IndexGroup) AndAlso _
                      (WorkingSubscriptionList(SubIndex + NumberToRead).Symbol.IndexOffset - WorkingSubscriptionList(SubIndex).Symbol.IndexOffset) <= 20
                    NumberToRead += 1
                End While
            Catch ex As Exception
                'Dim nr = 0
            End Try

            BytesToRead = Convert.ToInt32((WorkingSubscriptionList(SubIndex + NumberToRead - 1).Symbol.IndexOffset + WorkingSubscriptionList(SubIndex + NumberToRead - 1).Symbol.Size) _
                                               - WorkingSubscriptionList(SubIndex).Symbol.IndexOffset)


            Gr = New SubscriptionInfo
            Gr.Symbol = New MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
            Gr.Symbol.IndexGroup = WorkingSubscriptionList(SubIndex).Symbol.IndexGroup
            Gr.Symbol.IndexOffset = WorkingSubscriptionList(SubIndex).Symbol.IndexOffset
            Gr.Symbol.DataType = WorkingSubscriptionList(SubIndex).Symbol.DataType
            '* It will always be 1 because it is based on total bytes
            Gr.Symbol.Size = 1 '  SubscriptionList(SubIndex).Symbol.Size


            Gr.NumberOfElements = BytesToRead

            If Gr.Symbol.IndexGroup > 0 Then
                CombinedSubscriptionReads.Add(Gr)
            End If

            SubIndex += NumberToRead
        End While

        '*************************************************************
        '* Next step is to prepare for Group Reads
        '*************************************************************
        GroupedSubscriptionReads.Clear()
        Dim i As Integer
        While i < CombinedSubscriptionReads.Count
            NumberToRead = 0
            BytesToRead = CombinedSubscriptionReads(i).Symbol.Size * CombinedSubscriptionReads(i).NumberOfElements
            Try
                '* Reading consecutive addresses only seems to work on %M (&H4020), Variable Memory (&H4040)
                'If GroupedSubscriptionReads(i).Symbol.IndexGroup <> &H4040 And GroupedSubscriptionReads(i).Symbol.IndexGroup <> &H4020 Then
                '* Do a Sum Up Read
                'NumberToRead = 0
                Dim ReadBytes As Int32 = 0
                Dim j As Integer = 0

                Dim itemGroup As New System.Collections.ObjectModel.Collection(Of MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite)

                While (j + i) < CombinedSubscriptionReads.Count AndAlso (ReadBytes < 220)

                    Dim a As New MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite
                    a.IndexGroup = CombinedSubscriptionReads(j + i).Symbol.IndexGroup
                    a.IndexOffset = CombinedSubscriptionReads(j + i).Symbol.IndexOffset
                    a.Size = CombinedSubscriptionReads(j + i).Symbol.Size
                    a.Elements = Convert.ToInt32(CombinedSubscriptionReads(j + i).NumberOfElements)

                    ReadBytes += (CombinedSubscriptionReads(j + i).Symbol.Size * CombinedSubscriptionReads(j + i).NumberOfElements)
                    itemGroup.Add(a)

                    NumberToRead += 1
                    j += 1
                End While

                If itemGroup.Count > 0 Then
                    GroupedSubscriptionReads.Add(itemGroup)
                Else
                    'Dim dbg = 0
                End If


            Catch ex As Exception
                'Dim y = 0
            End Try

            i += NumberToRead
        End While
    End Sub

    Private Sub SendToSubscriptions(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Dim Index As Integer
        Dim TNSByte As Integer = e.TransactionNumber And 255
        While Index < WorkingSubscriptionList.Count
            For s = 0 To Requests(TNSByte).SymbolInfo.Count - 1
                If WorkingSubscriptionList(Index).Symbol.IndexGroup = Requests(TNSByte).SymbolInfo(s).IndexGroup Then
                    Dim ByteIndexOffset As Long = WorkingSubscriptionList(Index).Symbol.IndexOffset
                    Dim HighestRequiredByteIndex As Long = WorkingSubscriptionList(Index).Symbol.IndexOffset + (WorkingSubscriptionList(Index).NumberOfElements * WorkingSubscriptionList(Index).Symbol.Size)
                    Dim HighestByteIndexReturned As Long = Requests(TNSByte).SymbolInfo(s).IndexOffset + (Requests(TNSByte).SymbolInfo(s).Size * Requests(TNSByte).SymbolInfo(s).Elements)
                    If (HighestRequiredByteIndex <= HighestByteIndexReturned) _
                                 AndAlso (WorkingSubscriptionList(Index).Symbol.IndexOffset >= Requests(TNSByte).SymbolInfo(s).IndexOffset) Then
                        Dim f As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(e.ErrorId, e.ErrorMessage, CUShort(TNSByte), MyObjectID)
                        'Dim si As Integer
                        Dim Sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
                        Dim v As System.Collections.ObjectModel.Collection(Of String)
                        Try
                            f.PlcAddress = WorkingSubscriptionList(Index).Symbol.Name
                            f.SubscriptionID = WorkingSubscriptionList(Index).ID
                            f.Values = New System.Collections.ObjectModel.Collection(Of String)
                            'Try
                            Sym = GetSymbolInfo(WorkingSubscriptionList(Index).Symbol.Name)
                            'Catch ex As Exception
                            'Dim gsi = 0
                            ' End Try
                            Try
                                v = ExtractData(Requests(TNSByte).SymbolInfo(s).RawData, Convert.ToInt32(ByteIndexOffset - Requests(TNSByte).SymbolInfo(s).IndexOffset), Convert.ToInt32(WorkingSubscriptionList(Index).NumberOfElements), Sym)
                                For i As Integer = 0 To Convert.ToInt32(WorkingSubscriptionList(Index).NumberOfElements - 1)
                                    f.Values.Add(v(i))
                                Next
                            Catch ex As Exception
                                'Dim vv = 0
                            End Try
                        Catch ex As Exception
                            'Dim dbg = 0
                        End Try

                        'SubscriptionList(Index).dlgCallBack(Me, f)
                        If m_SynchronizingObject IsNot Nothing Then
                            Dim x As Object() = {Me, f}
                            m_SynchronizingObject.BeginInvoke(WorkingSubscriptionList(Index).dlgCallBack, x)
                        Else
                            WorkingSubscriptionList(Index).dlgCallBack(Me, f)
                        End If
                    End If
                End If
            Next
            Index += 1
        End While
    End Sub

    Public Function UnSubscribe(ByVal ID As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Unsubscribe
        SyncLock (SubscribeLock)
            Dim i As Integer = 0
            While i < WorkingSubscriptionList.Count AndAlso WorkingSubscriptionList(i).ID <> ID
                i += 1
            End While

            If i < WorkingSubscriptionList.Count Then
                'SubscriptionListChanges += 1
                WorkingSubscriptionList.RemoveAt(i)
                If WorkingSubscriptionList.Count <= 0 And NewSubscriptions.Count <= 0 Then
                    StopSubscriptions = True
                Else
                    CreateGroupedReadList()
                End If
            End If
        End SyncLock
    End Function
#End Region

#Region "Public Methods"
    Public Function Read(ByVal startAddress As String) As String
        Return Read(startAddress, 1)(0)
    End Function

    Public Function Read(ByVal startAddress As String, ByVal numberOfElements As Integer) As String() Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
        Dim TransactionID As Integer = BeginRead(startAddress, numberOfElements)
        Dim TIDByte As Integer = (TransactionID And 255)

        If waitHandle(TIDByte).WaitOne(5000) Then
            ' If WaitForResponse(TIDByte, 3000) = 0 Then
            'Dim data(numberOfElements - 1) As String
            If Requests(TIDByte).Response IsNot Nothing Then
                If Requests(TIDByte).Response.ErrorId = 0 Then
                    '* Put the resulting values into an array of strings to return to the caller
                    Dim tmp(Requests(TIDByte).Response.Values.Count - 1) As String
                    Requests(TIDByte).Response.Values.CopyTo(tmp, 0)
                    Return tmp
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException(Requests(TIDByte).Response.ErrorId, MfgControl.AdvancedHMI.Drivers.ADSUtilities.DecodeMessage(Requests(TIDByte).Response.ErrorId))
                End If
            Else
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException(-25, "No Response Packet Received")
            End If
        Else
            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Response from PLC. Ensure driver settings are correct.")
        End If
    End Function


    Public Function BeginRead(ByVal symbolName As String, ByVal numberOfElements As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginRead
        'Dim SymIndex As Integer
        Dim Sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
        Sym = GetSymbolInfo(symbolName)

        Dim TransactionID As Integer = BeginRead(Sym.IndexGroup, Sym.IndexOffset, _
                                                 Convert.ToInt32(Sym.Size * numberOfElements), symbolName)

        Return TransactionID
    End Function


    Public Function BeginRead(ByVal symbolName As String) As Integer
        Return BeginRead(symbolName, 1)
    End Function

    Public Function BeginRead(ByVal indexGroup As Int64, ByVal indexOffset As Int64, ByVal bytesToRead As Int32, ByVal symbolName As String) As Integer
        Dim TransactionID As Integer
        TransactionID = GetNextTransactionID(32767)

        Dim ar As New MfgControl.AdvancedHMI.Drivers.TwinCATAMSRequest

        Dim TIDByte As Integer = TransactionID And 255
        ar.TransactionNumber = CUShort(TransactionID)

        '* 8-JUN_15 - when reading arrays, it was only returning first element
        Dim sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo = GetSymbolInfo(symbolName)

        Dim a As New MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite
        a.IndexGroup = indexGroup
        a.IndexOffset = indexOffset
        a.Size = sym.Size
        a.Name = symbolName
        a.Elements = CInt(bytesToRead / a.Size)
        Requests(TIDByte) = ar
        Requests(TIDByte).SymbolInfo.Add(a)

        waitHandle(TIDByte).Reset()

        DLL(MyDLLInstance).ReadAny(indexGroup, indexOffset, bytesToRead, TransactionID, MyObjectID)

        Return TransactionID
    End Function

    ' Public Function ReadGroup(ByVal indexGroup As System.Collections.Generic.List(Of UInt32), ByVal indexOffset As System.Collections.Generic.List(Of UInt32), ByVal bytesToRead As System.Collections.Generic.List(Of UInt32)) As String()
    Public Function ReadGroup(ByVal itemGroup As System.Collections.ObjectModel.Collection(Of MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite)) As Integer
        Dim TransactionID As Integer
        TransactionID = GetNextTransactionID(32767)
        Dim TIDByte As Integer = (TransactionID And 255)
        Requests(TIDByte) = New MfgControl.AdvancedHMI.Drivers.TwinCATAMSRequest
        Requests(TIDByte).SymbolInfo = itemGroup
        Requests(TIDByte).CommandIndexGroup = &HF080

        waitHandle(TIDByte).Reset()

        'Dim data() As String = Read(startAddress, numberOfElements)
        DLL(MyDLLInstance).ReadGroup(itemGroup, TransactionID, MyObjectID)

        Return TransactionID
    End Function

    Public Function Write(ByVal startAddress As String, ByVal dataToWrite As String) As String Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Write
        Dim tmp() As String = {dataToWrite}
        Write(startAddress, tmp, 1)

        Return "0"
    End Function

    Public Function Write(ByVal startAddress As String, ByVal dataToWrite() As String, ByVal numberOfElements As UInt16) As Integer
        Dim TransActionID As Integer = BeginWrite(startAddress, numberOfElements, dataToWrite)
        '* Make write synchronous
        ' WaitForResponse(TransactionID, 3000)
        Dim TIDByte As Integer = TransActionID And 255
        waitHandle(TIDByte).WaitOne(3000)
    End Function



    Public Function BeginWrite(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As String) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginWrite
        If Not DLL.ContainsKey(MyDLLInstance) OrElse DLL(MyDLLInstance) Is Nothing Then CreateDLLInstance()

        Dim TransactionID As Integer
        TransactionID = GetNextTransactionID(32767)
        Try
            Dim Sym As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
            'Dim SymIndex As Integer
            Try
                Sym = GetSymbolInfo(startAddress)
            Catch ex2 As Exception
                System.Windows.Forms.MessageBox.Show("WriteData2b. " & ex2.Message)
                Throw
            End Try


            Dim LengthOfData As Int32 = Sym.Size
            If Sym.IsArray And Sym.ArrayElements > 0 Then
                LengthOfData = CInt(Sym.Size / Sym.ArrayElements)
            End If
            Dim BytesToWrite(Convert.ToInt32(numberOfElements * LengthOfData - 1)) As Byte

            Try
                For i As Integer = 0 To numberOfElements - 1
                    Select Case Sym.DataType
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Bit, MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int8, MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt8
                            BytesToWrite(Convert.ToInt32(i * LengthOfData)) = CByte(dataToWrite(i))
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int16
                            BitConverter.GetBytes(CType(dataToWrite(i), Int16)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt16
                            BitConverter.GetBytes(CType(dataToWrite(i), UInt16)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int32
                            BitConverter.GetBytes(CType(dataToWrite(i), Int32)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt32
                            BitConverter.GetBytes(CType(dataToWrite(i), UInt32)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int64
                            BitConverter.GetBytes(CType(dataToWrite(i), Int64)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt64
                            BitConverter.GetBytes(CType(dataToWrite(i), UInt64)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Real32
                            BitConverter.GetBytes(CType(dataToWrite(i), Single)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Real64
                            BitConverter.GetBytes(CType(dataToWrite(i), Double)).CopyTo(BytesToWrite, i * LengthOfData)
                        Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_String
                            'Return System.Text.Encoding.ASCII.GetString(values, 0, UsedSymbols(i).ByteCount / NumberOfElements).Trim(Chr(0))
                            'System.Text.Encoding.ASCII.GetBytes(dataToWrite(i).Trim(Chr(0)), 0, UsedSymbols(i).ByteCount).CopyTo(BytesToWrite( 12 + i * LengthOfData)
                            Dim TrimmedString As String = dataToWrite(i).Trim(Convert.ToChar(0))
                            System.Text.Encoding.ASCII.GetBytes(TrimmedString.ToCharArray, 0, TrimmedString.Length).CopyTo(BytesToWrite, i * LengthOfData)
                        Case Else
                            BitConverter.GetBytes(CType(dataToWrite(i), Byte)).CopyTo(BytesToWrite, i * LengthOfData)
                    End Select
                Next
            Catch ex As Exception
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException(-22, numberOfElements & "Elements in Loop " & ex.Message)
            End Try


            Try
                Dim ar As New MfgControl.AdvancedHMI.Drivers.TwinCATAMSRequest

                Dim TIDByte As Integer = TransactionID And 255
                ar.TransactionNumber = CUShort(TransactionID)
                Requests(TIDByte) = ar

                Dim a As New MfgControl.AdvancedHMI.Drivers.TcAdsSymbolReadWrite
                a.IndexGroup = Sym.IndexGroup
                a.IndexOffset = Sym.IndexOffset
                a.Name = startAddress
                Requests(TIDByte).SymbolInfo.Add(a)



                DLL(MyDLLInstance).WriteAny(Sym.IndexGroup, Sym.IndexOffset, BytesToWrite, TransactionID, MyObjectID)

            Catch ex1 As Exception
                System.Windows.Forms.MessageBox.Show("WriteData2a. " & ex1.Message)
                Throw
            End Try
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show("WriteData2. " & ex.Message)
            Throw
        End Try

        Return TransactionID
    End Function

    Private GetSymbolSync As New Object
    Public Function GetSymbolInfo(ByVal symbolName As String) As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
        '* Added for v3.99m
        If Not DLL.ContainsKey(MyDLLInstance) OrElse DLL(MyDLLInstance) Is Nothing Then CreateDLLInstance()

        ' SyncLock (GetSymbolSync)

        '* Look for the existing Symbol Information
        Dim i As Integer
        While i < UsedSymbols.Count AndAlso String.Compare(UsedSymbols(i).Name, symbolName.ToUpper, True) <> 0
            i += 1
        End While

        If i < UsedSymbols.Count Then
            'Return i
            Return UsedSymbols(i)
        End If

        '***************************************************
        '* Was not found, so get it from controller by
        '* using a Read/Write(Command ID 9) to Group 0xF009 (ADSIGRP_SYM_INFOBYNAMEEX)
        '****************************************************
        Dim SymbolNameMod As String = symbolName
        '* All addresses require a ".", even globals need it in fron
        '* If there is not ".", then assume it's global and add a period
        If SymbolNameMod.IndexOf(".") < 0 Then
            SymbolNameMod = "." & symbolName
        End If

        Dim symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo
        Dim ID As Integer
        ID = GetNextTransactionID(32767)
        'Try
        symbol = DLL(MyDLLInstance).ReadSymbolInfo(SymbolNameMod, ID, MyObjectID)
        'Catch ex As Exception
        'Dim dbg = 0
        'Exit Function
        'End Try

        If symbol.IndexGroup > 0 Then
            Dim AlreadyInList As Boolean
            If UsedSymbols.Count > 0 Then
                Dim index2 As Integer
                While (index2 < UsedSymbols.Count) AndAlso (Not AlreadyInList)
                    If (symbol.IndexGroup = UsedSymbols(index2).IndexGroup) AndAlso (symbol.IndexOffset = UsedSymbols(index2).IndexOffset) AndAlso _
                             (String.Compare(symbolName, UsedSymbols(index2).Name, True) = 0) Then
                        AlreadyInList = True
                    End If
                    index2 += 1
                End While
            End If

            If Not AlreadyInList Then
                UsedSymbols.Add(symbol)
            End If
        End If

        '* Was a period added to the beginning? If so, then take back off
        If String.Compare(SymbolNameMod, symbolName, True) <> 0 Then
            symbol.Name = symbolName.ToUpper
            'UsedSymbols(UsedSymbols.Count - 1).Name = symbolName.ToUpper
        End If

        'Return UsedSymbols.Count - 1
        Return symbol
        'End SyncLock
    End Function
#End Region

#Region "Private Methods"
    Private Shared Function ExtractData(ByVal dataIN As System.Collections.ObjectModel.Collection(Of Byte), ByVal startIndex As Integer, ByVal elementCount As UInt32, ByVal symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo) As System.Collections.ObjectModel.Collection(Of String)
        Dim data(dataIN.Count - startIndex - 1) As Byte
        For index = 0 To data.Length - 1
            data(index) = dataIN(index + startIndex)
        Next

        Return ExtractData(data, startIndex, Convert.ToInt32(elementCount), symbol)
    End Function


    Private Shared Function ExtractData(ByVal dataIN As List(Of Byte), ByVal startIndex As Integer, ByVal elementCount As UInt32, ByVal symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo) As System.Collections.ObjectModel.Collection(Of String)
        Dim data(dataIN.Count - startIndex - 1) As Byte
        For index = 0 To data.Length - 1
            data(index) = dataIN(index + startIndex)
        Next

        Return ExtractData(data, startIndex, Convert.ToInt32(elementCount), symbol)
    End Function


    Private Shared Function ExtractData(ByVal dataIN As System.Collections.ObjectModel.Collection(Of String), ByVal startIndex As Integer, ByVal elementCount As Integer, ByVal symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo) As System.Collections.ObjectModel.Collection(Of String)
        Dim v(dataIN.Count - 1 - startIndex) As Byte

        For index = startIndex To (startIndex + v.Length - 1)
            v(index - startIndex) = CByte(Convert.ToInt32(dataIN(index)) And 255)
        Next

        Return ExtractData(v, 0, elementCount, symbol)
    End Function

    Private Shared Function ExtractData(ByVal dataIN() As Byte, ByVal startIndex As Integer, ByVal elementCount As Integer, ByVal symbol As MfgControl.AdvancedHMI.Drivers.TcAdsSymbolInfo) As System.Collections.ObjectModel.Collection(Of String)
        Dim LengthOfData As Integer
        LengthOfData = MfgControl.AdvancedHMI.Drivers.ADSUtilities.GetByteCount(symbol)

        Dim data(dataIN.Length - startIndex - 1) As Byte
        For index = 0 To data.Length - 1
            data(index) = dataIN(index + startIndex)
        Next
        'Dim ValueCount As Integer = Convert.ToInt32(Math.Floor(data.Length / LengthOfData))
        Dim values As New System.Collections.ObjectModel.Collection(Of String)

        Dim i As Integer
        While (i) < elementCount
            Select Case symbol.DataType
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Bit
                    ' Dim BitValue As Integer = Convert.ToInt32(Math.Floor(UsedSymbols(symbolIndex).IndexOffset / 8.0))
                    ' BitValue = Convert.ToInt32(2 ^ (UsedSymbols(symbolIndex).IndexOffset - BitValue * 8))
                    ' values.Add(Convert.ToString(Convert.ToBoolean((Convert.ToInt32(data(i * LengthOfData)) And BitValue) > 0)))
                    values.Add(Convert.ToString(Convert.ToBoolean(data(i * LengthOfData))))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int8
                    values.Add(data(i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt8
                    values.Add(data(i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int16
                    values.Add(BitConverter.ToInt16(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt16
                    values.Add(BitConverter.ToUInt16(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int32
                    values.Add(BitConverter.ToInt32(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt32
                    values.Add(BitConverter.ToUInt32(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Int64
                    values.Add(BitConverter.ToInt64(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_UInt64
                    values.Add(BitConverter.ToUInt64(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Real32
                    values.Add(BitConverter.ToSingle(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_Real64
                    values.Add(BitConverter.ToDouble(data, i * LengthOfData).ToString(System.Globalization.CultureInfo.InvariantCulture))
                Case MfgControl.AdvancedHMI.Drivers.ADSDatatypeId.ADST_String
                    Dim str As String = System.Text.Encoding.ASCII.GetString(data, i * LengthOfData, Convert.ToInt32(symbol.Size))
                    values.Add(str.Substring(0, str.IndexOf(Convert.ToChar(0))))
                Case Else
                    values.Add(data(i).ToString)
            End Select
            i += 1
        End While

        Return values
    End Function


    Private Function GetNextTransactionID(ByVal max As Integer) As Integer
        Return DLL(MyDLLInstance).GetInvokeID(max)
    End Function



    '****************************************************
    '* Wait for a response from PLC before returning
    '* Used for Synchronous communications
    '****************************************************
    'Private MaxTicks As Integer = 600  '* 100 ticks per second
    'Private WaitLock As New Object
    'Private Function WaitForResponse(ByVal ID As Integer, ByVal waitTime As Integer) As Integer
    '    ID = ID And 255

    '    '* Make sure there is a request to wait for
    '    If Requests(ID) Is Nothing OrElse Requests(ID).SymbolInfo.Count <= 0 Then
    '        Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException(-21, "A Wait was called for a request that doesn't exist")
    '    End If

    '    If waitTime > 0 Then
    '        MaxTicks = CInt(waitTime / 5)
    '    Else
    '        MaxTicks = 600
    '    End If

    '    SyncLock (WaitLock)
    '        Dim Loops As Integer = 0
    '        While ((Requests(ID).Response Is Nothing) OrElse (Not Requests(ID).Responded)) And (Loops < MaxTicks)
    '            System.Threading.Thread.Sleep(5)
    '            Loops += 1
    '        End While

    '        If Loops >= MaxTicks Then
    '            Return -20
    '        Else
    '            '* Only let the 1st time be a long delay
    '            MaxTicks = 250
    '            Return 0
    '        End If
    '    End SyncLock
    'End Function
#End Region

#Region "Events"
    Private Sub DataLinkLayerDataReceived(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If e.OwnerObjectID = MyObjectID Then
            Try
                '*************************************
                '* handle the data received
                '*************************************
                Dim AMSPacket As New MfgControl.AdvancedHMI.Drivers.AMSTCPPacket(e.RawData, e.RawData.Length)
                Dim TIDByte As Integer = (AMSPacket.TransactionNumber And 255)
                '* Is there a Request to match this response
                If Requests(TIDByte) Is Nothing Then
                    Exit Sub
                End If

                If Requests(TIDByte).Response IsNot Nothing Then
                    Exit Sub
                End If

                Try
                    '* Put a copy of this response with each symbol requested
                    For i = 0 To Requests(TIDByte).SymbolInfo.Count - 1
                        Requests(TIDByte).Response = CType(e.Clone, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
                    Next
                    'e.TransactionNumber = Packet.InvokeId
                Catch ex As Exception
                    OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(3, "eDataReceived (4) - " & ex.Message))
                End Try



                '* Error Code =0 (no errors)
                If AMSPacket.ErrorCode = 0 Then
                    '* Make sure this is a response and not a request from the remote system
                    If AMSPacket.IsResponse Then
                        If AMSPacket.CommandId = MfgControl.AdvancedHMI.Drivers.ADSCommand.Read Then
                            Dim Resp As New MfgControl.AdvancedHMI.Drivers.ADSReadResponse(AMSPacket.EncapsulatedData)
                            Requests(TIDByte).Response.Values = New System.Collections.ObjectModel.Collection(Of String)
                            'For index = 0 To Resp.EncapsulatedData.Count - 1
                            Dim result(Convert.ToInt32(Requests(TIDByte).SymbolInfo(0).Size * Requests(TIDByte).SymbolInfo(0).Elements) - 1) As Byte
                            For index = 0 To result.Length - 1
                                result(index) = Resp.EncapsulatedData(index)
                            Next
                            'Array.ConstrainedCopy(Resp.EncapsulatedData.ToArray, 0, result, 0, result.Length)

                            Requests(TIDByte).SymbolInfo(0).RawData = result
                            'Requests(TIDByte).Response.Values.Add(Convert.ToString(Resp.EncapsulatedData(index)))
                            'Next

                            If Not String.IsNullOrEmpty(Requests(TIDByte).SymbolInfo(0).Name) Then
                                'e.Values = ExtractData(AMSPacket.EncapsulatedData, 0, Convert.ToUInt32(AMSPacket.EncapsulatedData.Count), GetSymbolInfo(Requests(TIDByte).SymbolInfo(0).Name))
                                e.Values = ExtractData(Resp.EncapsulatedData, 0, Convert.ToUInt32(Resp.EncapsulatedData.Count / Requests(TIDByte).SymbolInfo(0).Size), GetSymbolInfo(Requests(TIDByte).SymbolInfo(0).Name))

                                Requests(TIDByte).Response.Values = e.Values
                            End If

                            '* Let the parent class know that data was rcvd
                            Try
                                If AMSPacket.SourceAMSPort >= 800 And AMSPacket.SourceAMSPort <= 804 Then
                                    '* Mark the packet with this Invoke ID as responded
                                    If Requests(TIDByte) IsNot Nothing AndAlso Requests(TIDByte).SymbolInfo.Count > 0 Then
                                        Requests(TIDByte).Responded = True
                                        waitHandle(TIDByte).Set()
                                    End If
                                    OnDataReceived(e)
                                End If
                            Catch ex As Exception
                                OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(3, "eDataReceived RaiseEvent DataReceived - " & ex.Message))
                            End Try
                        ElseIf AMSPacket.CommandId = MfgControl.AdvancedHMI.Drivers.ADSCommand.ReadWrite Then
                            If Requests(TIDByte) IsNot Nothing Then
                                Dim Resp As New MfgControl.AdvancedHMI.Drivers.ADSReadWriteResponse(AMSPacket.EncapsulatedData)
                                '* &HF080 is a group read (sum up)
                                If Requests(TIDByte).CommandIndexGroup <> &HF080 Then
                                    For index = 0 To Resp.EncapsulatedData.Count - 1
                                        Requests(TIDByte).Response.Values.Add(Convert.ToString(Resp.EncapsulatedData(index)))
                                    Next
                                Else
                                    Dim DataOffset As Integer = Requests(TIDByte).SymbolInfo.Count * 4
                                    'Dim result As UInt32
                                    For i = 0 To Requests(TIDByte).SymbolInfo.Count - 1
                                        '* First 4 bytes is the ADS Error code
                                        Dim b() As Byte = {Resp.EncapsulatedData(i * 4), Resp.EncapsulatedData(i * 4 + 1),
                                                           Resp.EncapsulatedData(i * 4 + 2), Resp.EncapsulatedData(i * 4 + 3)}
                                        Requests(TIDByte).SymbolInfo(i).ReadResult = BitConverter.ToUInt32(b, 0)

                                        If Requests(TIDByte).SymbolInfo(i).ReadResult = 0 Then
                                            Dim result(Convert.ToInt32(Requests(TIDByte).SymbolInfo(i).Size * Requests(TIDByte).SymbolInfo(i).Elements) - 1) As Byte
                                            For index = 0 To result.Length - 1
                                                result(index) = Resp.EncapsulatedData(index + DataOffset)
                                            Next
                                            'Array.ConstrainedCopy(Resp.EncapsulatedData.ToArray, DataOffset, result, 0, result.Length)

                                            Requests(TIDByte).SymbolInfo(i).RawData = result
                                        End If
                                        DataOffset += Convert.ToInt32(Requests(TIDByte).SymbolInfo(i).Size * Requests(TIDByte).SymbolInfo(i).Elements)
                                    Next
                                End If
                                '* Mark the packet with this Invoke ID as responded
                                Requests(TIDByte).Responded = True
                                waitHandle(TIDByte).Set()
                            End If
                        ElseIf AMSPacket.CommandId = MfgControl.AdvancedHMI.Drivers.ADSCommand.Write Then
                            If Requests(TIDByte) IsNot Nothing Then
                                Requests(TIDByte).Responded = True
                                waitHandle(TIDByte).Set()
                            End If
                        End If
                    End If
                Else
                    Requests(TIDByte).Responded = True
                    waitHandle(TIDByte).Set()
                    '* An ADS error
                    OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(AMSPacket.ErrorCode, "ADS Error " & AMSPacket.ErrorCode & ". " & MfgControl.AdvancedHMI.Drivers.ADSUtilities.DecodeMessage(AMSPacket.ErrorCode)))
                End If
            Catch ex As Exception
                OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(3, "eDataReceived (3) - " & ex.Message))
            End Try

            'Requests(e.TransactionNumber).Responded = True
            'OnDataReceived(e)
        End If
    End Sub

    Protected Overridable Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '* If a SynchronizingObject is specified, then sync it back to the same thread
        If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
            m_SynchronizingObject.BeginInvoke(drsd, New Object() {Me, e})
        Else
            RaiseEvent DataReceived(Me, e)
        End If
    End Sub

    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Dim drsd As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs) = AddressOf DataReceivedSync
    Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataReceived(Me, e)
    End Sub

    '***************************************************************************************
    '* If an error comes back from the driver, return the description back to the control
    '***************************************************************************************
    Protected Friend Sub DataLinkLayerComError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If e IsNot Nothing AndAlso e.OwnerObjectID = MyObjectID Then
            If e.TransactionNumber >= 0 Then
                Dim TIDByte As Integer = e.TransactionNumber And 255

                If Requests(TIDByte) IsNot Nothing Then
                    '* Save this for other uses
                    Requests(TIDByte).Response = e

                    ''* This is kind of a patch because the response can occur too fast
                    'If Requests(TIDByte) Is Nothing Then
                    '    System.Threading.Thread.Sleep(250)
                    'End If

                    Requests(TIDByte).Responded = True
                    waitHandle(TIDByte).Set()

                    OnComError(e)

                    SendToSubscriptions(e)
                End If
            End If
        End If
    End Sub


    '***********************************************************************************************************
    Protected Overridable Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        If (m_SynchronizingObject IsNot Nothing) AndAlso DirectCast(m_SynchronizingObject, System.Windows.Forms.Control).IsHandleCreated Then
            Dim Parameters() As Object = {Me, e}
            m_SynchronizingObject.BeginInvoke(errorsd, Parameters)
        Else
            RaiseEvent ComError(Me, e)
        End If
    End Sub

    Private errorsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf ErrorReceivedSync)
    Private Sub ErrorReceivedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent ComError(sender, e)
    End Sub
#End Region

    Private Initializing As Boolean
    Private Sub BeginInit() Implements ISupportInitialize.BeginInit
        Initializing = True
    End Sub

    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        If Not Me.DesignMode Then
            If Not String.IsNullOrEmpty(m_IniFileName) Then
                Try
                    Utilities.SetPropertiesByIniFile(Me, m_IniFileName, m_IniFileSection)
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show("INI File - " & ex.Message)
                End Try
            End If
        End If

        Initializing = False


        'CreateDLLInstance()
    End Sub
End Class
