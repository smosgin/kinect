#pragma once

#include "ColorFrameData.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public delegate void ColorFrameReadyHandler(Platform::Object^ source, ColorFrameData^ args);

				public ref class ColorClient sealed
				{
					public:
						property bool IsConnected;
						property ColorFrameData^ ColorFrame;

						event ColorFrameReadyHandler^ ColorFrameReady;

						Windows::Foundation::IAsyncAction^ Connect(Platform::String^ address, int port);
						void Disconnect();

					private:
						Windows::Networking::Sockets::StreamSocket^ _socket;
						Windows::UI::Core::CoreDispatcher^ _dispatcher;
						void ReceiveFrameLoop(Windows::Storage::Streams::DataReader^ dr);
				};
			}
		}
	}
}