' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.PhoneClient
	Public Class DepthClient
		Inherits KinectServiceClient
		Public Event DepthFrameReady As EventHandler(Of DepthFrameReadyEventArgs)
		Private privateDepthFrame As DepthFrameData
		Public Property DepthFrame() As DepthFrameData
			Get
				Return privateDepthFrame
			End Get
			Private Set(ByVal value As DepthFrameData)
				privateDepthFrame = value
			End Set
		End Property

		Private _depthShort() As Short

		Public Sub New()
			AddHandler Me.FrameReady, AddressOf DepthClient_FrameReady
		End Sub

		Private Sub DepthClient_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
			Dim args As New DepthFrameReadyEventArgs()
			Dim dfd As New DepthFrameData()

			Dim ms As New MemoryStream(e.Data)
			Dim br As New BinaryReader(ms)

			dfd.PlayerIndexBitmask = br.ReadInt32()
			dfd.PlayerIndexBitmaskWidth = br.ReadInt32()

			Dim frame As DepthImageFrame = br.ReadDepthImageFrame()
			dfd.ImageFrame = frame

			Dim dataLength As Integer = CInt(ms.Length - ms.Position)

			If _depthShort Is Nothing OrElse _depthShort.Length <> dataLength / 2 Then
				_depthShort = New Short(dataLength \ 2 - 1){}
			End If

			Buffer.BlockCopy(e.Data, CInt(br.BaseStream.Position), _depthShort, 0, dataLength)

			dfd.DepthData = _depthShort

			DepthFrame = dfd
			args.DepthFrame = dfd

			RaiseEvent DepthFrameReady(Me, args)
		End Sub
	End Class
End Namespace
