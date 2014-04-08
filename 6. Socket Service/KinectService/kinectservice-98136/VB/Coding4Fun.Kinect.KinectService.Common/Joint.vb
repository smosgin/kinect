' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Coding4Fun.Kinect.KinectService.Common
	Public Structure Joint
		Public Property JointType() As JointType
		Public Property Position() As SkeletonPoint
		Public Property TrackingState() As JointTrackingState
	End Structure
End Namespace