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
				public ref struct Vector4 sealed
				{
					property float32 X;
					property float32 Y;
					property float32 Z;
					property float32 W;

					internal:
						void ReadVector4Data(Windows::Storage::Streams::DataReader^ dr)
						{
							X = dr->ReadSingle();
							Y = dr->ReadSingle();
							Z = dr->ReadSingle();
							W = dr->ReadSingle();
						}
				};
			}
		}
	}
}