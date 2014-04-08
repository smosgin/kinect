#pragma once

#include "AudioFrameData.h"

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public delegate void AudioFrameReadyHandler(Platform::Object^ source, AudioFrameData^ args);

				public ref class AudioClient sealed
				{
					public:
						property bool IsConnected;
						property AudioFrameData^ AudioFrame;

						event AudioFrameReadyHandler^ AudioFrameReady;

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