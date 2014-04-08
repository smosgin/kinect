' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Namespace Coding4Fun.Kinect.KinectService.Common
	Public Class SkeletonFrameData
		Public Property FloorClipPlane() As Tuple(Of Single,Single,Single,Single)
		Public Property FrameNumber() As Integer
		Public Property SkeletonArrayLength() As Integer
		Public Property Timestamp() As Long
		Public Property Skeletons() As Skeleton()
	End Class
End Namespace
