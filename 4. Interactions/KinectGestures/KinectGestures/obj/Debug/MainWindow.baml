   M S B A M L   `   `   `  ����  KinectGestures    KinectGestures.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   S OMicrosoft.Kinect.Toolkit, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null\ XMicrosoft.Kinect.Toolkit.Controls, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null2k(http://schemas.microsoft.com/kinect/2013   5   	   <d2http://schemas.microsoft.com/expression/blend/2008   5   	   Fmc;http://schemas.openxmlformats.org/markup-compatibility/2006   5   	   -   5          � Title$  
MainWindow��5	   	   $	��800��6   $
��1280��6)    � WindowState$ 	Maximized=�6O   .��5
      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Center=�6    �� Orientation$ 
Horizontal=�62   .+�5      4  .Microsoft.Kinect.Toolkit.KinectSensorChooserUI  -   5          Name$ uxSensorChooser��5   &   $��Center=�6?   $	��Top=�6]   5      4  .Microsoft.Kinect.Toolkit.Controls.KinectRegion  5   
   -   5          Name$ uxKinectRegion��5   1   $	��Top=�6   $��1��6I      KinectSensor 6W   �� *KinectSensorChooser.Kinect+ �� ElementName$ uxSensorChooser�� �� Mode$ OneWay=�.��5      :  4Microsoft.Kinect.Toolkit.Controls.KinectScrollViewer  $
��Left=�6'   $��Visible=�6B   $��Hidden=�6k   .��5      :  4Microsoft.Kinect.Toolkit.Controls.KinectItemsControl  -   5        ��5   +   �� ��5      �� 5      .R�5   "   �� $ 
Horizontal=�6-   5      ��5      �� 5      .�5   "   �� 	 �� Property	 � ��
 �� Value$	
 500��6E   �� 5   "   	 � ��$	
 500��6E   5      ��5       �� 5!      .R�5"   "   �� $ Vertical=�6.   $
��Auto��6E   .+�5#   &   �� $	��Top=�6,   $��Center=�6D   $��0,0,0,10q�5$   ,   $	��220��6>   $	��165��6J   ��5#   a   �� *SessionImage+5$   Z   �� 5%   &   
�������$��Center=�6C   $��30(�5&   0   $
��Bold"�6>   $��WrapWithOverflow=�6P   ��5%   `   �� *Name+5&   r   5'   #   5(      5*      5+      5,      5/      50      