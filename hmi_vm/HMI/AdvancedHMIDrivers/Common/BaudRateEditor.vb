Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports System.Windows.Forms.Design

' This UITypeEditor can be associated with Int32, Double and Single
' properties to provide a design-mode angle selection interface.
<System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name:="FullTrust")> _
Public Class BaudRateEditor
    Inherits System.Drawing.Design.UITypeEditor

    Public Sub New()
    End Sub

    ' Indicates whether the UITypeEditor provides a form-based (modal) dialog, 
    ' drop down dialog, or no UI outside of the properties window.
    Public Overloads Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
        Return UITypeEditorEditStyle.DropDown
    End Function

    ' Displays the UI for value selection.
    Dim edSvc As IWindowsFormsEditorService
    Private WithEvents lb As ListBox
    Public Overloads Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As System.IServiceProvider, ByVal value As Object) As Object
        ' Uses the IWindowsFormsEditorService to display a 
        ' drop-down UI in the Properties window.
        edSvc = CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
        lb = New ListBox
        If (edSvc IsNot Nothing) Then
            lb.Items.Add("AUTO")
            lb.Items.Add("38400")
            lb.Items.Add("19200")
            lb.Items.Add("9600")
            lb.Items.Add("2400")
            lb.Items.Add("1200")
            AddHandler lb.SelectedIndexChanged, AddressOf ListItemSelected

            edSvc.DropDownControl(lb)
        End If
        Return lb.SelectedItem
    End Function

    Private Sub ListItemSelected(ByVal sender As Object, ByVal e As System.EventArgs)
        edSvc.CloseDropDown()
    End Sub



    ' Indicates whether the UITypeEditor supports painting a 
    ' representation of a property's value.
    Public Overloads Overrides Function GetPaintValueSupported(ByVal context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return False
    End Function
End Class
