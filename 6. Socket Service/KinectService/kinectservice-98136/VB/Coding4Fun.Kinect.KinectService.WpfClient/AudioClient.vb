' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Net.Sockets
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.WpfClient
	Public Class AudioClient
		Inherits KinectServiceClient
		Public Event AudioFrameReady As EventHandler(Of AudioFrameReadyEventArgs)
		Private privateAudioFrame As AudioFrameData
		Public Property AudioFrame() As AudioFrameData
			Get
				Return privateAudioFrame
			End Get
			Private Set(ByVal value As AudioFrameData)
				privateAudioFrame = value
			End Set
		End Property

		Public Sub New()
			Me.ThreadProcessor = AddressOf AudioThread
		End Sub

		Private Sub AudioThread()
			Try
				Dim ns As NetworkStream = Client.GetStream()
				Dim reader As New BinaryReader(ns)

				Do While Client.Connected
					Dim size As Integer = reader.ReadInt32()
					Dim bytes() As Byte = reader.ReadBytes(size)

					Dim args As New AudioFrameReadyEventArgs()
					Dim afd As New AudioFrameData()

					afd.AudioData = bytes
					args.AudioFrame = afd

					Context.Send(Sub()
						RaiseEvent AudioFrameReady(Me, args)
					End Sub, Nothing)
				Loop
			Catch e1 As IOException
				Client.Close()
			End Try
		End Sub
	End Class
End Namespace
