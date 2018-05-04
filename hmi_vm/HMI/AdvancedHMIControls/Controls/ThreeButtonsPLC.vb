'****************************************************************************
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 12-JUN-11
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
'* 04-OCT-11 Created
'****************************************************************************
Public Class ThreeButtonPLC
    Inherits ThreeButtons


#Region "PLC Related Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property ComComponent()  As MfgControl.AdvancedHMI.Drivers.IComComponent
        Get
            Return m_ComComponent
        End Get
        Set(ByVal value As  MfgControl.AdvancedHMI.Drivers.IComComponent)
            m_ComComponent = value
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private NotificationIDHandStatus As Integer
    Private m_PLCAddressStatusHand As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressStatusHand() As String
        Get
            Return m_PLCAddressStatusHand
        End Get
        Set(ByVal value As String)
            If m_PLCAddressStatusHand <> value Then
                m_PLCAddressStatusHand = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property


    Private NotificationIDAutoStatus As Integer
    Private m_PLCAddressStatusAuto As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressStatusAuto() As String
        Get
            Return m_PLCAddressStatusAuto
        End Get
        Set(ByVal value As String)
            If m_PLCAddressStatusAuto <> value Then
                m_PLCAddressStatusAuto = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private NotificationIDOffStatus As Integer
    Private m_PLCAddressStatusOff As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressStatusOff() As String
        Get
            Return m_PLCAddressStatusOff
        End Get
        Set(ByVal value As String)
            If m_PLCAddressStatusOff <> value Then
                m_PLCAddressStatusOff = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private NotificationIDVisibility As Integer
    Private InvertVisible As Boolean
    Private m_PLCAddressVisible As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
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



    '********************************************
    '* Property - Address in PLC for click event
    '********************************************
    Private m_PLCAddressClick1 As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressClickAuto() As String
        Get
            Return m_PLCAddressClick1
        End Get
        Set(ByVal value As String)
            m_PLCAddressClick1 = value
        End Set
    End Property

    Private m_PLCAddressClick2 As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressClickHand() As String
        Get
            Return m_PLCAddressClick2
        End Get
        Set(ByVal value As String)
            m_PLCAddressClick2 = value
        End Set
    End Property

    Private m_PLCAddressClick3 As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressClickOff() As String
        Get
            Return m_PLCAddressClick3
        End Get
        Set(ByVal value As String)
            m_PLCAddressClick3 = value
        End Set
    End Property

    '*****************************************
    '* Property - What to do to bit in PLC
    '*****************************************
    Private m_OutputType As MfgControl.AdvancedHMI.Controls.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property OutputType() As MfgControl.AdvancedHMI.Controls.OutputType
        Get
            Return m_OutputType
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Controls.OutputType)
            m_OutputType = value
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

    '****************************
    '* Event - Button Click
    '****************************
    Private Sub _Click1(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles AutoButton.MouseDown
        MouseDownActon(m_PLCAddressClick1)
    End Sub

    Private Sub _MouseUp1(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles AutoButton.MouseUp
        MouseUpAction(m_PLCAddressClick1)
    End Sub

    Private Sub _MouseDown2(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles HandButton.MouseDown
        MouseDownActon(m_PLCAddressClick2)
    End Sub

    Private Sub _MouseUp2(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles HandButton.MouseUp
        MouseUpAction(m_PLCAddressClick2)
    End Sub

    Private Sub _click3(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles OffButton.MouseDown
        MouseUpAction(m_PLCAddressClick3)
    End Sub

    Private Sub _MouseUp3(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles OffButton.MouseUp
        MouseDownActon(m_PLCAddressClick3)
    End Sub

    Private Sub MouseDownActon(ByVal PLCAddress As String)
        If PLCAddress IsNot Nothing AndAlso (String.Compare(PLCAddress, "") <> 0) Then
            Try
                Select Case m_OutputType
                    Case MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet : m_ComComponent.Write(PLCAddress, 1)
                    Case MfgControl.AdvancedHMI.Controls.OutputType.MomentaryReset : m_ComComponent.Write(PLCAddress, 0)
                    Case MfgControl.AdvancedHMI.Controls.OutputType.SetTrue : m_ComComponent.Write(PLCAddress, 1)
                    Case MfgControl.AdvancedHMI.Controls.OutputType.SetFalse : m_ComComponent.Write(PLCAddress, 0)
                    Case MfgControl.AdvancedHMI.Controls.OutputType.Toggle
                        Dim CurrentValue As Boolean
                        CurrentValue = m_ComComponent.Read(PLCAddress, 1)(0)
                        If CurrentValue Then
                            m_ComComponent.Write(PLCAddress, 0)
                        Else
                            m_ComComponent.Write(PLCAddress, 1)
                        End If
                End Select
            Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
                If ex.ErrorCode = 1808 Then
                    DisplayError("""" & PLCAddress & """ PLC Address not found")
                Else
                    DisplayError(ex.Message)
                End If
            End Try
        End If
    End Sub


    Private Sub MouseUpAction(ByVal PLCAddress As String)
        If PLCAddress IsNot Nothing AndAlso (String.Compare(PLCAddress, "") <> 0) And Enabled Then
            Try
                Select Case OutputType
                    Case MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet : m_ComComponent.Write(PLCAddress, 0)
                    Case MfgControl.AdvancedHMI.Controls.OutputType.MomentaryReset : m_ComComponent.Write(PLCAddress, 1)
                End Select
            Catch ex As MfgControl.AdvancedHMI.Drivers.common.PLCDriverException
                If ex.ErrorCode = 1808 Then
                    DisplayError("""" & PLCAddress & """ PLC Address not found")
                Else
                    DisplayError(ex.Message)
                End If
            End Try
        End If
    End Sub




    '****************************************************************
    '* UserControl overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing And m_ComComponent IsNot Nothing Then
                m_ComComponent.UnSubscribe(NotificationIDAutoStatus)
                m_ComComponent.UnSubscribe(NotificationIDHandStatus)
                m_ComComponent.UnSubscribe(NotificationIDOffStatus)
                m_ComComponent.UnSubscribe(NotificationIDVisibility)
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
#End Region

#Region "Subscribing and PLC data receiving"
    '**************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '**************************************************
    Private SubscribedPLCAddressAutoStatus As String
    Private SubscribedPLCAddressHandStatus As String
    Private SubscribedPLCAddressOffStatus As String
    Private SubscribedPLCAddressVisible As String
    Private Sub SubscribeToComDriver()
        If Not DesignMode And IsHandleCreated Then
            '*******************************
            '* Subscription for Auto Status
            '*******************************
            SubscribeTo(m_PLCAddressStatusAuto, SubscribedPLCAddressAutoStatus, NotificationIDAutoStatus, AddressOf PolledDataReturnedAutoStatus)

            '*******************************
            '* Subscription for Hand Status
            '*******************************
            SubscribeTo(m_PLCAddressStatusHand, SubscribedPLCAddressHandStatus, NotificationIDHandStatus, AddressOf PolledDataReturnedHandStatus)

            '*******************************
            '* Subscription for Off Status
            '*******************************
            SubscribeTo(m_PLCAddressStatusOff, SubscribedPLCAddressOffStatus, NotificationIDOffStatus, AddressOf PolledDataReturnedOffStatus)

            '*************************
            '* Visbility Subscription
            '*************************
            If m_PLCAddressVisible IsNot Nothing AndAlso (String.Compare(m_PLCAddressVisible, "") <> 0) Then
                Dim PLCAddress As String = m_PLCAddressVisible
                If PLCAddress.ToUpper.IndexOf("NOT ") = 0 Then
                    PLCAddress = m_PLCAddressVisible.Substring(4).Trim
                    InvertVisible = True
                Else
                    InvertVisible = False
                End If
                SubscribeTo(PLCAddress, SubscribedPLCAddressVisible, NotificationIDVisibility, AddressOf PolledDataReturnedVisible)
            End If
        End If
    End Sub

    '******************************************************
    '* Attempt to create a subscription to the PLC driver
    '******************************************************
    Private Sub SubscribeTo(ByVal PLCAddress As String, ByRef SubscribedPLCAddress As String, ByRef NotificationID As Integer, ByVal callBack As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs))
        If SubscribedPLCAddress <> PLCAddress Then
            '* If already subscribed, but address is changed, then unsubscribe first
            If NotificationID > 0 Then
                m_ComComponent.UnSubscribe(NotificationID)
            End If
            '* Is there an address to subscribe to?
            If PLCAddress IsNot Nothing AndAlso (String.Compare(PLCAddress, "") <> 0) Then
                Try
                    If m_ComComponent IsNot Nothing Then
                        NotificationID = m_ComComponent.Subscribe(PLCAddress, 1, 250, callBack)

                        '* If subscription succeedded, save the address
                        SubscribedPLCAddress = PLCAddress
                    Else
                        DisplayError("ComComponent Property not set")
                    End If
                Catch ex As MfgControl.AdvancedHMI.Drivers.common.PLCDriverException
                    '* If subscribe fails, set up for retry
                    InitializeSubscribeRetry(ex, PLCAddress)
                End Try
            End If
        End If

    End Sub

    '********************************************
    '* Show the error and start the retry time
    '********************************************
    Private Sub InitializeSubscribeRetry(ByVal ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException, ByVal PLCAddress As String)
        If ex.ErrorCode = 1808 Then
            DisplayError("""" & PLCAddress & """ PLC Address not found")
        Else
            DisplayError(ex.Message)
        End If

        If SubscribeRetryTimer Is Nothing Then
            SubscribeRetryTimer = New Windows.Forms.Timer
            SubscribeRetryTimer.Interval = 10000
            AddHandler SubscribeRetryTimer.Tick, AddressOf Retry_Tick
        End If

        SubscribeRetryTimer.Enabled = True
    End Sub


    '********************************************
    '* Keep retrying to subscribe if it failed
    '********************************************
    Private SubscribeRetryTimer As Windows.Forms.Timer
    Private Sub Retry_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SubscribeRetryTimer.Enabled = False
        SubscribeRetryTimer.Dispose()
        SubscribeRetryTimer = Nothing

        SubscribeToComDriver()
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturnedAutoStatus(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            If e.Values(0) Then
                AutoButton.BackColor = Color.Green
            Else
                AutoButton.BackColor = Color.LightGray
            End If
        Catch
            DisplayError("INVALID Auto Status RETURNED!")
        End Try
    End Sub

    Private Sub PolledDataReturnedHandStatus(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            If e.Values(0) Then
                HandButton.BackColor = Color.Green
            Else
                HandButton.BackColor = Color.LightGray
            End If
        Catch
            DisplayError("INVALID Hand Status RETURNED!")
        End Try
    End Sub

    Private Sub PolledDataReturnedOffStatus(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            If e.Values(0) Then
                OffButton.BackColor = Color.Green
            Else
                OffButton.BackColor = Color.LightGray
            End If
        Catch
            DisplayError("INVALID Off Status RETURNED!")
        End Try
    End Sub

    Private Sub PolledDataReturnedVisible(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Try
            If InvertVisible Then
                MyBase.Visible = Not CBool(e.Values(0))
            Else
                MyBase.Visible = e.Values(0)
            End If
        Catch
            DisplayError("INVALID Visibilty VALUE RETURNED!")
        End Try
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
            ErrorDisplayTime.Interval = 6000
        End If

        '* Save the text to return to
        If Not ErrorDisplayTime.Enabled Then
            OriginalText = Me.Text
        End If

        ErrorDisplayTime.Enabled = True

        Text = ErrorMessage
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
#End Region

End Class
