Namespace Coding4Fun.Kinect.KinectService.WindowsService
	Partial Public Class ServiceHostInstaller
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Component Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.serviceProcessInstaller1 = New System.ServiceProcess.ServiceProcessInstaller()
			Me.serviceInstaller1 = New System.ServiceProcess.ServiceInstaller()
			' 
			' serviceProcessInstaller1
			' 
			Me.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem
			Me.serviceProcessInstaller1.Password = Nothing
			Me.serviceProcessInstaller1.Username = Nothing
			' 
			' serviceInstaller1
			' 
			Me.serviceInstaller1.Description = "Coding4Fun Kinect socket service"
			Me.serviceInstaller1.DisplayName = "Coding4Fun Kinect Service"
			Me.serviceInstaller1.ServiceName = "KinectService"
			Me.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic
			' 
			' ServiceHostInstaller
			' 
			Me.Installers.AddRange(New System.Configuration.Install.Installer() { Me.serviceProcessInstaller1, Me.serviceInstaller1})

		End Sub

		#End Region

		Private serviceProcessInstaller1 As System.ServiceProcess.ServiceProcessInstaller
		Private serviceInstaller1 As System.ServiceProcess.ServiceInstaller
	End Class
End Namespace