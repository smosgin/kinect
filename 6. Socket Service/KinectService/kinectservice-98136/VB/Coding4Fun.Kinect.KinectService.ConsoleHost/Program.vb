' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Net
Imports Coding4Fun.Kinect.KinectService.Common
Imports Coding4Fun.Kinect.KinectService.Listeners
Imports Microsoft.Kinect
Imports ColorImageFormat = Microsoft.Kinect.ColorImageFormat
Imports DepthImageFormat = Microsoft.Kinect.DepthImageFormat

Namespace Coding4Fun.Kinect.KinectService.ConsoleHost
	Friend Class Program
		Private Shared ReadOnly VideoPort As Integer = My.Settings.Default.ColorPort
		Private Shared ReadOnly AudioPort As Integer = My.Settings.Default.AudioPort
		Private Shared ReadOnly DepthPort As Integer = My.Settings.Default.DepthPort
		Private Shared ReadOnly SkeletonPort As Integer = My.Settings.Default.SkeletonPort

		Private Shared _depthListener As DepthListener
		Private Shared _videoListener As ColorListener
		Private Shared _skeletonListener As SkeletonListener
		Private Shared _audioListener As AudioListener

		Private Shared _kinect As KinectSensor

		Shared Sub Main()
			Do While KinectSensor.KinectSensors.Count = 0
				Console.WriteLine("Please insert a Kinect sensor and press any key to continue.")
				Console.ReadKey()
			Loop

			_kinect = KinectSensor.KinectSensors(0)

			_kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30)
			_kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30)
			_kinect.SkeletonStream.Enable()

			_kinect.Start()

			_videoListener = New ColorListener(_kinect, VideoPort, ImageFormat.Jpeg)
			_videoListener.Start()

			_depthListener = New DepthListener(_kinect, DepthPort)
			_depthListener.Start()

			_skeletonListener = New SkeletonListener(_kinect, SkeletonPort)
			_skeletonListener.Start()

			_audioListener = New AudioListener(_kinect, AudioPort)
			_audioListener.Start()

			Dim addresses() As IPAddress = Dns.GetHostAddresses(Dns.GetHostName())
			Dim s As String = String.Empty
			For Each address As IPAddress In addresses
				s &= Environment.NewLine & address.ToString
			Next address

			Console.WriteLine("Waiting for client on..." & s)

			Console.WriteLine("Press any key to exit...")
			Console.ReadKey()

			If _depthListener IsNot Nothing Then
				_depthListener.Stop()
			End If

			If _videoListener IsNot Nothing Then
				_videoListener.Stop()
			End If

			If _skeletonListener IsNot Nothing Then
				_skeletonListener.Stop()
			End If

			If _audioListener IsNot Nothing Then
				_audioListener.Stop()
			End If
		End Sub
	End Class
End Namespace
