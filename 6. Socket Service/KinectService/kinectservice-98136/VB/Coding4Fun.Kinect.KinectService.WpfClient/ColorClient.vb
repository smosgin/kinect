' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Net.Sockets
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.WpfClient
	Public Class ColorClient
		Inherits KinectServiceClient
		Public Event ColorFrameReady As EventHandler(Of ColorFrameReadyEventArgs)
		Private privateColorFrame As ColorFrameData
		Public Property ColorFrame() As ColorFrameData
			Get
				Return privateColorFrame
			End Get
			Private Set(ByVal value As ColorFrameData)
				privateColorFrame = value
			End Set
		End Property

		Public Sub New()
			Me.ThreadProcessor = AddressOf ColorThread
		End Sub

		Private Sub ColorThread()
			Try
				Dim ns As NetworkStream = Client.GetStream()
				Dim reader As New BinaryReader(ns)

				Do While Client.Connected
					Dim args As New ColorFrameReadyEventArgs()
					Dim cfd As New ColorFrameData()

					Dim size As Integer = reader.ReadInt32()
					Dim data() As Byte = reader.ReadBytes(size)

					Dim ms As New MemoryStream(data)
					Dim br As New BinaryReader(ms)

					cfd.Format = CType(br.ReadInt32(), ImageFormat)
					cfd.ImageFrame = br.ReadColorImageFrame()

					Dim msData As New MemoryStream(data, CInt(ms.Position), CInt(ms.Length - ms.Position))

					Context.Send(Sub()
						If cfd.Format = ImageFormat.Raw Then
							cfd.RawImage = ms.ToArray()
							Else
								Dim bi As New BitmapImage()
								bi.BeginInit()
								bi.StreamSource = msData
								bi.EndInit()
								cfd.BitmapImage = bi
							End If
							ColorFrame = cfd
							args.ColorFrame = cfd
							RaiseEvent ColorFrameReady(Me, args)
					End Sub, Nothing)
				Loop
			Catch e1 As IOException
				Client.Close()
			End Try
		End Sub
	End Class
End Namespace
