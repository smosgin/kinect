Imports System.Net
Imports System.Net.Sockets
Imports Microsoft.Kinect

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Public Class KinectListener
		Protected Property Kinect() As KinectSensor

		Protected Property Port() As Integer

		Friend Event OnConnectionCompleted As EventHandler(Of ConnectionEventArgs)
		Friend Property ClientList() As List(Of SocketClient)

		Private _listener As TcpListener

		Public Sub Start()
			ClientList = New List(Of SocketClient)()

			_listener = New TcpListener(IPAddress.Any, Port)
			_listener.Start(10)
			_listener.BeginAcceptTcpClient(AddressOf OnConnection, Nothing)
		End Sub

		Public Sub [Stop]()
			For Each client As SocketClient In ClientList
				client.Close()
			Next client
		End Sub

		Private Sub OnConnection(ByVal ar As IAsyncResult)
			Dim client As TcpClient = _listener.EndAcceptTcpClient(ar)
			Dim sc As New SocketClient(client)
			ClientList.Add(sc)
			_listener.BeginAcceptTcpClient(AddressOf OnConnection, Nothing)

			RaiseEvent OnConnectionCompleted(Me, New ConnectionEventArgs With {.TcpClient = client, .SocketClient = sc})
		End Sub

		Protected Sub RemoveClients()
			SyncLock ClientList
				Dim i As Integer = 0
				Do While i < ClientList.Count
					If Not ClientList(i).IsConnected Then
						ClientList.Remove(ClientList(i))
					End If
					i += 1
				Loop
			End SyncLock
		End Sub

		Protected Sub VerifyConstructorArguments(ByVal kinect As KinectSensor, ByVal port As Integer)
			If kinect Is Nothing Then
				Throw New ArgumentException("A valid KinectSensor object must be provided.", "kinect")
			End If

			If port < 1 OrElse port > 65535 Then
				Throw New ArgumentException("Ports must be between 0 and 65535, inclusive", "port")
			End If
		End Sub
	End Class
End Namespace
