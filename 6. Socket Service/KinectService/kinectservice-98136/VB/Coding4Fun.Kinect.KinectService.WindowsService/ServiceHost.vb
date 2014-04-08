' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ServiceProcess

Namespace Coding4Fun.Kinect.KinectService.WindowsService
	Partial Public Class ServiceHost
		Inherits ServiceBase
		Private se As New ServiceEngine()

		Public Sub New()
			InitializeComponent()
		End Sub

		Protected Overrides Sub OnStart(ByVal args() As String)
			se.Start()
		End Sub

		Protected Overrides Sub OnStop()
			se.Stop()
		End Sub
	End Class
End Namespace
