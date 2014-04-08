' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Coding4Fun.Kinect.KinectService.Common
	Public Class Skeleton
		Public Property ClippedEdges() As FrameEdges
		Public Property Joints() As Joint()
		Public Property Position() As SkeletonPoint
		Public Property TrackingId() As Integer
		Public Property TrackingState() As SkeletonTrackingState
	End Class
End Namespace
