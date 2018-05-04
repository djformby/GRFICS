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
'* 31-DEC-11 Added BooleanDisplay property
'* 28-SEP-12 Catch specific PLCDriverException when trying to subscribe
'* 29-JAN-13 Added KeypadMinValue and KeypadMaxValue
'* 10-JUL-13 Added Value property
'****************************************************************************
Public Class BasicLabel
    Inherits System.Windows.Forms.Label

    Public Event ValueChanged As EventHandler

#Region "Constructor"
    Public Sub New()
        MyBase.new()

        Value = "BasicLabel"

        If (MyBase.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText) Or ForeColor = Color.FromArgb(0, 0, 0)) Then
            ForeColor = System.Drawing.Color.WhiteSmoke
        End If
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
    'Private SavedBackColor As System.Drawing.Color

    '* Remove Text from the property window so users do not attempt to use it
    <System.ComponentModel.Browsable(False)> _
    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
        End Set
    End Property

    '******************************************************************************************
    '* Use the base control's text property and make it visible as a property on the designer
    '******************************************************************************************
    Private m_Value As String
    Public Property Value As String
        Get
            Return m_Value
        End Get
        Set(ByVal value As String)
            If value <> m_Value Then
                If value IsNot Nothing Then
                    m_Value = value
                    UpdateText()
                    OnvalueChanged(EventArgs.Empty)
                Else
                    '* version 3.99f
                    If m_Value <> "" Then OnvalueChanged(EventArgs.Empty)
                    m_Value = ""
                    MyBase.Text = ""
                End If
                '* Be sure error handler doesn't revert back to an incorrect text
                OriginalText = MyBase.Text
            End If
        End Set
    End Property

    Private m_ValueLeftPadCharacter As Char = " "c
    Public Property ValueLeftPadCharacter() As Char
        Get
            Return m_ValueLeftPadCharacter
        End Get
        Set(ByVal value As Char)
            m_ValueLeftPadCharacter = value
            UpdateText()
        End Set
    End Property

    Private m_ValueLeftPadLength As Integer
    Public Property ValueLeftPadLength As Integer
        Get
            Return m_ValueLeftPadLength
        End Get
        Set(ByVal value As Integer)
            m_ValueLeftPadLength = value
            UpdateText()
        End Set
    End Property


    '**********************************
    '* Prefix and suffixes to text
    '**********************************
    Private m_Prefix As String
    Public Property ValuePrefix() As String
        Get
            Return m_Prefix
        End Get
        Set(ByVal value As String)
            If m_Prefix <> value Then
                m_Prefix = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_Suffix As String
    Public Property ValueSuffix() As String
        Get
            Return m_Suffix
        End Get
        Set(ByVal value As String)
            If m_Suffix <> value Then
                m_Suffix = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_ValueToSubtractFrom As Single
    Public Property ValueToSubtractFrom As Single
        Get
            Return m_ValueToSubtractFrom
        End Get
        Set(value As Single)
            m_ValueToSubtractFrom = value
        End Set
    End Property

    Private m_InterpretValueAsBCD As Boolean
    Public Property InterpretValueAsBCD As Boolean
        Get
            Return m_InterpretValueAsBCD
        End Get
        Set(value As Boolean)
            m_InterpretValueAsBCD = value
        End Set
    End Property

    Private m_BackColor As Color = Color.Black
    Public Shadows Property BackColor As Color
        Get
            Return m_BackColor
        End Get
        Set(value As Color)
            If m_BackColor <> value Then
                m_BackColor = value
                UpdateText()
            End If
        End Set
    End Property

    '***************************************************************
    '* Property - Highlight Color
    '***************************************************************
    Private m_Highlightcolor As Drawing.Color = Drawing.Color.Red
    <System.ComponentModel.Category("Appearance")> _
    Public Property HighlightColor() As Drawing.Color
        Get
            Return m_Highlightcolor
        End Get
        Set(ByVal value As Drawing.Color)
            If m_Highlightcolor <> value Then
                m_Highlightcolor = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_HighlightForecolor As Drawing.Color = Drawing.Color.White
    <System.ComponentModel.Category("Appearance")> _
    Public Property HighlightForeColor() As Drawing.Color
        Get
            Return m_HighlightForecolor
        End Get
        Set(ByVal value As Drawing.Color)
            If m_HighlightForecolor <> value Then
                m_HighlightForecolor = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_ForeColor As Color = Color.White
    Public Shadows Property ForeColor As Color
        Get
            Return m_ForeColor
        End Get
        Set(value As Color)
            If m_ForeColor <> value Then
                m_ForeColor = value
                UpdateText()
            End If
        End Set
    End Property


    Private _HighlightKeyChar As String = "!"
    <System.ComponentModel.Category("Appearance")> _
    Public Property HighlightKeyCharacter() As String
        Get
            Return _HighlightKeyChar
        End Get
        Set(ByVal value As String)
            If _HighlightKeyChar <> value Then
                _HighlightKeyChar = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_Highlight As Boolean
    <System.ComponentModel.Category("Appearance"), System.ComponentModel.Description("Switches to Highlight colors")> _
    Public Property Highlight As Boolean
        Get
            Return m_Highlight
        End Get
        Set(value As Boolean)
            If m_Highlight <> value Then
                m_Highlight = value
                UpdateText()
            End If
        End Set
    End Property


    Private m_NumericFormat As String
    Public Property NumericFormat() As String
        Get
            Return m_NumericFormat
        End Get
        Set(ByVal value As String)
            If m_NumericFormat <> value Then
                m_NumericFormat = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_ValueScaleFactor As Double = 1
    Public Property ValueScaleFactor() As Double
        Get
            Return m_ValueScaleFactor
        End Get
        Set(ByVal value As Double)
            If m_ValueScaleFactor <> value Then
                m_ValueScaleFactor = value
                UpdateText()
            End If
            'TODO: Does not refresh in designmode
            'Text = MyBase.Text
        End Set
    End Property

    Public Enum BooleanDisplayOption
        TrueFalse
        YesNo
        OnOff
        OneZero
    End Enum

    Private m_BooleanDisplay As BooleanDisplayOption
    Public Property BooleanDisplay() As BooleanDisplayOption
        Get
            Return m_BooleanDisplay
        End Get
        Set(ByVal value As BooleanDisplayOption)
            If m_BooleanDisplay <> value Then
                m_BooleanDisplay = value
                UpdateText()
            End If
        End Set
    End Property

    Private m_DisplayAsTime As Boolean
    Public Property DisplayAsTime As Boolean
        Get
            Return m_DisplayAsTime
        End Get
        Set(value As Boolean)
            If m_DisplayAsTime <> value Then
                m_DisplayAsTime = value
                UpdateText()
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

    Private _PollRate As Integer
    Public Property PollRate() As Integer
        Get
            Return _PollRate
        End Get
        Set(ByVal value As Integer)
            _PollRate = value
        End Set
    End Property

    Private m_KeypadText As String
    Public Property KeypadText() As String
        Get
            Return m_KeypadText
        End Get
        Set(ByVal value As String)
            m_KeypadText = value
        End Set
    End Property

    Private m_KeypadFont As Font = New Font("Arial", 10)
    Public Property KeypadFont() As Font
        Get
            Return m_KeypadFont
        End Get
        Set(ByVal value As Font)
            m_KeypadFont = value
        End Set
    End Property

    Private m_KeypadForeColor As Color = Color.WhiteSmoke
    Public Property KeypadFontColor() As Color
        Get
            Return m_KeypadForeColor
        End Get
        Set(ByVal value As Color)
            m_KeypadForeColor = value
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

    '* 29-JAN-13
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

    Private m_KeypadScaleFactor As Double = 1
    <System.ComponentModel.DefaultValue(1)> _
    Public Property KeypadScaleFactor() As Double
        Get
            Return m_KeypadScaleFactor
        End Get
        Set(ByVal value As Double)
            m_KeypadScaleFactor = value
        End Set
    End Property

    Private m_KeypadAlphaNumeric As Boolean
    Property KeypadAlphaNumeric As Boolean
        Get
            Return m_KeypadAlphaNumeric
        End Get
        Set(value As Boolean)
            m_KeypadAlphaNumeric = value
        End Set
    End Property

    Private m_KeypadShowCurrentValue As Boolean
    Property KeypadShowCurrentValue As Boolean
        Get
            Return m_KeypadShowCurrentValue
        End Get
        Set(value As Boolean)
            m_KeypadShowCurrentValue = value
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

    'Private m_PLCAddressValue2 As New MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    '<System.ComponentModel.Category("PLC Properties")> _
    'Public Property PLCAddressValue2() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    '    Get
    '        Return m_PLCAddressValue2
    '    End Get
    '    Set(value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
    '        m_PLCAddressValue2 = value
    '    End Set
    'End Property

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
    Private m_PLCAddressHighlight As String = ""
    <System.ComponentModel.DefaultValue("")> _
    <System.ComponentModel.Category("PLC Properties")> _
    Public Property PLCAddressHighlight() As String
        Get
            Return m_PLCAddressHighlight
        End Get
        Set(ByVal value As String)
            If m_PLCAddressHighlight <> value Then
                m_PLCAddressHighlight = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

#End Region

#Region "Private Methods"
    Private Sub UpdateText()
        '* Build the string with a temporary variable because Mybase.Text will keep firing Me.Invalidate
        Dim ResultText As String = m_Value

        If Not String.IsNullOrEmpty(ResultText) Then
            '* True/False comes from driver, change if BooleanDisplay is different 31-DEC-11
            If (String.Compare(m_Value, "True", True) = 0) Then
                If m_BooleanDisplay = BooleanDisplayOption.OnOff Then ResultText = "On"
                If m_BooleanDisplay = BooleanDisplayOption.YesNo Then ResultText = "Yes"
                If m_BooleanDisplay = BooleanDisplayOption.TrueFalse Then ResultText = "True"
                If m_BooleanDisplay = BooleanDisplayOption.OneZero Then ResultText = "1"
            ElseIf (String.Compare(m_Value, "False", True) = 0) Then
                If m_BooleanDisplay = BooleanDisplayOption.OnOff Then ResultText = "Off"
                If m_BooleanDisplay = BooleanDisplayOption.YesNo Then ResultText = "No"
                If m_BooleanDisplay = BooleanDisplayOption.TrueFalse Then ResultText = "False"
                If m_BooleanDisplay = BooleanDisplayOption.OneZero Then ResultText = "0"
            Else
                '* V3.99v
                If m_InterpretValueAsBCD Then
                    Try
                        Dim b() As Byte = BitConverter.GetBytes(CInt(ResultText))
                        ResultText = ""

                        For index = 3 To 0 Step -1
                            If (b(index) And 240) > 0 Or ResultText.Length > 0 Then
                                ResultText &= CStr((b(index) And 240) >> 4)
                            End If
                            If (b(index) And 15) > 0 Or ResultText.Length > 0 Then
                                ResultText &= CStr((b(index) And 15))
                            End If
                        Next
                    Catch ex As Exception
                        ResultText = "BCD Error"
                    End Try
                End If

                '******************************************************
                '* Scale Factor and Format only applied to non-Boolean
                '******************************************************
                '* Apply the scale factor
                Try
                    If m_ValueScaleFactor <> 1 Then
                        ResultText = CStr(ResultText * m_ValueScaleFactor)
                    End If
                Catch ex As Exception
                    If Not DesignMode Then DisplayError("Scale Factor Error - " & ex.Message)
                End Try

                '* Apply the format
                If (Not String.IsNullOrEmpty(m_NumericFormat)) And Not m_DisplayAsTime Then
                    Try
                        '* 31-MAY-13, 17-JUN-15 Changed from Single to Double to prevent rounding problems
                        Dim v As Double
                        If Double.TryParse(ResultText, v) Then
                            ResultText = v.ToString(m_NumericFormat)
                        End If
                    Catch exC As InvalidCastException
                        If Not DesignMode Then
                            ResultText = "----"
                        Else
                            ResultText = Value
                        End If
                    Catch ex As Exception
                        If Not DesignMode Then
                            ResultText = "Check NumericFormat and variable type"
                        End If
                    End Try
                End If


                If m_DisplayAsTime Then
                    Try
                        If Value <> "" Then
                            Dim ScaledValue As Double
                            ScaledValue = CDbl(Value) * m_ValueScaleFactor
                            Dim remainder As Integer
                            ResultText = Math.DivRem(CInt(ScaledValue), 3600, remainder) & ":" &
                                Math.DivRem(remainder, 60, remainder).ToString("00") & ":" &
                                remainder.ToString("00")
                        End If
                    Catch ex As Exception
                        If Not Me.DesignMode Then MyBase.Text = ex.Message
                        Exit Sub
                    End Try
                End If
            End If
            'End If

            If m_ValueToSubtractFrom <> 0 Then
                Try
                    ResultText = m_ValueToSubtractFrom - CSng(ResultText)
                Catch ex As Exception

                End Try
            End If


            '* Apply the left padding
            If m_ValueLeftPadLength > 0 Then
                ResultText = ResultText.PadLeft(m_ValueLeftPadLength, m_ValueLeftPadCharacter)
            End If

        Else
            ResultText = ""
        End If

        '* Highlight in red if a Highlightcharacter found mark is in text
        'If Not DesignMode Then
        If Value.IndexOf(_HighlightKeyChar) >= 0 Or m_Highlight Then
            'If MyBase.BackColor <> _Highlightcolor Then SavedBackColor = MyBase.BackColor
            MyBase.BackColor = m_Highlightcolor
            MyBase.ForeColor = m_HighlightForecolor
        Else
            'If SavedBackColor <> Nothing Then MyBase.BackColor = SavedBackColor
            MyBase.BackColor = m_BackColor
            MyBase.ForeColor = m_ForeColor
        End If
        'End If


        '* Apply the Prefix and Suffix
        If Not String.IsNullOrEmpty(m_Prefix) Then
            ResultText = m_Prefix & ResultText
        End If
        If Not String.IsNullOrEmpty(m_Suffix) Then
            ResultText &= m_Suffix
        End If

        'If m_DisplayAsTime Then
        '    Try
        '        If Value <> "" And Not Me.DesignMode Then
        '            Dim Hours As Double
        '            Hours = CDbl(Value) * m_ValueScaleFactor
        '            Dim Minutes As Double = (Hours - Math.Floor(Hours)) * 60
        '            ResultText = Math.Floor(Hours) & ":" & Format(Math.Floor(Minutes), "00")
        '        End If
        '    Catch ex As Exception
        '        MyBase.Text = ex.Message
        '        Exit Sub
        '    End Try
        'End If




        MyBase.Text = ResultText
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

        'If ForeColor.R = Me.Parent.BackColor.R And ForeColor.G = Me.Parent.BackColor.G And ForeColor.B = Me.Parent.BackColor.B Then
        '    ForeColor = Drawing.Color.FromArgb(Not Me.ForeColor.R, Not Me.ForeColor.G, Not Me.ForeColor.B)
        'End If
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

#Region "Keypad popup for data entry"
    '*****************************************
    '* Property - Address in PLC to Write Data To
    '*****************************************
    Private m_PLCAddressKeypad As String = ""
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

    Private WithEvents KeypadPopUp As MfgControl.AdvancedHMI.Controls.IKeyboard

    Private Sub KeypadPopUp_ButtonClick(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Controls.KeyPadEventArgs) Handles KeypadPopUp.ButtonClick
        If e.Key = "Quit" Then
            KeypadPopUp.Visible = False
        ElseIf e.Key = "Enter" Then
            If m_ComComponent Is Nothing Then
                DisplayError("ComComponent Property not set")
            Else
                If KeypadPopUp.Value IsNot Nothing AndAlso (String.Compare(KeypadPopUp.Value, "") <> 0) Then
                    '* 29-JAN-13 - Validate value if a Min/Max was specified
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
                        '* 29-JAN-13 - reduced code and checked for divide by 0
                        If KeypadScaleFactor = 1 Or KeypadScaleFactor = 0 Then
                            m_ComComponent.Write(m_PLCAddressKeypad, KeypadPopUp.Value)
                        Else
                            m_ComComponent.Write(m_PLCAddressKeypad, CDbl(KeypadPopUp.Value) / m_KeypadScaleFactor)
                        End If
                    Catch ex As Exception
                        System.Windows.Forms.MessageBox.Show("Failed to write value - " & ex.Message)
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
            ActivateKeypad()
        End If
    End Sub

    Public Sub ActivateKeypad()
        If KeypadPopUp Is Nothing Then
            If m_KeypadAlphaNumeric Then
                KeypadPopUp = New MfgControl.AdvancedHMI.Controls.AlphaKeyboard(m_KeypadWidth)
            Else
                KeypadPopUp = New MfgControl.AdvancedHMI.Controls.KeypadV2(m_KeypadWidth)
            End If
            KeypadPopUp.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
            'KeypadPopUp.Location = New Point(0, 0)
            KeypadPopUp.TopMost = True
        End If

        '***************************
        '*Set the font and forecolor
        '****************************
        'KeypadPopUp.Font = New Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Point)
        If m_KeypadFont IsNot Nothing Then KeypadPopUp.Font = m_KeypadFont
        'If m_KeypadForeColor IsNot Nothing Then
        KeypadPopUp.ForeColor = m_KeypadForeColor
        'End If


        KeypadPopUp.Text = m_KeypadText
        If m_KeypadShowCurrentValue Then
            Try
                Dim CurrentValue As String = m_ComComponent.Read(m_PLCAddressKeypad, 1)(0)
                '* v3.99p - added scaling
                If m_ValueScaleFactor = 1 Then
                    KeypadPopUp.Value = CurrentValue
                Else
                    Try
                        Dim ScaledValue As Double = CDbl(CurrentValue) * m_ValueScaleFactor
                        KeypadPopUp.Value = ScaledValue
                    Catch ex As Exception
                        System.Windows.Forms.MessageBox.Show("Failed to Scale current value of " & CurrentValue)
                    End Try
                End If

            Catch ex As Exception
                System.Windows.Forms.MessageBox.Show("Failed to read current value of " & m_PLCAddressKeypad)
            End Try
        Else
            KeypadPopUp.Value = ""
        End If
        KeypadPopUp.Visible = True
    End Sub
#End Region
End Class
