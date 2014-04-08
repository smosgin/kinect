' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Windows.Media.Imaging
Imports Coding4Fun.Kinect.KinectService.Common

Namespace Coding4Fun.Kinect.KinectService.PhoneClient
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
			AddHandler Me.FrameReady, AddressOf ColorClient_FrameReady
		End Sub

		Private Sub ColorClient_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
			Dim ms As New MemoryStream(e.Data)
			Dim br As New BinaryReader(ms)

			Dim args As New ColorFrameReadyEventArgs()
			Dim cfd As ColorFrameData = New ColorFrameData With {.Format = CType(br.ReadInt32(), ImageFormat), .ImageFrame = br.ReadColorImageFrame()}

			If cfd.Format = ImageFormat.Raw Then
				cfd.RawImage = br.ReadBytes(e.Data.Length - BoolSize)
			Else
				Dim bi As New BitmapImage()
				bi.SetSource(New MemoryStream(e.Data, CInt(ms.Position), CInt(ms.Length - ms.Position)))
				cfd.BitmapImage = bi
			End If

			ColorFrame = cfd
			args.ColorFrame = cfd

			RaiseEvent ColorFrameReady(Me, args)
		End Sub
	End Class
End Namespace
