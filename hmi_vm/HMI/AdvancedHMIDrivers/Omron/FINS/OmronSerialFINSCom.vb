Option Strict On
'***********************************************************************
'* Omron FINS over Host Link (RS232) Com
'*
'* Copyright 2011 Archie Jacobs
'*
'* Reference : Omron W342-E1-15 (W342-E1-15+CS-CJ-CP-NSJ+RefManual.pdf)
'* Revision February 2010
'*
'* 29-NOV-11 Ensure a DLL instance exists before sending data
'* 14-DEC-11 Remove the DLL from collection when disposed
'***********************************************************************
'Imports MfgControl.AdvancedHMI.Drivers.Common
Namespace Omron
    Public Class OmronSerialFINSCom
        Inherits Omron.FINSBaseCom
        Implements MfgControl.AdvancedHMI.Drivers.IComComponent

        Private Shared DLL As List(Of MfgControl.AdvancedHMI.Drivers.Omron.HostLinkDataLinkLayer)
        Protected Shared InstanceCount As Integer


#Region "Properties"
        Private m_PortName As String = "COM1"
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property PortName() As String
            Get
                Return m_PortName
            End Get
            Set(ByVal value As String)
                m_PortName = value

                CreateDLLInstance()

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).PortName = value
                End If
            End Set
        End Property

        Private m_BaudRate As Integer = 115200
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property BaudRate() As Integer
            Get
                Return m_BaudRate
            End Get
            Set(ByVal value As Integer)
                m_BaudRate = value

                CreateDLLInstance()

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).BaudRate = value
                End If
            End Set
        End Property

        Private m_Parity As System.IO.Ports.Parity = IO.Ports.Parity.Even
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property Parity() As System.IO.Ports.Parity
            Get
                Return m_Parity
            End Get
            Set(ByVal value As System.IO.Ports.Parity)
                m_Parity = value
                CreateDLLInstance()

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).Parity = value
                End If
            End Set
        End Property

        Private m_DataBits As Integer = 7
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property DataBits() As Integer
            Get
                Return m_DataBits
            End Get
            Set(ByVal value As Integer)
                m_DataBits = value
                CreateDLLInstance()

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).DataBits = value
                End If
            End Set
        End Property

        Private m_StopBits As IO.Ports.StopBits = IO.Ports.StopBits.Two
        <System.ComponentModel.Category("Communication Settings")> _
        Public Property StopBits() As IO.Ports.StopBits
            Get
                Return m_StopBits
            End Get
            Set(ByVal value As IO.Ports.StopBits)
                m_StopBits = value

                CreateDLLInstance()

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).StopBits = value
                End If
            End Set
        End Property


        <System.ComponentModel.Category("Communication Settings")> _
        Public Property TargetUnitAddress() As Byte
            Get
                Return TargetAddress.UnitAddress
            End Get
            Set(ByVal value As Byte)
                TargetAddress.UnitAddress = value

                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    DLL(MyDLLInstance).UnitNumber = value
                End If
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub New()
            MyBase.new()

            If DLL Is Nothing Then
                DLL = New List(Of MfgControl.AdvancedHMI.Drivers.Omron.HostLinkDataLinkLayer)
            End If

            TargetAddress = New MfgControl.AdvancedHMI.Drivers.Omron.DeviceAddress
            '* default port 1 (&HFC)
            SourceAddress = New MfgControl.AdvancedHMI.Drivers.Omron.DeviceAddress(0, 0, &HFC)

            InstanceCount += 1
        End Sub

        Public Sub New(ByVal container As System.ComponentModel.IContainer)
            MyClass.New()

            'Required for Windows.Forms Class Composition Designer support
            container.Add(Me)
        End Sub


        Private IsDisposed As Boolean '* Without this, it can dispose the DLL completely
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not IsDisposed Then
                MyBase.Dispose(disposing)

                '* The handle linked to the DataLink Layer has to be removed, otherwise it causes a problem when a form is closed
                If DLL.Count > 0 AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                    RemoveHandler DLL(MyDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
                    RemoveHandler DLL(MyDLLInstance).ComError, AddressOf DataLinkLayerComError

                    InstanceCount -= 1

                    '* 14-DEC-11 - Added the Remove from collection to fix problem where new DLL was not created
                    '* if it the port were previously closed
                    If InstanceCount <= 0 Then
                        DLL(MyDLLInstance).Dispose(True)
                        DLL(MyDLLInstance) = Nothing
                        'DLL.Remove(DLL(MyDLLInstance))
                    End If
                End If
            End If

            IsDisposed = True
        End Sub

        Private Sub FINSHostLinkCom_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            Dim dbg = 0
        End Sub
#End Region


#Region "Private Methods"
        '***************************************************************
        '* Create the Data Link Layer Instances
        '* if the IP Address is the same, then resuse a common instance
        '***************************************************************
        Protected Overrides Sub CreateDLLInstance()
            'If Me.DesignMode Then Exit Sub
            '*** For Windows CE port, this checks designmode and works in full .NET also***
            If AppDomain.CurrentDomain.FriendlyName.IndexOf("DefaultDomain", System.StringComparison.CurrentCultureIgnoreCase) >= 0 Then
                Exit Sub
            End If

            If DLL.Count > 0 Then
                '* At least one DLL instance already exists,
                '* so check to see if it has the same IP address
                '* if so, reuse the instance, otherwise create a new one
                Dim i As Integer
                While i < DLL.Count AndAlso ((DLL(i) Is Nothing) Or (DLL(i) IsNot Nothing AndAlso DLL(i).PortName <> m_PortName))
                    i += 1
                End While
                MyDLLInstance = i
            End If

            If MyDLLInstance >= DLL.Count Then
                '* See if there are any unused items in collection
                Dim i As Integer
                While i < DLL.Count AndAlso DLL(i) IsNot Nothing
                    i += 1
                End While
                MyDLLInstance = i

                Dim NewDLL As New MfgControl.AdvancedHMI.Drivers.Omron.HostLinkDataLinkLayer(m_PortName)
                NewDLL.BaudRate = m_BaudRate
                NewDLL.DataBits = m_DataBits
                NewDLL.Parity = m_Parity
                NewDLL.StopBits = m_StopBits
                If i >= DLL.Count Then
                    DLL.Add(NewDLL)
                Else
                    DLL(i) = NewDLL
                End If
            End If

            '* Have we already attached event handler to this data link layer?
            If EventHandlerDLLInstance <> (MyDLLInstance + 1) Then
                '* If event handler to another layer has been created, remove them
                If EventHandlerDLLInstance > 0 Then
                    RemoveHandler DLL(EventHandlerDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
                    RemoveHandler DLL(EventHandlerDLLInstance).ComError, AddressOf DataLinkLayerComError
                End If

                AddHandler DLL(MyDLLInstance).DataReceived, AddressOf DataLinkLayerDataReceived
                AddHandler DLL(MyDLLInstance).ComError, AddressOf DataLinkLayerComError
                EventHandlerDLLInstance = MyDLLInstance + 1
            End If
        End Sub


        Friend Overrides Function SendData(ByVal FinsF As MfgControl.AdvancedHMI.Drivers.Omron.FINSFrame) As Boolean
            If IsDisposed Then
                Throw New Exception("FINSHostLinkCom. Object is disposed")
            End If
            '* If a Subscription (Internal Request) begin to overflow the que, ignore some
            '* This can occur from too fast polling
            If DLL.Count <= 0 Then
                CreateDLLInstance()
            End If

            '****************************************************
            '* Do not send an internal request (subscription),
            '*  if the send que has 10 or more requests pending
            '****************************************************
            If (DLL(MyDLLInstance).SendQueDepth < 10) Then
                '* if reuqested by user code, do not let buffer exceed 30 deep
                If (DLL(MyDLLInstance).SendQueDepth < 30) Then
                    Try
                        DLL(MyDLLInstance).SendFinsFrame(FinsF)
                    Catch ex As Exception
                        '* 15-MAR-12
                        Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("1,FINSHostLink,SendData-" & ex.Message)
                    End Try
                    Return True
                Else
                    '* Buffer is full from client requests
                    Return False
                End If
            Else
                Return False
            End If
        End Function
#End Region

        Protected Overrides Function GetNextTransactionID(ByVal maxValue As Integer) As Integer
            If DLL.Count > MyDLLInstance AndAlso DLL(MyDLLInstance) IsNot Nothing Then
                Return DLL(MyDLLInstance).GetNextTransactionNumber(maxValue)
            Else
                Return 0
            End If
        End Function

    End Class
End Namespace
