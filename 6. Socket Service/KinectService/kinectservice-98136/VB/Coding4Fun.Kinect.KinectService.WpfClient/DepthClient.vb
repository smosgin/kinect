' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Net.Sockets
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.WpfClient
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

		Public Sub New()
			Me.ThreadProcessor = AddressOf DepthThread
		End Sub

		Private Sub DepthThread()
			Try
				Dim ns As NetworkStream = Client.GetStream()
				Dim networkReader As New BinaryReader(ns)
				Dim depthShort() As Short = Nothing

				Do While Client.Connected
					Dim args As New DepthFrameReadyEventArgs()
					Dim dfd As New DepthFrameData()

					Dim size As Integer = networkReader.ReadInt32()
					Dim data() As Byte = networkReader.ReadBytes(size)

					Dim ms As New MemoryStream(data)
					Dim br As New BinaryReader(ms)

					dfd.PlayerIndexBitmask = br.ReadInt32()
					dfd.PlayerIndexBitmaskWidth = br.ReadInt32()

					Dim frame As DepthImageFrame = br.ReadDepthImageFrame()
					dfd.ImageFrame = frame

					Dim dataLength As Integer = CInt(ms.Length - ms.Position)

					If depthShort Is Nothing OrElse depthShort.Length <> dataLength / 2 Then
						depthShort = New Short(dataLength \ 2 - 1){}
					End If

					Buffer.BlockCopy(data, CInt(br.BaseStream.Position), depthShort, 0, dataLength)

					dfd.DepthData = depthShort

					DepthFrame = dfd
					args.DepthFrame = dfd

					Context.Send(Sub()
						RaiseEvent DepthFrameReady(Me, args)
					End Sub, Nothing)
				Loop
			Catch e1 As IOException
				Client.Close()
			End Try
		End Sub
	End Class
End Namespace
