#pragma once

#include "Enums.h"
#include "ColorImageFrame.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class ColorFrameData sealed
				{
					public:
						property ColorImageFrame^ ImageFrame;
						property ImageFormat Format;
						property Windows::Storage::Streams::IBuffer^ PixelBuffer;

					internal:
						void ReadColorFrameData(Windows::Storage::Streams::DataReader^ dr)
						{
							Format = (ImageFormat)dr->ReadInt32();
							ImageFrame = ref new Coding4Fun::Kinect::KinectService::WinRTClient::ColorImageFrame();
							ImageFrame->ReadImageFrameData(dr);
						}
				};
			}
		}
	}
}