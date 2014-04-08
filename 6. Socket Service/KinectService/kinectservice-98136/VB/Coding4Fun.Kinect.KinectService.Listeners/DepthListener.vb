' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.Kinect

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Public Class DepthListener
		Inherits KinectListener
		Private ReadOnly _fps As Integer = -1
		Private _frameCount As Integer
		Private ReadOnly _memoryStream As New MemoryStream()
		Private ReadOnly binaryWriter As BinaryWriter
		Private _depth() As Short
		Private _depthBytes() As Byte

		Private privateDepthImageFrame As DepthImageFrame
		Public Property DepthImageFrame() As DepthImageFrame
			Get
				Return privateDepthImageFrame
			End Get
			Private Set(ByVal value As DepthImageFrame)
				privateDepthImageFrame = value
			End Set
		End Property

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer, ByVal fps As Integer)
			VerifyConstructorArguments(kinect, port)

			If fps < -1 OrElse fps > 30 Then
				Throw New ArgumentException("FPS value must be between 1 and 30, inclusive.")
			End If

			_fps = fps

			Me.Kinect = kinect
			Me.Port = port
			AddHandler Me.Kinect.DepthFrameReady, AddressOf Kinect_DepthFrameReady

			binaryWriter = New BinaryWriter(_memoryStream)
		End Sub

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer)
			Me.New(kinect, port, -1)
		End Sub

		Private Sub Kinect_DepthFrameReady(ByVal sender As Object, ByVal e As DepthImageFrameReadyEventArgs)
			If ClientList.Count > 0 Then
				Dim frame As DepthImageFrame = e.OpenDepthImageFrame()
				Me.DepthImageFrame = frame

				If frame IsNot Nothing Then
					_memoryStream.SetLength(0)

					binaryWriter.Write(DepthImageFrame.PlayerIndexBitmask)
					binaryWriter.Write(DepthImageFrame.PlayerIndexBitmaskWidth)

					binaryWriter.Write(frame)
					binaryWriter.Write(CInt(Fix(frame.Format)))

					If _depth Is Nothing OrElse _depth.Length <> frame.PixelDataLength Then
						_depth = New Short(frame.PixelDataLength - 1){}
					End If
					frame.CopyPixelDataTo(_depth)

					If _depthBytes Is Nothing OrElse _depthBytes.Length <> _depth.Length * 2 Then
						_depthBytes = New Byte(_depth.Length * 2 - 1){}
					End If

					Buffer.BlockCopy(_depth, 0, _depthBytes, 0, _depthBytes.Length)

					binaryWriter.Write(_depthBytes)

					_frameCount += 1

					If _fps = -1 OrElse (_frameCount > 0 AndAlso (_frameCount Mod (GetFps(frame.Format) \ _fps)) = 0) Then
						Parallel.For(0, ClientList.Count, Sub(index)
							Dim sc As SocketClient = ClientList(index)
							Dim data() As Byte = _memoryStream.ToArray()
							sc.Send(BitConverter.GetBytes(data.Length))
							sc.Send(data)
						End Sub)
					End If
					frame.Dispose()

					RemoveClients()
				End If
			End If
		End Sub

		Private Function GetFps(ByVal format As DepthImageFormat) As Integer
			Select Case format
				Case DepthImageFormat.Undefined
					Return 0
				Case DepthImageFormat.Resolution640x480Fps30, DepthImageFormat.Resolution320x240Fps30, DepthImageFormat.Resolution80x60Fps30
					Return 30
				Case Else
					Throw New ArgumentOutOfRangeException("format")
			End Select
		End Function
	End Class
End Namespace
