#pragma once

#include "DepthImageFrame.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class DepthFrameData sealed
				{
					public:
						property int PlayerIndexBitmask;
						property int PlayerIndexBitmaskWidth;
						property DepthImageFrame^ ImageFrame;
						property Windows::Storage::Streams::IBuffer^ DepthData;

					internal:
						void ReadDepthFrameData(Windows::Storage::Streams::DataReader^ dr)
						{
							PlayerIndexBitmask = dr->ReadInt32();
							PlayerIndexBitmaskWidth = dr->ReadInt32();
							this->ImageFrame = ref new Coding4Fun::Kinect::KinectService::WinRTClient::DepthImageFrame();
							this->ImageFrame->ReadImageFrameData(dr);
						}
				};
			}
		}
	}
}