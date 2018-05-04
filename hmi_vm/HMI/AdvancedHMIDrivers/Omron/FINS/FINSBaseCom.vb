Option Strict On
'******************************************************************************
'* Base FINS
'*
'* Copyright 2011 Archie Jacobs
'*
'* Reference : Omron W342-E1-15 (W342-E1-15+CS-CJ-CP-NSJ+RefManual.pdf)
'* Revision February 2010
'*
'* This class must be inherited by a class that implements the
'* data link layer (e.g RS232 (Host Link) or Ethernet TCP)
'*
'* 08-JAN-12 Changed to use only 1 poll rate timer with divisors
'* 10-JAN-12 Check if this packet belongs to this instance in ComError
'* 31-JAN-12 Added IsSubscriptionActive function
'* 31-JAN-12 Added GetSubscriptionAddress function
'* 06-MAR-12 V1.11 Verify that enough elements were returned before continuing
'* 07-MAR-12 V1.12 If subscription count changes while processing data, then ignore
'* 09-MAR-12 V1.15 
'* 09-MAR-12 V1.16 Report error on exception
'* 10-MAR-12 V1.20 Remove subscriptions just before updating
'*              changed pollAddressList to class from structure to allow MarkedForDeletion to be set
'* 22-MAR-12 V1.26 Added UnsubscribeAll function
'***************************************************************************************************

Namespace Omron
    Public MustInherit Class FINSBaseCom
        Inherits OmronBaseCom
        Implements MfgControl.AdvancedHMI.Drivers.IComComponent
        Implements IDisposable
        Implements System.ComponentModel.ISupportInitialize

        Friend MustOverride Function SendData(ByVal FinsF As MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame) As Boolean

        Private GroupedSubscriptionReads As New System.Collections.Concurrent.ConcurrentDictionary(Of Integer, MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress)
        Private GroupChangeLock As New Object

        Private IsDisposed As Boolean '* Without this, it can dispose the DLL completely

        Private Shared ObjectIDs As Int64
        Private MyObjectID As Int64


#Region "Properties"
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property TargetNetworkAddress() As Byte
            Get
                Return TargetAddress.NetworkAddress
            End Get
            Set(ByVal value As Byte)
                TargetAddress.NetworkAddress = value
            End Set
        End Property



        '<System.ComponentModel.Category("Communication Settings")> _
        'Public Property TargetNodeAddress() As Byte
        '    Get
        '        Return TargetAddress.NodeAddress
        '    End Get
        '    Set(ByVal value As Byte)
        '        TargetAddress.NodeAddress = value
        '    End Set
        'End Property


        ''************************************************************
        ''* If this is false, then wait for response before returning
        ''* from read and writes
        ''************************************************************
        'Private m_AsyncMode As Boolean
        'Public Property AsyncMode() As Boolean
        '    Get
        '        Return m_AsyncMode
        '    End Get
        '    Set(ByVal value As Boolean)
        '        m_AsyncMode = value
        '    End Set
        'End Property

        'Private m_TreatDataAsHex As Boolean
        '<System.ComponentModel.Category("Communication Settings")> _
        'Public Property TreatDataAsHex() As Boolean
        '    Get
        '        Return m_TreatDataAsHex
        '    End Get
        '    Set(ByVal value As Boolean)
        '        m_TreatDataAsHex = value
        '    End Set
        'End Property

        'Private m_PollRateOverride As Integer
        '<System.ComponentModel.Category("Communication Settings")> _
        'Public Property PollRateOverride() As Integer
        '    Get
        '        Return m_PollRateOverride
        '    End Get
        '    Set(ByVal value As Integer)
        '        m_PollRateOverride = value
        '    End Set
        'End Property

        '**************************************************
        '* Its purpose is to fetch
        '* the main form in order to synchronize the
        '* notification thread/event
        '**************************************************
        'Private m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
        'Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        '    Get
        '        'If (m_SynchronizingObject Is Nothing) AndAlso Me.DesignMode Then
        '        If (m_SynchronizingObject Is Nothing) AndAlso AppDomain.CurrentDomain.FriendlyName.IndexOf("DefaultDomain", System.StringComparison.CurrentCultureIgnoreCase) >= 0 Then
        '            Dim host1 As System.ComponentModel.Design.IDesignerHost
        '            host1 = CType(Me.GetService(GetType(System.ComponentModel.Design.IDesignerHost)), System.ComponentModel.Design.IDesignerHost)
        '            If host1 IsNot Nothing Then
        '                m_SynchronizingObject = CType(host1.RootComponent, System.ComponentModel.ISynchronizeInvoke)
        '            End If
        '            '* Windows CE, comment above 5 lines
        '        End If
        '        Return m_SynchronizingObject
        '    End Get

        '    Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
        '        If Value IsNot Nothing Then
        '            m_SynchronizingObject = Value
        '        End If
        '    End Set
        'End Property

        ''*********************************************************************************
        ''* Used to stop subscription updates when not needed to reduce communication load
        ''*********************************************************************************
        'Private m_DisableSubscriptions As Boolean
        'Public Property DisableSubscriptions() As Boolean Implements MfgControl.AdvancedHMI.Drivers.IComComponent.DisableSubscriptions
        '    Get
        '        Return m_DisableSubscriptions
        '    End Get
        '    Set(ByVal value As Boolean)
        '        m_DisableSubscriptions = value
        '    End Set
        'End Property

        'Private m_Tag As String
        'Public Property Tag() As String
        '    Get
        '        Return m_Tag
        '    End Get
        '    Set(ByVal value As String)
        '        m_Tag = value
        '    End Set
        'End Property
#End Region

#Region "Constructor"
        Protected Sub New()
            ObjectIDs += 1
            MyObjectID = ObjectIDs

            For index = 0 To 255
                waitHandle(index) = New System.Threading.EventWaitHandle(False, System.Threading.EventResetMode.AutoReset)
            Next

            m_PollRateOverride = 500
        End Sub


        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            DisableSubscriptions = True
            QueHoldRelease.Set()

            '* remove all subscriptions
            For i As Integer = 0 To SubscriptionList.Count - 1
                SubscriptionList(i).MarkForDeletion = True
            Next

            MyBase.Dispose(disposing)
        End Sub
#End Region

#Region "IniFileHandling"
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


        Private Initializing As Boolean
        Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
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
        End Sub
#End Region

#Region "Subscription"
        '***************************************************************
        '* Used to sort polled addresses by File Type and element
        '* This helps in optimizing reading
        '**************************************************************
        Private Shared Function SortPolledAddresses(ByVal A1 As MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo, ByVal A2 As MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo) As Integer
            If (A1.Address.MemoryAreaCode > A2.Address.MemoryAreaCode) Or
                (A1.Address.MemoryAreaCode = A2.Address.MemoryAreaCode And (A1.Address.ElementNumber > A2.Address.ElementNumber Or
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
                            ' m_AsyncMode = True
                            SubscriptionList(i).Address.InternalRequest = True
                            'PolledAddressList(i).Address.Tag = PolledAddressList(i).ID

                            j = 0
                            HighestElement = SubscriptionList(i).Address.ElementNumber + SubscriptionList(i).Address.NumberOfElements - 1
                            ElementSpan = HighestElement - SubscriptionList(i).Address.ElementNumber
                            While (i + j + 1) < ItemCountToUpdate AndAlso _
                                SubscriptionList(i + j).Address.MemoryAreaCode = SubscriptionList(i + j + 1).Address.MemoryAreaCode AndAlso _
                                SubscriptionList(i + j).Address.BitsPerElement = SubscriptionList(i + j + 1).Address.BitsPerElement AndAlso _
                                ((SubscriptionList(i + j + 1).Address.ElementNumber + SubscriptionList(i + j + 1).Address.NumberOfElements) - SubscriptionList(i).Address.ElementNumber) < 20

                                'If SubscriptionList(i + j + 1).Address.ElementNumber = 146 Then
                                '    Dim dbg = 0
                                'End If
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
                            'Dim debug = 0
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
                                fw.WriteLine(String.Format("{0:MM/dd/yyyy HH:mm:ss}", DateTime.Now) & " -2,PollUpdate-" & ex1.Message)
                                fw.Close()
                            End Try

                            'm_AsyncMode = SavedAsync
                        Catch ex As Exception
                            OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(-1, ex.Message))
                        End Try
                    End If

                    i += 1 + j
                End While
            End SyncLock
        End Sub

#End Region

#Region "Public Methods"
        'Public Function BeginRead(ByVal startAddress As String, ByVal numberOfElements As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginRead
        '    Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, numberOfElements)
        '    Return BeginRead(address)
        'End Function

        '* Memory Area Read (FINS 1,1)
        '* Reference : Section 3-5-2
        Public Overrides Function BeginRead(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As Integer
            If IsDisposed Then
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("BeginRead. Object is disposed")
            End If

            'Dim address As New OmronPLCAddress(plcAddress, numberOfElements)
            'address.InternalRequest = InternalRequest

            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))

            'SavedRequests(CurrentTNS) = DirectCast(address.Clone, MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress)
            Requests(CurrentTNS) = address
            waitHandle(CurrentTNS).Reset()

            Dim Header As MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame
            Header = New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(MfgControl.AdvancedHMI.Drivers.Omron.GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

            Dim FINSPacket As MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame
            '* MR=1, SR=1
            '* 11-JUN-15 A 0xff was going in the bit position if no bit specified
            Dim BitNumberByte As Integer = Requests(CurrentTNS).BitNumber
            If Requests(CurrentTNS).BitNumber < 0 Or Requests(CurrentTNS).BitNumber > 64 Then
                BitNumberByte = 0
            End If
            Dim data() As Byte = {Requests(CurrentTNS).MemoryAreaCode, CByte((Requests(CurrentTNS).ElementNumber >> 8) And 255), CByte(Requests(CurrentTNS).ElementNumber And 255), _
                                  CByte(BitNumberByte), CByte((Requests(CurrentTNS).NumberOfElements >> 8) And 255), CByte(Requests(CurrentTNS).NumberOfElements And 255)}
            FINSPacket = New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(Header, 1, 1, data, MyObjectID)

            Try
                If SendData(FINSPacket) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    If Not Requests(CurrentTNS).InternalRequest Then
                        Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    End If
                End If
            Catch ex1 As Exception
                Using fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                    fw.WriteLine("1,ReadAny-" & ex1.Message)
                    fw.Close()
                End Using
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try

            Return CurrentTNS
        End Function

        'Public Function BeginRead(ByVal startAddress As String) As Integer  'Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
        '    Return BeginRead(startAddress, 1)
        'End Function

        'Public Function Read(ByVal startAddress As String) As String
        '    Return Read(startAddress, 1)(0)
        'End Function

        'Public Function Read(ByVal startAddress As String, ByVal numberOfElements As Integer) As String() Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
        '    Dim TransactionNumber As Integer = BeginRead(startAddress, numberOfElements)

        '    Dim TransactionByte As Integer = TransactionNumber And 255
        '    Dim Signalled As Boolean = waitHandle(TransactionByte).WaitOne(3000)

        '    If Signalled Then
        '        ' If WaitForResponse(CUShort(TransactionNumber)) = 0 Then
        '        If Requests(TransactionNumber).Response IsNot Nothing Then
        '            Dim tmp(Requests(TransactionNumber).Response.Values.Count - 1) As String
        '            For i As Integer = 0 To tmp.Length - 1
        '                tmp(i) = Requests(TransactionByte).Response.Values(i)
        '            Next
        '            Return tmp
        '        Else
        '            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No reponse from PLC. Ensure baud rate is correct.")
        '        End If
        '    Else
        '        If Requests(TransactionByte).ErrorReturned Then
        '            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error returned " & SavedErrorEventArgs(TransactionNumber).ErrorId)
        '        Else
        '            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No reponse from PLC. Ensure baud rate is correct.")
        '        End If
        '    End If
        'End Function


        '******************************
        '* Read Clock (FINS 7,1)
        '* Reference : Section 5-3-19
        '******************************
        Public Function ReadClock() As String()
            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))

            Dim a As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress
            a.NumberOfElements = 7
            a.BitsPerElement = 8
            a.Address = "CLOCK"
            Requests(CurrentTNS) = a

            Dim Header As MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame
            Header = New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(MfgControl.AdvancedHMI.Drivers.Omron.GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

            Dim FINSPacket As MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame
            '* MR=7, SR=1
            FINSPacket = New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(Header, 7, 1, MyObjectID)

            waitHandle(CurrentTNS And 255).Reset()
            If Not SendData(FINSPacket) Then
                '* not a valid use, so release
                'TNS.ReleaseNumber(CurrentTNS)
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
            End If

            Dim Signalled As Boolean = waitHandle(CurrentTNS And 255).WaitOne(3000)


            ' If m_AsyncMode Then
            'Return New String() {"0"}
            'Else
            Dim index As Integer = Convert.ToInt32(CurrentTNS)
            If Signalled Then
                'If WaitForResponse(CUShort(index)) = 0 Then
                Dim tmp(Requests(index).Response.Values.Count - 1) As String
                For i As Integer = 0 To tmp.Length - 1
                    tmp(i) = Requests(index).Response.Values(i)
                Next
                Return tmp
            Else
                If Requests(index).ErrorReturned Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error Returned " & SavedErrorEventArgs(index).ErrorId)
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No response from PLC. Ensure baud rate is correct.")
                End If
            End If
            'End If
        End Function

        '*******************************************************
        'Public Function Write(ByVal startAddress As String, ByVal dataToWrite As String) As String Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Write
        '    Dim DataAsArray() As String = {dataToWrite}
        '    Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, 1)

        '    Write(address, DataAsArray)
        '    Return "0"
        'End Function

        'Public Function Write(ByVal startAddress As String, ByVal dataToWrite() As String) As Integer
        '    BeginWrite(startAddress, dataToWrite.Length, dataToWrite)
        '    Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, dataToWrite.Length)
        '    Return Write(address, dataToWrite)
        'End Function

        'Public Overrides Function BeginWrite(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As String) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginWrite
        '    If dataToWrite.Length = numberOfElements Then
        '        Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, dataToWrite.Length)
        '        Return Write(address, dataToWrite)
        '    Else
        '        Dim ElementCount As Integer = Math.Min(numberOfElements, dataToWrite.Length)
        '        Dim data(ElementCount - 1) As String
        '        For i = 0 To data.Length - 1
        '            data(i) = dataToWrite(i)
        '        Next
        '        Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(startAddress, data.Length)
        '        Return Write(address, data)
        '    End If
        'End Function

        '* Memory Area Read (FINS 1,2)
        '* Reference : Section 5-3-3
        Public Overrides Function BeginWrite(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress, ByVal dataToWrite() As String) As Integer
            If address Is Nothing Then Throw New ArgumentNullException("WriteData Address parameter cannot be null.")


            '* No data was sent, so exit
            If dataToWrite.Length <= 0 Then Return 0

            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))

            Dim Header As New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(MfgControl.AdvancedHMI.Drivers.Omron.GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

            '* Save this TNS to check if data received was requested by this instance
            'ActiveTNSs.Add(CurrentTNS)

            '* Mark as a write and Save
            address.IsWrite = True
            Requests(CurrentTNS) = address

            '* Attach the instruction data
            Dim dataPacket As New List(Of Byte)
            dataPacket.Add(address.MemoryAreaCode)
            dataPacket.Add(CByte((address.ElementNumber >> 8) And 255))
            dataPacket.Add(CByte((address.ElementNumber) And 255))

            '* 24-SEP-15 A 0xff was going in the bit position if no bit specified. This is so returned data doesn't think a bit was requested
            Dim BitNumberByte As Integer = Requests(CurrentTNS).BitNumber
            If Requests(CurrentTNS).BitNumber < 0 Or Requests(CurrentTNS).BitNumber > 64 Then
                BitNumberByte = 0
            End If
            dataPacket.Add(CByte(BitNumberByte))

            dataPacket.Add(CByte((address.NumberOfElements >> 8) And 255))
            dataPacket.Add(CByte((address.NumberOfElements) And 255))

            If address.BitsPerElement = 16 Then
                Dim x(1) As Byte
                For i As Integer = 0 To dataToWrite.Length - 1
                    If m_TreatDataAsHex Then
                        Dim data As Integer
                        Try
                            data = Convert.ToUInt16(dataToWrite(i), 16)
                        Catch ex As Exception
                            Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Invalid hexadecimal value " & dataToWrite(i))
                        End Try
                        x(0) = CByte(data And 255)
                        x(1) = CByte(data >> 8)
                    Else
                        x = BitConverter.GetBytes(CUShort(dataToWrite(i)))
                        If address.IsBCD Then
                            '* Convert to BCD
                            x(1) = CByte(CUShort(Math.Floor(CDbl(dataToWrite(i)) / 100)))
                            x(0) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(CUShort(dataToWrite(i)) - (x(1) * 100), Globalization.CultureInfo.CurrentCulture))
                            x(1) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(x(1), Globalization.CultureInfo.CurrentCulture))
                        End If
                    End If
                    '* Bitconverter uses LittleEndian
                    '* Omron uses BigEndian, so reverse
                    dataPacket.Add(x(1))
                    dataPacket.Add(x(0))
                Next
            Else
                '* Bit level
                For i As Integer = 0 To dataToWrite.Length - 1
                    If Convert.ToInt32(dataToWrite(i), Globalization.CultureInfo.CurrentCulture) > 0 Then
                        dataPacket.Add(1)
                    Else
                        dataPacket.Add(0)
                    End If
                Next
            End If


            '* MR=1, SR=2
            Dim FINSPacket As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(Header, 1, 2, dataPacket.ToArray, MyObjectID)

            'Dim FINSPacketStream() As Byte = FINSPacket.GetBytes

            '* Wrap the FINS Packet in a Host Link Frame
            'Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, "FA", FINSPacketStream)


            'DLL(MyDLLInstance).SendData(HostLinkPacket.GetByteStream)

            'DLL(MyDLLInstance).SendFinsFrame(FINSPacketStream)

            If SendData(FINSPacket) Then
                Return 0
            Else
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
            End If
            'End SyncLock
        End Function


        '* Write Clock (FINS 7,2)
        '* Reference : Section 5-3-20
        Public Function WriteClock(ByVal dataToWrite() As String) As String()
            '* No data was sent, so exit
            If dataToWrite.Length <= 0 Then Return New String() {"0"}

            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))

            Dim Header As New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(MfgControl.AdvancedHMI.Drivers.Omron.GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

            Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress

            '* Mark as a write and Save
            address.IsWrite = True
            address.BitsPerElement = 8
            address.NumberOfElements = dataToWrite.Length

            Requests(CurrentTNS) = address

            '* Attach the instruction data
            Dim dataPacket As New List(Of Byte)

            Dim x(1) As Byte
            For i As Integer = 0 To dataToWrite.Length - 1
                If m_TreatDataAsHex Then
                    Dim data As Integer
                    Try
                        data = Convert.ToByte(dataToWrite(i), 16)
                    Catch ex As Exception
                        Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Invalid hexadecimal value " & dataToWrite(i))
                    End Try
                    x(0) = CByte(data And 255)
                    x(1) = CByte(data >> 8)

                Else
                    x = BitConverter.GetBytes(CUShort(dataToWrite(i)))
                    If address.IsBCD Then
                        '* Convert to BCD
                        x(1) = CByte(CUShort(Math.Floor(CDbl(dataToWrite(i)) / 100)))
                        x(0) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(CUShort(dataToWrite(i)) - (x(1) * 100), Globalization.CultureInfo.CurrentCulture))
                        x(1) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(x(1), Globalization.CultureInfo.CurrentCulture))
                    End If
                End If
                '* Bitconverter uses LittleEndian
                '* Omron uses BigEndian, so reverse
                'dataPacket.Add(x(1))

                '* TODO:
                '* This command only accepts BCD, so when it converts to hex by host link frame
                Dim tmp As Byte = CByte(Math.Floor(x(0) / 10) * 16)
                Dim tmp2 As Byte = CByte(x(0) - Math.Floor(x(0) / 10) * 10)
                x(0) = tmp + tmp2

                dataPacket.Add(x(0))
            Next


            '* MR=7, SR=2
            Dim FINSPacket As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(Header, 7, 2, dataPacket.ToArray, MyObjectID)

            'Dim FINSPacketStream() As Byte = FINSPacket.GetBytes


            If SendData(FINSPacket) Then
                Return New String() {"0"}
            Else
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
            End If
        End Function
#End Region

#Region "Private Methods"
        'Private dr As New DataReceivedEventHandler(AddressOf DataLinkLayerDataReceived)
        '************************************************
        '* Process data recieved from controller
        '************************************************
        Protected Sub DataLinkLayerDataReceived(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            '* Not enough data to make up a FINS packet
            If e.RawData Is Nothing OrElse e.RawData.Length < 12 Then
                Exit Sub
            End If


            Dim Fins As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(e.RawData)

            Dim sid As UShort = Fins.Header.ServiceId


            '* Does this request belong to this driver instance?
            'If Not MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber.IsMyTNS(sid, MyObjectID) Then
            If MyObjectID <> e.OwnerObjectID Then
                Exit Sub
            End If

            'TNS.ReleaseNumber(sid)

            Requests(sid).Response = e
            'Dim HostFrame As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(e.RawData)

            '* 9-MAR-12 V1.15
            If Requests(sid) Is Nothing OrElse Requests(sid).Response Is Nothing _
                OrElse (sid < 0 Or sid > (Requests.Length - 1)) Then
                Exit Sub
            End If

            e.PlcAddress = Requests(sid).Address

            '* Is it a FINS excapsulated command?
            'If HostFrame.HeaderCode = "FA" Then
            '* Extract the FINS Frame
            'Dim Fins As New FINS.FINSFrame(HostFrame.EncapsulatedData)

            If Fins.EndCode = 0 Then
                If Not Requests(sid).IsWrite Then
                    '* Extract the data - First 2 bytes is the End Code (status)
                    Dim values() As String = ExtractData(Fins.CommandData, 2, Requests(sid))
                    For i As Integer = 0 To values.Length - 1
                        e.Values.Add(values(i))
                    Next

                    '* Verify that enough elements were returned before continuing V1.11 6-MAR-12
                    If e.Values.Count >= Requests(sid).NumberOfElements Then
                        '* Is this from a subscription?
                        ' If Not Requests(sid).InternalRequest Then
                        OnDataReceived(e)
                        'Else
                        '   SendToSubscriptions(e)
                        'End If
                    End If
                End If
                Requests(sid).Responded = True
            Else
                'Dim x As Object = {New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(Fins.EndCode, "Fins Error Code " & MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(CByte(Fins.EndCode >> 8)) & "-" & MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(CByte(Fins.EndCode And 255)))}
                e.ErrorId = Fins.EndCode
                OnComError(e)
                Requests(sid).ErrorReturned = True
            End If


            waitHandle(sid).Set()
        End Sub

        'Private Sub SendToSubscriptions(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    Dim i As Integer
        '    If e.RawData Is Nothing Then
        '        Return
        '    End If

        '    Dim Fins As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(e.RawData)
        '    Dim sid As UShort = Fins.Header.ServiceId
        '    While i < SubscriptionList.Count
        '        '* trap and ignore because subscription may change in the middle of processin
        '        Try
        '            '* 11-MAR-12 V1.20 If a subscription was deleted, then ignore
        '            If Not SubscriptionList(i).MarkForDeletion Then
        '                '* 06-MAR-12 V1.11 Make sure there are enough values returned (4th condition)
        '                If Requests(sid).MemoryAreaCode = SubscriptionList(i).Address.MemoryAreaCode AndAlso _
        '                                                    Requests(sid).ElementNumber <= SubscriptionList(i).Address.ElementNumber AndAlso _
        '                                                   (Requests(sid).ElementNumber + Requests(sid).NumberOfElements) >= (SubscriptionList(i).Address.ElementNumber + SubscriptionList(i).Address.NumberOfElements) AndAlso _
        '                                                   (SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber + SubscriptionList(i).Address.NumberOfElements) <= Requests(sid).Response.Values.Count AndAlso _
        '                                                   SubscriptionList(i).Address.BitNumber >= Requests(sid).BitNumber Then
        '                    Dim f As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(New Byte() {0}, SubscriptionList(i).Address.Address, sid, MyObjectID)
        '                    Dim index As Integer = 0
        '                    Dim StartElement As Integer = SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber
        '                    If Requests(sid).BitsPerElement = 1 Then
        '                        StartElement = SubscriptionList(i).Address.BitNumber - Requests(sid).BitNumber
        '                        StartElement += (SubscriptionList(i).Address.ElementNumber - Requests(sid).ElementNumber) * 16
        '                    End If

        '                    If (StartElement + SubscriptionList(i).Address.NumberOfElements) < (Requests(sid).BitNumber + Requests(sid).Response.Values.Count) Then
        '                        While index < SubscriptionList(i).Address.NumberOfElements
        '                            f.Values.Add(Requests(sid).Response.Values(StartElement + index))
        '                            index += 1
        '                        End While

        '                        Try
        '                            '* 11-MAR-12 V1.20
        '                            If Not SubscriptionList(i).MarkForDeletion Then
        '                                Dim x As Object() = {Me, f}
        '                                If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
        '                                    m_SynchronizingObject.BeginInvoke(SubscriptionList(i).dlgCallback, x)
        '                                Else
        '                                    SubscriptionList(i).dlgCallback(Me, f)
        '                                End If
        '                            End If
        '                        Catch ex As Exception
        '                            Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
        '                            fw.WriteLine("1,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
        '                            fw.Close()
        '                            'Dim debug = 0
        '                            '* V1.16 - mark so it can continue
        '                            Requests(sid).ErrorReturned = True
        '                        End Try
        '                    End If
        '                End If
        '            End If
        '        Catch ex As Exception
        '            Using fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
        '                fw.WriteLine("2,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
        '            End Using
        '            'Dim debug = 0
        '            '* V1.16 - mark so it can continue
        '            Requests(sid).ErrorReturned = True
        '        End Try
        '        i += 1
        '    End While

        'End Sub


#Region "Events"
        'Protected Overridable Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    If m_SynchronizingObject IsNot Nothing Then
        '        Dim Parameters() As Object = {Me, e}
        '        m_SynchronizingObject.BeginInvoke(drsd, Parameters)
        '    Else
        '        RaiseEvent DataReceived(Me, e)
        '    End If
        'End Sub

        'Protected Overridable Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    If m_SynchronizingObject IsNot Nothing Then
        '        m_SynchronizingObject.BeginInvoke(errorsd, New Object() {Me, e})
        '    Else
        '        RaiseEvent ComError(Me, e)
        '    End If
        'End Sub
        ''***********************************************************
        ''* Used to synchronize the event back to the calling thread
        ''***********************************************************
        'Private drsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf DataReceivedSync)
        'Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As EventArgs)
        '    RaiseEvent DataReceived(sender, DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs))
        'End Sub

        'Private errorsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf ErrorReceivedSync)
        'Private Sub ErrorReceivedSync(ByVal sender As Object, ByVal e As EventArgs)
        '    RaiseEvent ComError(sender, DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs))
        'End Sub
#End Region

        'Protected Friend Sub DataLinkLayerComError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    If e.TransactionNumber >= 0 Then
        '        If e.OwnerObjectID <> MyObjectID Then
        '            'If Not MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber.IsMyTNS(e.TransactionNumber, MyObjectID) Then
        '            Exit Sub
        '        End If

        '        '* Save this for other uses
        '        SavedErrorEventArgs(e.TransactionNumber) = e

        '        Requests(e.TransactionNumber).ErrorReturned = True
        '    End If

        '    OnComError(e)

        '    SendToSubscriptions(e)
        'End Sub


        '***************************************
        '* Extract the returned data
        '***************************************
        Private Function ExtractData(ByVal RawData As System.Collections.ObjectModel.Collection(Of Byte), ByVal startByte As Integer, ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As String()
            Dim values(address.NumberOfElements - 1) As String

            Dim NumberOfBytes As Integer = Convert.ToInt32(Math.Ceiling(address.BitsPerElement / 8))


            Dim i As Integer
            While i < address.NumberOfElements And (startByte + i) < RawData.Count   ' (startByte + i) < Math.Floor(RawData.Count / NumberOfBytes)
                'Dim HexByte1 As String = Chr(RawData(startByte + i * NumberOfBytes)) & Chr(RawData(startByte + (i * NumberOfBytes) + 1))
                If NumberOfBytes > 1 Then
                    'Dim HexByte2 As String = Chr(RawData(startByte + (i * NumberOfBytes) + 2)) & Chr(RawData(startByte + (i * NumberOfBytes) + 3))
                    If Not address.IsBCD And Not m_TreatDataAsHex Then
                        'values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes) * 256 + RawData(startByte + i * NumberOfBytes + 1))
                        Dim b() As Byte = {RawData(startByte + i * NumberOfBytes + 1), RawData(startByte + i * NumberOfBytes)}
                        values(i) = Convert.ToString(BitConverter.ToInt16(b, 0), Globalization.CultureInfo.CurrentCulture)
                    Else
                        values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes)) & MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes + 1))
                    End If
                Else
                    If Not m_TreatDataAsHex Then
                        values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes), Globalization.CultureInfo.CurrentCulture)
                    Else
                        values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes))
                    End If
                End If

                i += 1
            End While

            Return values
        End Function
#End Region

        'Private Sub FINSBaseCom_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        '    'Dim dbg = 0
        'End Sub
    End Class
End Namespace
