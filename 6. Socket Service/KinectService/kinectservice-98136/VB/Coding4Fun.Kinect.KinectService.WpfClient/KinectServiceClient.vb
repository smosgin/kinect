' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Net.Sockets
Imports System.Threading

Namespace Coding4Fun.Kinect.KinectService.WpfClient
	Public Class KinectServiceClient
		Public Event OnConnectionCompleted As EventHandler(Of ConnectionEventArgs)

		Protected Property ThreadProcessor() As ThreadStart
		Protected Property Client() As TcpClient
		Protected Property Context() As SynchronizationContext

		Public ReadOnly Property IsConnected() As Boolean
			Get
				Return Client IsNot Nothing AndAlso Client.Connected
			End Get
		End Property

		Public Sub New()
			Context = SynchronizationContext.Current
		End Sub

		Public Sub Connect(ByVal address As String, ByVal port As Integer)
			Client = New TcpClient()
			Client.BeginConnect(address, port, AddressOf OnSocketConnectCompleted, Nothing)
		End Sub

		Private Sub OnSocketConnectCompleted(ByVal ar As IAsyncResult)
			Dim connected As Boolean = Client.Connected

			RaiseEvent OnConnectionCompleted(Me, New ConnectionEventArgs With {.Connected = connected})

			If Not connected Then
				Return
			End If

			Dim thread As New Thread(ThreadProcessor) With {.IsBackground = True}
			thread.Start()
		End Sub

		Public Sub Disconnect()
			If Client IsNot Nothing Then
				Client.Close()
			End If
		End Sub
	End Class

	Public Class ConnectionEventArgs
		Inherits EventArgs
		Public Property Connected() As Boolean
	End Class
End Namespace
