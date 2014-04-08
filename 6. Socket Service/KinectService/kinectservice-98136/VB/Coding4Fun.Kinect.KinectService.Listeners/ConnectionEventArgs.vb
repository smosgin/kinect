' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Net.Sockets

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Friend Class ConnectionEventArgs
		Inherits EventArgs
		Public Property TcpClient() As TcpClient
		Public Property SocketClient() As SocketClient
	End Class
End Namespace