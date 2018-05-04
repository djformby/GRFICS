Public Class ChartByArray
    Inherits System.Windows.Forms.DataVisualization.Charting.Chart

#Region "Properties"
    Private m_ArrayIndex As Integer
    Public Property ArrayIndex As Integer
        Get
            Return m_ArrayIndex
        End Get
        Set(value As Integer)
            If m_ArrayIndex <> value Then
                m_ArrayIndex = value

                If Not Me.DesignMode Then
                    AddPointsToTheChart()
                End If
            End If
        End Set
    End Property

    Private m_RefreshAllPoints As Boolean
    Public Property RefreshAllPoints As Boolean
        Get
            Return m_RefreshAllPoints
        End Get
        Set(value As Boolean)
            If value And Not m_RefreshAllPoints Then
                LastIndex = 0
                For sIndex = 0 To Me.Series.Count - 1
                    Me.Series(sIndex).Points.Clear()
                Next
                AddPointsToTheChart()
            End If
            m_RefreshAllPoints = value
        End Set
    End Property

    Private m_PLCAddressItems As List(Of MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
    <System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)> _
    Public ReadOnly Property PLCAddressItems As List(Of MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
        Get
            Return m_PLCAddressItems
        End Get
    End Property

    <System.ComponentModel.Editor(GetType(MfgControl.AdvancedHMI.Controls.AutoToDoubleEditor), GetType(System.Drawing.Design.UITypeEditor))>
 <System.ComponentModel.TypeConverter(GetType(MfgControl.AdvancedHMI.Controls.AutoToDoubleTypeConverter(Of Double)))> _
    Public Property YAxisMax As Double
        Get
            Return Me.ChartAreas(0).AxisY.Maximum
        End Get
        Set(value As Double)
            Try
                If Me.ChartAreas(0).AxisY.Maximum <> value Then
                    '* If the Max is less than minimum, it will throw an exception
                    If Me.ChartAreas(0).AxisY.Minimum > value Then
                        Me.ChartAreas(0).AxisY.Minimum = value - 1
                    End If
                    Me.ChartAreas(0).AxisY.Maximum = value
                End If
            Catch ex As Exception
            End Try
        End Set
    End Property

    <System.ComponentModel.Editor(GetType(MfgControl.AdvancedHMI.Controls.AutoToDoubleEditor), GetType(System.Drawing.Design.UITypeEditor))>
<System.ComponentModel.TypeConverter(GetType(MfgControl.AdvancedHMI.Controls.AutoToDoubleTypeConverter(Of Double)))> _
    Public Property YAxisMin As Double
        Get
            Return Me.ChartAreas(0).AxisY.Minimum
        End Get
        Set(value As Double)
            Try
                If Me.ChartAreas(0).AxisY.Minimum <> value Then
                    '* If the Min is greater than maximum, it will throw an exception
                    If Me.ChartAreas(0).AxisY.Maximum < value Then
                        Me.ChartAreas(0).AxisY.Maximum = value + 1
                    End If
                    Me.ChartAreas(0).AxisY.Minimum = value
                End If
            Catch ex As Exception
            End Try
        End Set
    End Property
#End Region


#Region "Constructor"
    Public Sub New()
        MyBase.New()

        m_PLCAddressItems = New List(Of MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
    End Sub

    '********************************************************************
    '* When an instance is added to the form, set the comm component
    '* property. If a comm component does not exist, add one to the form
    '********************************************************************
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        If Me.DesignMode Then
            '********************************************************
            '* Search for AdvancedHMIDrivers.IComComponent component
            '*   in the Designer Host Container
            '* If one exists, set the client of this component to it
            '********************************************************
            Dim i As Integer
            While m_ComComponent Is Nothing And i < Me.Site.Container.Components.Count
                If Me.Site.Container.Components(i).GetType.GetInterface("IComComponent") IsNot Nothing Then m_ComComponent = Me.Site.Container.Components(i)
                i += 1
            End While

            '************************************************
            '* If no comm component was found, then add one and
            '* point the ComComponent property to it
            '*********************************************
            If m_ComComponent Is Nothing Then
                m_ComComponent = New AdvancedHMIDrivers.EthernetIPforCLXCom(Me.Site.Container)
            End If
        Else
            SubscribeToComDriver()
        End If
    End Sub

#End Region


#Region "PLC Related Properties"
    '*****************************************************
    '* Property - Component to communicate to PLC through
    '*****************************************************
    Private m_ComComponent As MfgControl.AdvancedHMI.Drivers.IComComponent
    <System.ComponentModel.Description("Driver Instance for data reading and writing")> _
    <System.ComponentModel.Category("PLC Properties")> _
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

    Private m_SuppressErrorDisplay As Boolean
    <System.ComponentModel.DefaultValue(False)> _
    Public Property SuppressErrorDisplay As Boolean
        Get
            Return m_SuppressErrorDisplay
        End Get
        Set(value As Boolean)
            m_SuppressErrorDisplay = value
        End Set
    End Property

    '*****************************************
    '* Property - Address in PLC to Link to
    '*****************************************
    Private m_PLCAddressArrayIndex As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
    <System.ComponentModel.Description("Last Index of array to chart")> _
    Public Property PLCAddressArrayIndex() As String
        Get
            Return m_PLCAddressArrayIndex
        End Get
        Set(ByVal value As String)
            If m_PLCAddressArrayIndex <> value Then
                m_PLCAddressArrayIndex = value

                '* When address is changed, re-subscribe to new address
                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private m_PLCAddressRefreshAllPoints As String = ""
    <System.ComponentModel.Category("PLC Properties")> _
<System.ComponentModel.Description("Re-read the complete array")> _
    Public Property PLCAddressRefreshAllPoints() As String
        Get
            Return m_PLCAddressRefreshAllPoints
        End Get
        Set(value As String)
            If m_PLCAddressRefreshAllPoints <> value Then
                m_PLCAddressRefreshAllPoints = value

                SubscribeToComDriver()
            End If
        End Set
    End Property


    Private m_PLCAddressYAxisMin As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.Category("PLC Properties")>
    <System.ComponentModel.Description("Set the minimum of the Y Axis")>
    Public Property PLCAddressYAxisMin() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressYAxisMin
        End Get
        Set(value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressYAxisMin IsNot value Then
                m_PLCAddressYAxisMin = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

    Private m_PLCAddressYAxisMax As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
    <System.ComponentModel.Category("PLC Properties")> _
<System.ComponentModel.Description("Set the maximum of the Y Axis")> _
    Public Property PLCAddressYAxisMax() As MfgControl.AdvancedHMI.Drivers.PLCAddressItem
        Get
            Return m_PLCAddressYAxisMax
        End Get
        Set(value As MfgControl.AdvancedHMI.Drivers.PLCAddressItem)
            If m_PLCAddressYAxisMax IsNot value Then
                m_PLCAddressYAxisMax = value

                SubscribeToComDriver()
            End If
        End Set
    End Property

#End Region


#Region "Subscribing and PLC data receiving"
    Private SubScriptions As SubscriptionHandler
    '*******************************************************************************
    '* Subscribe to addresses in the Comm(PLC) Driver
    '* This code will look at properties to find the "PLCAddress" + property name
    '*
    '*******************************************************************************
    Private Sub SubscribeToComDriver()
        If Not DesignMode And IsHandleCreated Then
            '* Create a subscription handler object
            If SubScriptions Is Nothing Then
                SubScriptions = New SubscriptionHandler
                SubScriptions.Parent = Me
                AddHandler SubScriptions.DisplayError, AddressOf DisplaySubscribeError
            End If
            SubScriptions.ComComponent = m_ComComponent

            SubScriptions.SubscribeAutoProperties()
        End If
    End Sub

    '***************************************
    '* Call backs for returned data
    '***************************************
    Private OriginalText As String
    Private Sub PolledDataReturned(ByVal sender As Object, ByVal e As SubscriptionHandlerEventArgs)
    End Sub

#End Region

#Region "Error Display"
    Private Sub DisplaySubscribeError(ByVal sender As Object, ByVal e As MfgControl.AdvancedHMI.Drivers.Common.PlcComEventArgs)
        DisplayError(e.ErrorMessage)
    End Sub

    '********************************************************
    '* Show an error via the text property for a short time
    '********************************************************
    Private ErrorDisplayTime As System.Windows.Forms.Timer
    Private ErrorLock As New Object
    Private Sub DisplayError(ByVal ErrorMessage As String)
        If Not m_SuppressErrorDisplay Then
            '* Create the error display timer
            If ErrorDisplayTime Is Nothing Then
                ErrorDisplayTime = New System.Windows.Forms.Timer
                AddHandler ErrorDisplayTime.Tick, AddressOf ErrorDisplay_Tick
                ErrorDisplayTime.Interval = 5000
            End If

            '* Save the text to return to
            SyncLock (ErrorLock)
                If Not ErrorDisplayTime.Enabled Then
                    ErrorDisplayTime.Enabled = True
                    OriginalText = MyBase.Text
                    MyBase.Text = ErrorMessage
                End If
            End SyncLock
        End If
    End Sub


    '**************************************************************************************
    '* Return the text back to its original after displaying the error for a few seconds.
    '**************************************************************************************
    Private Sub ErrorDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SyncLock (ErrorLock)
            MyBase.Text = OriginalText
            ErrorDisplayTime.Enabled = False
        End SyncLock
    End Sub
#End Region

    Private LastIndex As Integer
#Region "Private Methods"
    Private Sub AddPointsToTheChart()
        '* Check to see if we need to add more series
        While m_PLCAddressItems.Count > MyBase.Series.Count
            MyBase.Series.Add("Series" & MyBase.Series.Count + 1)
        End While


        '* If the index went down (e.g to 0) , then clear the chart
        If m_ArrayIndex < LastIndex Then
            For sIndex = 0 To Me.Series.Count - 1
                Me.Series(sIndex).Points.Clear()
            Next
            LastIndex = 0
        End If


        Dim NewValues() As String
        Dim ItemIndex As Integer
        While m_ArrayIndex > LastIndex

            Dim IndexPartial As Integer = m_ArrayIndex

            '* Limit to 100 points per read
            If IndexPartial - LastIndex > 100 Then
                IndexPartial = LastIndex + 100
            End If

            Try
                ItemIndex = 0
                While ItemIndex < m_PLCAddressItems.Count
                    NewValues = m_ComComponent.Read(m_PLCAddressItems(ItemIndex).PLCAddress & "[" & LastIndex & "]", IndexPartial - LastIndex)
                    If NewValues IsNot Nothing Then
                        For i = 0 To NewValues.Length - 1
                            If m_PLCAddressItems(ItemIndex).ScaleFactor = 1 And m_PLCAddressItems(ItemIndex).ScaleOffset = 0 Then
                                Me.Series(ItemIndex).Points.Add(CDbl((NewValues(i))))
                            Else
                                Dim v As Double
                                If Double.TryParse(NewValues(i), v) Then
                                    Me.Series(ItemIndex).Points.Add(m_PLCAddressItems(ItemIndex).GetScaledValue(v))
                                Else
                                    '* Could not convert to value, so chart a 0 at this point
                                    Me.Series(ItemIndex).Points.Add(0)
                                End If
                            End If
                        Next
                    End If

                    ItemIndex += 1
                End While
            Catch ex As Exception
                Me.Text = ex.Message
            End Try

            LastIndex = IndexPartial
        End While

        LastIndex = m_ArrayIndex
    End Sub
#End Region

End Class
