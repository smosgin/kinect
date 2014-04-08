#pragma once

#include "SkeletonFrameData.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public delegate void SkeletonFrameReadyHandler(Platform::Object^ source, SkeletonFrameData^ args);

				public ref class SkeletonClient sealed
				{
					public:
						property bool IsConnected;
						property SkeletonFrameData^ SkeletonFrame;

						event SkeletonFrameReadyHandler^ SkeletonFrameReady;

						Windows::Foundation::IAsyncAction^ Connect(Platform::String^ address, int port);
						void Disconnect();

					private:
						Windows::Networking::Sockets::StreamSocket^ _socket;
						Windows::UI::Core::CoreDispatcher^ _dispatcher;
						void ReceiveFrameLoop(Windows::Storage::Streams::DataReader^ dr);
						SkeletonFrameData^ ReadSkeletonFrameData(Windows::Storage::Streams::DataReader^ dr);
				};
			}
		}
	}
}