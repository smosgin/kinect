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

Namespace Coding4Fun.Kinect.KinectService.WindowsService
	Friend Class ServiceEngine
		Private Shared ReadOnly ColorPort As Integer = My.Settings.Default.ColorPort
		Private Shared ReadOnly AudioPort As Integer = My.Settings.Default.AudioPort
		Private Shared ReadOnly DepthPort As Integer = My.Settings.Default.DepthPort
		Private Shared ReadOnly SkeletonPort As Integer = My.Settings.Default.SkeletonPort

		Private _depthListener As DepthListener
		Private _colorListener As ColorListener
		Private _skeletonListener As SkeletonListener

		Private _kinect As KinectSensor
		Private _audioListener As AudioListener

		Public Sub Start()
			If KinectSensor.KinectSensors.Count = 0 Then
				Debug.WriteLine("No Kinects found.")
				Environment.Exit(-1)
			End If

			_kinect = KinectSensor.KinectSensors(0)

			_kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30)
			_kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30)
			_kinect.SkeletonStream.Enable()

			_kinect.Start()

			_colorListener = New ColorListener(_kinect, ColorPort, ImageFormat.Jpeg)
			_colorListener.Start()

			_depthListener = New DepthListener(_kinect, DepthPort)
			_depthListener.Start()

			_skeletonListener = New SkeletonListener(_kinect, SkeletonPort)
			_skeletonListener.Start()

			_audioListener = New AudioListener(_kinect, AudioPort)
			_audioListener.Start()

			Dim addresses() As IPAddress = Dns.GetHostAddresses(Dns.GetHostName())
			Dim s As String = String.Empty
			For Each address As IPAddress In addresses
				s &= Environment.NewLine & address.ToString()
			Next address

			Debug.WriteLine("Waiting for client on..." & s)
		End Sub

		Public Sub [Stop]()
			_depthListener.Stop()
			_colorListener.Stop()
			_skeletonListener.Stop()
			_audioListener.Stop()
		End Sub
	End Class
End Namespace
