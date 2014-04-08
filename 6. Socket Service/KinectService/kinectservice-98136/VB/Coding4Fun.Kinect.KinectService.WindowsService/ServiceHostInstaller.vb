' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Collections
Imports System.ComponentModel
Imports System.Configuration.Install


Namespace Coding4Fun.Kinect.KinectService.WindowsService
	<RunInstaller(True)> _
	Partial Public Class ServiceHostInstaller
		Inherits System.Configuration.Install.Installer
		Public Sub New()
			InitializeComponent()
		End Sub
	End Class
End Namespace
