<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    '   <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    ' <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ForceItemsIntoToolBox1 = New MfgControl.AdvancedHMI.Drivers.ForceItemsIntoToolbox()
        Me.ModbusTCPCom1 = New AdvancedHMIDrivers.ModbusTCPCom(Me.components)
        Me.Tank1 = New AdvancedHMIControls.Tank()
        Me.Pipe1 = New AdvancedHMIControls.Pipe()
        Me.Pipe2 = New AdvancedHMIControls.Pipe()
        Me.PneumaticBallValve1 = New AdvancedHMIControls.PneumaticBallValve()
        Me.Pipe4 = New AdvancedHMIControls.Pipe()
        Me.PneumaticBallValve2 = New AdvancedHMIControls.PneumaticBallValve()
        Me.PneumaticBallValve3 = New AdvancedHMIControls.PneumaticBallValve()
        Me.Pipe6 = New AdvancedHMIControls.Pipe()
        Me.BasicLabel1 = New AdvancedHMIControls.BasicLabel()
        Me.PneumaticBallValve4 = New AdvancedHMIControls.PneumaticBallValve()
        Me.Pipe3 = New AdvancedHMIControls.Pipe()
        Me.BasicLabel2 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel3 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel4 = New AdvancedHMIControls.BasicLabel()
        Me.AnalogValueDisplay1 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.BasicLabel5 = New AdvancedHMIControls.BasicLabel()
        Me.AnalogValueDisplay2 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay3 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay4 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay5 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay6 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay7 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.AnalogValueDisplay8 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.DigitalPanelMeter1 = New AdvancedHMIControls.DigitalPanelMeter()
        Me.Pipe8 = New AdvancedHMIControls.Pipe()
        Me.Pipe5 = New AdvancedHMIControls.Pipe()
        Me.BasicLabel6 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel7 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel8 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel9 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel10 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel11 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel12 = New AdvancedHMIControls.BasicLabel()
        Me.BasicLabel13 = New AdvancedHMIControls.BasicLabel()
        Me.DataSubscriber1 = New AdvancedHMIControls.DataSubscriber(Me.components)
        Me.AnalogValueDisplay9 = New AdvancedHMIControls.AnalogValueDisplay()
        Me.KeyboardInput1 = New AdvancedHMIControls.KeyboardInput()
        Me.PilotLight1 = New AdvancedHMIControls.PilotLight()
        Me.PilotLight2 = New AdvancedHMIControls.PilotLight()
        Me.BasicLabel14 = New AdvancedHMIControls.BasicLabel()
        CType(Me.ModbusTCPCom1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataSubscriber1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!)
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(6, 662)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(194, 32)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "For Development Source Code Visit" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "http://www.advancedhmi.com"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'ModbusTCPCom1
        '
        Me.ModbusTCPCom1.DisableSubscriptions = False
        Me.ModbusTCPCom1.IniFileName = ""
        Me.ModbusTCPCom1.IniFileSection = Nothing
        Me.ModbusTCPCom1.IPAddress = "192.168.95.2"
        Me.ModbusTCPCom1.MaxReadGroupSize = 20
        Me.ModbusTCPCom1.PollRateOverride = 500
        Me.ModbusTCPCom1.SwapBytes = True
        Me.ModbusTCPCom1.SwapWords = False
        Me.ModbusTCPCom1.TcpipPort = CType(502US, UShort)
        Me.ModbusTCPCom1.TimeOut = 3000
        Me.ModbusTCPCom1.UnitId = CType(247, Byte)
        '
        'Tank1
        '
        Me.Tank1.ComComponent = Me.ModbusTCPCom1
        Me.Tank1.ForeColor = System.Drawing.Color.White
        Me.Tank1.HighlightColor = System.Drawing.Color.Red
        Me.Tank1.HighlightKeyCharacter = "!"
        Me.Tank1.KeypadText = Nothing
        Me.Tank1.Location = New System.Drawing.Point(299, 150)
        Me.Tank1.MaxValue = 100
        Me.Tank1.MinValue = 0
        Me.Tank1.Name = "Tank1"
        Me.Tank1.NumericFormat = Nothing
        Me.Tank1.PLCAddressKeypad = ""
        Me.Tank1.PLCAddressText = ""
        Me.Tank1.PLCAddressValue = "41046"
        Me.Tank1.PLCAddressVisible = ""
        Me.Tank1.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Tank1.Size = New System.Drawing.Size(150, 215)
        Me.Tank1.TabIndex = 55
        Me.Tank1.Tag = "test"
        Me.Tank1.TankContentColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Tank1.TextPrefix = Nothing
        Me.Tank1.TextSuffix = Nothing
        Me.Tank1.Value = 0!
        Me.Tank1.ValueScaleFactor = New Decimal(New Integer() {5, 0, 0, 65536})
        '
        'Pipe1
        '
        Me.Pipe1.ComComponent = Me.ModbusTCPCom1
        Me.Pipe1.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Straight
        Me.Pipe1.ForeColor = System.Drawing.Color.White
        Me.Pipe1.HighlightColor = System.Drawing.Color.Red
        Me.Pipe1.HighlightKeyCharacter = "!"
        Me.Pipe1.KeypadText = Nothing
        Me.Pipe1.Location = New System.Drawing.Point(94, 163)
        Me.Pipe1.Name = "Pipe1"
        Me.Pipe1.NumericFormat = Nothing
        Me.Pipe1.PLCAddressKeypad = ""
        Me.Pipe1.PLCAddressRotate = ""
        Me.Pipe1.PLCAddressText = ""
        Me.Pipe1.PLCAddressVisible = ""
        Me.Pipe1.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.Pipe1.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe1.Size = New System.Drawing.Size(225, 23)
        Me.Pipe1.TabIndex = 56
        Me.Pipe1.TextPrefix = Nothing
        Me.Pipe1.TextSuffix = Nothing
        '
        'Pipe2
        '
        Me.Pipe2.BackColor = System.Drawing.Color.Black
        Me.Pipe2.ComComponent = Me.ModbusTCPCom1
        Me.Pipe2.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Straight
        Me.Pipe2.ForeColor = System.Drawing.Color.Black
        Me.Pipe2.HighlightColor = System.Drawing.Color.Red
        Me.Pipe2.HighlightKeyCharacter = "!"
        Me.Pipe2.KeypadText = Nothing
        Me.Pipe2.Location = New System.Drawing.Point(94, 248)
        Me.Pipe2.Name = "Pipe2"
        Me.Pipe2.NumericFormat = Nothing
        Me.Pipe2.PLCAddressKeypad = ""
        Me.Pipe2.PLCAddressRotate = ""
        Me.Pipe2.PLCAddressText = ""
        Me.Pipe2.PLCAddressVisible = ""
        Me.Pipe2.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.Pipe2.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe2.Size = New System.Drawing.Size(225, 23)
        Me.Pipe2.TabIndex = 57
        Me.Pipe2.TextPrefix = Nothing
        Me.Pipe2.TextSuffix = Nothing
        '
        'PneumaticBallValve1
        '
        Me.PneumaticBallValve1.ComComponent = Me.ModbusTCPCom1
        Me.PneumaticBallValve1.ForeColor = System.Drawing.Color.LightGray
        Me.PneumaticBallValve1.Location = New System.Drawing.Point(112, 123)
        Me.PneumaticBallValve1.Name = "PneumaticBallValve1"
        Me.PneumaticBallValve1.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.WriteValue
        Me.PneumaticBallValve1.PLCAddressClick = ""
        Me.PneumaticBallValve1.PLCAddressText = ""
        Me.PneumaticBallValve1.PLCAddressValue = ""
        Me.PneumaticBallValve1.PLCAddressVisible = ""
        Me.PneumaticBallValve1.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.PneumaticBallValve1.Size = New System.Drawing.Size(65, 65)
        Me.PneumaticBallValve1.TabIndex = 59
        Me.PneumaticBallValve1.Value = False
        '
        'Pipe4
        '
        Me.Pipe4.ComComponent = Me.ModbusTCPCom1
        Me.Pipe4.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Straight
        Me.Pipe4.ForeColor = System.Drawing.Color.Black
        Me.Pipe4.HighlightColor = System.Drawing.Color.Red
        Me.Pipe4.HighlightKeyCharacter = "!"
        Me.Pipe4.KeypadText = Nothing
        Me.Pipe4.Location = New System.Drawing.Point(369, 342)
        Me.Pipe4.Name = "Pipe4"
        Me.Pipe4.NumericFormat = Nothing
        Me.Pipe4.PLCAddressKeypad = ""
        Me.Pipe4.PLCAddressRotate = ""
        Me.Pipe4.PLCAddressText = ""
        Me.Pipe4.PLCAddressVisible = ""
        Me.Pipe4.Rotation = System.Drawing.RotateFlipType.Rotate270FlipXY
        Me.Pipe4.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe4.Size = New System.Drawing.Size(21, 51)
        Me.Pipe4.TabIndex = 61
        Me.Pipe4.TextPrefix = Nothing
        Me.Pipe4.TextSuffix = Nothing
        '
        'PneumaticBallValve2
        '
        Me.PneumaticBallValve2.ComComponent = Me.ModbusTCPCom1
        Me.PneumaticBallValve2.ForeColor = System.Drawing.Color.LightGray
        Me.PneumaticBallValve2.Location = New System.Drawing.Point(112, 208)
        Me.PneumaticBallValve2.Name = "PneumaticBallValve2"
        Me.PneumaticBallValve2.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet
        Me.PneumaticBallValve2.PLCAddressClick = ""
        Me.PneumaticBallValve2.PLCAddressText = ""
        Me.PneumaticBallValve2.PLCAddressValue = ""
        Me.PneumaticBallValve2.PLCAddressVisible = ""
        Me.PneumaticBallValve2.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.PneumaticBallValve2.Size = New System.Drawing.Size(65, 65)
        Me.PneumaticBallValve2.TabIndex = 62
        Me.PneumaticBallValve2.Value = False
        '
        'PneumaticBallValve3
        '
        Me.PneumaticBallValve3.ComComponent = Me.ModbusTCPCom1
        Me.PneumaticBallValve3.ForeColor = System.Drawing.Color.LightGray
        Me.PneumaticBallValve3.Location = New System.Drawing.Point(418, 371)
        Me.PneumaticBallValve3.Name = "PneumaticBallValve3"
        Me.PneumaticBallValve3.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet
        Me.PneumaticBallValve3.PLCAddressClick = ""
        Me.PneumaticBallValve3.PLCAddressText = ""
        Me.PneumaticBallValve3.PLCAddressValue = ""
        Me.PneumaticBallValve3.PLCAddressVisible = ""
        Me.PneumaticBallValve3.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.PneumaticBallValve3.Size = New System.Drawing.Size(69, 69)
        Me.PneumaticBallValve3.TabIndex = 65
        Me.PneumaticBallValve3.Value = False
        '
        'Pipe6
        '
        Me.Pipe6.ComComponent = Me.ModbusTCPCom1
        Me.Pipe6.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Straight
        Me.Pipe6.ForeColor = System.Drawing.Color.Black
        Me.Pipe6.HighlightColor = System.Drawing.Color.Red
        Me.Pipe6.HighlightKeyCharacter = "!"
        Me.Pipe6.KeypadText = Nothing
        Me.Pipe6.Location = New System.Drawing.Point(485, 414)
        Me.Pipe6.Name = "Pipe6"
        Me.Pipe6.NumericFormat = Nothing
        Me.Pipe6.PLCAddressKeypad = ""
        Me.Pipe6.PLCAddressRotate = ""
        Me.Pipe6.PLCAddressText = ""
        Me.Pipe6.PLCAddressVisible = ""
        Me.Pipe6.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.Pipe6.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe6.Size = New System.Drawing.Size(155, 22)
        Me.Pipe6.TabIndex = 66
        Me.Pipe6.TextPrefix = Nothing
        Me.Pipe6.TextSuffix = Nothing
        '
        'BasicLabel1
        '
        Me.BasicLabel1.AutoSize = True
        Me.BasicLabel1.BackColor = System.Drawing.Color.Black
        Me.BasicLabel1.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel1.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel1.DisplayAsTime = False
        Me.BasicLabel1.ForeColor = System.Drawing.Color.White
        Me.BasicLabel1.Highlight = False
        Me.BasicLabel1.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel1.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel1.HighlightKeyCharacter = "!"
        Me.BasicLabel1.InterpretValueAsBCD = False
        Me.BasicLabel1.KeypadAlphaNumeric = False
        Me.BasicLabel1.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel1.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel1.KeypadMaxValue = 0R
        Me.BasicLabel1.KeypadMinValue = 0R
        Me.BasicLabel1.KeypadScaleFactor = 1.0R
        Me.BasicLabel1.KeypadShowCurrentValue = False
        Me.BasicLabel1.KeypadText = Nothing
        Me.BasicLabel1.KeypadWidth = 300
        Me.BasicLabel1.Location = New System.Drawing.Point(646, 415)
        Me.BasicLabel1.Name = "BasicLabel1"
        Me.BasicLabel1.NumericFormat = Nothing
        Me.BasicLabel1.PLCAddressKeypad = ""
        Me.BasicLabel1.PollRate = 0
        Me.BasicLabel1.Size = New System.Drawing.Size(62, 18)
        Me.BasicLabel1.TabIndex = 67
        Me.BasicLabel1.Text = "Product"
        Me.BasicLabel1.Value = "Product"
        Me.BasicLabel1.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel1.ValueLeftPadLength = 0
        Me.BasicLabel1.ValuePrefix = Nothing
        Me.BasicLabel1.ValueScaleFactor = 1.0R
        Me.BasicLabel1.ValueSuffix = Nothing
        Me.BasicLabel1.ValueToSubtractFrom = 0!
        '
        'PneumaticBallValve4
        '
        Me.PneumaticBallValve4.ComComponent = Me.ModbusTCPCom1
        Me.PneumaticBallValve4.ForeColor = System.Drawing.Color.LightGray
        Me.PneumaticBallValve4.Location = New System.Drawing.Point(460, 69)
        Me.PneumaticBallValve4.Name = "PneumaticBallValve4"
        Me.PneumaticBallValve4.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.MomentarySet
        Me.PneumaticBallValve4.PLCAddressClick = ""
        Me.PneumaticBallValve4.PLCAddressText = ""
        Me.PneumaticBallValve4.PLCAddressValue = ""
        Me.PneumaticBallValve4.PLCAddressVisible = ""
        Me.PneumaticBallValve4.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.PneumaticBallValve4.Size = New System.Drawing.Size(65, 65)
        Me.PneumaticBallValve4.TabIndex = 69
        Me.PneumaticBallValve4.Value = False
        '
        'Pipe3
        '
        Me.Pipe3.ComComponent = Me.ModbusTCPCom1
        Me.Pipe3.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Straight
        Me.Pipe3.ForeColor = System.Drawing.Color.Black
        Me.Pipe3.HighlightColor = System.Drawing.Color.Red
        Me.Pipe3.HighlightKeyCharacter = "!"
        Me.Pipe3.KeypadText = Nothing
        Me.Pipe3.Location = New System.Drawing.Point(525, 109)
        Me.Pipe3.Name = "Pipe3"
        Me.Pipe3.NumericFormat = Nothing
        Me.Pipe3.PLCAddressKeypad = ""
        Me.Pipe3.PLCAddressRotate = ""
        Me.Pipe3.PLCAddressText = ""
        Me.Pipe3.PLCAddressVisible = ""
        Me.Pipe3.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.Pipe3.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe3.Size = New System.Drawing.Size(109, 20)
        Me.Pipe3.TabIndex = 70
        Me.Pipe3.TextPrefix = Nothing
        Me.Pipe3.TextSuffix = Nothing
        '
        'BasicLabel2
        '
        Me.BasicLabel2.AutoSize = True
        Me.BasicLabel2.BackColor = System.Drawing.Color.Black
        Me.BasicLabel2.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel2.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel2.DisplayAsTime = False
        Me.BasicLabel2.ForeColor = System.Drawing.Color.White
        Me.BasicLabel2.Highlight = False
        Me.BasicLabel2.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel2.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel2.HighlightKeyCharacter = "!"
        Me.BasicLabel2.InterpretValueAsBCD = False
        Me.BasicLabel2.KeypadAlphaNumeric = False
        Me.BasicLabel2.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel2.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel2.KeypadMaxValue = 0R
        Me.BasicLabel2.KeypadMinValue = 0R
        Me.BasicLabel2.KeypadScaleFactor = 1.0R
        Me.BasicLabel2.KeypadShowCurrentValue = False
        Me.BasicLabel2.KeypadText = Nothing
        Me.BasicLabel2.KeypadWidth = 300
        Me.BasicLabel2.Location = New System.Drawing.Point(640, 111)
        Me.BasicLabel2.Name = "BasicLabel2"
        Me.BasicLabel2.NumericFormat = Nothing
        Me.BasicLabel2.PLCAddressKeypad = ""
        Me.BasicLabel2.PollRate = 0
        Me.BasicLabel2.Size = New System.Drawing.Size(50, 18)
        Me.BasicLabel2.TabIndex = 71
        Me.BasicLabel2.Text = "Purge"
        Me.BasicLabel2.Value = "Purge"
        Me.BasicLabel2.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel2.ValueLeftPadLength = 0
        Me.BasicLabel2.ValuePrefix = Nothing
        Me.BasicLabel2.ValueScaleFactor = 1.0R
        Me.BasicLabel2.ValueSuffix = Nothing
        Me.BasicLabel2.ValueToSubtractFrom = 0!
        '
        'BasicLabel3
        '
        Me.BasicLabel3.AutoSize = True
        Me.BasicLabel3.BackColor = System.Drawing.Color.Black
        Me.BasicLabel3.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel3.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel3.DisplayAsTime = False
        Me.BasicLabel3.ForeColor = System.Drawing.Color.White
        Me.BasicLabel3.Highlight = False
        Me.BasicLabel3.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel3.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel3.HighlightKeyCharacter = "!"
        Me.BasicLabel3.InterpretValueAsBCD = False
        Me.BasicLabel3.KeypadAlphaNumeric = False
        Me.BasicLabel3.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel3.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel3.KeypadMaxValue = 0R
        Me.BasicLabel3.KeypadMinValue = 0R
        Me.BasicLabel3.KeypadScaleFactor = 1.0R
        Me.BasicLabel3.KeypadShowCurrentValue = False
        Me.BasicLabel3.KeypadText = Nothing
        Me.BasicLabel3.KeypadWidth = 300
        Me.BasicLabel3.Location = New System.Drawing.Point(29, 163)
        Me.BasicLabel3.Name = "BasicLabel3"
        Me.BasicLabel3.NumericFormat = Nothing
        Me.BasicLabel3.PLCAddressKeypad = ""
        Me.BasicLabel3.PollRate = 0
        Me.BasicLabel3.Size = New System.Drawing.Size(58, 18)
        Me.BasicLabel3.TabIndex = 72
        Me.BasicLabel3.Text = "Feed 1"
        Me.BasicLabel3.Value = "Feed 1"
        Me.BasicLabel3.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel3.ValueLeftPadLength = 0
        Me.BasicLabel3.ValuePrefix = Nothing
        Me.BasicLabel3.ValueScaleFactor = 1.0R
        Me.BasicLabel3.ValueSuffix = Nothing
        Me.BasicLabel3.ValueToSubtractFrom = 0!
        '
        'BasicLabel4
        '
        Me.BasicLabel4.AutoSize = True
        Me.BasicLabel4.BackColor = System.Drawing.Color.Black
        Me.BasicLabel4.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel4.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel4.DisplayAsTime = False
        Me.BasicLabel4.ForeColor = System.Drawing.Color.White
        Me.BasicLabel4.Highlight = False
        Me.BasicLabel4.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel4.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel4.HighlightKeyCharacter = "!"
        Me.BasicLabel4.InterpretValueAsBCD = False
        Me.BasicLabel4.KeypadAlphaNumeric = False
        Me.BasicLabel4.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel4.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel4.KeypadMaxValue = 0R
        Me.BasicLabel4.KeypadMinValue = 0R
        Me.BasicLabel4.KeypadScaleFactor = 1.0R
        Me.BasicLabel4.KeypadShowCurrentValue = False
        Me.BasicLabel4.KeypadText = Nothing
        Me.BasicLabel4.KeypadWidth = 300
        Me.BasicLabel4.Location = New System.Drawing.Point(29, 248)
        Me.BasicLabel4.Name = "BasicLabel4"
        Me.BasicLabel4.NumericFormat = Nothing
        Me.BasicLabel4.PLCAddressKeypad = ""
        Me.BasicLabel4.PollRate = 0
        Me.BasicLabel4.Size = New System.Drawing.Size(58, 18)
        Me.BasicLabel4.TabIndex = 73
        Me.BasicLabel4.Text = "Feed 2"
        Me.BasicLabel4.Value = "Feed 2"
        Me.BasicLabel4.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel4.ValueLeftPadLength = 0
        Me.BasicLabel4.ValuePrefix = Nothing
        Me.BasicLabel4.ValueScaleFactor = 1.0R
        Me.BasicLabel4.ValueSuffix = Nothing
        Me.BasicLabel4.ValueToSubtractFrom = 0!
        '
        'AnalogValueDisplay1
        '
        Me.AnalogValueDisplay1.AutoSize = True
        Me.AnalogValueDisplay1.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay1.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay1.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay1.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay1.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay1.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay1.KeypadMaxValue = 0R
        Me.AnalogValueDisplay1.KeypadMinValue = 0R
        Me.AnalogValueDisplay1.KeypadPasscode = Nothing
        Me.AnalogValueDisplay1.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay1.KeypadText = Nothing
        Me.AnalogValueDisplay1.KeypadWidth = 300
        Me.AnalogValueDisplay1.Location = New System.Drawing.Point(104, 274)
        Me.AnalogValueDisplay1.MaximumSize = New System.Drawing.Size(58, 18)
        Me.AnalogValueDisplay1.Name = "AnalogValueDisplay1"
        Me.AnalogValueDisplay1.NumericFormat = Nothing
        Me.AnalogValueDisplay1.PLCAddressKeypad = ""
        Me.AnalogValueDisplay1.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay1.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay1.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay1.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay1.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay1.ShowValue = True
        Me.AnalogValueDisplay1.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay1.TabIndex = 74
        Me.AnalogValueDisplay1.Text = "0000"
        Me.AnalogValueDisplay1.Value = "0000"
        Me.AnalogValueDisplay1.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay1.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay1.ValuePrefix = Nothing
        Me.AnalogValueDisplay1.ValueSuffix = Nothing
        Me.AnalogValueDisplay1.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'BasicLabel5
        '
        Me.BasicLabel5.AutoSize = True
        Me.BasicLabel5.BackColor = System.Drawing.Color.Black
        Me.BasicLabel5.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel5.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel5.DisplayAsTime = False
        Me.BasicLabel5.ForeColor = System.Drawing.Color.White
        Me.BasicLabel5.Highlight = False
        Me.BasicLabel5.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel5.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel5.HighlightKeyCharacter = "!"
        Me.BasicLabel5.InterpretValueAsBCD = False
        Me.BasicLabel5.KeypadAlphaNumeric = False
        Me.BasicLabel5.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel5.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel5.KeypadMaxValue = 0R
        Me.BasicLabel5.KeypadMinValue = 0R
        Me.BasicLabel5.KeypadScaleFactor = 1.0R
        Me.BasicLabel5.KeypadShowCurrentValue = False
        Me.BasicLabel5.KeypadText = Nothing
        Me.BasicLabel5.KeypadWidth = 300
        Me.BasicLabel5.Location = New System.Drawing.Point(441, 253)
        Me.BasicLabel5.Name = "BasicLabel5"
        Me.BasicLabel5.NumericFormat = Nothing
        Me.BasicLabel5.PLCAddressKeypad = ""
        Me.BasicLabel5.PollRate = 0
        Me.BasicLabel5.Size = New System.Drawing.Size(111, 18)
        Me.BasicLabel5.TabIndex = 76
        Me.BasicLabel5.Text = "Pressure [kPa]"
        Me.BasicLabel5.Value = "Pressure [kPa]"
        Me.BasicLabel5.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel5.ValueLeftPadLength = 0
        Me.BasicLabel5.ValuePrefix = Nothing
        Me.BasicLabel5.ValueScaleFactor = 1.0R
        Me.BasicLabel5.ValueSuffix = Nothing
        Me.BasicLabel5.ValueToSubtractFrom = 0!
        '
        'AnalogValueDisplay2
        '
        Me.AnalogValueDisplay2.AutoSize = True
        Me.AnalogValueDisplay2.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay2.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay2.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay2.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay2.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay2.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay2.KeypadMaxValue = 0R
        Me.AnalogValueDisplay2.KeypadMinValue = 0R
        Me.AnalogValueDisplay2.KeypadPasscode = Nothing
        Me.AnalogValueDisplay2.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay2.KeypadText = Nothing
        Me.AnalogValueDisplay2.KeypadWidth = 300
        Me.AnalogValueDisplay2.Location = New System.Drawing.Point(200, 189)
        Me.AnalogValueDisplay2.MaximumSize = New System.Drawing.Size(93, 18)
        Me.AnalogValueDisplay2.Name = "AnalogValueDisplay2"
        Me.AnalogValueDisplay2.NumericFormat = Nothing
        Me.AnalogValueDisplay2.PLCAddressKeypad = ""
        Me.AnalogValueDisplay2.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay2.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay2.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay2.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay2.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay2.ShowValue = True
        Me.AnalogValueDisplay2.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay2.TabIndex = 79
        Me.AnalogValueDisplay2.Text = "0000"
        Me.AnalogValueDisplay2.Value = "0000"
        Me.AnalogValueDisplay2.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay2.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay2.ValuePrefix = Nothing
        Me.AnalogValueDisplay2.ValueSuffix = Nothing
        Me.AnalogValueDisplay2.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay3
        '
        Me.AnalogValueDisplay3.AutoSize = True
        Me.AnalogValueDisplay3.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay3.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay3.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay3.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay3.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay3.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay3.KeypadMaxValue = 0R
        Me.AnalogValueDisplay3.KeypadMinValue = 0R
        Me.AnalogValueDisplay3.KeypadPasscode = Nothing
        Me.AnalogValueDisplay3.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay3.KeypadText = Nothing
        Me.AnalogValueDisplay3.KeypadWidth = 300
        Me.AnalogValueDisplay3.Location = New System.Drawing.Point(104, 102)
        Me.AnalogValueDisplay3.MaximumSize = New System.Drawing.Size(58, 18)
        Me.AnalogValueDisplay3.Name = "AnalogValueDisplay3"
        Me.AnalogValueDisplay3.NumericFormat = Nothing
        Me.AnalogValueDisplay3.PLCAddressKeypad = ""
        Me.AnalogValueDisplay3.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay3.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay3.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay3.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay3.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay3.ShowValue = True
        Me.AnalogValueDisplay3.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay3.TabIndex = 80
        Me.AnalogValueDisplay3.Text = "0000"
        Me.AnalogValueDisplay3.Value = "0000"
        Me.AnalogValueDisplay3.ValueLimitLower = 0R
        Me.AnalogValueDisplay3.ValueLimitUpper = 65535.0R
        Me.AnalogValueDisplay3.ValuePrefix = Nothing
        Me.AnalogValueDisplay3.ValueSuffix = Nothing
        Me.AnalogValueDisplay3.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay4
        '
        Me.AnalogValueDisplay4.AutoSize = True
        Me.AnalogValueDisplay4.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay4.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay4.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay4.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay4.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay4.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay4.KeypadMaxValue = 0R
        Me.AnalogValueDisplay4.KeypadMinValue = 0R
        Me.AnalogValueDisplay4.KeypadPasscode = Nothing
        Me.AnalogValueDisplay4.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay4.KeypadText = Nothing
        Me.AnalogValueDisplay4.KeypadWidth = 300
        Me.AnalogValueDisplay4.Location = New System.Drawing.Point(200, 274)
        Me.AnalogValueDisplay4.MaximumSize = New System.Drawing.Size(93, 18)
        Me.AnalogValueDisplay4.Name = "AnalogValueDisplay4"
        Me.AnalogValueDisplay4.NumericFormat = Nothing
        Me.AnalogValueDisplay4.PLCAddressKeypad = ""
        Me.AnalogValueDisplay4.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay4.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay4.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay4.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay4.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay4.ShowValue = True
        Me.AnalogValueDisplay4.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay4.TabIndex = 81
        Me.AnalogValueDisplay4.Text = "0000"
        Me.AnalogValueDisplay4.Value = "0000"
        Me.AnalogValueDisplay4.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay4.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay4.ValuePrefix = Nothing
        Me.AnalogValueDisplay4.ValueSuffix = Nothing
        Me.AnalogValueDisplay4.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay5
        '
        Me.AnalogValueDisplay5.AutoSize = True
        Me.AnalogValueDisplay5.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay5.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay5.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay5.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay5.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay5.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay5.KeypadMaxValue = 0R
        Me.AnalogValueDisplay5.KeypadMinValue = 0R
        Me.AnalogValueDisplay5.KeypadPasscode = Nothing
        Me.AnalogValueDisplay5.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay5.KeypadText = Nothing
        Me.AnalogValueDisplay5.KeypadWidth = 300
        Me.AnalogValueDisplay5.Location = New System.Drawing.Point(428, 443)
        Me.AnalogValueDisplay5.MaximumSize = New System.Drawing.Size(58, 18)
        Me.AnalogValueDisplay5.Name = "AnalogValueDisplay5"
        Me.AnalogValueDisplay5.NumericFormat = Nothing
        Me.AnalogValueDisplay5.PLCAddressKeypad = ""
        Me.AnalogValueDisplay5.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay5.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay5.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay5.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay5.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay5.ShowValue = True
        Me.AnalogValueDisplay5.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay5.TabIndex = 82
        Me.AnalogValueDisplay5.Text = "0000"
        Me.AnalogValueDisplay5.Value = "0000"
        Me.AnalogValueDisplay5.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay5.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay5.ValuePrefix = Nothing
        Me.AnalogValueDisplay5.ValueSuffix = Nothing
        Me.AnalogValueDisplay5.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay6
        '
        Me.AnalogValueDisplay6.AutoSize = True
        Me.AnalogValueDisplay6.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay6.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay6.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay6.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay6.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay6.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay6.KeypadMaxValue = 0R
        Me.AnalogValueDisplay6.KeypadMinValue = 0R
        Me.AnalogValueDisplay6.KeypadPasscode = Nothing
        Me.AnalogValueDisplay6.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay6.KeypadText = Nothing
        Me.AnalogValueDisplay6.KeypadWidth = 300
        Me.AnalogValueDisplay6.Location = New System.Drawing.Point(522, 439)
        Me.AnalogValueDisplay6.MaximumSize = New System.Drawing.Size(93, 18)
        Me.AnalogValueDisplay6.Name = "AnalogValueDisplay6"
        Me.AnalogValueDisplay6.NumericFormat = Nothing
        Me.AnalogValueDisplay6.PLCAddressKeypad = ""
        Me.AnalogValueDisplay6.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay6.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay6.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay6.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay6.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay6.ShowValue = True
        Me.AnalogValueDisplay6.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay6.TabIndex = 83
        Me.AnalogValueDisplay6.Text = "0000"
        Me.AnalogValueDisplay6.Value = "0000"
        Me.AnalogValueDisplay6.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay6.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay6.ValuePrefix = Nothing
        Me.AnalogValueDisplay6.ValueSuffix = Nothing
        Me.AnalogValueDisplay6.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay7
        '
        Me.AnalogValueDisplay7.AutoSize = True
        Me.AnalogValueDisplay7.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay7.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay7.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay7.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay7.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay7.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay7.KeypadMaxValue = 0R
        Me.AnalogValueDisplay7.KeypadMinValue = 0R
        Me.AnalogValueDisplay7.KeypadPasscode = Nothing
        Me.AnalogValueDisplay7.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay7.KeypadText = Nothing
        Me.AnalogValueDisplay7.KeypadWidth = 300
        Me.AnalogValueDisplay7.Location = New System.Drawing.Point(531, 132)
        Me.AnalogValueDisplay7.MaximumSize = New System.Drawing.Size(93, 18)
        Me.AnalogValueDisplay7.Name = "AnalogValueDisplay7"
        Me.AnalogValueDisplay7.NumericFormat = Nothing
        Me.AnalogValueDisplay7.PLCAddressKeypad = ""
        Me.AnalogValueDisplay7.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay7.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay7.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay7.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay7.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay7.ShowValue = True
        Me.AnalogValueDisplay7.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay7.TabIndex = 84
        Me.AnalogValueDisplay7.Text = "0000"
        Me.AnalogValueDisplay7.Value = "0000"
        Me.AnalogValueDisplay7.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay7.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay7.ValuePrefix = Nothing
        Me.AnalogValueDisplay7.ValueSuffix = Nothing
        Me.AnalogValueDisplay7.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'AnalogValueDisplay8
        '
        Me.AnalogValueDisplay8.AutoSize = True
        Me.AnalogValueDisplay8.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay8.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay8.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay8.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay8.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay8.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay8.KeypadMaxValue = 0R
        Me.AnalogValueDisplay8.KeypadMinValue = 0R
        Me.AnalogValueDisplay8.KeypadPasscode = Nothing
        Me.AnalogValueDisplay8.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay8.KeypadText = Nothing
        Me.AnalogValueDisplay8.KeypadWidth = 300
        Me.AnalogValueDisplay8.Location = New System.Drawing.Point(463, 48)
        Me.AnalogValueDisplay8.MaximumSize = New System.Drawing.Size(58, 18)
        Me.AnalogValueDisplay8.Name = "AnalogValueDisplay8"
        Me.AnalogValueDisplay8.NumericFormat = Nothing
        Me.AnalogValueDisplay8.PLCAddressKeypad = ""
        Me.AnalogValueDisplay8.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay8.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay8.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay8.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay8.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay8.ShowValue = True
        Me.AnalogValueDisplay8.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay8.TabIndex = 85
        Me.AnalogValueDisplay8.Text = "0000"
        Me.AnalogValueDisplay8.Value = "0000"
        Me.AnalogValueDisplay8.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay8.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay8.ValuePrefix = Nothing
        Me.AnalogValueDisplay8.ValueSuffix = Nothing
        Me.AnalogValueDisplay8.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'DigitalPanelMeter1
        '
        Me.DigitalPanelMeter1.BackColor = System.Drawing.Color.Black
        Me.DigitalPanelMeter1.ComComponent = Me.ModbusTCPCom1
        Me.DigitalPanelMeter1.DecimalPosition = 0
        Me.DigitalPanelMeter1.ForeColor = System.Drawing.Color.LightGray
        Me.DigitalPanelMeter1.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.DigitalPanelMeter1.KeypadMaxValue = 0R
        Me.DigitalPanelMeter1.KeypadMinValue = 0R
        Me.DigitalPanelMeter1.KeypadScaleFactor = 1.0R
        Me.DigitalPanelMeter1.KeypadText = Nothing
        Me.DigitalPanelMeter1.KeypadWidth = 300
        Me.DigitalPanelMeter1.Location = New System.Drawing.Point(447, 208)
        Me.DigitalPanelMeter1.Name = "DigitalPanelMeter1"
        Me.DigitalPanelMeter1.NumberOfDigits = 5
        Me.DigitalPanelMeter1.PLCAddressKeypad = ""
        Me.DigitalPanelMeter1.PLCAddressValue = "41045"
        Me.DigitalPanelMeter1.Resolution = New Decimal(New Integer() {1, 0, 0, 0})
        Me.DigitalPanelMeter1.Size = New System.Drawing.Size(94, 41)
        Me.DigitalPanelMeter1.TabIndex = 95
        Me.DigitalPanelMeter1.Value = 0R
        Me.DigitalPanelMeter1.ValueScaleFactor = New Decimal(New Integer() {916, 0, 0, 262144})
        Me.DigitalPanelMeter1.ValueScaleOffset = New Decimal(New Integer() {0, 0, 0, 0})
        '
        'Pipe8
        '
        Me.Pipe8.BackColor = System.Drawing.Color.Black
        Me.Pipe8.ComComponent = Me.ModbusTCPCom1
        Me.Pipe8.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Elbow
        Me.Pipe8.ForeColor = System.Drawing.Color.White
        Me.Pipe8.HighlightColor = System.Drawing.Color.Red
        Me.Pipe8.HighlightKeyCharacter = "!"
        Me.Pipe8.KeypadText = Nothing
        Me.Pipe8.Location = New System.Drawing.Point(410, 104)
        Me.Pipe8.Name = "Pipe8"
        Me.Pipe8.NumericFormat = Nothing
        Me.Pipe8.PLCAddressKeypad = ""
        Me.Pipe8.PLCAddressRotate = ""
        Me.Pipe8.PLCAddressText = ""
        Me.Pipe8.PLCAddressVisible = ""
        Me.Pipe8.Rotation = System.Drawing.RotateFlipType.Rotate270FlipXY
        Me.Pipe8.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe8.Size = New System.Drawing.Size(50, 50)
        Me.Pipe8.TabIndex = 98
        Me.Pipe8.TextPrefix = Nothing
        Me.Pipe8.TextSuffix = Nothing
        '
        'Pipe5
        '
        Me.Pipe5.BackColor = System.Drawing.Color.Black
        Me.Pipe5.ComComponent = Me.ModbusTCPCom1
        Me.Pipe5.Fitting = MfgControl.AdvancedHMI.Controls.Pipe.FittingType.Elbow
        Me.Pipe5.ForeColor = System.Drawing.Color.White
        Me.Pipe5.HighlightColor = System.Drawing.Color.Red
        Me.Pipe5.HighlightKeyCharacter = "!"
        Me.Pipe5.KeypadText = Nothing
        Me.Pipe5.Location = New System.Drawing.Point(363, 385)
        Me.Pipe5.Name = "Pipe5"
        Me.Pipe5.NumericFormat = Nothing
        Me.Pipe5.PLCAddressKeypad = ""
        Me.Pipe5.PLCAddressRotate = ""
        Me.Pipe5.PLCAddressText = ""
        Me.Pipe5.PLCAddressVisible = ""
        Me.Pipe5.Rotation = System.Drawing.RotateFlipType.RotateNoneFlipNone
        Me.Pipe5.ScaleFactor = New Decimal(New Integer() {1, 0, 0, 0})
        Me.Pipe5.Size = New System.Drawing.Size(55, 55)
        Me.Pipe5.TabIndex = 100
        Me.Pipe5.TextPrefix = Nothing
        Me.Pipe5.TextSuffix = Nothing
        '
        'BasicLabel6
        '
        Me.BasicLabel6.AutoSize = True
        Me.BasicLabel6.BackColor = System.Drawing.Color.Black
        Me.BasicLabel6.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel6.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel6.DisplayAsTime = False
        Me.BasicLabel6.ForeColor = System.Drawing.Color.White
        Me.BasicLabel6.Highlight = False
        Me.BasicLabel6.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel6.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel6.HighlightKeyCharacter = "!"
        Me.BasicLabel6.InterpretValueAsBCD = False
        Me.BasicLabel6.KeypadAlphaNumeric = False
        Me.BasicLabel6.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel6.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel6.KeypadMaxValue = 0R
        Me.BasicLabel6.KeypadMinValue = 0R
        Me.BasicLabel6.KeypadScaleFactor = 1.0R
        Me.BasicLabel6.KeypadShowCurrentValue = False
        Me.BasicLabel6.KeypadText = Nothing
        Me.BasicLabel6.KeypadWidth = 300
        Me.BasicLabel6.Location = New System.Drawing.Point(155, 102)
        Me.BasicLabel6.Name = "BasicLabel6"
        Me.BasicLabel6.NumericFormat = Nothing
        Me.BasicLabel6.PLCAddressKeypad = ""
        Me.BasicLabel6.PollRate = 0
        Me.BasicLabel6.Size = New System.Drawing.Size(22, 18)
        Me.BasicLabel6.TabIndex = 101
        Me.BasicLabel6.Text = "%"
        Me.BasicLabel6.Value = "%"
        Me.BasicLabel6.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel6.ValueLeftPadLength = 0
        Me.BasicLabel6.ValuePrefix = Nothing
        Me.BasicLabel6.ValueScaleFactor = 1.0R
        Me.BasicLabel6.ValueSuffix = Nothing
        Me.BasicLabel6.ValueToSubtractFrom = 0!
        '
        'BasicLabel7
        '
        Me.BasicLabel7.AutoSize = True
        Me.BasicLabel7.BackColor = System.Drawing.Color.Black
        Me.BasicLabel7.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel7.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel7.DisplayAsTime = False
        Me.BasicLabel7.ForeColor = System.Drawing.Color.White
        Me.BasicLabel7.Highlight = False
        Me.BasicLabel7.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel7.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel7.HighlightKeyCharacter = "!"
        Me.BasicLabel7.InterpretValueAsBCD = False
        Me.BasicLabel7.KeypadAlphaNumeric = False
        Me.BasicLabel7.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel7.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel7.KeypadMaxValue = 0R
        Me.BasicLabel7.KeypadMinValue = 0R
        Me.BasicLabel7.KeypadScaleFactor = 1.0R
        Me.BasicLabel7.KeypadShowCurrentValue = False
        Me.BasicLabel7.KeypadText = Nothing
        Me.BasicLabel7.KeypadWidth = 300
        Me.BasicLabel7.Location = New System.Drawing.Point(478, 443)
        Me.BasicLabel7.Name = "BasicLabel7"
        Me.BasicLabel7.NumericFormat = Nothing
        Me.BasicLabel7.PLCAddressKeypad = ""
        Me.BasicLabel7.PollRate = 0
        Me.BasicLabel7.Size = New System.Drawing.Size(22, 18)
        Me.BasicLabel7.TabIndex = 102
        Me.BasicLabel7.Text = "%"
        Me.BasicLabel7.Value = "%"
        Me.BasicLabel7.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel7.ValueLeftPadLength = 0
        Me.BasicLabel7.ValuePrefix = Nothing
        Me.BasicLabel7.ValueScaleFactor = 1.0R
        Me.BasicLabel7.ValueSuffix = Nothing
        Me.BasicLabel7.ValueToSubtractFrom = 0!
        '
        'BasicLabel8
        '
        Me.BasicLabel8.AutoSize = True
        Me.BasicLabel8.BackColor = System.Drawing.Color.Black
        Me.BasicLabel8.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel8.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel8.DisplayAsTime = False
        Me.BasicLabel8.ForeColor = System.Drawing.Color.White
        Me.BasicLabel8.Highlight = False
        Me.BasicLabel8.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel8.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel8.HighlightKeyCharacter = "!"
        Me.BasicLabel8.InterpretValueAsBCD = False
        Me.BasicLabel8.KeypadAlphaNumeric = False
        Me.BasicLabel8.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel8.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel8.KeypadMaxValue = 0R
        Me.BasicLabel8.KeypadMinValue = 0R
        Me.BasicLabel8.KeypadScaleFactor = 1.0R
        Me.BasicLabel8.KeypadShowCurrentValue = False
        Me.BasicLabel8.KeypadText = Nothing
        Me.BasicLabel8.KeypadWidth = 300
        Me.BasicLabel8.Location = New System.Drawing.Point(513, 48)
        Me.BasicLabel8.Name = "BasicLabel8"
        Me.BasicLabel8.NumericFormat = Nothing
        Me.BasicLabel8.PLCAddressKeypad = ""
        Me.BasicLabel8.PollRate = 0
        Me.BasicLabel8.Size = New System.Drawing.Size(22, 18)
        Me.BasicLabel8.TabIndex = 103
        Me.BasicLabel8.Text = "%"
        Me.BasicLabel8.Value = "%"
        Me.BasicLabel8.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel8.ValueLeftPadLength = 0
        Me.BasicLabel8.ValuePrefix = Nothing
        Me.BasicLabel8.ValueScaleFactor = 1.0R
        Me.BasicLabel8.ValueSuffix = Nothing
        Me.BasicLabel8.ValueToSubtractFrom = 0!
        '
        'BasicLabel9
        '
        Me.BasicLabel9.AutoSize = True
        Me.BasicLabel9.BackColor = System.Drawing.Color.Black
        Me.BasicLabel9.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel9.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel9.DisplayAsTime = False
        Me.BasicLabel9.ForeColor = System.Drawing.Color.White
        Me.BasicLabel9.Highlight = False
        Me.BasicLabel9.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel9.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel9.HighlightKeyCharacter = "!"
        Me.BasicLabel9.InterpretValueAsBCD = False
        Me.BasicLabel9.KeypadAlphaNumeric = False
        Me.BasicLabel9.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel9.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel9.KeypadMaxValue = 0R
        Me.BasicLabel9.KeypadMinValue = 0R
        Me.BasicLabel9.KeypadScaleFactor = 1.0R
        Me.BasicLabel9.KeypadShowCurrentValue = False
        Me.BasicLabel9.KeypadText = Nothing
        Me.BasicLabel9.KeypadWidth = 300
        Me.BasicLabel9.Location = New System.Drawing.Point(154, 274)
        Me.BasicLabel9.Name = "BasicLabel9"
        Me.BasicLabel9.NumericFormat = Nothing
        Me.BasicLabel9.PLCAddressKeypad = ""
        Me.BasicLabel9.PollRate = 0
        Me.BasicLabel9.Size = New System.Drawing.Size(22, 18)
        Me.BasicLabel9.TabIndex = 104
        Me.BasicLabel9.Text = "%"
        Me.BasicLabel9.Value = "%"
        Me.BasicLabel9.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel9.ValueLeftPadLength = 0
        Me.BasicLabel9.ValuePrefix = Nothing
        Me.BasicLabel9.ValueScaleFactor = 1.0R
        Me.BasicLabel9.ValueSuffix = Nothing
        Me.BasicLabel9.ValueToSubtractFrom = 0!
        '
        'BasicLabel10
        '
        Me.BasicLabel10.AutoSize = True
        Me.BasicLabel10.BackColor = System.Drawing.Color.Black
        Me.BasicLabel10.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel10.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel10.DisplayAsTime = False
        Me.BasicLabel10.ForeColor = System.Drawing.Color.White
        Me.BasicLabel10.Highlight = False
        Me.BasicLabel10.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel10.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel10.HighlightKeyCharacter = "!"
        Me.BasicLabel10.InterpretValueAsBCD = False
        Me.BasicLabel10.KeypadAlphaNumeric = False
        Me.BasicLabel10.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel10.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel10.KeypadMaxValue = 0R
        Me.BasicLabel10.KeypadMinValue = 0R
        Me.BasicLabel10.KeypadScaleFactor = 1.0R
        Me.BasicLabel10.KeypadShowCurrentValue = False
        Me.BasicLabel10.KeypadText = Nothing
        Me.BasicLabel10.KeypadWidth = 300
        Me.BasicLabel10.Location = New System.Drawing.Point(250, 274)
        Me.BasicLabel10.Name = "BasicLabel10"
        Me.BasicLabel10.NumericFormat = Nothing
        Me.BasicLabel10.PLCAddressKeypad = ""
        Me.BasicLabel10.PollRate = 0
        Me.BasicLabel10.Size = New System.Drawing.Size(57, 18)
        Me.BasicLabel10.TabIndex = 105
        Me.BasicLabel10.Text = " kmol/h"
        Me.BasicLabel10.Value = " kmol/h"
        Me.BasicLabel10.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel10.ValueLeftPadLength = 0
        Me.BasicLabel10.ValuePrefix = Nothing
        Me.BasicLabel10.ValueScaleFactor = 1.0R
        Me.BasicLabel10.ValueSuffix = Nothing
        Me.BasicLabel10.ValueToSubtractFrom = 0!
        '
        'BasicLabel11
        '
        Me.BasicLabel11.AutoSize = True
        Me.BasicLabel11.BackColor = System.Drawing.Color.Black
        Me.BasicLabel11.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel11.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel11.DisplayAsTime = False
        Me.BasicLabel11.ForeColor = System.Drawing.Color.White
        Me.BasicLabel11.Highlight = False
        Me.BasicLabel11.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel11.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel11.HighlightKeyCharacter = "!"
        Me.BasicLabel11.InterpretValueAsBCD = False
        Me.BasicLabel11.KeypadAlphaNumeric = False
        Me.BasicLabel11.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel11.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel11.KeypadMaxValue = 0R
        Me.BasicLabel11.KeypadMinValue = 0R
        Me.BasicLabel11.KeypadScaleFactor = 1.0R
        Me.BasicLabel11.KeypadShowCurrentValue = False
        Me.BasicLabel11.KeypadText = Nothing
        Me.BasicLabel11.KeypadWidth = 300
        Me.BasicLabel11.Location = New System.Drawing.Point(250, 189)
        Me.BasicLabel11.Name = "BasicLabel11"
        Me.BasicLabel11.NumericFormat = Nothing
        Me.BasicLabel11.PLCAddressKeypad = ""
        Me.BasicLabel11.PollRate = 0
        Me.BasicLabel11.Size = New System.Drawing.Size(57, 18)
        Me.BasicLabel11.TabIndex = 106
        Me.BasicLabel11.Text = " kmol/h"
        Me.BasicLabel11.Value = " kmol/h"
        Me.BasicLabel11.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel11.ValueLeftPadLength = 0
        Me.BasicLabel11.ValuePrefix = Nothing
        Me.BasicLabel11.ValueScaleFactor = 1.0R
        Me.BasicLabel11.ValueSuffix = Nothing
        Me.BasicLabel11.ValueToSubtractFrom = 0!
        '
        'BasicLabel12
        '
        Me.BasicLabel12.AutoSize = True
        Me.BasicLabel12.BackColor = System.Drawing.Color.Black
        Me.BasicLabel12.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel12.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel12.DisplayAsTime = False
        Me.BasicLabel12.ForeColor = System.Drawing.Color.White
        Me.BasicLabel12.Highlight = False
        Me.BasicLabel12.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel12.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel12.HighlightKeyCharacter = "!"
        Me.BasicLabel12.InterpretValueAsBCD = False
        Me.BasicLabel12.KeypadAlphaNumeric = False
        Me.BasicLabel12.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel12.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel12.KeypadMaxValue = 0R
        Me.BasicLabel12.KeypadMinValue = 0R
        Me.BasicLabel12.KeypadScaleFactor = 1.0R
        Me.BasicLabel12.KeypadShowCurrentValue = False
        Me.BasicLabel12.KeypadText = Nothing
        Me.BasicLabel12.KeypadWidth = 300
        Me.BasicLabel12.Location = New System.Drawing.Point(577, 439)
        Me.BasicLabel12.Name = "BasicLabel12"
        Me.BasicLabel12.NumericFormat = Nothing
        Me.BasicLabel12.PLCAddressKeypad = ""
        Me.BasicLabel12.PollRate = 0
        Me.BasicLabel12.Size = New System.Drawing.Size(57, 18)
        Me.BasicLabel12.TabIndex = 107
        Me.BasicLabel12.Text = " kmol/h"
        Me.BasicLabel12.Value = " kmol/h"
        Me.BasicLabel12.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel12.ValueLeftPadLength = 0
        Me.BasicLabel12.ValuePrefix = Nothing
        Me.BasicLabel12.ValueScaleFactor = 1.0R
        Me.BasicLabel12.ValueSuffix = Nothing
        Me.BasicLabel12.ValueToSubtractFrom = 0!
        '
        'BasicLabel13
        '
        Me.BasicLabel13.AutoSize = True
        Me.BasicLabel13.BackColor = System.Drawing.Color.Black
        Me.BasicLabel13.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel13.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel13.DisplayAsTime = False
        Me.BasicLabel13.ForeColor = System.Drawing.Color.White
        Me.BasicLabel13.Highlight = False
        Me.BasicLabel13.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel13.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel13.HighlightKeyCharacter = "!"
        Me.BasicLabel13.InterpretValueAsBCD = False
        Me.BasicLabel13.KeypadAlphaNumeric = False
        Me.BasicLabel13.KeypadFont = New System.Drawing.Font("Arial", 10.0!)
        Me.BasicLabel13.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel13.KeypadMaxValue = 0R
        Me.BasicLabel13.KeypadMinValue = 0R
        Me.BasicLabel13.KeypadScaleFactor = 1.0R
        Me.BasicLabel13.KeypadShowCurrentValue = False
        Me.BasicLabel13.KeypadText = Nothing
        Me.BasicLabel13.KeypadWidth = 300
        Me.BasicLabel13.Location = New System.Drawing.Point(583, 132)
        Me.BasicLabel13.Name = "BasicLabel13"
        Me.BasicLabel13.NumericFormat = Nothing
        Me.BasicLabel13.PLCAddressKeypad = ""
        Me.BasicLabel13.PollRate = 0
        Me.BasicLabel13.Size = New System.Drawing.Size(57, 18)
        Me.BasicLabel13.TabIndex = 108
        Me.BasicLabel13.Text = " kmol/h"
        Me.BasicLabel13.Value = " kmol/h"
        Me.BasicLabel13.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel13.ValueLeftPadLength = 0
        Me.BasicLabel13.ValuePrefix = Nothing
        Me.BasicLabel13.ValueScaleFactor = 1.0R
        Me.BasicLabel13.ValueSuffix = Nothing
        Me.BasicLabel13.ValueToSubtractFrom = 0!
        '
        'DataSubscriber1
        '
        Me.DataSubscriber1.ComComponent = Me.ModbusTCPCom1
        Me.DataSubscriber1.PLCAddressValue = CType(resources.GetObject("DataSubscriber1.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.DataSubscriber1.Value = Nothing
        '
        'AnalogValueDisplay9
        '
        Me.AnalogValueDisplay9.AutoSize = True
        Me.AnalogValueDisplay9.ComComponent = Me.ModbusTCPCom1
        Me.AnalogValueDisplay9.ForeColor = System.Drawing.Color.White
        Me.AnalogValueDisplay9.ForeColorInLimits = System.Drawing.Color.White
        Me.AnalogValueDisplay9.ForeColorOverLimit = System.Drawing.Color.Red
        Me.AnalogValueDisplay9.ForeColorUnderLimit = System.Drawing.Color.Yellow
        Me.AnalogValueDisplay9.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.AnalogValueDisplay9.KeypadMaxValue = 0R
        Me.AnalogValueDisplay9.KeypadMinValue = 0R
        Me.AnalogValueDisplay9.KeypadPasscode = Nothing
        Me.AnalogValueDisplay9.KeypadScaleFactor = 1.0R
        Me.AnalogValueDisplay9.KeypadText = Nothing
        Me.AnalogValueDisplay9.KeypadWidth = 300
        Me.AnalogValueDisplay9.Location = New System.Drawing.Point(669, 526)
        Me.AnalogValueDisplay9.MaximumSize = New System.Drawing.Size(58, 18)
        Me.AnalogValueDisplay9.Name = "AnalogValueDisplay9"
        Me.AnalogValueDisplay9.NumericFormat = Nothing
        Me.AnalogValueDisplay9.PLCAddressKeypad = ""
        Me.AnalogValueDisplay9.PLCAddressValue = CType(resources.GetObject("AnalogValueDisplay9.PLCAddressValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.AnalogValueDisplay9.PLCAddressValueLimitLower = Nothing
        Me.AnalogValueDisplay9.PLCAddressValueLimitUpper = Nothing
        Me.AnalogValueDisplay9.PLCAddressVisible = Nothing
        Me.AnalogValueDisplay9.ShowValue = True
        Me.AnalogValueDisplay9.Size = New System.Drawing.Size(44, 18)
        Me.AnalogValueDisplay9.TabIndex = 109
        Me.AnalogValueDisplay9.Text = "0000"
        Me.AnalogValueDisplay9.Value = "0000"
        Me.AnalogValueDisplay9.ValueLimitLower = -999999.0R
        Me.AnalogValueDisplay9.ValueLimitUpper = 999999.0R
        Me.AnalogValueDisplay9.ValuePrefix = Nothing
        Me.AnalogValueDisplay9.ValueSuffix = Nothing
        Me.AnalogValueDisplay9.VisibleControl = AdvancedHMIControls.AnalogValueDisplay.VisibleControlEnum.Always
        '
        'KeyboardInput1
        '
        Me.KeyboardInput1.ClearAfterEnterKey = True
        Me.KeyboardInput1.ComComponent = Me.ModbusTCPCom1
        Me.KeyboardInput1.GetFocusMatchValue = 1
        Me.KeyboardInput1.GetFocusValue = 0
        Me.KeyboardInput1.Location = New System.Drawing.Point(563, 523)
        Me.KeyboardInput1.Name = "KeyboardInput1"
        Me.KeyboardInput1.PLCAddressGetFocusValue = Nothing
        Me.KeyboardInput1.PLCAddressWriteValue = CType(resources.GetObject("KeyboardInput1.PLCAddressWriteValue"), MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Me.KeyboardInput1.Size = New System.Drawing.Size(100, 26)
        Me.KeyboardInput1.TabIndex = 112
        '
        'PilotLight1
        '
        Me.PilotLight1.Blink = False
        Me.PilotLight1.ComComponent = Me.ModbusTCPCom1
        Me.PilotLight1.LegendPlate = MfgControl.AdvancedHMI.Controls.PilotLight.LegendPlates.Large
        Me.PilotLight1.LightColor = MfgControl.AdvancedHMI.Controls.PilotLight.LightColors.Green
        Me.PilotLight1.LightColorOff = MfgControl.AdvancedHMI.Controls.PilotLight.LightColors.White
        Me.PilotLight1.Location = New System.Drawing.Point(157, 426)
        Me.PilotLight1.Name = "PilotLight1"
        Me.PilotLight1.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.SetTrue
        Me.PilotLight1.PLCAddressClick = "00001"
        Me.PilotLight1.PLCAddressText = ""
        Me.PilotLight1.PLCAddressValue = "00001"
        Me.PilotLight1.PLCAddressVisible = ""
        Me.PilotLight1.Size = New System.Drawing.Size(75, 110)
        Me.PilotLight1.TabIndex = 113
        Me.PilotLight1.Text = "Run"
        Me.PilotLight1.Value = True
        Me.PilotLight1.ValueToWrite = 1
        '
        'PilotLight2
        '
        Me.PilotLight2.Blink = False
        Me.PilotLight2.ComComponent = Me.ModbusTCPCom1
        Me.PilotLight2.LegendPlate = MfgControl.AdvancedHMI.Controls.PilotLight.LegendPlates.Large
        Me.PilotLight2.LightColor = MfgControl.AdvancedHMI.Controls.PilotLight.LightColors.White
        Me.PilotLight2.LightColorOff = MfgControl.AdvancedHMI.Controls.PilotLight.LightColors.Red
        Me.PilotLight2.Location = New System.Drawing.Point(52, 426)
        Me.PilotLight2.Name = "PilotLight2"
        Me.PilotLight2.OutputType = MfgControl.AdvancedHMI.Controls.OutputType.SetFalse
        Me.PilotLight2.PLCAddressClick = "00001"
        Me.PilotLight2.PLCAddressText = ""
        Me.PilotLight2.PLCAddressValue = "00001"
        Me.PilotLight2.PLCAddressVisible = ""
        Me.PilotLight2.Size = New System.Drawing.Size(75, 110)
        Me.PilotLight2.TabIndex = 114
        Me.PilotLight2.Text = "Stop"
        Me.PilotLight2.Value = False
        Me.PilotLight2.ValueToWrite = 0
        '
        'BasicLabel14
        '
        Me.BasicLabel14.AutoSize = True
        Me.BasicLabel14.BackColor = System.Drawing.Color.Black
        Me.BasicLabel14.BooleanDisplay = AdvancedHMIControls.BasicLabel.BooleanDisplayOption.TrueFalse
        Me.BasicLabel14.ComComponent = Me.ModbusTCPCom1
        Me.BasicLabel14.DisplayAsTime = False
        Me.BasicLabel14.ForeColor = System.Drawing.Color.White
        Me.BasicLabel14.Highlight = False
        Me.BasicLabel14.HighlightColor = System.Drawing.Color.Red
        Me.BasicLabel14.HighlightForeColor = System.Drawing.Color.White
        Me.BasicLabel14.HighlightKeyCharacter = "!"
        Me.BasicLabel14.InterpretValueAsBCD = False
        Me.BasicLabel14.KeypadAlphaNumeric = False
        Me.BasicLabel14.KeypadFont = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BasicLabel14.KeypadFontColor = System.Drawing.Color.WhiteSmoke
        Me.BasicLabel14.KeypadMaxValue = 0R
        Me.BasicLabel14.KeypadMinValue = 0R
        Me.BasicLabel14.KeypadScaleFactor = 1.0R
        Me.BasicLabel14.KeypadShowCurrentValue = False
        Me.BasicLabel14.KeypadText = Nothing
        Me.BasicLabel14.KeypadWidth = 300
        Me.BasicLabel14.Location = New System.Drawing.Point(522, 502)
        Me.BasicLabel14.Name = "BasicLabel14"
        Me.BasicLabel14.NumericFormat = Nothing
        Me.BasicLabel14.PLCAddressKeypad = ""
        Me.BasicLabel14.PollRate = 0
        Me.BasicLabel14.Size = New System.Drawing.Size(218, 18)
        Me.BasicLabel14.TabIndex = 115
        Me.BasicLabel14.Text = "Product Flow Setpoint [kmol/h]"
        Me.BasicLabel14.Value = "Product Flow Setpoint [kmol/h]"
        Me.BasicLabel14.ValueLeftPadCharacter = Global.Microsoft.VisualBasic.ChrW(32)
        Me.BasicLabel14.ValueLeftPadLength = 0
        Me.BasicLabel14.ValuePrefix = Nothing
        Me.BasicLabel14.ValueScaleFactor = 1.0R
        Me.BasicLabel14.ValueSuffix = Nothing
        Me.BasicLabel14.ValueToSubtractFrom = 0!
        '
        'MainForm
        '
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.BasicLabel14)
        Me.Controls.Add(Me.PilotLight2)
        Me.Controls.Add(Me.PilotLight1)
        Me.Controls.Add(Me.KeyboardInput1)
        Me.Controls.Add(Me.AnalogValueDisplay9)
        Me.Controls.Add(Me.BasicLabel13)
        Me.Controls.Add(Me.BasicLabel12)
        Me.Controls.Add(Me.BasicLabel11)
        Me.Controls.Add(Me.BasicLabel10)
        Me.Controls.Add(Me.BasicLabel9)
        Me.Controls.Add(Me.BasicLabel8)
        Me.Controls.Add(Me.BasicLabel7)
        Me.Controls.Add(Me.BasicLabel6)
        Me.Controls.Add(Me.Pipe5)
        Me.Controls.Add(Me.Pipe8)
        Me.Controls.Add(Me.PneumaticBallValve4)
        Me.Controls.Add(Me.DigitalPanelMeter1)
        Me.Controls.Add(Me.AnalogValueDisplay8)
        Me.Controls.Add(Me.AnalogValueDisplay7)
        Me.Controls.Add(Me.AnalogValueDisplay6)
        Me.Controls.Add(Me.AnalogValueDisplay5)
        Me.Controls.Add(Me.AnalogValueDisplay4)
        Me.Controls.Add(Me.AnalogValueDisplay3)
        Me.Controls.Add(Me.AnalogValueDisplay2)
        Me.Controls.Add(Me.BasicLabel5)
        Me.Controls.Add(Me.AnalogValueDisplay1)
        Me.Controls.Add(Me.BasicLabel4)
        Me.Controls.Add(Me.BasicLabel3)
        Me.Controls.Add(Me.BasicLabel2)
        Me.Controls.Add(Me.Pipe3)
        Me.Controls.Add(Me.BasicLabel1)
        Me.Controls.Add(Me.PneumaticBallValve3)
        Me.Controls.Add(Me.Tank1)
        Me.Controls.Add(Me.PneumaticBallValve2)
        Me.Controls.Add(Me.Pipe4)
        Me.Controls.Add(Me.PneumaticBallValve1)
        Me.Controls.Add(Me.Pipe2)
        Me.Controls.Add(Me.Pipe1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Pipe6)
        Me.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.ForeColor = System.Drawing.Color.Black
        Me.KeyPreview = True
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Simplified Tennesee Eastman"
        CType(Me.ModbusTCPCom1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataSubscriber1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DF1ComWF1 As AdvancedHMIDrivers.SerialDF1forSLCMicroCom
    Friend WithEvents ForceItemsIntoToolBox1 As MfgControl.AdvancedHMI.Drivers.ForceItemsIntoToolbox
    Friend WithEvents ModbusTCPCom1 As AdvancedHMIDrivers.ModbusTCPCom
    Friend WithEvents Tank1 As AdvancedHMIControls.Tank
    Friend WithEvents Pipe1 As AdvancedHMIControls.Pipe
    Friend WithEvents Pipe2 As AdvancedHMIControls.Pipe
    Friend WithEvents PneumaticBallValve1 As AdvancedHMIControls.PneumaticBallValve
    Friend WithEvents Pipe4 As AdvancedHMIControls.Pipe
    Friend WithEvents PneumaticBallValve2 As AdvancedHMIControls.PneumaticBallValve
    Friend WithEvents PneumaticBallValve3 As AdvancedHMIControls.PneumaticBallValve
    Friend WithEvents Pipe6 As AdvancedHMIControls.Pipe
    Friend WithEvents BasicLabel1 As AdvancedHMIControls.BasicLabel
    Friend WithEvents PneumaticBallValve4 As AdvancedHMIControls.PneumaticBallValve
    Friend WithEvents Pipe3 As AdvancedHMIControls.Pipe
    Friend WithEvents BasicLabel2 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel3 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel4 As AdvancedHMIControls.BasicLabel
    Friend WithEvents AnalogValueDisplay1 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents BasicLabel5 As AdvancedHMIControls.BasicLabel
    Friend WithEvents AnalogValueDisplay2 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay3 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay4 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay5 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay6 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay7 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents AnalogValueDisplay8 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents DigitalPanelMeter1 As AdvancedHMIControls.DigitalPanelMeter
    Friend WithEvents Pipe8 As AdvancedHMIControls.Pipe
    Friend WithEvents Pipe5 As AdvancedHMIControls.Pipe
    Friend WithEvents BasicLabel6 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel7 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel8 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel9 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel10 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel11 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel12 As AdvancedHMIControls.BasicLabel
    Friend WithEvents BasicLabel13 As AdvancedHMIControls.BasicLabel
    Friend WithEvents DataSubscriber1 As AdvancedHMIControls.DataSubscriber
    Friend WithEvents AnalogValueDisplay9 As AdvancedHMIControls.AnalogValueDisplay
    Friend WithEvents KeyboardInput1 As AdvancedHMIControls.KeyboardInput
    Friend WithEvents PilotLight1 As AdvancedHMIControls.PilotLight
    Friend WithEvents PilotLight2 As AdvancedHMIControls.PilotLight
    Friend WithEvents BasicLabel14 As AdvancedHMIControls.BasicLabel
End Class
