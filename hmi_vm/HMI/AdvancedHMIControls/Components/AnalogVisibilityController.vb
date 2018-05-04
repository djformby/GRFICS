Public Class AnalogVisibilityController
    Inherits DataSubscriber

#Region "Properties"
    Private m_TargetObject As Control
    '<System.ComponentModel.TypeConverter(GetType(MfgControl.AdvancedHMI.Controls.EmptyTypeConverter()))> _
    Public Property TargetObject As Control
        Get
            Return m_TargetObject
        End Get
        Set(value As Control)
            m_TargetObject = value
        End Set
    End Property


    Public Enum CompareTypeEnum
        AboveTarget
        EqualToTarget
        BelowTarget
    End Enum
    Private m_ValueCompareType As CompareTypeEnum = CompareTypeEnum.AboveTarget
    Public Property ValueCompareType As CompareTypeEnum
        Get
            Return m_ValueCompareType
        End Get
        Set(value As CompareTypeEnum)
            m_ValueCompareType = value
        End Set
    End Property

    Private m_ValueTarget As Double = 1000
    Public Property ValueTarget As Double
        Get
            Return m_ValueTarget
        End Get
        Set(value As Double)
            m_ValueTarget = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Overrides Sub OnDataChanged(e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        MyBase.OnDataChanged(e)

        If m_TargetObject IsNot Nothing Then
            If e IsNot Nothing AndAlso e.Values IsNot Nothing AndAlso e.Values.Count > 0 Then
                Dim v As Double
                If Double.TryParse(e.Values(0), v) Then
                    If m_ValueCompareType = CompareTypeEnum.AboveTarget Then
                        m_TargetObject.Visible = (v > m_ValueTarget)
                    ElseIf m_ValueCompareType = CompareTypeEnum.EqualToTarget Then
                        m_TargetObject.Visible = (v = m_ValueTarget)
                    ElseIf m_ValueCompareType = CompareTypeEnum.BelowTarget Then
                        m_TargetObject.Visible = (v < m_ValueTarget)
                    End If
                End If
            End If
        End If
    End Sub
#End Region

End Class
