Imports System.Threading
Imports System.Windows
Imports Microsoft.Xna.Framework

Namespace Coding4Fun.Kinect.KinectService.Samples.PhoneSample
	Public Class XnaFrameworkDispatcherService
		Implements IApplicationService
		Private _threadTimer As Timer

		Public Sub TimerAction(ByVal state As Object)
			FrameworkDispatcher.Update()
		End Sub

		Public Sub IApplicationService_StopService() Implements IApplicationService.StopService
			_threadTimer.Dispose()
		End Sub

		Public Sub IApplicationService_StartService(ByVal context As ApplicationServiceContext) Implements IApplicationService.StartService
			_threadTimer = New Timer(AddressOf TimerAction, Nothing, TimeSpan.FromMilliseconds(33), TimeSpan.FromMilliseconds(33))
		End Sub
	End Class
End Namespace
