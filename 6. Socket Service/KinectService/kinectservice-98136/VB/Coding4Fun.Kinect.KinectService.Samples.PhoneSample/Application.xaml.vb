﻿' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell

Namespace Coding4Fun.Kinect.KinectService.Samples.PhoneSample
	Partial Public Class App
		Inherits Application
		''' <summary>
		''' Provides easy access to the root frame of the Phone Application.
		''' </summary>
		''' <returns>The root frame of the Phone Application.</returns>
		Private privateRootFrame As PhoneApplicationFrame
		Public Property RootFrame() As PhoneApplicationFrame
			Get
				Return privateRootFrame
			End Get
			Private Set(ByVal value As PhoneApplicationFrame)
				privateRootFrame = value
			End Set
		End Property

		''' <summary>
		''' Constructor for the Application object.
		''' </summary>
		Public Sub New()
			' Global handler for uncaught exceptions. 
			AddHandler UnhandledException, AddressOf Application_UnhandledException

			' Standard Silverlight initialization
			InitializeComponent()

			' Phone-specific initialization
			InitializePhoneApplication()

			' Show graphics profiling information while debugging.
			If System.Diagnostics.Debugger.IsAttached Then
				' Display the current frame rate counters.
				Application.Current.Host.Settings.EnableFrameRateCounter = True

				' Show the areas of the app that are being redrawn in each frame.
				'Application.Current.Host.Settings.EnableRedrawRegions = true;

				' Enable non-production analysis visualization mode, 
				' which shows areas of a page that are handed off to GPU with a colored overlay.
				'Application.Current.Host.Settings.EnableCacheVisualization = true;

				' Disable the application idle detection by setting the UserIdleDetectionMode property of the
				' application's PhoneApplicationService object to Disabled.
				' Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
				' and consume battery power when the user is not using the phone.
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled
			End If

		End Sub

		' Code to execute when the application is launching (eg, from Start)
		' This code will not execute when the application is reactivated
		Private Sub Application_Launching(ByVal sender As Object, ByVal e As LaunchingEventArgs)
		End Sub

		' Code to execute when the application is activated (brought to foreground)
		' This code will not execute when the application is first launched
		Private Sub Application_Activated(ByVal sender As Object, ByVal e As ActivatedEventArgs)
		End Sub

		' Code to execute when the application is deactivated (sent to background)
		' This code will not execute when the application is closing
		Private Sub Application_Deactivated(ByVal sender As Object, ByVal e As DeactivatedEventArgs)
		End Sub

		' Code to execute when the application is closing (eg, user hit Back)
		' This code will not execute when the application is deactivated
		Private Sub Application_Closing(ByVal sender As Object, ByVal e As ClosingEventArgs)
		End Sub

		' Code to execute if a navigation fails
		Private Sub RootFrame_NavigationFailed(ByVal sender As Object, ByVal e As NavigationFailedEventArgs)
			If System.Diagnostics.Debugger.IsAttached Then
				' A navigation has failed; break into the debugger
				System.Diagnostics.Debugger.Break()
			End If
		End Sub

		' Code to execute on Unhandled Exceptions
		Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs)
			If System.Diagnostics.Debugger.IsAttached Then
				' An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break()
			End If
		End Sub

		#Region "Phone application initialization"

		' Avoid double-initialization
		Private phoneApplicationInitialized As Boolean = False

		' Do not add any additional code to this method
		Private Sub InitializePhoneApplication()
			If phoneApplicationInitialized Then
				Return
			End If

			' Create the frame but don't set it as RootVisual yet; this allows the splash
			' screen to remain active until the application is ready to render.
			RootFrame = New PhoneApplicationFrame()
			AddHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication

			' Handle navigation failures
			AddHandler RootFrame.NavigationFailed, AddressOf RootFrame_NavigationFailed

			' Ensure we don't initialize again
			phoneApplicationInitialized = True
		End Sub

		' Do not add any additional code to this method
		Private Sub CompleteInitializePhoneApplication(ByVal sender As Object, ByVal e As NavigationEventArgs)
			' Set the root visual to allow the application to render
			If RootVisual IsNot RootFrame Then
				RootVisual = RootFrame
			End If

			' Remove this handler since it is no longer needed
			RemoveHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication
		End Sub

		#End Region
	End Class
End Namespace