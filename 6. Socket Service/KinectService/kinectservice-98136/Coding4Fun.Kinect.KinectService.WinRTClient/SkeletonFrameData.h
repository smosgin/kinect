#pragma once

#include "Skeleton.h"
#include "Vector4.h"
#include "Enums.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class SkeletonFrameData sealed
				{
					public:
						property Vector4^ FloorClipPlane;
						property int FrameNumber;
						property int SkeletonArrayLength;
						property int64 Timestamp;
						property Windows::Foundation::Collections::IVector<Skeleton^>^ Skeletons;

					internal:
						void ReadSkeletonFrameData(Windows::Storage::Streams::DataReader^ dr)
						{
							FloorClipPlane = ref new Vector4();

							FloorClipPlane->ReadVector4Data(dr);
							FrameNumber = dr->ReadInt32();
							SkeletonArrayLength = dr->ReadInt32();
							Timestamp = dr->ReadInt64();

							Skeletons = ref new Platform::Collections::Vector<Skeleton^>(SkeletonArrayLength);
							for(int i = 0; i < SkeletonArrayLength; i++)
							{
								Skeleton^ s = ref new Skeleton();
								Skeletons->SetAt(i, s);
								s->ReadSkeletonData(dr);
							}
						}
				};
			}
		}
	}
}