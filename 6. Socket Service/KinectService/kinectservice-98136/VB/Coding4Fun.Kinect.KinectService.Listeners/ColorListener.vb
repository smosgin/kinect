' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.IO
Imports System.Threading.Tasks
Imports Coding4Fun.Kinect.KinectService.Common
Imports Microsoft.Kinect
Imports ColorImageFormat = Microsoft.Kinect.ColorImageFormat
Imports ColorImageFrame = Microsoft.Kinect.ColorImageFrame

Namespace Coding4Fun.Kinect.KinectService.Listeners
	Public Class ColorListener
		Inherits KinectListener
		Private ReadOnly _format As ImageFormat
		Private ReadOnly _scale As Double = Double.NaN
		Private ReadOnly _fps As Integer = -1
		Private _frameCount As Integer

		Private _bitmap As WriteableBitmap
		Private _image() As Byte
		Private ReadOnly _memoryStream As New MemoryStream()
		Private ReadOnly _binaryWriter As BinaryWriter

		Private privateColorImageFrame As Microsoft.Kinect.ColorImageFrame
		Public Property ColorImageFrame() As Microsoft.Kinect.ColorImageFrame
			Get
				Return privateColorImageFrame
			End Get
			Private Set(ByVal value As Microsoft.Kinect.ColorImageFrame)
				privateColorImageFrame = value
			End Set
		End Property

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer, ByVal format As ImageFormat, ByVal scale As Double, ByVal fps As Integer)
			VerifyConstructorArguments(kinect, port)

			If scale < 0 Then
				Throw New ArgumentException("Scale must be > 0", "scale")
			End If

			If fps < -1 OrElse fps > 30 Then
				Throw New ArgumentException("FPS value must be between 1 and 30, inclusive.")
			End If

			_format = format
			_scale = scale
			_fps = fps

			Me.Kinect = kinect
			Me.Port = port
			AddHandler Me.Kinect.ColorFrameReady, AddressOf Kinect_ColorFrameReady

			_binaryWriter = New BinaryWriter(_memoryStream)
		End Sub

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer, ByVal format As ImageFormat, ByVal scale As Double)
			Me.New(kinect, port, format, scale, -1)
		End Sub

		Public Sub New(ByVal kinect As KinectSensor, ByVal port As Integer, ByVal format As ImageFormat)
			Me.New(kinect, port, format, Double.NaN)
		End Sub

		Private Sub Kinect_ColorFrameReady(ByVal sender As Object, ByVal e As ColorImageFrameReadyEventArgs)
			If ClientList.Count > 0 Then
				Dim frame As ColorImageFrame = e.OpenColorImageFrame()
				Me.ColorImageFrame = frame

				If frame IsNot Nothing Then
					If _format <> ImageFormat.Raw AndAlso (frame.Format = ColorImageFormat.RawYuvResolution640x480Fps15 OrElse frame.Format = ColorImageFormat.YuvResolution640x480Fps15) Then
						Throw New ArgumentException("YUV color formats are not supported.  Please use an RGB ColorImageFormat.")
					End If

					_memoryStream.SetLength(0)

					_binaryWriter.Write(CInt(_format))

					_binaryWriter.Write(frame)
					_binaryWriter.Write(CInt(Fix(frame.Format)))

					If _image Is Nothing OrElse _image.Length < frame.PixelDataLength Then
						_image = New Byte(frame.PixelDataLength - 1){}
					End If

					frame.CopyPixelDataTo(_image)

					Dim encoder As BitmapEncoder = Nothing

					Select Case _format
						Case ImageFormat.Jpeg
							encoder = New JpegBitmapEncoder()
						Case ImageFormat.Png
							encoder = New PngBitmapEncoder()
						Case ImageFormat.Raw
					End Select

					If encoder IsNot Nothing Then
						If _bitmap Is Nothing OrElse _bitmap.PixelWidth <> frame.Width OrElse _bitmap.PixelHeight <> frame.Height Then
							_bitmap = New WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, Nothing)
						End If

						_bitmap.WritePixels(New Int32Rect(0, 0, frame.Width, frame.Height), _image, frame.Width * frame.BytesPerPixel, 0)

						If Double.IsNaN(_scale) Then
							encoder.Frames.Add(BitmapFrame.Create(_bitmap))
						Else
							Dim bs As BitmapSource = New TransformedBitmap(_bitmap, New ScaleTransform(_scale, _scale))
							encoder.Frames.Add(BitmapFrame.Create(bs))
						End If

						encoder.Save(_memoryStream)
					Else
						_binaryWriter.Write(_image)
					End If

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

		Private Function GetFps(ByVal format As ColorImageFormat) As Integer
			Select Case format
				Case ColorImageFormat.Undefined
					Return 0
				Case ColorImageFormat.RgbResolution640x480Fps30
					Return 30
				Case ColorImageFormat.RgbResolution1280x960Fps12
					Return 12
				Case ColorImageFormat.YuvResolution640x480Fps15, ColorImageFormat.RawYuvResolution640x480Fps15
					Return 15
				Case Else
					Throw New ArgumentOutOfRangeException("format")
			End Select
		End Function
	End Class
End Namespace
