'****************************************************************************
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 25-SEP-11
'*
'* Copyright 2011 Archie Jacobs
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
'*
'* 22-SEP-11 Created
'****************************************************************************
Public Class MessageDisplayByBit
    Inherits MfgControl.AdvancedHMI.Controls.MessageDisplayByBit


#Region "Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
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


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressVisible As String = ""
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

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private NotificationIDValue As Integer
    Private m_PLCAddressValues As String = ""
    Public Property PLCAddressValues() As String
        Get
            Return m_PLCAddressValues
        End Get
        Set(ByVal value As String)
            If m_PLCAddressValues <> value Then
                m_PLCAddressValues = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private m_PLCNumberOfElements As Integer = 1
    Public Property PLCNumberOfElements As Integer
        Get
            Return m_PLCNumberOfElements
        End Get
        Set(value As Integer)
            m_PLCNumberOfElements = Math.Min(64 / PLCElementBitWidth, value)
            m_PLCNumberOfElements = Math.Max(1, m_PLCNumberOfElements)
        End Set
    End Property


    Private m_PLCElementBitWidth As Integer = 32
    Public Property PLCElementBitWidth As Integer
        Get
            Return m_PLCElementBitWidth
        End Get
        Set(value As Integer)
            value = Math.Min(64, value)
            m_PLCElementBitWidth = CInt(Math.Ceiling(value / 8) * 8)
            If m_PLCElementBitWidth < 8 Then
                m_PLCElementBitWidth = 8
            End If
        End Set
    End Property


    Private m_SuppressErrorDisplay As Boolean
    <System.ComponentModel.DefaultValue(False)> _
    Public Property SuppressErrorDisplay As Boolean
        Get
            Return m_SuppressErrorDisplay
        End Get
        Set(value As Boolean)
            m_SuppressErrorDisplay = value
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
            Dim i = 0
            Dim j As Integer = Me.Parent.Site.Container.Components.Count
            While m_ComComponent Is Nothing And i < j
                If Me.Parent.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then m_ComComponent = CType(Me.Parent.Site.Container.Components(i), MfgControl.AdvancedHMI.Drivers.IComComponent)
                i += 1
            End While

            '************************************************
            '* If no comm component was found, then add one and
            '* point the ComComponent property to it
            '*********************************************
            If m_ComComponent Is Nothing Then
                m_ComComponent = New AdvancedHMIDrivers.EthernetIPforCLXCom(Me.Site.Container)
            End If
        Else
            SubscribeToComDriver()
        End If
    End Sub
#End Region

#Region "Constructor/Destructor"
    '****************************************************************
    '* UserControl overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                m_ComComponent.Unsubscribe(NotificationIDValue)
                If SubScriptions IsNot Nothing Then
                    SubScriptions.dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
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
                AddHandler SubScriptions.DisplayError, AddressOf DisplaySubscribeError
            End If
            SubScriptions.ComComponent = m_ComComponent

            SubScriptions.SubscribeAutoProperties()

            '*V3.99v - added if condition
            If Not String.IsNullOrEmpty(m_PLCAddressValues) Then
                NotificationIDValue = m_ComComponent.Subscribe(m_PLCAddressValues, m_PLCNumberOfElements, 500, AddressOf PolledDataReturned)
            End If
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        '* The value of the base class is a 64 bit number, so combine the individual elements into a 64 bit number
        Dim Result As UInt64
        If e.Values.Count >= m_PLCNumberOfElements And e.PlcAddress = m_PLCAddressValues Then
            Dim ElementValue As UInt64
            For index = 0 To m_PLCNumberOfElements - 1
                If e.Values(index) < 0 Then
                    Select Case m_PLCElementBitWidth
                        Case 8
                        Case 16
                            Dim In16 As Int16 = e.Values(index)
                            Dim In16s As String = Convert.ToString(In16, 16)
                            Dim r As UInt16 = Convert.ToUInt32(In16s, 16)
                            ElementValue = r
                        Case 32
                            Dim In32 As Int32 = e.Values(index)
                            Dim In32s As String = Convert.ToString(In32, 16)
                            Dim r As UInt32 = Convert.ToUInt32(In32s, 16)
                            ElementValue = r
                        Case 64

                    End Select
                Else
                    ElementValue = e.Values(index)
                End If
                Result += ElementValue * 2 ^ (m_PLCElementBitWidth * index)
            Next

            'Dim In64s As String = Convert.ToString(Result, 16)
            'Value = Convert.ToInt64(In64s, 16)


            '* remove highest bit to avoid overflow
            Value = Convert.ToInt64(Result And Convert.ToUInt64(&H7FFFFFFFFFFFFFFF))
        End If
    End Sub

    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
    End Sub
#End Region

    '********************************************************
    '* Show an error via the text property for a short time
    '********************************************************
    Private WithEvents ErrorDisplayTime As System.Windows.Forms.Timer
    Private Sub DisplayError(ByVal ErrorMessage As String)
        If Not m_SuppressErrorDisplay Then
            If ErrorDisplayTime Is Nothing Then
                ErrorDisplayTime = New System.Windows.Forms.Timer
                AddHandler ErrorDisplayTime.Tick, AddressOf ErrorDisplay_Tick
                ErrorDisplayTime.Interval = 5000
            End If

            '* Save the text to return to
            If Not ErrorDisplayTime.Enabled Then
                OriginalText = Me.Text
            End If

            ErrorDisplayTime.Enabled = True

            Me.Text = ErrorMessage
        End If
    End Sub


    '**************************************************************************************
    '* Return the text back to its original after displaying the error for a few seconds.
    '**************************************************************************************
    Private Sub ErrorDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Text = OriginalText

        If ErrorDisplayTime IsNot Nothing Then
            ErrorDisplayTime.Enabled = False
            ErrorDisplayTime.Dispose()
            ErrorDisplayTime = Nothing
        End If
    End Sub


#Region "Logging"
    '* 23-OCT-16 V3.99s
    Private m_LogFile As String
    Public Property LogFile As String
        Get
            Return m_LogFile
        End Get
        Set(value As String)
            m_LogFile = value
        End Set
    End Property

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)

        If Not String.IsNullOrEmpty(m_LogFile) Then
            Using sw As New System.IO.StreamWriter(m_LogFile, True)
                sw.WriteLine(Now & " - " & Me.Text)
            End Using
        End If
    End Sub
#End Region
End Class
