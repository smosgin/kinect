' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Kinect

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Public Class AudioListener
		Inherits KinectListener
		Private _running As Boolean

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer)
			VerifyConstructorArguments(kinect, port)

			Me.Kinect = kinect
			Me.Port = port

			Dim t As New Thread(AddressOf AudioThread) With {.IsBackground = True}
			t.Start()
		End Sub

		Private Sub AudioThread()
			_running = True

			Dim buffer(4095) As Byte

			Dim kinectAudioStream As Stream = Kinect.AudioSource.Start()

			Do While Me._running
				Dim count As Integer = kinectAudioStream.Read(buffer, 0, buffer.Length)

				Parallel.For(0, ClientList.Count, Sub(index)
					Dim sc As SocketClient = ClientList(index)
					sc.Send(BitConverter.GetBytes(count))
					sc.Send(buffer, count)
				End Sub)

				RemoveClients()
			Loop
		End Sub
	End Class
End Namespace
