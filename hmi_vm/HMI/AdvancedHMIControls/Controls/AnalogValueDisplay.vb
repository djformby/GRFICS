Public Class AnalogValueDisplay
    Inherits System.Windows.Forms.Label

    Public Event ValueChanged As EventHandler
    Public Event ValueLimitUpperChanged As EventHandler
    Public Event ValueLimitLowerChanged As EventHandler



#Region "Constructor"
    Public Sub New()
        MyBase.New()

        Value = "0000"

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
    <System.ComponentModel.Browsable(False)>
    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
        End Set
    End Property

    <System.ComponentModel.Browsable(False)>
    Public Overrides Property ForeColor As Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(value As Color)
            MyBase.ForeColor = value
        End Set
    End Property

    Private m_ForeColorInLimits As Color = Color.White
    Public Property ForeColorInLimits As Color
        Get
            Return m_ForeColorInLimits
        End Get
        Set(value As Color)
            m_ForeColorInLimits = value
        End Set
    End Property


    Private m_ForeColorOverLimit As Color = Color.Red
    Public Property ForeColorOverLimit As Color
        Get
            Return m_ForeColorOverLimit
        End Get
        Set(value As Color)
            m_ForeColorOverLimit = value
        End Set
    End Property


    Private m_ForeColorUnderLimit As Color = Color.Yellow
    Public Property ForeColorUnderLimit As Color
        Get
            Return m_ForeColorUnderLimit
        End Get
        Set(value As Color)
            m_ForeColorUnderLimit = value
        End Set
    End Property


    '******************************************************************************************
    '* Use the base control's text property and make it visible as a property on the designer
    '******************************************************************************************
    Private m_Value As String
    Private ValueAsDouble As Double
    Public Property Value As String
        Get
            Return m_Value
        End Get
        Set(ByVal value As String)
            If value <> m_Value Then
                If value IsNot Nothing Then
                    m_Value = value
                    Double.TryParse(value, ValueAsDouble)
                    UpdateText()
                    OnValueChanged(EventArgs.Empty)
                Else
                    '* version 3.99f
                    If m_Value <> "" Then OnValueChanged(EventArgs.Empty)
                    m_Value = ""
                    MyBase.Text = ""
                End If
                '* Be sure error handler doesn't revert back to an incorrect text
                OriginalText = MyBase.Text
                UpdateVisible()
            End If
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

    Private m_ValueLimitUpper As Double = 999999
    Public Property ValueLimitUpper As Double
        Get
            Return m_ValueLimitUpper
        End Get
        Set(value As Double)
            If m_ValueLimitUpper <> value Then
                m_ValueLimitUpper = value
                UpdateVisible()
                UpdateText()
                OnValueLimitUpperChanged(System.EventArgs.Empty)
            End If
        End Set
    End Property

    Private m_ValueLimitLower As Double = -999999
    Public Property ValueLimitLower As Double
        Get
            Return m_ValueLimitLower
        End Get
        Set(value As Double)
            If m_ValueLimitLower <> value Then
                m_ValueLimitLower = value
                UpdateVisible()
                UpdateText()
                OnValueLimitLowerChanged(System.EventArgs.Empty)
            End If
        End Set
    End Property

    Private m_ShowValue As Boolean = True
    Public Property ShowValue As Boolean
        Get
            Return m_ShowValue
        End Get
        Set(value As Boolean)
            If m_ShowValue <> value Then
                m_ShowValue = value
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

    Public Enum VisibleControlEnum
        Always
        BelowLimit
        WithinLimits
        AboveLimits
    End Enum

    Private m_VisibleControl As VisibleControlEnum = VisibleControlEnum.Always
    Public Property VisibleControl As VisibleControlEnum
        Get
            Return m_VisibleControl
        End Get
        Set(value As VisibleControlEnum)
            If m_VisibleControl <> value Then
                m_VisibleControl = value
                If m_VisibleControl = VisibleControlEnum.Always Then
                    Me.Visible = True
                End If
            End If
        End Set
    End Property
#End Region

#Region "Private Methods"
    Private Sub UpdateText()
        Dim ResultText As String = ""

        Dim v As Double

        If m_ShowValue Then
            ResultText = m_Value
            If Not String.IsNullOrEmpty(m_NumericFormat) Then
                If Double.TryParse(Value, v) Then
                    Try
                        ResultText = v.ToString(m_NumericFormat)
                    Catch ex As Exception
                        ResultText = "Check Numeric Format"
                    End Try
                End If
            Else
                ResultText = Value
            End If
        End If

        '* Apply the Prefix and Suffix
        If Not String.IsNullOrEmpty(m_Prefix) Then
            ResultText = m_Prefix & ResultText
        End If
        If Not String.IsNullOrEmpty(m_Suffix) Then
            ResultText &= m_Suffix
        End If

        MyBase.Text = ResultText


        If Double.TryParse(Value, v) Then
            If v > m_ValueLimitUpper Then
                MyBase.ForeColor = m_ForeColorOverLimit
            ElseIf v < m_ValueLimitLower Then
                MyBase.ForeColor = m_ForeColorUnderLimit
            Else
                MyBase.ForeColor = m_ForeColorInLimits
            End If
        End If

    End Sub


    Private Sub UpdateVisible()
        If m_VisibleControl <> VisibleControlEnum.Always Then
            If m_VisibleControl = VisibleControlEnum.AboveLimits Then
                Me.Visible = (ValueAsDouble > ValueLimitUpper)
            ElseIf m_VisibleControl = VisibleControlEnum.BelowLimit Then
                Me.Visible = (ValueAsDouble < ValueLimitLower)
            ElseIf m_VisibleControl = VisibleControlEnum.WithinLimits Then
                Me.Visible = (ValueAsDouble <= ValueLimitUpper And ValueAsDouble >= ValueLimitLower)
            End If
        End If
    End Sub
#End Region

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
                    SubScriptions.ComComponent = value
                End If

                m_ComComponent = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

    Public Property PLCAddressVisible() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValue As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.DefaultValue("")>
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressValue() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Try
                Return m_PLCAddressValue
            Catch ex As Exception
                Return Nothing
            End Try

        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If ((value Is Nothing Or m_PLCAddressValue Is Nothing) OrElse ((value.GetType) Is (m_PLCAddressValue.GetType))) Then
                Try
                    If m_PLCAddressValue IsNot value Then
                        m_PLCAddressValue = value
                    End If
                    Try
                        '* When address is changed, re-subscribe to new address
                        SubscribeToComDriver()
                    Catch ex As Exception
                        'MsgBox("5 - " & ex.Message)
                    End Try
                Catch ex As Exception
                    'MsgBox("6 - " & ex.Message)
                    'Console.WriteLine("AnalogValueDisplay PLCAddressValue setter exception")
                End Try
            End If
            'MsgBox("7 - Exit Setter")
        End Set
    End Property


    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValueLimitUpper As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.DefaultValue("")>
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressValueLimitUpper() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressValueLimitUpper
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressValueLimitUpper IsNot value Then
                m_PLCAddressValueLimitUpper = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressValueLimitLower As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.DefaultValue("")>
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressValueLimitLower() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressValueLimitLower
        End Get
        Set(ByVal value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressValueLimitLower IsNot value Then
                m_PLCAddressValueLimitLower = value

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



    Protected Overridable Sub OnValueChanged(ByVal e As EventArgs)
        RaiseEvent ValueChanged(Me, e)
    End Sub

    Protected Overridable Sub OnValueLimitUpperChanged(ByVal e As EventArgs)
        RaiseEvent ValueLimitUpperChanged(Me, e)
    End Sub

    Protected Overridable Sub OnValueLimitLowerChanged(ByVal e As EventArgs)
        RaiseEvent ValueLimitLowerChanged(Me, e)
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
    '*****************************************
    '* Property - Address in PLC to Write Data To
    '*****************************************
    Private m_PLCAddressKeypad As String = ""
    <System.ComponentModel.Category("PLC Properties")>
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

    Private m_KeypadPasscode As String
    Public Property KeypadPasscode As String
        Get
            Return m_KeypadPasscode
        End Get
        Set(value As String)
            m_KeypadPasscode = value
        End Set
    End Property


    Private Sub KeypadPopUp_ButtonClick(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Controls.KeyPadEventArgs)
        If e.Key = "Quit" Then
            KeypadPopUp.Visible = False
        ElseIf e.Key = "Enter" Then
            If String.IsNullOrEmpty(m_KeypadPasscode) Or PasscodeValidated Then
                If m_ComComponent Is Nothing Then
                    DisplayError("ComComponent Property not set")
                Else
                    If Not String.IsNullOrEmpty(KeypadPopUp.Value) Then
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
            Else
                '* A passcode was entered, so validate
                If Not String.IsNullOrEmpty(KeypadPopUp.Value) AndAlso KeypadPopUp.Value = m_KeypadPasscode Then
                    PasscodeValidated = True
                    PromptNewValue()
                Else
                    System.Windows.Forms.MessageBox.Show("Invalid Passcode!")
                    KeypadPopUp.Visible = False
                End If
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

            If String.IsNullOrEmpty(m_KeypadPasscode) Then
                PromptNewValue()
            Else
                PromptPasscode()
            End If
        End If
    End Sub

    Private Sub PromptNewValue()
        KeypadPopUp.Text = m_KeypadText
        KeypadPopUp.ForeColor = m_KeypadFontColor
        KeypadPopUp.Value = ""
        KeypadPopUp.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
        KeypadPopUp.TopMost = True
        KeypadPopUp.Show()
    End Sub

    Private PasscodeValidated As Boolean
    Private Sub PromptPasscode()
        PasscodeValidated = False
        KeypadPopUp.Text = "Enter Passcode"
        KeypadPopUp.ForeColor = m_KeypadFontColor
        KeypadPopUp.Value = ""
        KeypadPopUp.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
        KeypadPopUp.TopMost = True
        KeypadPopUp.Show()
    End Sub
#End Region
End Class
