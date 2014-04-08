﻿' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Coding4Fun.Kinect.KinectService.Common
Imports Coding4Fun.Kinect.KinectService.WpfClient

Namespace Coding4Fun.Kinect.KinectService.Samples.WpfSample
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Private _outputBitmap As WriteableBitmap

		Private _colorClient As ColorClient
		Private _depthClient As DepthClient
		Private _skeletonClient As SkeletonClient

		Public Sub New()
			InitializeComponent()

			_colorClient = New ColorClient()
			AddHandler _colorClient.ColorFrameReady, AddressOf client_ColorFrameReady

			_depthClient = New DepthClient()
			AddHandler _depthClient.DepthFrameReady, AddressOf client_DepthFrameReady

			_skeletonClient = New SkeletonClient()
			AddHandler _skeletonClient.SkeletonFrameReady, AddressOf client_SkeletonFrameReady
		End Sub

		Private Sub StartColor_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If Not _colorClient.IsConnected Then
				_colorClient.Connect(ServerIp.Text, 4530)
			Else
				_colorClient.Disconnect()
			End If
		End Sub

		Private Sub client_ColorFrameReady(ByVal sender As Object, ByVal e As ColorFrameReadyEventArgs)
			Me.Color.Source = e.ColorFrame.BitmapImage
		End Sub

		Private Sub StartDepth_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If Not _depthClient.IsConnected Then
				_depthClient.Connect(ServerIp.Text, 4531)
			Else
				_depthClient.Disconnect()
			End If
		End Sub

		Private Sub client_DepthFrameReady(ByVal sender As Object, ByVal e As DepthFrameReadyEventArgs)
			If _outputBitmap Is Nothing OrElse _outputBitmap.PixelWidth <> e.DepthFrame.ImageFrame.Width OrElse _outputBitmap.PixelHeight <> e.DepthFrame.ImageFrame.Height Then
				Me._outputBitmap = New WriteableBitmap(e.DepthFrame.ImageFrame.Width, e.DepthFrame.ImageFrame.Height, 96, 96, PixelFormats.Bgr32, Nothing) ' DpiY -  DpiX

				Me.Depth.Source = Me._outputBitmap
			End If

			Dim convertedDepthBits() As Byte = Me.ConvertDepthFrame(e.DepthFrame.DepthData, e)

			Me._outputBitmap.WritePixels(New Int32Rect(0, 0, e.DepthFrame.ImageFrame.Width, e.DepthFrame.ImageFrame.Height), convertedDepthBits, e.DepthFrame.ImageFrame.Width * 4, 0)
		End Sub

		Private Function ConvertDepthFrame(ByVal depthFrame() As Short, ByVal args As DepthFrameReadyEventArgs) As Byte()
			Dim intensityShiftByPlayerR() As Integer = { 1, 2, 0, 2, 0, 0, 2, 0 }
			Dim intensityShiftByPlayerG() As Integer = { 1, 2, 2, 0, 2, 0, 0, 1 }
			Dim intensityShiftByPlayerB() As Integer = { 1, 0, 2, 2, 0, 2, 0, 2 }

			Const RedIndex As Integer = 2
			Const GreenIndex As Integer = 1
			Const BlueIndex As Integer = 0

			Dim depthFrame32(args.DepthFrame.ImageFrame.Width * args.DepthFrame.ImageFrame.Height * 4 - 1) As Byte

			Dim i16 As Integer = 0
			Dim i32 As Integer = 0
			Do While i16 < depthFrame.Length AndAlso i32 < depthFrame.Length * 4
				Dim player As Integer = depthFrame(i16) And args.DepthFrame.PlayerIndexBitmask
				Dim realDepth As Integer = depthFrame(i16) >> args.DepthFrame.PlayerIndexBitmaskWidth

				' transform 13-bit depth information into an 8-bit intensity appropriate
				' for display (we disregard information in most significant bit)
				Dim intensity As Byte = CByte(Not(realDepth >> 4))

				If player = 0 AndAlso realDepth = 0 Then
					' white 
					depthFrame32(i32 + RedIndex) = 255
					depthFrame32(i32 + GreenIndex) = 255
					depthFrame32(i32 + BlueIndex) = 255
				Else
					' tint the intensity by dividing by per-player values
					depthFrame32(i32 + RedIndex) = CByte(intensity >> intensityShiftByPlayerR(player))
					depthFrame32(i32 + GreenIndex) = CByte(intensity >> intensityShiftByPlayerG(player))
					depthFrame32(i32 + BlueIndex) = CByte(intensity >> intensityShiftByPlayerB(player))
				End If
				i16 += 1
				i32 += 4
			Loop

			Return depthFrame32
		End Function


		Private Sub StartSkeleton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If Not _skeletonClient.IsConnected Then
				_skeletonClient.Connect(ServerIp.Text, 4532)
			Else
				_skeletonClient.Disconnect()
			End If
		End Sub

		Private Sub client_SkeletonFrameReady(ByVal sender As Object, ByVal e As SkeletonFrameReadyEventArgs)
			Dim skeleton As Skeleton = ( _
			    From s In e.SkeletonFrame.Skeletons _
			    Where s.TrackingState = SkeletonTrackingState.Tracked _
			    Select s).FirstOrDefault()

			If skeleton Is Nothing Then
				Return
			End If

			SetEllipsePosition(headEllipse, skeleton.Joints(CInt(JointType.Head)))
			SetEllipsePosition(leftEllipse, skeleton.Joints(CInt(JointType.HandLeft)))
			SetEllipsePosition(rightEllipse, skeleton.Joints(CInt(JointType.HandRight)))
		End Sub

		Private Sub SetEllipsePosition(ByVal ellipse As FrameworkElement, ByVal joint As Joint)
			Dim scaledJoint = ScaleTo(joint, CInt(Skeleton.Width), CInt(Skeleton.Height),.5F,.5F)

			Canvas.SetLeft(ellipse, scaledJoint.Position.X)
			Canvas.SetTop(ellipse, scaledJoint.Position.Y)
		End Sub

		Private Function ScaleTo(ByVal joint As Joint, ByVal width As Integer, ByVal height As Integer, ByVal skeletonMaxX As Single, ByVal skeletonMaxY As Single) As Joint
			Dim pos As New SkeletonPoint() With {.X = Scale(width, skeletonMaxX, joint.Position.X), .Y = Scale(height, skeletonMaxY, -joint.Position.Y), .Z = joint.Position.Z}

			Dim j As New Joint() With {.TrackingState = joint.TrackingState, .Position = pos}

			Return j
		End Function

		Private Function Scale(ByVal maxPixel As Integer, ByVal maxSkeleton As Single, ByVal position As Single) As Single
			Dim value As Single = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel\2))
			If value > maxPixel Then
				Return maxPixel
			End If
			If value < 0 Then
				Return 0
			End If
			Return value
		End Function
	End Class
End Namespace
