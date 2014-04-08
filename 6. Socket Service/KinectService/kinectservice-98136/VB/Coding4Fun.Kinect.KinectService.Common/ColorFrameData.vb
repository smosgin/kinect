' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Media.Imaging

Namespace Coding4Fun.Kinect.KinectService.Common
	Public Class ColorFrameData
		Public Property ImageFrame() As ColorImageFrame
		Public Property Format() As ImageFormat
		Public Property BitmapImage() As BitmapImage
		Public Property RawImage() As Byte()
	End Class
End Namespace