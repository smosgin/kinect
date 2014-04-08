' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports Microsoft.Kinect

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Friend Module Extensions
		<System.Runtime.CompilerServices.Extension> _
		Public Sub Write(ByVal bw As BinaryWriter, ByVal frame As ImageFrame)
			bw.Write(frame.BytesPerPixel)
			bw.Write(frame.FrameNumber)
			bw.Write(frame.Height)
			bw.Write(frame.PixelDataLength)
			bw.Write(frame.Timestamp)
			bw.Write(frame.Width)
		End Sub
	End Module
End Namespace
