﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System.Timers;
using System.Text;
using System.Windows.Controls;

namespace FaceTrackingBasics
{

	/// <summary>
	/// Interaction logic for NoddingWindow.xaml
	/// </summary>
	public partial class NoddingWindow : Window
	{
        //private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        double _thoughtBubbleOffset = 30;
		private readonly KinectSensorChooser _sensorChooser = new KinectSensorChooser();
		private FaceTracker _faceTracker;
		private WriteableBitmap _colorImageWritableBitmap;
        ObservableCollection<FaceReading> _faceReadings;
        Stopwatch _yesNo = new Stopwatch();
        bool yesStarted = false;
        bool noStarted = false;
        short[] depthData;
        Skeleton firstSkeleton;
		
		private byte[] _colorImageData;
		private ColorImageFormat _currentColorImageFormat = ColorImageFormat.Undefined;

		public NoddingWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            _faceReadings = InitializeReadings();

			_sensorChooser.KinectChanged += sensorChooser_KinectChanged;
			_sensorChooser.Start();
		}

        private ObservableCollection<FaceReading> InitializeReadings()
        {
            ObservableCollection<FaceReading> all = new ObservableCollection<FaceReading>();

            foreach (var enumName in Enum.GetNames(typeof(FaceEmotion)))
            {
                all.Add(new FaceReading() { Name = enumName, 
                                            ReadingType = (FaceEmotion)Enum.Parse(typeof(FaceEmotion), enumName) });
            }
            return all; 
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
					newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
					newSensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

					try
					{
						// This will throw on non Kinect For Windows devices.
						newSensor.DepthStream.Range = DepthRange.Near;
						newSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        newSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
					}
					catch (InvalidOperationException)
					{
						newSensor.DepthStream.Range = DepthRange.Default;
						newSensor.SkeletonStream.EnableTrackingInNearRange = false;
					}

					newSensor.SkeletonStream.Enable();
					newSensor.AllFramesReady += newSensor_AllFramesReady;
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			if (_faceTracker == null)
			{
				try
				{
					_faceTracker = new FaceTracker(_sensorChooser.Kinect);
				}
				catch (InvalidOperationException)
				{
					// During some shutdown scenarios the FaceTracker
					// is unable to be instantiated.  Catch that exception
					// and don't track a face.
					Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
					_faceTracker = null;
				}
			}

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

			#region copying depth data
			using (var depthFrame = e.OpenDepthImageFrame())
			{
				if (depthFrame == null)
				{
					return;
				}

				depthData = new short[depthFrame.PixelDataLength];
				depthFrame.CopyPixelDataTo(depthData);
			}
			#endregion

			#region copying skeleton data
			using (var skeletonFrame = e.OpenSkeletonFrame())
			{
				if (skeletonFrame == null)
				{
					return;
				}

				Skeleton[] allSkeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
				skeletonFrame.CopySkeletonDataTo(allSkeletons);

				firstSkeleton = (from c in allSkeletons
								 where c.TrackingState == SkeletonTrackingState.Tracked
								 select c).FirstOrDefault();

				if (firstSkeleton == null)
				{
					return;
				}
			}
			#endregion

			if (_faceTracker != null)
			{
				FaceTrackFrame frame = _faceTracker.Track(_sensorChooser.Kinect.ColorStream.Format,
					_colorImageData, _sensorChooser.Kinect.DepthStream.Format, depthData, firstSkeleton);

				if (frame.TrackSuccessful)
				{
                    AnalyzeFace(frame);

                    if (_thoughtBubble.Visibility != System.Windows.Visibility.Visible)
                    {
                        _thoughtBubble.Visibility = System.Windows.Visibility.Visible;
                    }

				}
				else
				{
				}
			}
		}

        private void AnalyzeFace(FaceTrackFrame frame)
        {

            //Move ThoughtBubble 
            System.Windows.Point facePoint = new System.Windows.Point();
            facePoint.X = frame.FaceRect.Right + _thoughtBubbleOffset;
            facePoint.Y = frame.FaceRect.Top - (_thoughtBubble.ActualHeight / 2) ;
            MoveToCameraPosition(_thoughtBubble, facePoint);

            var animationUnits = frame.GetAnimationUnitCoefficients();

            if (animationUnits[AnimationUnit.JawLower] > .7)
            {
                _thoughtBubble.SetThoughtBubble("openmouth.png");
            }
            if (animationUnits[AnimationUnit.BrowRaiser] > .5)
            {
                _thoughtBubble.SetThoughtBubble("eyebrow.png");
            }


            //Check for yes
            if (frame.Rotation.X > 10 && yesStarted == false)
            {
                yesStarted = true;
                _yesNo.Reset();
                _yesNo.Start();
                //start YES timer
                return;
            }

            //check for no
            if (frame.Rotation.Y < -10 && noStarted == false)
            {
                noStarted = true;
                _yesNo.Reset();
                _yesNo.Start();
                return;
            }


            if (_yesNo.Elapsed.TotalSeconds > 3)
            {
                _yesNo.Stop();
                yesStarted = false;
                noStarted = false;
            }
            else
            {
                if (frame.Rotation.X < -5 && yesStarted)
                {
                    //YES!!
                    _thoughtBubble.SetThoughtBubble("yes.png");
                }
                if (frame.Rotation.Y > 10 && noStarted)
                {
                    //NO!!!
                    _thoughtBubble.SetThoughtBubble("no.png");
                }
            }

            UpdateTextReading(frame, animationUnits[AnimationUnit.BrowRaiser],animationUnits[AnimationUnit.JawLower]); 

        }

        StringBuilder sb = new StringBuilder();
        private void UpdateTextReading(FaceTrackFrame frame, float outerBrowRaiser = 0, float openMouth = 0)
        {
            sb.Clear();

            foreach (var item in _faceReadings)
            {
                switch (item.ReadingType)
                {
                    case FaceEmotion.OpenMouth:
                        SetReading(item, openMouth);
                        break;
                    case FaceEmotion.EyeBrow:
                        SetReading(item, outerBrowRaiser);
                        break;
                    case FaceEmotion.UpDownValue:
                        SetReading(item, frame.Rotation.X);
                        break;
                    case FaceEmotion.LeftRight:
                        SetReading(item, frame.Rotation.Y);
                        break;
                    default:
                        break;
                }

                sb.AppendLine(item.ToString()); 
            }

            _CurrentReading.Text = sb.ToString();
        }

        int _currentIndex = 0; 

        private void SetReading(FaceReading item, float newValue)
        {
            //Set Values
            item.Current = newValue;

            if (_currentIndex >= 90)
            {
                _currentIndex = 0; 
            }
            item.AllValues[_currentIndex] = newValue;
            _currentIndex++;

            item.Average = item.AllValues.Average();

            //Min & Max
            if (newValue < item.Min)
            {
                item.Min = newValue;
            }
            if (newValue > item.Max)
            {
                item.Max = newValue;
            }
        }

        private void MoveToCameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            Canvas.SetLeft(element, point.X );
            Canvas.SetTop(element, point.Y );
        }

        private void MoveToCameraPosition(FrameworkElement element, System.Windows.Point point)
        {
            Canvas.SetLeft(element, point.X);
            Canvas.SetTop(element, point.Y);
        }

	}
}
