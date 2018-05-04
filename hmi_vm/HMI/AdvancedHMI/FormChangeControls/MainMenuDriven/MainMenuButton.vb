'**************************************************************************************
'* Main Menu Button
'* 15-NOV-16
'*
'* This button is designed for use with a form that will stay visible at all times
'* To use:
'* 1) Create a new form and give it the name MainMenu
'* 2) Set StartPosition to Manual
'* 3) Set Location to 0,0
'* 4) Set TopMost to True
'* 5) Double click the for to get to the FormLoad event
'* 6) Enter this code:
'*
'Private Sub MainMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'    Dim index As Integer
'    While index < Me.Controls.Count
'        If TypeOf Me.Controls(index) Is MainMenuButton Then
'            If DirectCast(Me.Controls(index), MainMenuButton).OpenOnStartup Then
'                DirectCast(Me.Controls(index), MainMenuButton).PerformClick()
'                Exit While
'            End If
'        End If
'        index += 1
'    End While
'End Sub
'*
'* 7) From the ToolBox, add MainMenu buttons
'* 8) Set the FormToOpen property of each MainMenuButton
'* 9) Determine which form you want as startup and select the button then set the OpenOnStartup property to True
'* 10) In Solution Explorer, Right click the AdvancedHMI project and select Properties
'* 11) Set StartupForm to MainMenu
'*
'* If you desire to use a button to exit the application:
'* 1) From the Toolbox group AllWindowsForms, add a Button to the MainMenu form
'* 2) Double click the button and enter this code:
'*
'Private Sub ExitButton_Click(sender As Object, e As EventArgs) Handles Button1.Click
'    Dim index As Integer = 0
'    While index < My.Application.OpenForms.Count
'        If My.Application.OpenForms(index) IsNot Me Then
'            My.Application.OpenForms(index).Close()
'        End If
'        index += 1
'    End While

'    Me.Close()
'End Sub
'*
'*************************************************************************************************************************

Public Class MainMenuButton
    Inherits Button
    Implements IDisposable

    Dim ht As Windows.Forms.Timer


#Region "Properties"
    Private m_Passcode As String
    Public Property Passcode As String
        Get
            Return m_Passcode
        End Get
        Set(value As String)
            m_Passcode = value
        End Set
    End Property

    Private m_KeypadWidth As Integer = 300
    Public Property KeypadWidth As Integer
        Get
            Return m_KeypadWidth
        End Get
        Set(value As Integer)
            m_KeypadWidth = value
        End Set
    End Property

    Private m_FormToOpen As Type
    <System.ComponentModel.EditorAttribute(GetType(FormListEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Property FormToOpen() As Type
        Get
            Return m_FormToOpen
        End Get
        Set(ByVal value As Type)
            m_FormToOpen = value
        End Set
    End Property

    Private m_PasswordChar As Boolean
    Public Property PasswordChar As Boolean
        Get
            Return m_PasswordChar
        End Get
        Set(value As Boolean)
            m_PasswordChar = value
        End Set
    End Property

    Private m_OpenOnStartup As Boolean
    Public Property OpenOnStartup As Boolean
        Get
            Return m_OpenOnStartup
        End Get
        Set(value As Boolean)
            m_OpenOnStartup = value
        End Set
    End Property

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
                End If

                m_ComComponent = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private _PLCAddressVisible As String = ""
    <System.ComponentModel.Category("PLC Properties")>
    Public Property PLCAddressVisible() As String
        Get
            Return _PLCAddressVisible
        End Get
        Set(ByVal value As String)
            If _PLCAddressVisible <> value Then
                _PLCAddressVisible = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property
#End Region

#Region "Constructor/Destructor"
    Public Sub New()
        MyBase.New()

        ht = New Windows.Forms.Timer
        ht.Interval = 200
        AddHandler ht.Tick, AddressOf HideForm

        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ForeColor = Color.Black
    End Sub

    '****************************************************************
    '* UserControl overrides dispose to clean up the component list.
    '****************************************************************
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If ht IsNot Nothing Then
                    ht.Dispose()
                End If
                If SubScriptions IsNot Nothing Then
                    SubScriptions.dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
#End Region

#Region "Events"
    Protected Overrides Sub OnClick(e As System.EventArgs)
        If m_FormToOpen IsNot Nothing Then
            If (m_Passcode IsNot Nothing) AndAlso (String.Compare(m_Passcode, "") <> 0) Then
                '* Create a keypad to prompt for passcode
                Dim kpd As New MfgControl.AdvancedHMI.Controls.Keypad(m_KeypadWidth)
                kpd.PasswordChar = m_PasswordChar

                kpd.Text = "Enter pass code to continue"

                kpd.StartPosition = FormStartPosition.CenterParent

                If kpd.ShowDialog() = Windows.Forms.DialogResult.OK And kpd.Value = m_Passcode Then
                    MyBase.OnClick(e)

                    OpenForm()
                End If
            Else
                OpenForm()
            End If
        End If

        MyBase.OnClick(e)
    End Sub

    Private Sub HideAllForms(ByVal excludedForm As Form)
        For i = 0 To My.Application.OpenForms.Count - 1
            If My.Application.OpenForms(i) IsNot excludedForm And My.Application.OpenForms(i) IsNot pf Then
                My.Application.OpenForms(i).Hide()
            End If
        Next
    End Sub


    Private Sub OpenForm()
        If FormToOpen IsNot Nothing Then
            Dim index As Integer
            'Dim f As System.Windows.Forms.FormCollection = My.Application.OpenForms
            'While index < f.Count AndAlso f(index).GetType.Name <> m_FormToOpen.Name
            '    index += 1
            'End While

            'Dim i As Form = DirectCast(Activator.CreateInstance(m_FormToOpen), Form)


            Dim f1 As Form
            '* My.Forms has a property for each form in the Application
            '* This is default instances of the forms
            Dim p() As Reflection.PropertyInfo = My.Forms.GetType().GetProperties

            '* Check to see if the name is a form in the list
            While index < p.Length AndAlso p(index).Name <> m_FormToOpen.Name
                index += 1
            End While


            If index < p.Length Then
                f1 = DirectCast(p(index).GetValue(My.Forms, Nothing), Form)
                f1.Show()
                f1.BringToFront()
                '* Assume it is a left menu panel, so open form just to the right of it
                f1.Location = New Point(Me.Parent.Width, 0)


                '* Keep going up the tree until we find the top level parent
                pf = Parent
                While (pf IsNot Nothing) AndAlso (Not (TypeOf (pf) Is Form))
                    If pf.Parent IsNot Nothing Then
                        pf = pf.Parent
                    Else
                        Exit While
                    End If
                End While


                HideAllForms(f1)

                '    '* Hide the form this event came from
                '    '* It may be embedded in other containers
                '    '*If it is the same form, then do not hide 04-aug-14
                '    If pf IsNot Nothing AndAlso (f1 IsNot pf) Then
                '        '* Delay hiding the previous form so that it transitions busy forms smoother
                '        ht.Start()
                '        HideAllForms(f1)
                '    End If
            End If
        End If
    End Sub

    Dim pf As Object
    Private Sub HideForm(ByVal e As Object, ByVal ef As EventArgs)
        e.stop()
        ht.Enabled = False
        If pf IsNot Nothing Then pf.hide()
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)

        If Not Me.DesignMode Then SubscribeToComDriver()
    End Sub
#End Region


#Region "Subscribing and PLC data receiving"
    Private SubScriptions As AdvancedHMIControls.SubscriptionHandler
    '*******************************************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '* This code will look at properties to find the "PLCAddress" + property name
    '*
    '*******************************************************************************
    Private Sub SubscribeToComDriver()
        If Not DesignMode And IsHandleCreated Then
            '* Create a subscription handler object
            If SubScriptions Is Nothing Then
                SubScriptions = New AdvancedHMIControls.SubscriptionHandler

                AddHandler SubScriptions.DisplayError, AddressOf DisplaySubscribeError
            End If
            SubScriptions.ComComponent = m_ComComponent

            '* Check through the properties looking for PLCAddress***, then see if the suffix matches an existing property
            Dim p() As Reflection.PropertyInfo = Me.GetType().GetProperties

            For i As Integer = 0 To p.Length - 1
                '* Does this property start with "PLCAddress"?
                If p(i).Name.IndexOf("PLCAddress", StringComparison.CurrentCultureIgnoreCase) = 0 Then
                    '* Get the property value
                    Dim PLCAddress As String = p(i).GetValue(Me, Nothing)
                    If (PLCAddress IsNot Nothing) AndAlso (String.Compare(PLCAddress, "") <> 0) Then
                        '* Get the text in the name after PLCAddress
                        Dim PropertyToWrite As String = p(i).Name.Substring(10)
                        Dim j As Integer = 0
                        '* See if there is a corresponding property with the extracted name
                        While j < p.Length AndAlso p(j).Name <> PropertyToWrite
                            j += 1
                        End While

                        '* If the proprty was found, then subscribe to the PLC Address
                        If j < p.Length Then
                            SubScriptions.SubscribeTo(PLCAddress, 1, AddressOf PolledDataReturned, PropertyToWrite, 1, 0)
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As AdvancedHMIControls.SubscriptionHandlerEventArgs)
        If e.PLCComEventArgs.ErrorId = 0 Then
            Try
                '* Write the value to the property that came from the end of the PLCAddress... property name
                Dim pi As System.Reflection.PropertyInfo
                pi = Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet)
                Dim value As Object
                value = Convert.ChangeType(e.PLCComEventArgs.Values(0), Me.GetType().GetProperty(e.SubscriptionDetail.PropertyNameToSet).PropertyType)
                pi.SetValue(Me, value, Nothing)
            Catch ex As Exception
                DisplayError("INVALID VALUE RETURNED!" & e.PLCComEventArgs.Values(0))
            End Try
        Else
            DisplayError("Com Error. " & e.PLCComEventArgs.ErrorMessage)
        End If
    End Sub

    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
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
            ErrorDisplayTime.Interval = 5000
        End If

        '* Save the text to return to
        If Not ErrorDisplayTime.Enabled Then
            OriginalText = Me.Text
        End If

        ErrorDisplayTime.Enabled = True

        MyBase.Text = ErrorMessage
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

'<System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name:="FullTrust")> _
'Public Class FormListEditor
'    Inherits System.Drawing.Design.UITypeEditor

'    Public Sub New()
'    End Sub

'    ' Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
'    ' drop down dialog, or no UI outside of the properties window.
'    Public Overloads Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
'        Return UITypeEditorEditStyle.DropDown
'    End Function

'    ' Displays the UI for value selection.
'    Dim edSvc As IWindowsFormsEditorService
'    Private WithEvents lb As ListBox
'    Private ListOfForms As New List(Of Type)

'    Public Overloads Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As System.IServiceProvider, ByVal value As Object) As Object
'        ' Uses the IWindowsFormsEditorService to display a 
'        ' drop-down UI in the Properties window.
'        edSvc = CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
'        lb = New ListBox

'        'Dim x = My.Forms
'        'For Each mm As Form In My.Forms

'        'Next


'        If (edSvc IsNot Nothing) Then
'            'Reflection.Assembly.GetEntryAssembly()
'            Dim MyTypes As Type() = Reflection.Assembly.GetExecutingAssembly.GetTypes
'            For Each mType As Type In MyTypes
'                If mType.BaseType Is GetType(Form) Then
'                    ListOfForms.Add(mType.UnderlyingSystemType)
'                    lb.Items.Add(mType.UnderlyingSystemType.Name)
'                End If
'            Next

'            AddHandler lb.SelectedIndexChanged, AddressOf ListItemSelected

'            lb.Height = lb.ItemHeight * (lb.Items.Count + 1)

'            edSvc.DropDownControl(lb)
'        End If

'        If lb.SelectedIndex >= 0 And lb.SelectedIndex < ListOfForms.Count Then
'            Return ListOfForms(lb.SelectedIndex)
'        Else
'            Return Nothing
'        End If
'    End Function

'    Private Sub ListItemSelected(ByVal sender As Object, ByVal e As System.EventArgs)
'        edSvc.CloseDropDown()
'    End Sub



'    ' Indicates whether the UITypeEditor supports painting a 
'    ' representation of a property's value.
'    Public Overloads Overrides Function GetPaintValueSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
'        Return False
'    End Function
'End Class

