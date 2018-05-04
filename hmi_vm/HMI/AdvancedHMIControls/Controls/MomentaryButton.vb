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
'* 12-JUN-11 Created
'****************************************************************************
Public Class MomentaryButton
    'Inherits MfgControl.AdvancedHMI.Controls.PushButton
    Inherits MfgControl.AdvancedHMI.Controls.MomemtaryButton

#Region "PLC Related Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Category("PLC Properties")>
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
    Private m_PLCAddressClick As String = ""
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressClick() As String
        Get
            Return m_PLCAddressClick
        End Get
        Set(ByVal value As String)
            m_PLCAddressClick = value
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressVisible As String = ""
    <System.ComponentModel.Category("PLC Properties")>
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

    Private m_SuppressErrorDisplay As Boolean
    <System.ComponentModel.DefaultValue(False)>
    Public Property SuppressErrorDisplay As Boolean
        Get
            Return m_SuppressErrorDisplay
        End Get
        Set(value As Boolean)
            m_SuppressErrorDisplay = value
        End Set
    End Property

    '*****************************************
    '* Property - Hold time before bit reset
    '*****************************************
    Private WithEvents MinHoldTimer As New System.Windows.Forms.Timer
    Private m_MinimumHoldTime As Integer = 500
    <System.ComponentModel.Category("PLC Properties")>
    Public Property MinimumHoldTime() As Integer
        Get
            Return m_MinimumHoldTime
        End Get
        Set(ByVal value As Integer)
            m_MinimumHoldTime = value
            If value > 0 Then MinHoldTimer.Interval = value
        End Set
    End Property

    '*****************************************
    '* Property - Hold time before bit reset
    '*****************************************
    Private WithEvents MaxHoldTimer As New System.Windows.Forms.Timer
    Private m_MaximumHoldTime As Integer = 3000
    <System.ComponentModel.Category("PLC Properties")>
    Public Property MaximumHoldTime() As Integer
        Get
            Return m_MaximumHoldTime
        End Get
        Set(ByVal value As Integer)
            m_MaximumHoldTime = value
            If value > 0 Then MaxHoldTimer.Interval = value
        End Set
    End Property
#End Region

#Region "Events"
    '****************************
    '* Event - Mouse Down
    '****************************
    Private Sub MomentaryButton_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        MouseIsDown = True
        HoldTimeMet = False

        If (m_PLCAddressClick IsNot Nothing AndAlso (String.Compare(m_PLCAddressClick, "") <> 0)) And Enabled AndAlso m_ComComponent IsNot Nothing Then
            Try
                Select Case MyBase.OutputType
                    Case OutputTypes.MomentarySet
                        m_ComComponent.Write(m_PLCAddressClick, 1)
                        If m_MinimumHoldTime > 0 Then MinHoldTimer.Enabled = True
                        If m_MaximumHoldTime > 0 Then MaxHoldTimer.Enabled = True
                    Case OutputTypes.MomentaryReset
                        m_ComComponent.Write(m_PLCAddressClick, 0)
                        If m_MinimumHoldTime > 0 Then MinHoldTimer.Enabled = True
                        If m_MaximumHoldTime > 0 Then MaxHoldTimer.Enabled = True
                    Case OutputTypes.SetTrue : m_ComComponent.Write(m_PLCAddressClick, 1)
                    Case OutputTypes.SetFalse : m_ComComponent.Write(m_PLCAddressClick, 0)
                    Case OutputTypes.Toggle
                        Dim CurrentValue As Boolean
                        CurrentValue = m_ComComponent.Read(m_PLCAddressClick, 1)(0)
                        If CurrentValue Then
                            m_ComComponent.Write(m_PLCAddressClick, 0)
                        Else
                            m_ComComponent.Write(m_PLCAddressClick, 1)
                        End If
                End Select
            Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
                If ex.ErrorCode = 1808 Then
                    DisplayError("""" & m_PLCAddressClick & """ PLC Address not found")
                Else
                    DisplayError(ex.Message)
                End If
            End Try
        End If
    End Sub


    ''****************************
    ''* Event - Mouse Up
    ''****************************
    'Private Sub MomentaryButton_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
    '    If (m_PLCAddressClick IsNot Nothing AndAlso (String.Compare(m_PLCAddressClick, "") <> 0)) And Enabled AndAlso m_ComComponent IsNot Nothing Then
    '        Try
    '            Select Case MyBase.OutputType
    '                Case OutputTypes.MomentarySet : m_ComComponent.Write(m_PLCAddressClick, 0)
    '                Case OutputTypes.MomentaryReset : m_ComComponent.Write(m_PLCAddressClick, 1)
    '            End Select
    '        Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
    '            If ex.ErrorCode = 1808 Then
    '                DisplayError("""" & m_PLCAddressClick & """ PLC Address not found")
    '            Else
    '                DisplayError(ex.Message)
    '            End If
    '        End Try
    '    End If
    'End Sub

    '****************************
    '* Event - Mouse Up
    '****************************
    Private Sub MomentaryButton_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        MouseIsDown = False
        If m_PLCAddressClick IsNot Nothing AndAlso (String.Compare(m_PLCAddressClick, "") <> 0) Then
            If HoldTimeMet Or m_MinimumHoldTime <= 0 Then
                MaxHoldTimer.Enabled = False
                ReleaseValue()
            End If
        End If
    End Sub


    Private Sub ReleaseValue()
        Try
            Select Case MyBase.OutputType
                Case MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet : m_ComComponent.Write(m_PLCAddressClick, 0)
                Case MfgControl.AdvancedHMI.Controls.OutputType.MomentaryReset : m_ComComponent.Write(m_PLCAddressClick, 1)
            End Select
        Catch ex As MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException
            If ex.ErrorCode = 1808 Then
                DisplayError("""" & m_PLCAddressClick & """ PLC Address not found")
            Else
                DisplayError(ex.Message)
            End If
        End Try
    End Sub

    Private MouseIsDown, HoldTimeMet As Boolean
    Private Sub HoldTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles MinHoldTimer.Tick
        MinHoldTimer.Enabled = False
        HoldTimeMet = True
        If Not MouseIsDown Then
            ReleaseValue()
        End If
    End Sub

    Private Sub MaxHoldTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles MaxHoldTimer.Tick
        MaxHoldTimer.Enabled = False
        ReleaseValue()
    End Sub


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
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As SubscriptionHandlerEventArgs)
    End Sub

    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
    End Sub
#End Region

#Region "Error Display"
    '********************************************************
    '* Show an error via the text property for a short time
    '********************************************************
    Private OriginalText As String
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
    Private Sub ErrorDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ErrorDisplayTime.Tick
        Text = OriginalText

        If ErrorDisplayTime IsNot Nothing Then
            ErrorDisplayTime.Enabled = False
            ErrorDisplayTime.Dispose()
            ErrorDisplayTime = Nothing
        End If
    End Sub
#End Region
End Class
