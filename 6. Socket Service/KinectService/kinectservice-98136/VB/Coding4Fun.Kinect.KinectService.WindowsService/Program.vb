' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ServiceProcess
Imports System.Text

Namespace Coding4Fun.Kinect.KinectService.WindowsService
	Friend NotInheritable Class Program

		Private Sub New()
		End Sub

		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Shared Sub Main()
			Dim ServicesToRun() As ServiceBase
			ServicesToRun = New ServiceBase() { New ServiceHost() }
			ServiceBase.Run(ServicesToRun)
		End Sub
	End Class
End Namespace
