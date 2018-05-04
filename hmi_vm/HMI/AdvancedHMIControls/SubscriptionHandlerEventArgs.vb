Option Strict On
Public Class SubscriptionHandlerEventArgs
    '*************************************************************************
    '* Subscription Handler Event Args
    '*
    '* Archie Jacobs
    '* Manufacturing Automation, LLC
    '* support@advancedhmi.com
    '* 25-OCT-14
    '*
    '* Copyright 2014 Archie Jacobs
    '*
    '* Used to pass response data to events handlers
    '************************************************************************

    Inherits EventArgs

    Private m_PLCComEventArgs As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs
    Public Property PLCComEventArgs As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs
        Get
            Return m_PLCComEventArgs
        End Get
        Set(value As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            m_PLCComEventArgs = value
        End Set
    End Property

    Private m_SubscriptionDetail As SubscriptionDetail
    Public Property SubscriptionDetail As SubscriptionDetail
        Get
            Return m_SubscriptionDetail
        End Get
        Set(value As SubscriptionDetail)
            m_SubscriptionDetail = value
        End Set
    End Property
End Class
