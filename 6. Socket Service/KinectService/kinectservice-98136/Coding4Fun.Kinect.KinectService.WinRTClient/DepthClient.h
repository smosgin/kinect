#pragma once

#include "DepthFrameData.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public delegate void DepthFrameReadyHandler(Platform::Object^ source, DepthFrameData^ args);

				public ref class DepthClient sealed
				{
					public:
						property bool IsConnected;
						property DepthFrameData^ DepthFrame;

						event DepthFrameReadyHandler^ DepthFrameReady;

						Windows::Foundation::IAsyncAction^ Connect(Platform::String^ address, int port);
						void Disconnect();

					private:
						Windows::Networking::Sockets::StreamSocket^ _socket;
						Windows::UI::Core::CoreDispatcher^ _dispatcher;
						void ReceiveFrameLoop(Windows::Storage::Streams::DataReader^ dr);
						DepthFrameData^ ReadDepthFrameData(Windows::Storage::Streams::DataReader^ dr);
				};
			}
		}
	}
}