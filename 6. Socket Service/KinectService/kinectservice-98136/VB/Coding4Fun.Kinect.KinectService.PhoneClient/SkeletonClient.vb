' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.PhoneClient
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
			AddHandler Me.FrameReady, AddressOf SkeletonClient_FrameReady
		End Sub

		Private Sub SkeletonClient_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
			Dim ms As New MemoryStream(e.Data)
			Dim br As New BinaryReader(ms)

			Dim frame As SkeletonFrameData = br.ReadSkeletonFrame()

			Dim args As SkeletonFrameReadyEventArgs = New SkeletonFrameReadyEventArgs With {.SkeletonFrame = frame}
			SkeletonFrame = frame

			RaiseEvent SkeletonFrameReady(Me, args)
		End Sub
	End Class
End Namespace
