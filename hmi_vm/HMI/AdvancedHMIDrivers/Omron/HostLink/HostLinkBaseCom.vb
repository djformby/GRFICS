Option Strict On
'******************************************************************************
'* Base Host Link
'*
'* Copyright 2012 Archie Jacobs
'*
'* Reference : Omron W342-E1-15 (W342-E1-15+CS-CJ-CP-NSJ+RefManual.pdf)
'* Revision February 2010
'*
'* This class must be inherited by a class that implements the
'* data link layer (e.g RS232 (Host Link))
'*
'* 29-DEC-12 Created based on HostBaseCom
'***************************************************************************************************
'Imports OmronDriver.Common
Namespace Omron
    Public MustInherit Class HostLinkBaseCom
        Inherits OmronBaseCom
        Implements MfgControl.AdvancedHMI.Drivers.IComComponent
        Implements IDisposable


        'Private HighestPollRateDivisor As Integer
        'Private PollCounts As Integer
        'Private PollRateDivisorList As New List(Of Integer)

        'Private SubscriptionList As New List(Of MfgControl.AdvancedHMI.Drivers.Omron.PolledAddressInfo)
        'Private NewSubscriptionsAdded As Boolean

        Private IsDisposed As Boolean '* Without this, it can dispose the DLL completely

        Private Shared ObjectIDs As Int64
        Private MyObjectID As Int64

        'Protected MustOverride Function GetNextTransactionNumber(ByVal max As Integer) As Integer
        Friend MustOverride Function SendData(ByVal HostLinkF As MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame, ByVal InternalRequest As Boolean) As Boolean



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

#End Region

#Region "Constructor"
        Protected Sub New()
            'If TNS Is Nothing Then
            'TNS = New MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber(0, SavedRequests.Length - 1)
            'End If

            ObjectIDs += 1
            MyObjectID = ObjectIDs
            'InstanceCount += 1
        End Sub

        'Public Sub New(ByVal container As System.ComponentModel.IContainer)
        '    MyClass.New()

        '    'Required for Windows.Forms Class Composition Designer support
        '    container.Add(Me)
        'End Sub


        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            DisableSubscriptions = True
            '* remove all subscriptions
            For i As Integer = 0 To SubscriptionList.Count - 1
                SubscriptionList(i).MarkForDeletion = True
            Next

            DisableSubscriptions = True
            QueHoldRelease.Set()

            MyBase.Dispose(disposing)
        End Sub
#End Region


#Region "Public Methods"
        '* Memory Area Read
        '* Reference : Section 4-3
        Public Overrides Function BeginRead(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As Integer
            If IsDisposed Then
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("BeginRead. Object is disposed")
            End If

            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))
            waitHandle(CurrentTNS).Reset()

            Requests(CurrentTNS) = address


            Dim b(7) As Byte

            Try
                MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ValueToBcdASCII(address.ElementNumber).CopyTo(b, 0)
                MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ValueToBcdASCII(address.NumberOfElements).CopyTo(b, 4)
            Catch ex As Exception
                Throw
            End Try

            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, address.HostLinkReadHeaderCode, b)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Using fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                    fw.WriteLine("1,ReadAny-" & ex1.Message)
                    fw.Close()
                End Using
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try



            '  If m_AsyncMode Then
            Return CurrentTNS
            'Else
            'End If
            'End SyncLock
        End Function





        'Public Function BeginWrite(ByVal address As String, ByVal dataToWrite() As String) As Integer
        '    Return BeginWrite(New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress(address), dataToWrite)
        'End Function


        '* Memory Area Read
        '* Reference : Section 4-3
        Public Overrides Function BeginWrite(ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress, ByVal dataToWrite() As String) As Integer
            '* No data was sent, so exit
            If dataToWrite.Length <= 0 Then Return 0

            Dim CurrentTNS As Byte
            '* Save this 
            CurrentTNS = CByte(GetNextTransactionID(255))
            waitHandle(CurrentTNS).Reset()

            'Dim Header As New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

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
            dataPacket.Add(address.BitNumber)
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
                            x(0) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(CUShort(dataToWrite(i)) - (x(1) * 100)))
                            x(1) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(x(1)))
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
                    If Convert.ToInt32(dataToWrite(i)) > 0 Then
                        dataPacket.Add(1)
                    Else
                        dataPacket.Add(0)
                    End If
                Next
            End If

            Dim b(3 + dataToWrite.Length * 4) As Byte

            '* The element number is in BCD
            MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ValueToBcdASCII(address.ElementNumber).CopyTo(b, 0)


            '* The data is in HEX
            '* 10-JUN-13 Removed the * 4 at the end
            For i As Integer = 0 To (dataToWrite.Length - 1)
                Dim HexBytes() As Byte
                If m_TreatDataAsHex Then
                    '* add the leading 0's
                    While dataToWrite(i).Length < 4
                        dataToWrite(i) = "0" & dataToWrite(i)
                    End While
                    HexBytes = System.Text.Encoding.ASCII.GetBytes(dataToWrite(i))
                Else
                    HexBytes = System.Text.Encoding.ASCII.GetBytes(MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.IntToHex(Convert.ToInt32(dataToWrite(i))))
                End If
                HexBytes.CopyTo(b, 4 + i * 4)
            Next

            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, address.HostLinkWriteHeaderCode, b)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            'Dim HostPacketStream() As Byte = HostLinkPacket.GetByteStream

            If SendData(HostLinkPacket, address.InternalRequest) Then
                Return 0
            Else
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
            End If
        End Function

        Public Function ReadProgram() As Byte()
            If IsDisposed Then
                Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("ReadProgram. Object is disposed")
            End If

            '***************************************************
            '* First initialize to cancel any previous commands
            '***************************************************
            InitializeComs()

            '*************************************************************
            '* READ THE PROGRAM
            '*************************************************************
            '* Save this 
            Dim CurrentTNS As UInt16 = CByte(GetNextTransactionID(255))

            Dim DummyAddress As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress = New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress("RP")
            Requests(CurrentTNS) = DummyAddress

            Dim HostLinkPacket As MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame = New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, DummyAddress.HostLinkReadHeaderCode, Nothing)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                fw.WriteLine("1,ReadProgram2-" & ex1.Message)
                fw.Close()
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try



            'If Not m_AsyncMode Then
            Dim index As Integer = Convert.ToInt32(CurrentTNS)
            If WaitForResponse(CUShort(index), 10000) = 0 Then
                If Requests(index).Response IsNot Nothing Then
                    '* Remove the @00RP.. from the rawdata and the FCS, CR, and *
                    Dim tmp(Requests(index).Response.RawData.Length - 11) As Byte
                    For i As Integer = 0 To tmp.Length - 1
                        tmp(i) = Requests(index).Response.RawData(i + 7)
                    Next
                    Return tmp
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            Else
                If Requests(index).ErrorReturned Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error Returned " & Requests(index).Response.ErrorId)
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            End If
            'End If

            'Dim TNSresult() As Byte = {CByte(CurrentTNS And 255)}
            'Return TNSresult
        End Function


        Public Function WriteProgram(ByVal data() As Byte) As Integer
            InitializeComs()

            '*************************************************************
            '* Write THE PROGRAM
            '*************************************************************
            '* Save this 
            Dim CurrentTNS As Byte = CByte(GetNextTransactionID(255))

            Dim DummyAddress As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress("WP")
            Requests(CurrentTNS) = DummyAddress

            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, DummyAddress.HostLinkWriteHeaderCode, data)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                fw.WriteLine("1,ReadProgram2-" & ex1.Message)
                fw.Close()
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try



            'If Not m_AsyncMode Then
            Dim index As Integer = Convert.ToInt32(CurrentTNS)
            If WaitForResponse(CUShort(index), 10000) = 0 Then
                If Requests(index).Response IsNot Nothing Then
                    Return 0
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            Else
                If Requests(index).ErrorReturned Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error Returned " & Requests(index).Response.ErrorId)
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            End If
            'End If
        End Function

        Public Sub InitializeComs()
            Dim CurrentTNS As Byte = CByte(GetNextTransactionID(255))

            Dim DummyAddress As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress("**")
            Requests(CurrentTNS) = DummyAddress

            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, DummyAddress.HostLinkReadHeaderCode, Nothing)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                fw.WriteLine("1,ReadProgram1-" & ex1.Message)
                fw.Close()
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try
        End Sub

        Public Function SetProgramMode() As Integer
            Return ChangeStatus(0)
        End Function

        Public Function SetMonitorMode() As Integer
            Return ChangeStatus(2)
        End Function

        Public Function SetRunMode() As Integer
            Return ChangeStatus(3)
        End Function

        Public Function ChangeStatus(ByVal Mode As Byte) As Integer
            '*************************************************************
            '* ChangeProcessor mode to PROGRAM
            '*************************************************************
            '* Save this 
            Dim CurrentTNS As Byte = CByte(GetNextTransactionID(255))

            Dim DummyAddress As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress("SC")
            Requests(CurrentTNS) = DummyAddress

            'Send a mode of "00"
            Dim ModeRequestinHex As String = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(Mode)
            Dim s() As Byte = {CByte(Convert.ToInt32(ModeRequestinHex.Substring(0, 1))), CByte(Convert.ToInt32(ModeRequestinHex.Substring(1, 1)))}
            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, DummyAddress.HostLinkWriteHeaderCode, s)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                fw.WriteLine("1,SetProgram2-" & ex1.Message)
                fw.Close()
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try



            '****************************************
            '* This is always a Synchronous Command
            '****************************************
            Dim index As Integer = Convert.ToInt32(CurrentTNS)
            If WaitForResponse(CUShort(index), 10000) = 0 Then
                If Requests(index).Response IsNot Nothing Then
                    Return 0
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            Else
                If Requests(index).ErrorReturned Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Error Returned " & Requests(index).Response.ErrorId)
                Else
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("No Reponse from PLC. Ensure baud rate is correct.")
                End If
            End If
        End Function

        '**********************************************************************
        '* This sends an undocumented MailBox command that unlocks the program
        '**********************************************************************
        Public Sub UnlockPLC(ByVal PassCode As String)
            Dim data(11) As Byte
            data = System.Text.Encoding.ASCII.GetBytes("31060000" & PassCode)
            SendMailBoxData(data)
        End Sub

        Public Sub SendMailBoxData(ByVal data() As Byte)
            '    @00MB31060000

            '* Save this 
            Dim CurrentTNS As Byte = CByte(GetNextTransactionID(255))

            Dim DummyAddress As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress("MB")
            Requests(CurrentTNS) = DummyAddress

            Dim HostLinkPacket As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(0, DummyAddress.HostLinkWriteHeaderCode, data)
            HostLinkPacket.TransactionNumber = CurrentTNS
            HostLinkPacket.OwnerObjectID = MyObjectID

            Try
                If SendData(HostLinkPacket, Requests(CurrentTNS).InternalRequest) Then
                    '* Save this TNS to check if data received was requested by this instance
                    'ActiveTNSs.Add(CurrentTNS)
                Else
                    '* Buffer is full, so release
                    'TNS.ReleaseNumber(CurrentTNS)
                    '* Throw an exception if not an internal request
                    'If Not Requests(CurrentTNS).InternalRequest Then
                    Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Send Buffer is full, may have lost communication.")
                    'End If
                End If
            Catch ex1 As Exception
                Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
                fw.WriteLine("1,SetProgram2-" & ex1.Message)
                fw.Close()
                'Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("ReadAny-Driver Instance Has been disposed")
                Throw
            End Try


        End Sub

        '* Write Clock (FINS 7,2)
        '* Reference : Section 5-3-20
        'Public Function WriteClock(ByVal dataToWrite() As String) As String()
        '    '* No data was sent, so exit
        '    If dataToWrite.Length <= 0 Then Return New String() {"0"}

        '    Dim CurrentTNS As Byte
        '    '* Save this 
        '    CurrentTNS = CByte(TNS.GetNextNumber(Tag))

        '    Dim Header As New MfgControl.AdvancedHMI.Drivers.Omron.FINSHeaderFrame(GatewayCountOption.Three, TargetAddress, SourceAddress, CByte(CurrentTNS))

        '    Dim address As New MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress

        '    '* Mark as a write and Save
        '    address.IsWrite = True
        '    address.BitsPerElement = 8
        '    address.NumberOfElements = dataToWrite.Length

        '    SavedRequests(CurrentTNS) = address

        '    '* Attach the instruction data
        '    Dim dataPacket As New List(Of Byte)

        '    Dim x(1) As Byte
        '    For i As Integer = 0 To dataToWrite.Length - 1
        '        If m_TreatDataAsHex Then
        '            Dim data As Integer
        '            Try
        '                data = Convert.ToByte(dataToWrite(i), 16)
        '            Catch ex As Exception
        '                Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("Invalid hexadecimal value " & dataToWrite(i))
        '            End Try
        '            x(0) = CByte(data And 255)
        '            x(1) = CByte(data >> 8)

        '        Else
        '            x = BitConverter.GetBytes(CUShort(dataToWrite(i)))
        '            If address.IsBCD Then
        '                '* Convert to BCD
        '                x(1) = CByte(CUShort(Math.Floor(CDbl(dataToWrite(i)) / 100)))
        '                x(0) =MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(CUShort(dataToWrite(i)) - (x(1) * 100)))
        '                x(1) =MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.HexToByte(Convert.ToString(x(1)))
        '            End If
        '        End If
        '        '* Bitconverter uses LittleEndian
        '        '* Omron uses BigEndian, so reverse
        '        'dataPacket.Add(x(1))

        '        '* TODO:
        '        '* This command only accepts BCD, so when it converts to hex by host link frame
        '        Dim tmp As Byte = CByte(Math.Floor(x(0) / 10) * 16)
        '        Dim tmp2 As Byte = CByte(x(0) - Math.Floor(x(0) / 10) * 10)
        '        x(0) = tmp + tmp2

        '        dataPacket.Add(x(0))
        '    Next


        '    '* MR=7, SR=2
        '    Dim FINSPacket As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(Header, 7, 2, dataPacket.ToArray)

        '    Dim FINSPacketStream() As Byte = FINSPacket.GetByteStream


        '    If SendData(FINSPacket, address.InternalRequest) Then
        '        Return New String() {"0"}
        '    Else
        '        Throw New MfgControl.AdvancedHMI.Drivers.Common.PlcDriverException("Send Buffer is full, may have lost communication.")
        '    End If
        'End Function


        '****************************************************
        '* Wait for a response from PLC before returning
        '* Used for Synchronous communications
        '****************************************************
        'Private MaxTicks As Integer = 100  '* 50 ticks per second
        Private Function WaitForResponse(ByVal ID As UInt16, ByVal maxTicks As Integer) As Integer
            SyncLock (Me)
                Dim Loops As Integer = 0
                While Not Requests(ID).Responded And Not Requests(ID).ErrorReturned And Loops < maxTicks
                    System.Threading.Thread.Sleep(25)
                    Loops += 1
                End While

                If Loops >= maxTicks Then
                    Return -20
                Else
                    '* Only let the 1st time be a long delay
                    maxTicks = 75
                    Return 0
                End If
            End SyncLock
        End Function
#End Region

#Region "Private Methods"
        '***************************************************************
        '* Create the Data Link Layer Instances
        '* if the IP Address is the same, then resuse a common instance
        '***************************************************************



        'Private dr As New DataReceivedEventHandler(AddressOf DataLinkLayerDataReceived)
        '************************************************
        '* Process data recieved from controller
        '************************************************
        Protected Sub DataLinkLayerDataReceived(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            '* Not enough data to make up a FINS packet
            '* 3-MAR-13 Changed from 12 to 10 for HostLinkCom
            If e.RawData Is Nothing OrElse e.RawData.Length < 10 Then
                Exit Sub
            End If


            '* Does this request belong to this driver instance?
            'If Not MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber.IsMyTNS(e.TransactionNumber, MyObjectID) Then
            If e.OwnerObjectID <> MyObjectID Then
                Exit Sub
            End If

            Dim TNSLowerbyte As Integer = e.TransactionNumber And 255

            Requests(TNSLowerbyte).Response = e
            Dim HostFrame As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkFrame(e.RawData)

            If HostFrame.EndCode <> 0 Then
                OnComError(New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(HostFrame.EndCode, "Error Returned From PLC=" & DecodeEndCode(HostFrame.EndCode)))
                Exit Sub
            End If

            '* 9-MAR-12 V1.15
            If Requests(TNSLowerbyte) Is Nothing OrElse Requests(TNSLowerbyte).Response Is Nothing _
                OrElse (e.TransactionNumber < 0 Or e.TransactionNumber > (Requests.Length - 1)) Then
                Exit Sub
            End If

            e.PlcAddress = Requests(TNSLowerbyte).Address

            If Not Requests(TNSLowerbyte).IsWrite Then
                '* Extract the data
                Dim values() As String = ExtractData(HostFrame.EncapsulatedData, 0, Requests(TNSLowerbyte))
                For i As Integer = 0 To values.Length - 1
                    e.Values.Add(values(i))
                Next

                '* Verify that enough elements were returned before continuing V1.11 6-MAR-12
                If e.Values.Count < Requests(TNSLowerbyte).NumberOfElements Then
                    Exit Sub
                End If

                '* Is this from a subscription?
                '               If Not Requests(e.TransactionNumber).InternalRequest Then
                OnDataReceived(e)
                'Else
                'SendToSubscriptions(e)
                'End If
            End If

            Requests(TNSLowerbyte).Responded = True
            waitHandle(TNSLowerbyte).Set()
        End Sub

        'Private Sub SendToSubscriptions(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    Dim i As Integer
        '    While i < PolledAddressList.Count
        '        '* trap and ignore because subscription may change in the middle of processing
        '        Try
        '            '* 11-MAR-12 V1.20 If a subscription was deleted, then ignore
        '            If Not PolledAddressList(i).MarkForDeletion Then
        '                '* 06-MAR-12 V1.11 Make sure there are enough values returned (4th condition)
        '                If SavedRequests(e.TransactionNumber).MemoryAreaCode = PolledAddressList(i).Address.MemoryAreaCode AndAlso _
        '                                                    SavedRequests(e.TransactionNumber).ElementNumber <= PolledAddressList(i).Address.ElementNumber AndAlso _
        '                                                   (SavedRequests(e.TransactionNumber).ElementNumber + SavedRequests(e.TransactionNumber).NumberOfElements) >= (PolledAddressList(i).Address.ElementNumber + PolledAddressList(i).Address.NumberOfElements) AndAlso _
        '                                                   (PolledAddressList(i).Address.ElementNumber - SavedRequests(e.TransactionNumber).ElementNumber + PolledAddressList(i).Address.NumberOfElements) <= SavedResponse(e.TransactionNumber).Values.Count Then
        '                    Dim f As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(New Byte() {0}, PolledAddressList(i).Address.Address, e.TransactionNumber)
        '                    Dim index As Integer = 0
        '                    While index < PolledAddressList(i).Address.NumberOfElements
        '                        If PolledAddressList(i).Address.BitsPerElement = 1 Then
        '                            f.Values.Add(SavedResponse(e.TransactionNumber).Values(PolledAddressList(i).Address.ElementNumber - SavedRequests(e.TransactionNumber).ElementNumber + index + _
        '                                                                      PolledAddressList(i).Address.BitNumber))
        '                        Else
        '                            f.Values.Add(SavedResponse(e.TransactionNumber).Values(PolledAddressList(i).Address.ElementNumber - SavedRequests(e.TransactionNumber).ElementNumber + index))
        '                        End If
        '                        index += 1
        '                    End While

        '                    Try
        '                        '* 11-MAR-12 V1.20
        '                        If Not PolledAddressList(i).MarkForDeletion Then
        '                            If m_SynchronizingObject IsNot Nothing Then
        '                                m_SynchronizingObject.BeginInvoke(PolledAddressList(i).dlgCallBack, New Object() {Me, f})
        '                            Else
        '                                PolledAddressList(i).dlgCallBack(Me, f)
        '                            End If
        '                        End If
        '                    Catch ex As Exception
        '                        Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
        '                        fw.WriteLine("1,HOstLinkBaseCom.DataLinkLayerDataReceived-" & ex.Message)
        '                        fw.Close()
        '                        'Dim debug = 0
        '                        '* V1.16 - mark so it can continue
        '                        SavedRequests(e.TransactionNumber).ErrorReturned = True
        '                    End Try
        '                End If
        '            End If
        '        Catch ex As Exception
        '            Dim fw As New System.IO.StreamWriter(".\DriverErrorLog.log", True)
        '            fw.WriteLine("2,FinsBaseCom.DataLinkLayerDataReceived-" & ex.Message)
        '            fw.Close()
        '            '* V1.16 - mark so it can continue
        '            SavedRequests(e.TransactionNumber).ErrorReturned = True
        '        End Try
        '        i += 1
        '    End While
        'End Sub

        'Private Sub SendToSubscriptions(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    Dim i As Integer
        '    If e.RawData Is Nothing Then
        '        Return
        '    End If
        '    'Dim Fins As New MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame(New List(Of Byte)(e.RawData))
        '    Dim sid As Int32 = e.TransactionNumber ' Fins.Header.ServiceId
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
        '                            Dim x As Object() = {Me, f}
        '                            '* Allow no synchronizing object V3.99b
        '                            If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
        '                                m_SynchronizingObject.BeginInvoke(SubscriptionList(i).dlgCallback, x)
        '                            Else
        '                                SubscriptionList(i).dlgCallback(Me, f)
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


        'Protected Friend Sub DataLinkLayerComError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    'If MfgControl.AdvancedHMI.Drivers.Common.TransactionNumber.IsMyTNS(e.TransactionNumber, MyObjectID) Then
        '    If e.OwnerObjectID <> MyObjectID Then
        '        If e.TransactionNumber >= 0 Then
        '            '* Save this for other uses
        '            Requests(e.TransactionNumber).Response = e

        '            Requests(e.TransactionNumber).ErrorReturned = True
        '        End If
        '    End If

        '    OnComError(e)

        '    SendToSubscriptions(e)
        'End Sub


        '***************************************
        '* Extract the returned data
        '***************************************
        Private Function ExtractData(ByVal RawData As List(Of Byte), ByVal startByte As Integer, ByVal address As MfgControl.AdvancedHMI.Drivers.Omron.OmronPlcAddress) As String()
            If address.Address = "RP" Then
                'Dim index As Integer
                'Dim values(RawData.Count - 11) As String
                Dim values(address.NumberOfElements - 1) As String
                For i As Integer = 0 To values.Length - 1
                    If Not address.IsBCD And Not m_TreatDataAsHex Then
                        values(i) = Convert.ToString(RawData(i))
                    Else
                        values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(i))
                    End If
                    'tmp(i) = RawData(i + 7)
                Next
                Return values
            Else
                Dim values(address.NumberOfElements - 1) As String

                Dim NumberOfBytes As Integer = Convert.ToInt32(Math.Ceiling(address.BitsPerElement / 8))


                Dim i As Integer
                While i < address.NumberOfElements And (startByte + i) < Math.Floor(RawData.Count / NumberOfBytes)
                    'Dim HexByte1 As String = Chr(RawData(startByte + i * NumberOfBytes)) & Chr(RawData(startByte + (i * NumberOfBytes) + 1))
                    'If NumberOfBytes > 1 Then
                    'Dim HexByte2 As String = Chr(RawData(startByte + (i * NumberOfBytes) + 2)) & Chr(RawData(startByte + (i * NumberOfBytes) + 3))
                    If address.BitsPerElement > 1 Then
                        If Not address.IsBCD And Not m_TreatDataAsHex Then
                            values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes) * 256 + RawData(startByte + i * NumberOfBytes + 1))
                        Else
                            values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes)) & MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes + 1))
                        End If
                    Else
                        '* Bits were read
                        Dim WordWithBit As Integer = CInt(Math.Floor((address.BitNumber + i) / 16))
                        Dim BitWithinTheWord As Integer = address.BitNumber + i - WordWithBit * 16
                        Dim v As Integer = RawData(startByte + WordWithBit * 2) * 256 + RawData(startByte + WordWithBit * 2 + 1)
                        'For BitIndex = i * 16 To values.Length - address.BitNumber - 1
                        If (v And CInt(2 ^ (BitWithinTheWord))) > 0 Then
                            values(i) = "1"
                        Else
                            values(i) = "0"
                        End If
                        'Next
                    End If
                    'Else
                    '    If Not m_TreatDataAsHex Then
                    '        values(i) = Convert.ToString(RawData(startByte + i * NumberOfBytes))
                    '    Else
                    '        values(i) = MfgControl.AdvancedHMI.Drivers.Common.CalculationsAndConversions.ByteToHex(RawData(startByte + i * NumberOfBytes))
                    '    End If
                    'End If


                    i += 1
                End While

                If values.Length > 4 AndAlso values(6) Is Nothing Then
                    Dim dbg = 0
                End If

                Return values
            End If
        End Function

        '**********************************************
        '* Translate from End code to message
        '* Ref Section 4-2
        '***********************************************
        Private Function DecodeEndCode(ByVal endCode As UShort) As String
            Select Case endCode
                Case 0
                    Return "Normal Completion"
                Case 1
                    Return "Not executable in RUN mode"
                Case 2
                    Return "Not executable in MONITOR mode"
                Case 3
                    Return "UM write-protected"
                Case 4
                    Return "Address over"
                Case &HB
                    Return "Not executable in PROGRAM mode"
                Case &H13
                    Return "FCS error"
                Case &H14
                    Return "Format error"
                Case &H15
                    Return "Entry number data error"
                Case &H16
                    Return "Command not supported"
                Case &H18
                    Return "Frame length error"
                Case &H19
                    Return "Not executable"
                Case &H20
                    Return "Could not create I/O table"
                Case &H21
                    Return "Not executable due to CPU Unit CPU error"
                Case &H23
                    Return "User Memory Protected"
                Case &HA3
                    Return "Aborted due to FCS error in transmission data"
                Case &HA4
                    Return "Aborted due to format error in transmission data"
                Case &HA5
                    Return "Aborted due to entry number data error in transmission data"
                Case &HA6
                    Return "Aborted due to frame length error in transmission data"
                Case Else
                    Return "Unknown End Code : " & endCode
            End Select
        End Function
#End Region

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
        'Delegate Sub DataReceive(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        'Private drsd As DataReceive = (AddressOf DataReceivedSync)
        'Private Sub DataReceivedSync(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    RaiseEvent DataReceived(Me, e)
        'End Sub

        'Private errorsd As New EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)(AddressOf ErrorReceivedSync)
        'Private Sub ErrorReceivedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '    RaiseEvent ComError(Me, e)
        'End Sub

#End Region
    End Class
End Namespace
