'**********************************************************************************************
'* AdvancedHMI Driver
'* http://www.advancedhmi.com
'* DF1 Servial for SLC/Micro
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* support@advancedhmi.com
'* 13-FEB-16
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
Imports System.ComponentModel.Design

'<CLSCompliant(True)>
<System.ComponentModel.DefaultEvent("DataReceived")> _
Public Class SerialDF1forSLCMicroCom
    Inherits MfgControl.AdvancedHMI.Drivers.DF1ForSLCMicroPLC5
    Implements System.ComponentModel.IComponent
    Implements System.ComponentModel.ISupportInitialize

    Private Shared ReadOnly EventDisposed As New Object()
    Public Event Disposed As EventHandler Implements System.ComponentModel.IComponent.Disposed

    Protected m_synchronizationContext As System.Threading.SynchronizationContext

#Region "Constructor"
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyBase.New()

        m_synchronizationContext = System.Windows.Forms.WindowsFormsSynchronizationContext.Current
        'Required for Windows.Forms Class Composition Designer support
        container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        m_synchronizationContext = System.Windows.Forms.WindowsFormsSynchronizationContext.Current
    End Sub


    'Public Overloads Sub dispose() 'Implements System.ComponentModel.IComponent.Dispose
    '    MyBase.Dispose(True)
    '    GC.SuppressFinalize(Me)
    'End Sub

    Protected Overrides Sub Dispose(disposing As Boolean) 'Implements System.ComponentModel.IComponent.Dispose
        If disposing Then
            SyncLock Me
                If Site IsNot Nothing AndAlso Site.Container IsNot Nothing Then
                    Site.Container.Remove(Me)
                End If
                'If events IsNot Nothing Then
                'Dim handler As EventHandler = DirectCast(events(EventDisposed), EventHandler)
                RaiseEvent Disposed(Me, EventArgs.Empty)
                'End If
            End SyncLock
        End If
    End Sub



    ''Component overrides dispose to clean up the component list.
    'Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    '    MyBase.Dispose(disposing)
    'End Sub
#End Region

#Region "Properties"
    <System.ComponentModel.EditorAttribute(GetType(BaudRateEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    Public Overrides Property BaudRate As String
        Get
            Return MyBase.BaudRate
        End Get
        Set(value As String)
            MyBase.BaudRate = value
        End Set
    End Property



    Private m_site As System.ComponentModel.ISite
    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)> _
    Public Overridable Property Site() As System.ComponentModel.ISite Implements System.ComponentModel.IComponent.Site
        Get
            Return m_site
        End Get
        Set(value As System.ComponentModel.ISite)
            m_site = value
        End Set
    End Property


    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)> _
    Protected ReadOnly Property DesignMode() As Boolean
        Get
            Dim s As System.ComponentModel.ISite = m_site
            If s Is Nothing Then
                Return False
            Else
                Return s.DesignMode
            End If
            ' Return If((s Is Nothing), False, s.DesignMode)
        End Get
    End Property
#End Region

    Protected Overridable Function GetService(service As Type) As Object
        Dim s As System.ComponentModel.ISite = Site
        Return (If((s Is Nothing), Nothing, s.GetService(service)))
    End Function


#Region "Events"
    'Protected Overrides Sub OnConnectionClosed(ByVal e As EventArgs)
    '    'If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
    '    'Dim Parameters() As Object = {Me, EventArgs.Empty}
    '    If m_synchronizationContext IsNot Nothing Then  ' m_SynchronizingObject IsNot Nothing Then
    '        Try
    '            'm_SynchronizingObject.BeginInvoke(occd, Parameters)
    '            m_synchronizationContext.Post(AddressOf ConnectionClosedSync, EventArgs.Empty)
    '        Catch ex As Exception
    '        End Try
    '        'End If
    '    Else
    '        MyBase.OnConnectionClosed(e)
    '    End If
    'End Sub

    'Private Sub ConnectionClosedSync(ByVal e As Object)
    '    Dim e1 As EventArgs = DirectCast(e, EventArgs)
    '    MyBase.OnConnectionClosed(e1)
    'End Sub
    '********************************************************************************************************************************


    'Protected Overrides Sub OnConnectionEstablished(ByVal e As EventArgs)
    '    'If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
    '    ' Dim Parameters() As Object = {Me, EventArgs.Empty}
    '    If m_synchronizationContext IsNot Nothing Then
    '        'If TypeOf (m_SynchronizingObject) Is System.Windows.Forms.Control AndAlso DirectCast(m_SynchronizingObject, System.Windows.Forms.Control).IsHandleCreated Then
    '        'm_SynchronizingObject.BeginInvoke(oced, Parameters)
    '        m_synchronizationContext.Post(AddressOf ConnectionEstabishedSync, EventArgs.Empty)
    '        'End If
    '        'End If
    '    Else
    '        MyBase.OnConnectionEstablished(e)
    '    End If
    'End Sub


    'Private Sub ConnectionEstabishedSync(ByVal e As Object)
    '    Dim e1 As EventArgs = DirectCast(e, EventArgs)
    '    MyBase.OnConnectionEstablished(e1)
    'End Sub


    Protected Overrides Sub OnDataReceived(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        'If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
        If m_synchronizationContext IsNot Nothing Then
            'm_SynchronizingObject.BeginInvoke(drsd, New Object() {Me, e})
            m_synchronizationContext.Post(AddressOf DataReceivedSync, e)
        Else
            MyBase.OnDataReceived(e)
        End If
    End Sub


    Private Sub DataReceivedSync(ByVal e As Object)
        Try
            Dim e1 As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            MyBase.OnDataReceived(e1)
        Catch ex As Exception
            Dim dbg = 0
        End Try
    End Sub


    '***********************************************************************************************************
    Protected Overrides Sub OnComError(ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        'If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
        If m_synchronizationContext IsNot Nothing Then
            Dim Parameters() As Object = {Me, e}
            'm_SynchronizingObject.BeginInvoke(errorsd, Parameters)
            m_synchronizationContext.Post(AddressOf ErrorReceivedSync, e)
        Else
            MyBase.OnComError(e)
        End If
    End Sub

    Private Sub ErrorReceivedSync(ByVal e As Object)
        Dim e1 As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        MyBase.OnComError(e1)
    End Sub
    '***********************************************************************************************************


    '***********************************************************************************************************
    Protected Overrides Sub OnSubscriptionDataReceived(e As MfgControl.AdvancedHMI.Drivers.Common.SubscriptionEventArgs)
        'If m_SynchronizingObject IsNot Nothing AndAlso m_SynchronizingObject.InvokeRequired Then
        If m_synchronizationContext IsNot Nothing Then
            Dim Parameters() As Object = {Me, e}
            'm_SynchronizingObject.BeginInvoke(e.dlgCallBack, Parameters)
            m_synchronizationContext.Post(AddressOf DataRecSync, e)
        Else
            MyBase.OnSubscriptionDataReceived(e)
        End If
    End Sub

    Private Sub DataRecSync(ByVal e As Object)
        Dim e1 As MfgControl.AdvancedHMI.Drivers.Common.SubscriptionEventArgs = DirectCast(e, MfgControl.AdvancedHMI.Drivers.Common.SubscriptionEventArgs)
        e1.dlgCallBack(Me, e1)
    End Sub

#End Region

#Region "IniFileHandling"
    Private m_IniFileName As String = ""
    Public Property IniFileName As String
        Get
            Return m_IniFileName
        End Get
        Set(value As String)
            m_IniFileName = value
        End Set
    End Property

    Private m_IniFileSection As String
    Public Property IniFileSection As String
        Get
            Return m_IniFileSection
        End Get
        Set(value As String)
            m_IniFileSection = value
        End Set
    End Property

    Private Initializing As Boolean
    Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
        Initializing = True
    End Sub

    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        If Not Me.DesignMode Then
            If Not String.IsNullOrEmpty(m_IniFileName) Then
                Try
                    Utilities.SetPropertiesByIniFile(Me, m_IniFileName, m_IniFileSection)
                Catch ex As Exception
                    System.Windows.Forms.MessageBox.Show("INI File - " & ex.Message)
                End Try
            End If
        End If
        Initializing = False
    End Sub
#End Region
End Class
