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
'* 08-JUN-13 Created to make extending easier
'****************************************************************************
Public Class AnimatingPictureBox
    Inherits MfgControl.AdvancedHMI.Controls.AnimatingPictureBox

    'Implements IDisposable

#Region "PLC Related Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property ComComponent() As MfgControl.AdvancedHMI.Drivers.IComComponent
        Get
            Return m_ComComponent
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.IComComponent)
            If m_ComComponent IsNot value Then
                If SubScriptions IsNot Nothing Then
                    SubScriptions.UnsubscribeAll()
                    SubScriptions.ComComponent = value
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

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressImageRotationValue As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressImageRotationValue() As String
        Get
            Return m_PLCAddressImageRotationValue
        End Get
        Set(ByVal value As String)
            If m_PLCAddressImageRotationValue <> value Then
                m_PLCAddressImageRotationValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressImageTranslateXValue As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressImageTranslateXValue() As String
        Get
            Return m_PLCAddressImageTranslateXValue
        End Get
        Set(ByVal value As String)
            If m_PLCAddressImageTranslateXValue <> value Then
                m_PLCAddressImageTranslateXValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressImageTranslateYValue As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressImageTranslateYValue() As String
        Get
            Return m_PLCAddressImageTranslateYValue
        End Get
        Set(ByVal value As String)
            If m_PLCAddressImageTranslateYValue <> value Then
                m_PLCAddressImageTranslateYValue = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressLocationOffsetX As String = ""
    <System.ComponentModel.DefaultValue("")>
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressLocationOffsetX() As String
        Get
            Return m_PLCAddressLocationOffsetX
        End Get
        Set(ByVal value As String)
            If m_PLCAddressLocationOffsetX <> value Then
                m_PLCAddressLocationOffsetX = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressLocationOffsetY As String = ""
    <System.ComponentModel.DefaultValue("")>
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressLocationOffsetY() As String
        Get
            Return m_PLCAddressLocationOffsetY
        End Get
        Set(ByVal value As String)
            If m_PLCAddressLocationOffsetY <> value Then
                m_PLCAddressLocationOffsetY = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Write Data To
    '*****************************************
    Private m_PLCAddressKeypad As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressKeypad() As String
        Get
            Return m_PLCAddressKeypad
        End Get
        Set(ByVal value As String)
            If m_PLCAddressKeypad <> value Then
                m_PLCAddressKeypad = value
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
            Dim i As Integer = 0
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
#End Region

#Region "Constructor/Destructor"
    '****************************************************************
    '* UserControl overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If Not IsDisposed Then
                If disposing Then
                    If SubScriptions IsNot Nothing Then
                        SubScriptions.dispose()
                    End If
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
    Private OriginalText As String
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
    Private ErrorDisplayTime As System.Windows.Forms.Timer
    Private Sub DisplayError(ByVal ErrorMessage As String)
        If Not m_SuppressErrorDisplay Then
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
#End Region

#Region "Keypad popup for data entry"
    Private KeypadPopUp As MfgControl.AdvancedHMI.Controls.Keypad

    'Public Property KPD As MfgControl.AdvancedHMI.Controls.Keypad

    Private m_KeypadText As String
    Public Property KeypadText() As String
        Get
            Return m_KeypadText
        End Get
        Set(ByVal value As String)
            m_KeypadText = value
        End Set
    End Property

    Private m_KeypadFontColor As Color = Color.WhiteSmoke
    Public Property KeypadFontColor() As Color
        Get
            Return m_KeypadFontColor
        End Get
        Set(ByVal value As Color)
            m_KeypadFontColor = value
        End Set
    End Property


    Private m_KeypadWidth As Integer = 300
    Public Property KeypadWidth() As Integer
        Get
            Return m_KeypadWidth
        End Get
        Set(ByVal value As Integer)
            m_KeypadWidth = value
        End Set
    End Property

    Private m_KeypadScaleFactor As Double = 1
    Public Property KeypadScaleFactor() As Double
        Get
            Return m_KeypadScaleFactor
        End Get
        Set(ByVal value As Double)
            m_KeypadScaleFactor = value
        End Set
    End Property

    Private m_KeypadMinValue As Double
    Public Property KeypadMinValue As Double
        Get
            Return m_KeypadMinValue
        End Get
        Set(value As Double)
            m_KeypadMinValue = value
        End Set
    End Property

    Private m_KeypadMaxValue As Double
    Public Property KeypadMaxValue As Double
        Get
            Return m_KeypadMaxValue
        End Get
        Set(value As Double)
            m_KeypadMaxValue = value
        End Set
    End Property


    Private Sub KeypadPopUp_ButtonClick(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Controls.KeyPadEventArgs)
        If e.Key = "Quit" Then
            KeypadPopUp.Visible = False
        ElseIf e.Key = "Enter" Then
            If m_ComComponent Is Nothing Then
                DisplayError("ComComponent Property not set")
            Else
                If KeypadPopUp.Value IsNot Nothing AndAlso (String.Compare(KeypadPopUp.Value, "") <> 0) Then
                    Try
                        If m_KeypadMaxValue <> m_KeypadMinValue Then
                            If KeypadPopUp.Value < m_KeypadMinValue Or KeypadPopUp.Value > m_KeypadMaxValue Then
                                System.Windows.Forms.MessageBox.Show("Value must be >" & m_KeypadMinValue & " and <" & m_KeypadMaxValue)
                                Exit Sub
                            End If
                        End If
                    Catch ex As Exception
                        System.Windows.Forms.MessageBox.Show("Failed to validate value. " & ex.Message)
                        Exit Sub
                    End Try

                    Try
                        If KeypadScaleFactor = 1 Or KeypadScaleFactor = 0 Then
                            m_ComComponent.Write(m_PLCAddressKeypad, KeypadPopUp.Value)
                        Else
                            m_ComComponent.Write(m_PLCAddressKeypad, CDbl(KeypadPopUp.Value) / m_KeypadScaleFactor)
                        End If
                    Catch ex As Exception
                        System.Windows.Forms.MessageBox.Show("Failed to write value. " & ex.Message)
                    End Try
                End If
                KeypadPopUp.Visible = False
            End If
        End If
    End Sub

    '***********************************************************
    '* If labeled is clicked, pop up a keypad for data entry
    '***********************************************************
    Protected Overrides Sub OnClick(e As System.EventArgs)
        MyBase.OnClick(e)

        If m_PLCAddressKeypad IsNot Nothing AndAlso (String.Compare(m_PLCAddressKeypad, "") <> 0) And Enabled Then
            If KeypadPopUp Is Nothing Then
                KeypadPopUp = New MfgControl.AdvancedHMI.Controls.Keypad(m_KeypadWidth)
                AddHandler KeypadPopUp.ButtonClick, AddressOf KeypadPopUp_ButtonClick
            End If

            KeypadPopUp.Text = m_KeypadText
            KeypadPopUp.ForeColor = m_KeypadFontColor
            KeypadPopUp.Value = ""
            KeypadPopUp.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
            KeypadPopUp.TopMost = True
            KeypadPopUp.Show()
        End If
    End Sub
#End Region
End Class
