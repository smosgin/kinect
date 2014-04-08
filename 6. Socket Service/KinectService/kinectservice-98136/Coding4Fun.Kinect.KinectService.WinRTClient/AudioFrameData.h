#pragma once

#include "Enums.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class AudioFrameData sealed
				{
					public:
						property Windows::Storage::Streams::IBuffer^ AudioData;
				};
			}
		}
	}
}