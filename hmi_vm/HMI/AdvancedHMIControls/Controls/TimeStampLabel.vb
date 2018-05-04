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
'****************************************************************************
Public Class TimeStampLabel
    Inherits System.Windows.Forms.Label

    Private FirstRun, LastValue As Boolean
    Private ResultText As String

    Public Event ValueChanged As EventHandler

#Region "Constructor"

    Public Sub New()
        MyBase.DoubleBuffered = True
        Me.DoubleBuffered = True
    End Sub

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

#Region "Basic Properties"

    Private m_Value As Boolean
    <System.ComponentModel.Browsable(False)> _
    <System.ComponentModel.Description("Boolean value (bit from PLC).")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property Value As Boolean
        Get
            Return m_Value
        End Get
        Set(ByVal value As Boolean)
            If Not FirstRun Then
                FirstRun = True
                Exit Property
            End If
            If value AndAlso Not m_Value AndAlso Not LastValue Then
                m_Value = True
                LastValue = False
                Exit Property
            ElseIf Not value AndAlso Not m_Value AndAlso Not LastValue Then
                m_Value = False
                LastValue = True
                Exit Property
            End If
            If m_Value <> value Then
                LastValue = m_Value
                m_Value = value
                UpdateText()
                OnvalueChanged(EventArgs.Empty)
                '* Be sure error handler doesn't revert back to an incorrect text
                OriginalText = MyBase.Text
            End If
        End Set
    End Property

    Private m_timeFormat As String = "T"
    <System.ComponentModel.DefaultValue("T")> _
    <System.ComponentModel.Description("Time/Date string format (standard or custom format strings).")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property TimeDateFormat() As String
        Get
            Return m_timeFormat
        End Get
        Set(ByVal value As String)
            If m_timeFormat <> value Then
                m_timeFormat = value
            End If
        End Set
    End Property

    '**********************************
    '* Prefixes to text
    '**********************************
    Private m_RisingEdgePrefix As String = "RisingEdge: "
    <System.ComponentModel.DefaultValue("RisingEdge: ")> _
    <System.ComponentModel.Description("String to show before the RisingEdge Time/Date value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property RisingEdgePrefix() As String
        Get
            Return m_RisingEdgePrefix
        End Get
        Set(ByVal value As String)
            If m_RisingEdgePrefix <> value Then
                m_RisingEdgePrefix = value
            End If
        End Set
    End Property

    Private m_FallingEdgePrefix As String = "FallingEdge: "
    <System.ComponentModel.DefaultValue("FallingEdge: ")> _
    <System.ComponentModel.Description("String to show before the FallingEdge Time/Date value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property FallingEdgePrefix() As String
        Get
            Return m_FallingEdgePrefix
        End Get
        Set(ByVal value As String)
            If m_FallingEdgePrefix <> value Then
                m_FallingEdgePrefix = value
            End If
        End Set
    End Property

    Private m_showPrefix As Boolean
    <System.ComponentModel.DefaultValue(False)> _
    <System.ComponentModel.Description("Show prefix before Time/Date value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property ShowPrefix() As Boolean
        Get
            Return m_showPrefix
        End Get
        Set(ByVal value As Boolean)
            If m_showPrefix <> value Then
                m_showPrefix = value
            End If
        End Set
    End Property

    Public Enum TriggerTypeEnum
        RisingEdge = 0
        FallingEdge = 1
    End Enum

    Private m_TriggerType As TriggerTypeEnum = TriggerTypeEnum.RisingEdge
    <System.ComponentModel.DefaultValue(TriggerTypeEnum.RisingEdge)> _
    <System.ComponentModel.Description("Trigger type to capture Time/Date value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property TriggerType As TriggerTypeEnum
        Get
            Return m_TriggerType
        End Get
        Set(value As TriggerTypeEnum)
            m_TriggerType = value
        End Set
    End Property

    '***************************************************************
    '* Property - RisingEdge Back Color
    '***************************************************************
    Private m_RisingEdgeColor As Drawing.Color = Drawing.Color.Green
    <System.ComponentModel.DefaultValue(GetType(Color), "Green")> _
    <System.ComponentModel.Description("Back color for the RisingEdge value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property RisingEdgeBackColor() As Drawing.Color
        Get
            Return m_RisingEdgeColor
        End Get
        Set(ByVal value As Drawing.Color)
            If m_RisingEdgeColor <> value Then
                m_RisingEdgeColor = value
            End If
        End Set
    End Property

    '***************************************************************
    '* Property - FallingEdge Back Color
    '***************************************************************
    Private m_FallingEdgeColor As Drawing.Color = Drawing.Color.Red
    <System.ComponentModel.DefaultValue(GetType(Color), "Red")> _
    <System.ComponentModel.Description("Back color for the FallingEdge value.")> _
    <System.ComponentModel.Category("Basic Properties")> _
    Public Property FallingEdgeBackColor() As Drawing.Color
        Get
            Return m_FallingEdgeColor
        End Get
        Set(ByVal value As Drawing.Color)
            If m_FallingEdgeColor <> value Then
                m_FallingEdgeColor = value
            End If
        End Set
    End Property

#End Region

#Region "PLC Related Properties"

    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Description("Driver Instance for data reading and writing")> _
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

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValue As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressValue() As String
        Get
            Return m_PLCAddressValue
        End Get
        Set(ByVal value As String)
            If m_PLCAddressValue <> value Then
                m_PLCAddressValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressVisible As String = ""
    <System.ComponentModel.DefaultValue("")> _
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

#End Region

#Region "Private Methods"

    Private Sub UpdateText()
        Try
            If m_TriggerType = TriggerTypeEnum.RisingEdge Then
                If m_Value = "True" AndAlso LastValue = "False" Then
                    ResultText = Now.ToString(m_timeFormat)
                    '* Apply the Prefix
                    If m_showPrefix Then
                        If Not String.IsNullOrEmpty(m_RisingEdgePrefix) Then
                            ResultText = m_RisingEdgePrefix & ResultText
                        End If
                    End If
                    MyBase.BackColor = RisingEdgeBackColor
                    MyBase.Text = ResultText
                End If
            Else
                If m_Value = "False" AndAlso LastValue = "True" Then
                    ResultText = Now.ToString(m_timeFormat)
                    '* Apply the Prefix
                    If m_showPrefix Then
                        If Not String.IsNullOrEmpty(m_FallingEdgePrefix) Then
                            ResultText = m_FallingEdgePrefix & ResultText
                        End If
                    End If
                    MyBase.BackColor = FallingEdgeBackColor
                    MyBase.Text = ResultText
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

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
            '* Search for AdvancedHMIDrivers.IComComponent component
            '*   in the Designer Host Container
            '* If one exists, set the client of this component to it
            '********************************************************
            Dim i As Integer
            While m_ComComponent Is Nothing And i < Me.Site.Container.Components.Count
                If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then m_ComComponent = Me.Site.Container.Components(i)
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

    Protected Overrides Sub OnHandleCreated(e As System.EventArgs)
        MyBase.OnHandleCreated(e)
    End Sub

    Protected Overridable Sub OnvalueChanged(ByVal e As EventArgs)
        RaiseEvent ValueChanged(Me, e)
    End Sub

#End Region

#Region "Subscribing and PLC data receiving"
    Private SubScriptions As SubscriptionHandler
    '*******************************************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '* This code will look at properties to find the "PLCAddress" + property name
    '*
    '*******************************************************************************
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
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As SubscriptionHandlerEventArgs)
    End Sub

#End Region

#Region "Error Display"
    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
    End Sub

    '********************************************************
    '* Show an error via the text property for a short time
    '********************************************************
    Private ErrorDisplayTime As System.Windows.Forms.Timer
    Private ErrorLock As New Object
    Private Sub DisplayError(ByVal ErrorMessage As String)
        If Not m_SuppressErrorDisplay Then
            '* Create the error display timer
            If ErrorDisplayTime Is Nothing Then
                ErrorDisplayTime = New System.Windows.Forms.Timer
                AddHandler ErrorDisplayTime.Tick, AddressOf ErrorDisplay_Tick
                ErrorDisplayTime.Interval = 5000
            End If

            '* Save the text to return to
            SyncLock (ErrorLock)
                If Not ErrorDisplayTime.Enabled Then
                    ErrorDisplayTime.Enabled = True
                    OriginalText = MyBase.Text
                    MyBase.Text = ErrorMessage
                End If
            End SyncLock
        End If
    End Sub


    '**************************************************************************************
    '* Return the text back to its original after displaying the error for a few seconds.
    '**************************************************************************************
    Private Sub ErrorDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'UpdateText()
        SyncLock (ErrorLock)
            MyBase.Text = OriginalText
            'If ErrorDisplayTime IsNot Nothing Then
            ErrorDisplayTime.Enabled = False
            ' ErrorIsDisplayed = False
        End SyncLock
        'RemoveHandler ErrorDisplayTime.Tick, AddressOf ErrorDisplay_Tick
        'ErrorDisplayTime.Dispose()
        'ErrorDisplayTime = Nothing
        'End If
    End Sub
#End Region

End Class
