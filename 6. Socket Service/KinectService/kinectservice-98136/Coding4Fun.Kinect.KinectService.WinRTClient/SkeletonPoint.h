#pragma once

#include "Skeleton.h"
#include "Enums.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref struct SkeletonPoint sealed
				{
					property float32 X;
					property float32 Y;
					property float32 Z;

					internal:
						void ReadSkeletonPointData(Windows::Storage::Streams::DataReader^ dr)
						{
							X = dr->ReadSingle();
							Y = dr->ReadSingle();
							Z = dr->ReadSingle();
						}

					private:
						float32 _x;
						float32 _y;
						float32 _z;
				};
			}
		}
	}
}