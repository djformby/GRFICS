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
Public Class ScreenCleanButton
    Inherits System.Windows.Forms.Button

    Private f As System.Windows.Forms.Form
    Private WithEvents t As New System.Windows.Forms.Timer
    Private TickCount As Integer

#Region "Basic Properties"

    Private m_ScreenCleanTimeInterval As Integer = 30
    <System.ComponentModel.Description("Time in seconds to allow for screen cleaning (valid values 10+).")> _
    <System.ComponentModel.DefaultValue(30)> _
    Public Property ScreenCleanTimeInterval As Integer
        Get
            Return m_ScreenCleanTimeInterval
        End Get
        Set(value As Integer)
            m_ScreenCleanTimeInterval = Math.Min(Math.Max(value, 5), 120)
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        MyBase.DoubleBuffered = True
        Me.DoubleBuffered = True
        Me.Size = New Size(81, 23)
    End Sub

#End Region

#Region "Events"

    Private Sub ScreenClean_Click(sender As Object, e As EventArgs) Handles Me.Click
        f = New System.Windows.Forms.Form
        f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        f.WindowState = System.Windows.Forms.FormWindowState.Maximized
        f.TopMost = True

        Dim l As New System.Windows.Forms.Label
        l.Size = f.Size
        l.Font = New Font("Arial", 48)
        l.Text = "Starting"
        l.Location = New Point(0, 0)
        l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        l.Dock = System.Windows.Forms.DockStyle.Fill
        f.Controls.Add(l)

        f.Show()

        TickCount = 0
        t.Interval = 1000
        t.Enabled = True
    End Sub

    Private Sub TickEvent(ByVal sender As Object, ByVal e As System.EventArgs) Handles t.Tick
        If f IsNot Nothing AndAlso Not f.IsDisposed Then
            Dim l As Label = f.Controls(0)
            l.Text = m_ScreenCleanTimeInterval - TickCount
            TickCount += 1

            If TickCount > m_ScreenCleanTimeInterval Then
                t.Enabled = False
                f.Close()
                f.Dispose()
            End If
        End If
    End Sub

#End Region

End Class
