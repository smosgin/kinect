' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.PhoneClient
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
			AddHandler Me.FrameReady, AddressOf AudioClient_FrameReady
		End Sub

		Private Sub AudioClient_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
			Dim args As New AudioFrameReadyEventArgs()
			Dim afd As New AudioFrameData()
			afd.AudioData = e.Data

			args.AudioFrame = afd
			AudioFrame = afd

			RaiseEvent AudioFrameReady(Me, args)
		End Sub
	End Class
End Namespace
