' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Namespace Coding4Fun.Kinect.KinectService.PhoneClient
	Public Class KinectServiceClient
		Protected Const IntSize As Integer = 4
		Protected Const BoolSize As Integer = 1

		Private _socket As Socket
		Private _endPoint As DnsEndPoint
		Private ReadOnly _context As SynchronizationContext

		Private _data() As Byte
		Private _totalBytesTransferred As Integer

		Private Enum State
			Size
			Data
		End Enum
		Private _state As State = State.Size

		Public Event FrameReady As EventHandler(Of FrameReadyEventArgs)
		Public Event OnConnectionCompleted As EventHandler(Of ConnectionEventArgs)

		Public ReadOnly Property IsConnected() As Boolean
			Get
				Return _socket IsNot Nothing AndAlso _socket.Connected
			End Get
		End Property

		Public Sub New()
			_context = SynchronizationContext.Current
		End Sub

		Public Sub Connect(ByVal address As String, ByVal port As Integer)
			_totalBytesTransferred = 0

			_endPoint = New DnsEndPoint(address, port)
			_socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

			Dim args As New SocketAsyncEventArgs()
			args.UserToken = _socket
			args.RemoteEndPoint = _endPoint
			AddHandler args.Completed, AddressOf OnSocketCompleted
			_socket.ConnectAsync(args)
		End Sub

		Private Sub OnSocketCompleted(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
			Select Case e.LastOperation
				Case SocketAsyncOperation.Connect

					Debug.WriteLine("Connected: " & _socket.Connected)

					RaiseEvent OnConnectionCompleted(Me, New ConnectionEventArgs With {.Connected = _socket.Connected})

					_state = State.Size
					_data = New Byte(IntSize - 1){}
					e.SetBuffer(_data, 0, _data.Length)

					TryReceiveAsync(e)

				Case SocketAsyncOperation.Receive
					Select Case _state
						Case State.Size
							_totalBytesTransferred += e.BytesTransferred

							If _totalBytesTransferred < IntSize Then
								e.SetBuffer(_data, _totalBytesTransferred, IntSize - _totalBytesTransferred)
								TryReceiveAsync(e)
							Else
								_state = State.Data
								Dim size As Integer = BitConverter.ToInt32(_data, 0)
								_data = New Byte(size - 1){}
								e.SetBuffer(_data, 0, _data.Length)

								_totalBytesTransferred = 0

								TryReceiveAsync(e)
							End If
						Case State.Data
							_totalBytesTransferred += e.BytesTransferred

							If _totalBytesTransferred < _data.Length Then
								e.SetBuffer(_data, _totalBytesTransferred, _data.Length - _totalBytesTransferred)
								TryReceiveAsync(e)
							Else
								' NOTE: .Post bogs down a WP7 device pretty badly...this *may* cause lag over time, though.  Needs further testing.
								_context.Send(Sub()
									RaiseEvent FrameReady(Me, New FrameReadyEventArgs With {.Data = _data})
								End Sub, Nothing)

								_totalBytesTransferred = 0
								_state = State.Size
								_data = New Byte(IntSize - 1){}

								e.SetBuffer(_data, 0, _data.Length)
								TryReceiveAsync(e)
							End If

						Case Else
							Throw New ArgumentOutOfRangeException()
					End Select

			End Select
		End Sub

		Public Sub Disconnect()
			If _socket IsNot Nothing Then
				_socket.Close()
			End If
		End Sub

		Private Sub TryReceiveAsync(ByVal args As SocketAsyncEventArgs)
			Try
				If _socket.Connected Then
					_socket.ReceiveAsync(args)
				End If
			Catch e1 As ObjectDisposedException
				Debug.WriteLine("Attempted to read on closed socket.")
			End Try
		End Sub
	End Class

	Public Class ConnectionEventArgs
		Inherits EventArgs
		Public Property Connected() As Boolean
	End Class

	Public Class FrameReadyEventArgs
		Inherits EventArgs
		Public Property Data() As Byte()
	End Class
End Namespace
