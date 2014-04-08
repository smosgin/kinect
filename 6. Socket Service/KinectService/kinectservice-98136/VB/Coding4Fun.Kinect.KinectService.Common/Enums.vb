' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Namespace Coding4Fun.Kinect.KinectService.Common
	Public Enum ImageFormat
		Jpeg
		Png
		Raw
	End Enum

	Public Enum ColorImageFormat
		Undefined
		RgbResolution640x480Fps30
		RgbResolution1280x960Fps12
		YuvResolution640x480Fps15
		RawYuvResolution640x480Fps15
	End Enum

	Public Enum DepthImageFormat
		Undefined
		Resolution640x480Fps30
		Resolution320x240Fps30
		Resolution80x60Fps30
	End Enum


	<Flags> _
	Public Enum FrameEdges
		Bottom = 8
		Left = 2
		None = 0
		Right = 1
		Top = 4
	End Enum

	Public Enum SkeletonTrackingState
		NotTracked
		PositionOnly
		Tracked
	End Enum

	Public Enum JointType
		HipCenter
		Spine
		ShoulderCenter
		Head
		ShoulderLeft
		ElbowLeft
		WristLeft
		HandLeft
		ShoulderRight
		ElbowRight
		WristRight
		HandRight
		HipLeft
		KneeLeft
		AnkleLeft
		FootLeft
		HipRight
		KneeRight
		AnkleRight
		FootRight
	End Enum

	Public Enum JointTrackingState
		NotTracked
		Inferred
		Tracked
	End Enum
End Namespace
