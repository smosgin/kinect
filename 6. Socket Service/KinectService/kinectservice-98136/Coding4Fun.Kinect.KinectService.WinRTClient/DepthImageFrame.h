#pragma once

#include "ImageFrame.h"
#include "Enums.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class DepthImageFrame sealed : public IImageFrame
				{
					public:
						virtual property int BytesPerPixel;
						virtual property int FrameNumber;
						virtual property int Height;
						virtual property int PixelDataLength;
						virtual property int64 Timestamp;
						virtual property int Width;

						property DepthImageFormat Format;

					internal:
						void ReadImageFrameData(Windows::Storage::Streams::DataReader^ dr)
						{
							BytesPerPixel = dr->ReadInt32();
							FrameNumber = dr->ReadInt32();
							Height = dr->ReadInt32();
							PixelDataLength = dr->ReadInt32();
							Timestamp = dr->ReadInt64();
							Width = dr->ReadInt32();

							Format = (DepthImageFormat)dr->ReadInt32();
						}
				};
			}
		}
	}
}