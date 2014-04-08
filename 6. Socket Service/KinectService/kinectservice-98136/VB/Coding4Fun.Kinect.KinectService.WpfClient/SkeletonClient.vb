' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Net.Sockets
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.WpfClient
	Public Class SkeletonClient
		Inherits KinectServiceClient
		Public Event SkeletonFrameReady As EventHandler(Of SkeletonFrameReadyEventArgs)
		Private privateSkeletonFrame As SkeletonFrameData
		Public Property SkeletonFrame() As SkeletonFrameData
			Get
				Return privateSkeletonFrame
			End Get
			Private Set(ByVal value As SkeletonFrameData)
				privateSkeletonFrame = value
			End Set
		End Property

		Public Sub New()
			Me.ThreadProcessor = AddressOf StreamSkeleton
		End Sub

		Private Sub StreamSkeleton()
			Try
				Dim ns As NetworkStream = Client.GetStream()
				Dim reader As New BinaryReader(ns)

				Do While Client.Connected
					Dim size As Integer = reader.ReadInt32()
					Dim data() As Byte = reader.ReadBytes(size)

					Dim ms As New MemoryStream(data)
					Dim br As New BinaryReader(ms)

					Dim frame As SkeletonFrameData = br.ReadSkeletonFrame()

					Dim args As SkeletonFrameReadyEventArgs = New SkeletonFrameReadyEventArgs With {.SkeletonFrame = frame}
					SkeletonFrame = frame

					Context.Send(Sub()
						RaiseEvent SkeletonFrameReady(Me, args)
					End Sub, Nothing)
				Loop
			Catch e1 As IOException
				Client.Close()
			End Try
		End Sub
	End Class
End Namespace
