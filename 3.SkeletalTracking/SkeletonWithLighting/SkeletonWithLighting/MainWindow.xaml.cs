using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Coding4Fun.Kinect.Wpf;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Newtonsoft.Json;

namespace SkeletonWithLighting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly int Bgr32BytesPerPixel = 4;

		private readonly KinectSensorChooser _sensorChooser = new KinectSensorChooser();
		private CoordinateMapper _coorMapper;

		private WriteableBitmap _colorImageWritableBitmap;

		Skeleton _firstSkeleton;
        Skeleton[] _allSkeletons; 
		private byte[] _colorImageData;
		private ColorImageFormat _currentColorImageFormat = ColorImageFormat.Undefined;

		private Color _currentColorSet;

		public SolidColorBrush SetColor
		{
			get { return (SolidColorBrush)GetValue(SetColorProperty); }
			set { SetValue(SetColorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SetColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SetColorProperty =
			DependencyProperty.Register("SetColor", typeof(SolidColorBrush), typeof(MainWindow), new PropertyMetadata(new SolidColorBrush()));

		public MainWindow()
		{
			InitializeComponent();

			DataContext = this;
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_sensorChooser.KinectChanged += sensorChooser_KinectChanged;
			_sensorChooser.Start();

		    HueLightingWrapper.RegisterUserWithHue();
		}

		void sensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
		{
			KinectSensor oldSensor = e.OldSensor;
			KinectSensor newSensor = e.NewSensor;

			if (oldSensor != null)
			{
				oldSensor.AllFramesReady -= newSensor_AllFramesReady;
                oldSensor.Stop();
			}

			if (newSensor != null)
			{
				try
				{
					_coorMapper = new CoordinateMapper(_sensorChooser.Kinect);

					newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
					newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    newSensor.AllFramesReady += newSensor_AllFramesReady;

					try
					{
						// This will throw on non Kinect For Windows devices.
						newSensor.DepthStream.Range = DepthRange.Near;
						newSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        
					}
					catch (InvalidOperationException)
					{
						newSensor.DepthStream.Range = DepthRange.Default;
						newSensor.SkeletonStream.EnableTrackingInNearRange = false;
					}
                    _allSkeletons = new Skeleton[newSensor.SkeletonStream.FrameSkeletonArrayLength];

                    //newSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; 


                    var parameters = new TransformSmoothParameters
                    {
                        Smoothing = 0.3f,
                        Correction = 0.0f,
                        Prediction = 0.0f,
                        JitterRadius = 1.0f,
                        MaxDeviationRadius = 0.5f
                    };


                    ////Choose between smoothing or no smooothing
                    newSensor.SkeletonStream.Enable(parameters);
                    //newSensor.SkeletonStream.Enable();
                    
				}
				catch (InvalidOperationException)
				{
					// This exception can be thrown when we are trying to
					// enable streams on a device that has gone away.  This
					// can occur, say, in app shutdown scenarios when the sensor
					// goes away between the time it changed status and the
					// time we get the sensor changed notification.
					//
					// Behavior here is to just eat the exception and assume
					// another notification will come along if a sensor
					// comes back.
				}
			}
		}

		private void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			
			#region copying color data
			using (var colorFrame = e.OpenColorImageFrame())
			{
				if (colorFrame == null)
				{
					return;
				}

				// Make a copy of the color frame for displaying.
				var haveNewFormat = _currentColorImageFormat != colorFrame.Format;
				if (haveNewFormat)
				{
					_currentColorImageFormat = colorFrame.Format;
					_colorImageData = new byte[colorFrame.PixelDataLength];
					_colorImageWritableBitmap = new WriteableBitmap(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null);

					_colorImage.Source = _colorImageWritableBitmap;
				}

				colorFrame.CopyPixelDataTo(_colorImageData);
				_colorImageWritableBitmap.WritePixels(
					new Int32Rect(0, 0, colorFrame.Width, colorFrame.Height),
					_colorImageData,
					colorFrame.Width * colorFrame.BytesPerPixel,
					0);
			}
			#endregion

			#region copying skeleton data
			using (var skeletonFrame = e.OpenSkeletonFrame())
			{
				if (skeletonFrame == null)
				{
					return;
				}

				
				skeletonFrame.CopySkeletonDataTo(_allSkeletons);
                
				_firstSkeleton = (from c in _allSkeletons
								 where c.TrackingState == SkeletonTrackingState.Tracked
								 select c).FirstOrDefault();

				if (_firstSkeleton == null)
				{
					return;
				}
			}
			#endregion

			UpdateInterface();
		}

		public void UpdateInterface()
		{
			ColorImagePoint handLeft = _coorMapper.MapSkeletonPointToColorPoint(
				_firstSkeleton.Joints[JointType.HandLeft].Position,
				_sensorChooser.Kinect.ColorStream.Format);

			ColorImagePoint handRight = _coorMapper.MapSkeletonPointToColorPoint(
				_firstSkeleton.Joints[JointType.HandRight].Position,
				_sensorChooser.Kinect.ColorStream.Format);

            //Use this to set exact joint position
            MoveToCameraPosition(_leftEllipse, handLeft);
            MoveToCameraPosition(_rightEllipse, handRight); 

			// Use this to scale the joint so you don't have to overstrech
            //ScalePosition(_leftEllipse, _firstSkeleton.Joints[JointType.HandLeft]);
            //ScalePosition(_rightEllipse, _firstSkeleton.Joints[JointType.HandRight]);

			//ExecuteLightChange(_rectCyan, _rectGreen, _rectOrange, _rectRed);
		}

		private void ExecuteLightChange(params Rectangle[] items)
		{
			foreach (var item in items)
			{
				if (IsItemMidpointInContainer(item, _rightEllipse))
				{
					ExecuteLightChange(item);
				}
			}
		}

		private void ExecuteLightChange(Rectangle item)
		{
			if (item == null || _currentColorSet == ((SolidColorBrush)item.Fill).Color)
				return;

			_currentColorSet = ((SolidColorBrush)item.Fill).Color;
			SetColor = new SolidColorBrush(_currentColorSet);

			Debug.WriteLine(SetColor.Color);

			// execute

            HueLightingWrapper.ExecuteHue(SetColor.Color); 
		}

		private void MoveToCameraPosition(FrameworkElement element, ColorImagePoint point)
		{
			//Divide by 2 for width and height so point is right in the middle 
			// instead of in top/left corner
			Canvas.SetLeft(element, point.X - element.Width / 2);
			Canvas.SetTop(element, point.Y - element.Height / 2);
		}

		private void ScalePosition(FrameworkElement element, Joint joint)
		{
			//convert the value to X/Y
			//convert & scale (.3 = means 1/3 of joint distance)
            //scale to width/height of window - can also set manually 1280x720
			Joint scaledJoint = joint.ScaleTo((int)this.ActualWidth, (int)this.ActualHeight, .3f, .3f);

			Canvas.SetLeft(element, scaledJoint.Position.X);
			Canvas.SetTop(element, scaledJoint.Position.Y);
		}

		public static bool IsItemMidpointInContainer(FrameworkElement container, FrameworkElement target)
		{
			var containerTopLeft = container.PointToScreen(new Point());
			var itemTopLeft = target.PointToScreen(new Point());

			double topBoundary = containerTopLeft.Y;
			double bottomBoundary = topBoundary + container.ActualHeight;
			double leftBoundary = containerTopLeft.X;
			double rightBoundary = leftBoundary + container.ActualWidth;

			//use midpoint of item (width or height divided by 2)
			double itemLeft = itemTopLeft.X + (target.ActualWidth / 2);
			double itemTop = itemTopLeft.Y + (target.ActualHeight / 2);

			if (itemTop < topBoundary || bottomBoundary < itemTop)
			{
				//Midpoint of target is outside of top or bottom
				return false;
			}

			if (itemLeft < leftBoundary || rightBoundary < itemLeft)
			{
				//Midpoint of target is outside of left or right
				return false;
			}

			return true;
		}




	}
}
