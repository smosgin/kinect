#include "pch.h"
#include "SkeletonClient.h"
#include "Enums.h"
#include "Skeleton.h"
#include "SkeletonFrameData.h"

using namespace Coding4Fun::Kinect::KinectService::WinRTClient;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::Networking;
using namespace Windows::Networking::Sockets;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Concurrency;

IAsyncAction^ SkeletonClient::Connect(String^ address, int port)
{
	_dispatcher = CoreWindow::GetForCurrentThread()->Dispatcher;

	return create_async([=]
	{
		_socket = ref new StreamSocket();
		task<void> (_socket->ConnectAsync(ref new HostName(address), port.ToString(), SocketProtectionLevel::PlainSocket)).get();
		IsConnected = true;
		auto dr = ref new DataReader(_socket->InputStream);
		dr->ByteOrder = ByteOrder::LittleEndian;
		ReceiveFrameLoop(dr);	
	});
}

void SkeletonClient::Disconnect()
{
	if(_socket)
		delete _socket;
	IsConnected = false;
}

void SkeletonClient::ReceiveFrameLoop(DataReader^ socketReader)
{
	std::shared_ptr<int> dataSize(new int);

	SkeletonFrameData^ sfd = ref new SkeletonFrameData();

	create_task(socketReader->LoadAsync(sizeof(int32))).then([=] (unsigned int sizeRead)
	{
		*dataSize = socketReader->ReadInt32();

		return socketReader->LoadAsync(*dataSize);
	}).then([=] (unsigned int bytesRead)
	{
		sfd->ReadSkeletonFrameData(socketReader);

		SkeletonFrame = sfd;

		if(_dispatcher != nullptr && !_dispatcher->HasThreadAccess)
		{
			// want this to run sync so we don't flood events to the client
			create_task(_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
				ref new Windows::UI::Core::DispatchedHandler([=] ()
				{
					SkeletonFrameReady(this, sfd);
				}, Platform::CallbackContext::Any))).get();
		}
		else
			SkeletonFrameReady(this, sfd);
	}).then([=] (task<void> t)
	{
		try
		{
			t.get();
		}
		catch(Exception^ ex)
		{
			if(ex->HResult != HRESULT_FROM_WIN32(ERROR_OPERATION_ABORTED))
				throw;
		}

		if(IsConnected)
			ReceiveFrameLoop(socketReader);
	});
}
