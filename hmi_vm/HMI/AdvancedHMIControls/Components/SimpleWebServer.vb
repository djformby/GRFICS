Imports System.Net
Imports System.Net.Sockets

Public Class SimpleWebServer
    Inherits System.ComponentModel.Component
    Implements System.ComponentModel.ISupportInitialize

    Private server As System.Net.Sockets.TcpListener
    Private StopThread As Boolean
    Private m_synchronizationContext As System.Threading.SynchronizationContext



#Region "Constructor"
    Public Sub New()
        m_synchronizationContext = System.Windows.Forms.WindowsFormsSynchronizationContext.Current
    End Sub

    Public Sub New(ByVal p As Form)
        Me.New()

        m_SourceForm = p
    End Sub


#End Region

#Region "Properties"
    Private m_TCPPort As Integer = 80
    Public Property TCPPort As Integer
        Get
            Return m_TCPPort
        End Get
        Set(value As Integer)
            m_TCPPort = value
        End Set
    End Property

    Private m_AutoStart As Boolean = True
    Public Property AutoStart As Boolean
        Get
            Return m_AutoStart
        End Get
        Set(value As Boolean)
            m_AutoStart = value
        End Set
    End Property

    Private m_RefreshTime As Integer = 10
    Public Property RefreshTime As Integer
        Get
            Return m_RefreshTime
        End Get
        Set(value As Integer)
            m_RefreshTime = Math.Max(0, value)
        End Set
    End Property

    Private m_SourceForm As Form
    Public Property SourceForm As Form
        Get
            Return m_SourceForm
        End Get
        Set(value As Form)
            m_SourceForm = value
        End Set
    End Property

    Protected m_SynchronizingObject As System.ComponentModel.ISynchronizeInvoke
    '* do not let this property show up in the property window
    ' <System.ComponentModel.Browsable(False)> _
    Public Property SynchronizingObject() As System.ComponentModel.ISynchronizeInvoke
        Get
            Dim host1 As System.ComponentModel.Design.IDesignerHost
            If (m_SynchronizingObject Is Nothing) AndAlso MyBase.DesignMode Then
                host1 = CType(Me.GetService(GetType(System.ComponentModel.Design.IDesignerHost)), System.ComponentModel.Design.IDesignerHost)
                If host1 IsNot Nothing Then
                    m_SynchronizingObject = CType(host1.RootComponent, System.ComponentModel.ISynchronizeInvoke)
                End If
            End If

            Return m_SynchronizingObject
        End Get

        Set(ByVal Value As System.ComponentModel.ISynchronizeInvoke)
            If Not Value Is Nothing Then
                m_SynchronizingObject = Value
            End If
        End Set
    End Property
#End Region

#Region "Public Methods"
    Public Sub StartServer()
        If m_SourceForm Is Nothing Then
            m_SourceForm = m_SynchronizingObject
        End If

        '* Find the IPV4 address of this computer
        Dim LocalComputerName As String = System.Net.Dns.GetHostName() '* same as My.Computer.Name
        Dim localAddr As System.Net.IPAddress = GetIPv4Address(LocalComputerName)

        If localAddr Is Nothing Then
            localAddr = System.Net.IPAddress.Parse("127.0.0.1")
        End If

        Try
            server = New TcpListener(localAddr, m_TCPPort)
            server.Start()
            server.BeginAcceptTcpClient(AddressOf ConnectionAccepted, server)
        Catch e As SocketException
            Console.WriteLine("SocketException: {0}", e)
        End Try
    End Sub

    Public Sub StopServer()
        StopThread = True
        server.Stop()
    End Sub

#End Region

#Region "Private Methods"
    Public Shared Function GetIPv4Address(ByVal HostName As String) As System.Net.IPAddress
        'Check first to see if an IpAddress is being passed
        If HostName IsNot Nothing AndAlso (String.Compare(HostName, "") <> 0) Then
            Try
                Dim IPAddress As System.Net.IPAddress = New System.Net.IPAddress(0)
                If System.Net.IPAddress.TryParse(HostName, IPAddress) Then
                    Return IPAddress
                End If
            Catch ex As Exception
            End Try
        End If



        '* If it is blank, then use this computer's name
        If HostName = "" Then
            HostName = System.Net.Dns.GetHostName()
        End If

        '* Get The IP Address list from the Host
        Dim IP As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(HostName)

        '* Find the IPv4 address of this PC and use those as the first 4 bytes of the 
        '* source AMS Net ID. Just to help find source of queries on TwinCAT pc
        Dim i As Integer
        Dim FirstFind As Integer
        While FirstFind < IP.AddressList.Length AndAlso (IP.AddressList(FirstFind).GetAddressBytes.Length <> 4)
            FirstFind += 1
        End While

        '* Let's look for an IP on the same subnet
        i = FirstFind
        While i < IP.AddressList.Length AndAlso (IP.AddressList(i).GetAddressBytes.Length <> 4)
            i += 1
        End While

        If i < IP.AddressList.Length Then
            Return IP.AddressList(i)
        ElseIf FirstFind < IP.AddressList.Length Then
            Return IP.AddressList(FirstFind)
        Else
            'Throw New MfgControl.AdvancedHMI.Drivers.Common.PLCDriverException("Un-resolvable address " & HostName)
        End If

        Return Nothing
    End Function

    Private Sub ConnectionAccepted(ar As IAsyncResult)
        If Not StopThread Then
            ' Dim listener As TcpListener = CType(ar.AsyncState, TcpListener)
            Dim client As TcpClient = server.EndAcceptTcpClient(ar)

            Dim bytes(1024) As Byte

            ' Get a stream object for reading and writing
            Dim stream As NetworkStream = client.GetStream()

            Dim i As Integer = stream.Read(bytes, 0, bytes.Length)


            Dim header As String =
                "<html xmlns : msxsl = ""urn:schemas-Microsoft - com: xslt""  meta content=""en-us"" http-equiv=""Content-Language"" /> " & _
                 "<meta content=""text/html; charset=utf-16"" http-equiv=""Content-Type"" /> " & _
                 "<meta http-equiv=""refresh"" content=""" & CStr(m_RefreshTime) & """> "

            header &= "<img src=""data:image/png;base64,"
            Dim b() As Byte = System.Text.ASCIIEncoding.Default.GetBytes(header)
            stream.Write(b, 0, b.Length)

            Dim Imgbytes() As Byte
            bmpScreenCapture = New Bitmap(m_SourceForm.Width, m_SourceForm.Height)
            m_SourceForm.Invoke(dcc)
            Using ms As New System.IO.MemoryStream
                bmpScreenCapture.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                Imgbytes = ms.ToArray
            End Using

            Dim s As String = Convert.ToBase64String(Imgbytes)
            Imgbytes = System.Text.ASCIIEncoding.Default.GetBytes(s)
            Try
                stream.Write(Imgbytes, 0, Imgbytes.Length)

                Dim trailer As String = " ""/>"
                b = System.Text.ASCIIEncoding.Default.GetBytes(trailer)
                stream.Write(b, 0, b.Length)


                ' Shutdown and end connection
                client.Close()
            Catch ex As Exception
            End Try
        End If

        If Not StopThread Then
            server.BeginAcceptTcpClient(AddressOf ConnectionAccepted, server)
        End If
    End Sub

    Private bmpScreenCapture As Bitmap
    Delegate Sub CaptureFormDelegate()
    Private dcc As CaptureFormDelegate = AddressOf CaptureForm
    Private Sub CaptureForm()
        m_SourceForm.DrawToBitmap(bmpScreenCapture, New Rectangle(0, 0, m_SourceForm.Width, m_SourceForm.Height))

        '    CaptureScreen.
    End Sub
#End Region

#Region "Initializing"
    Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
        'Throw New NotImplementedException()
    End Sub

    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        If Not Me.DesignMode And m_AutoStart Then
            StartServer()
        End If
    End Sub
#End Region
End Class
