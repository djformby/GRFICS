'**********************************************************************************************
'* AdvancedHMI Driver for Serial Com to PLC5
'* http://www.advancedhmi.com
'* DF1 Serial for PLC5
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 24-APR-16
'*
'*
'* Copyright 2016 Archie Jacobs
'*
'* This class creates and interface between a Serial driver and the AdvancedHMI
'* visual controls.
'*
'* NOTICE : If you received this code without a complete AdvancedHMI solution
'* please report to sales@advancedhmi.com
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
'* 13-FEB-16 New architecture further separating UI element connection from driver
'*******************************************************************************************************

Public Class SerialDF1forPLC5Com
    Inherits SerialDF1forSLCMicroCom

#Region "Constructor"
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        Me.New()

        'Required for Windows.Forms Class Composition Designer support
        container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        '* This tells it to use PLC5 compatible PCCC commands
        MyBase.IsPLC5 = True
    End Sub
#End Region
End Class
