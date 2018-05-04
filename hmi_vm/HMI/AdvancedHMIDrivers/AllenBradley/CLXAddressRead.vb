Option Strict On
Public Class CLXAddressRead
    Inherits MfgControl.AdvancedHMI.Drivers.CLXAddress
    'Implements ICloneable


#Region "Properties"
    Private m_TransactionNumber As Int32
    Public Property TransactionNumber As Int32
        Get
            Return m_TransactionNumber
        End Get
        Set(value As Int32)
            m_TransactionNumber = value
        End Set
    End Property

    Private m_Response As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs
    Public Property Response As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs
        Get
            Return m_Response
        End Get
        Set(value As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
            m_Response = value
        End Set
    End Property

    'Private m_Responded As Boolean
    'Public Property Responded As Boolean
    '    Get
    '        Return m_Responded
    '    End Get
    '    Set(value As Boolean)
    '        m_Responded = value
    '    End Set
    'End Property
#End Region

#Region "Constructors"
    Public Sub New()
        MyBase.new()
    End Sub

    Public Sub New(ByVal tagName As String)
        MyClass.new()
        Me.TagName = tagName
    End Sub


    Public Sub New(ByVal tagName As String, ByVal transactionNumber As Int32)
        MyClass.New(tagName)
        m_TransactionNumber = transactionNumber
    End Sub
#End Region

    Public Shadows Function Clone() As Object 'Implements System.ICloneable.Clone
        Dim x As New CLXAddressRead

        x.TagName = Me.TagName
        x.NumberOfElements = Me.NumberOfElements

        x.TransactionNumber = TransactionNumber
        x.AbbreviatedDataType = AbbreviatedDataType

        Return x
    End Function
End Class

