Public Class SimulatorCom
    Inherits ComponentModel.Component
    Implements MfgControl.AdvancedHMI.Drivers.IComComponent

    Public Delegate Sub EventHandler(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
    Public Event DataReceived As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)

    Private WithEvents PollUpdateTimer As New Windows.Forms.Timer

    Private Class SubscriptionDetail
        Friend StartAddress As String
        Friend Callback As EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        Friend ID As Integer
        Friend CurrentValue As String
    End Class

    Private Subscriptions As New List(Of SubscriptionDetail)
    Private CurrentSubscriptionID As Integer


    '**************************************************
    '* Its purpose is to fetch
    '* the main form in order to synchronize the
    '* notification thread/event
    '**************************************************
    Private m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
    '* do not let this property show up in the property window
    ' <System.ComponentModel.Browsable(False)> _
    Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        Get
            'If Me.Site.DesignMode Then

            Dim host1 As System.ComponentModel.Design.IDesignerHost
            Dim obj1 As Object
            If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
                host1 = CType(Me.GetService(GetType(System.ComponentModel.Design.IDesignerHost)), System.ComponentModel.Design.IDesignerHost)
                If host1 IsNot Nothing Then
                    obj1 = host1.RootComponent
                    m_SynchronizingObject = CType(obj1, System.ComponentModel.ISynchronizeInvoke)
                End If
            End If
            'End If
            Return m_SynchronizingObject

        End Get

        Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
            If Not Value Is Nothing Then
                m_SynchronizingObject = Value
            End If
        End Set
    End Property

    Public Property DisableSubscriptions As Boolean Implements MfgControl.AdvancedHMI.Drivers.IComComponent.DisableSubscriptions
        Get

        End Get
        Set(value As Boolean)

        End Set
    End Property

    Public Function ReadAsynchronouos(startAddress As String) As String
        Dim i As Integer
        While i < Subscriptions.Count AndAlso Subscriptions(i).StartAddress.ToUpper <> startAddress.ToUpper
            i += 1
        End While

        If i < Subscriptions.Count Then
            Return Subscriptions(i).CurrentValue
        Else
            Return "0"
        End If
    End Function

    Public Function BeginRead(startAddress As String, numberOfElements As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginRead
        Return 0
    End Function

    Public Function Read(startAddress As String, numberOfElements As Integer) As String() Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Read
        Dim v(numberOfElements - 1) As String
        Return v
    End Function

    Public Function Subscribe(plcAddress As String, numberOfElements As Short, pollRate As Integer, callback As System.EventHandler(Of MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Subscribe
        Dim ns As New SubscriptionDetail
        ns.StartAddress = plcAddress
        ns.Callback = callback
        ns.ID = CurrentSubscriptionID
        CurrentSubscriptionID += 1
        Subscriptions.Add(ns)

        PollUpdateTimer.Enabled = True

        Return ns.ID
    End Function

    Public Function Unsubscribe(id As Integer) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Unsubscribe
        PollUpdateTimer.Enabled = False

        Dim i As Integer
        While i < Subscriptions.Count AndAlso Subscriptions(i).ID <> id
            i += 1
        End While

        If i < Subscriptions.Count Then
            Subscriptions.RemoveAt(i)
        End If

        If Subscriptions.Count > 0 Then
            PollUpdateTimer.Enabled = True
        End If
    End Function

    Public Function Write(startAddress As String, dataToWrite As String) As String Implements MfgControl.AdvancedHMI.Drivers.IComComponent.Write
        Dim i As Integer
        While i < Subscriptions.Count
            If Subscriptions(i).StartAddress.ToUpper = startAddress.ToUpper Then
                Subscriptions(i).CurrentValue = dataToWrite
            End If
            i += 1
        End While

        Return "0"
    End Function

    Public Function Write(startAddress As String, ByVal numberOfElements As Integer, dataToWrite() As String) As Integer Implements MfgControl.AdvancedHMI.Drivers.IComComponent.BeginWrite
        Dim i As Integer
        While i < Subscriptions.Count
            If Subscriptions(i).StartAddress.ToUpper = startAddress.ToUpper Then
                Subscriptions(i).CurrentValue = dataToWrite(0)
            End If
            i += 1
        End While

        Return 0
    End Function

    Private Sub PollUpdateTimer_Tick(sender As Object, e As System.EventArgs) Handles PollUpdateTimer.Tick
        PollUpdateTimer.Enabled = False

        Dim index As Integer
        While index < Subscriptions.Count
            Dim x As New MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs(New String() {Subscriptions(index).CurrentValue}, Subscriptions(index).StartAddress, 0)
            If m_SynchronizingObject IsNot Nothing Then
                Dim Parameters() As Object = {Me, x}
                m_SynchronizingObject.Invoke(Subscriptions(index).Callback, Parameters)
            Else
                RaiseEvent DataReceived(Me, x)
            End If

            index += 1
        End While


        PollUpdateTimer.Enabled = True
    End Sub

    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Private drsd As EventHandler = AddressOf DataReceivedSync
    Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        RaiseEvent DataReceived(Me, e)
    End Sub

End Class
