#pragma once

#include "SkeletonPoint.h"
#include "Joint.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public ref class Skeleton sealed
				{
					public:
						property FrameEdges ClippedEdges;
						property Windows::Foundation::Collections::IVector<Joint^>^ Joints;
						property SkeletonPoint^ Position;
						property int TrackingId;
						property SkeletonTrackingState TrackingState;

					internal:
						void ReadSkeletonData(Windows::Storage::Streams::DataReader^ dr)
						{
							ClippedEdges = (FrameEdges)dr->ReadInt32();
							int jointCount = dr->ReadInt32();
							Joints = ref new Platform::Collections::Vector<Joint^>(jointCount);
						
							for(int jx = 0; jx < jointCount; jx++)
							{
								Joint^ j = ref new Joint();
								Joints->SetAt(jx, j);
								j->ReadJointData(dr);
							}

							Position = ref new SkeletonPoint();
							Position->ReadSkeletonPointData(dr);
							TrackingId = dr->ReadInt32();
							TrackingState = (SkeletonTrackingState)dr->ReadInt32();
						}
				};
			}
		}
	}
}