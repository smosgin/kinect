#pragma once

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public interface class IImageFrame
				{
					public:
						property int BytesPerPixel;
						property int FrameNumber;
						property int Height;
						property int PixelDataLength;
						property int64 Timestamp;
						property int Width;
				};
			}
		}
	}
}