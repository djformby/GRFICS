'********************************************************************
'* This component only serves the purpose to cover a bug in VS2015
'* If this is not on the form, not all of the drivers will show up
'* in the ToolBox
'********************************************************************
<System.ComponentModel.DesignTimeVisible(False)> _
<System.ComponentModel.ToolboxItem(False)> _
Public Class zForceItemsIntoToolBox
    Inherits MfgControl.AdvancedHMI.Drivers.ForceItemsIntoToolbox
End Class
