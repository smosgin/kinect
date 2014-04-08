' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Net.Sockets

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Friend Class SocketClient
		Public ReadOnly Property IsConnected() As Boolean
			Get
				Return _connected AndAlso _client.Connected
			End Get
		End Property

		Private ReadOnly _client As TcpClient
		Private _connected As Boolean

		Public Sub New(ByVal client As TcpClient)
			_client = client
			_connected = True
		End Sub

		Public Function Send(ByVal data() As Byte, ByVal length As Integer) As Boolean
			Try
				If IsConnected Then
					Dim stream As NetworkStream = _client.GetStream()
					stream.BeginWrite(data, 0, length, AddressOf WriteCompleted, stream)
				Else
					_connected = False
					Return False
				End If
			Catch ex As IOException
				Debug.WriteLine(ex.ToString())
				_connected = False
			Catch ex As ObjectDisposedException
				Debug.WriteLine(ex.ToString())
				_connected = False
			End Try

			Return True
		End Function

		Public Function Send(ByVal data() As Byte) As Boolean
			Return Send(data, data.Length)
		End Function

		Private Sub WriteCompleted(ByVal ar As IAsyncResult)
			Try
				If IsConnected Then
					Dim ns As NetworkStream = CType(ar.AsyncState, NetworkStream)
					ns.EndWrite(ar)
				End If
			Catch ex As IOException
				Debug.WriteLine(ex.ToString())
				_connected = False
			Catch ex As ObjectDisposedException
				Debug.WriteLine(ex.ToString())
				_connected = False
			End Try
		End Sub

		Public Sub Close()
			If _client.Connected Then
				_client.Close()
			End If
		End Sub
	End Class
End Namespace
