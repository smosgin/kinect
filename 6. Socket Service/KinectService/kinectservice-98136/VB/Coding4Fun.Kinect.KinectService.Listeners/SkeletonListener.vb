' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.Kinect

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Public Class SkeletonListener
		Inherits KinectListener
		Private ReadOnly _memoryStream As New MemoryStream()
		Private ReadOnly _binaryWriter As BinaryWriter
		Private _skeletons() As Skeleton

		Private privateSkeletonFrame As SkeletonFrame
		Public Property SkeletonFrame() As SkeletonFrame
			Get
				Return privateSkeletonFrame
			End Get
			Private Set(ByVal value As SkeletonFrame)
				privateSkeletonFrame = value
			End Set
		End Property

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer)
			VerifyConstructorArguments(kinect, port)

			Me.Kinect = kinect
			Me.Port = port

			AddHandler Me.Kinect.SkeletonFrameReady, AddressOf Kinect_SkeletonFrameReady

			_binaryWriter = New BinaryWriter(_memoryStream)
		End Sub

		Private Sub Kinect_SkeletonFrameReady(ByVal sender As Object, ByVal e As SkeletonFrameReadyEventArgs)
			If ClientList.Count > 0 Then
				Dim frame As SkeletonFrame = e.OpenSkeletonFrame()
				Me.SkeletonFrame = frame

				If frame IsNot Nothing Then
					_memoryStream.SetLength(0)

					If _skeletons Is Nothing OrElse _skeletons.Length <> frame.SkeletonArrayLength Then
						_skeletons = New Skeleton(frame.SkeletonArrayLength - 1){}
					End If

					frame.CopySkeletonDataTo(_skeletons)

					_binaryWriter.Write(frame.FloorClipPlane.Item1)
					_binaryWriter.Write(frame.FloorClipPlane.Item2)
					_binaryWriter.Write(frame.FloorClipPlane.Item3)
					_binaryWriter.Write(frame.FloorClipPlane.Item4)
					_binaryWriter.Write(frame.FrameNumber)
					_binaryWriter.Write(frame.SkeletonArrayLength)
					_binaryWriter.Write(frame.Timestamp)

					For Each s As Skeleton In _skeletons
						_binaryWriter.Write(CInt(s.ClippedEdges))
						_binaryWriter.Write(s.Joints.Count)
						For Each j As Joint In s.Joints
							_binaryWriter.Write(CInt(Fix(j.JointType)))
							_binaryWriter.Write(j.Position.X)
							_binaryWriter.Write(j.Position.Y)
							_binaryWriter.Write(j.Position.Z)
							_binaryWriter.Write(CInt(j.TrackingState))
						Next j
						_binaryWriter.Write(s.Position.X)
						_binaryWriter.Write(s.Position.Y)
						_binaryWriter.Write(s.Position.Z)
						_binaryWriter.Write(s.TrackingId)
						_binaryWriter.Write(CInt(s.TrackingState))
					Next s

					Parallel.For(0, ClientList.Count, Sub(index)
						Dim sc As SocketClient = ClientList(index)
						sc.Send(BitConverter.GetBytes(CInt(_memoryStream.Length)))
						sc.Send(_memoryStream.ToArray())
					End Sub)

					frame.Dispose()

					RemoveClients()
				End If
			End If
		End Sub
	End Class
End Namespace
