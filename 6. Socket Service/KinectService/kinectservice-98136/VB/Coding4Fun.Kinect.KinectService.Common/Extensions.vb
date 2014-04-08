' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO

Namespace Coding4Fun.Kinect.KinectService.Common
	Public Module Extensions
		<System.Runtime.CompilerServices.Extension> _
		Public Function ReadSkeletonFrame(ByVal br As BinaryReader) As SkeletonFrameData
			Dim frame As New SkeletonFrameData()

			frame.FloorClipPlane = New Tuple(Of Single,Single,Single,Single)(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
			frame.FrameNumber = br.ReadInt32()
			frame.SkeletonArrayLength = br.ReadInt32()
			frame.Timestamp = br.ReadInt64()

			frame.Skeletons = New Skeleton(frame.SkeletonArrayLength - 1){}
			For i As Integer = 0 To frame.Skeletons.Length - 1
				frame.Skeletons(i) = New Skeleton()
				frame.Skeletons(i).ClippedEdges = CType(br.ReadInt32(), FrameEdges)
				Dim jointCount As Integer = br.ReadInt32()
				frame.Skeletons(i).Joints = New Joint(jointCount - 1){}
				For jx As Integer = 0 To jointCount - 1
					frame.Skeletons(i).Joints(jx).JointType = CType(br.ReadInt32(), JointType)
					frame.Skeletons(i).Joints(jx).Position = New SkeletonPoint With {.X = br.ReadSingle(), .Y = br.ReadSingle(), .Z = br.ReadSingle()}
					frame.Skeletons(i).Joints(jx).TrackingState = CType(br.ReadInt32(), JointTrackingState)
				Next jx
				frame.Skeletons(i).Position = New SkeletonPoint With {.X = br.ReadSingle(), .Y = br.ReadSingle(), .Z = br.ReadSingle()}
				frame.Skeletons(i).TrackingId = br.ReadInt32()
				frame.Skeletons(i).TrackingState = CType(br.ReadInt32(), SkeletonTrackingState)
			Next i

			Return frame
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function ReadColorImageFrame(ByVal br As BinaryReader) As ColorImageFrame
			Dim frame As New ColorImageFrame()
			frame = CType(ReadImageFrame(frame, br), ColorImageFrame)
			frame.Format = CType(br.ReadInt32(), ColorImageFormat)
			Return frame
		End Function

		<System.Runtime.CompilerServices.Extension> _
		Public Function ReadDepthImageFrame(ByVal br As BinaryReader) As DepthImageFrame
			Dim frame As New DepthImageFrame()
			frame = CType(ReadImageFrame(frame, br), DepthImageFrame)
			frame.Format = CType(br.ReadInt32(), DepthImageFormat)
			Return frame
		End Function

		Private Function ReadImageFrame(ByVal frame As ImageFrame, ByVal br As BinaryReader) As ImageFrame
			frame.BytesPerPixel = br.ReadInt32()
			frame.FrameNumber = br.ReadInt32()
			frame.Height = br.ReadInt32()
			frame.PixelDataLength = br.ReadInt32()
			frame.Timestamp = br.ReadInt64()
			frame.Width = br.ReadInt32()

			Return frame
		End Function
	End Module
End Namespace