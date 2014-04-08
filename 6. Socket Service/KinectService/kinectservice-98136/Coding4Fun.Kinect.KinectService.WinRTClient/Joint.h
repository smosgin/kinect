#pragma once

#include "SkeletonPoint.h"
#include "Enums.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref struct Joint sealed
				{
					property JointType JointType;
					property SkeletonPoint^ Position;
					property JointTrackingState TrackingState;

					internal:
						void ReadJointData(Windows::Storage::Streams::DataReader^ dr)
						{
							JointType = (Coding4Fun::Kinect::KinectService::WinRTClient::JointType)dr->ReadInt32();
							
							Position = ref new SkeletonPoint();
							Position->ReadSkeletonPointData(dr);
							TrackingState = (JointTrackingState)dr->ReadInt32();
						}
				};
			}
		}
	}
}