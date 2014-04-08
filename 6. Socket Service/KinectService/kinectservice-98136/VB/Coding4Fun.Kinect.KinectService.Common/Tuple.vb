' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Coding4Fun.Kinect.KinectService.Common
	Public Class Tuple(Of T1, T2, T3, T4)
	   Public Property Item1() As T1
	   Public Property Item2() As T2
	   Public Property Item3() As T3
	   Public Property Item4() As T4

	   Public Sub New(ByVal item1 As T1, ByVal item2 As T2, ByVal item3 As T3, ByVal item4 As T4)
		  Me.Item1 = item1
		  Me.Item2 = item2
		  Me.Item3 = item3
		  Me.Item4 = item4
	   End Sub
	End Class
End Namespace
