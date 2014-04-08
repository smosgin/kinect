using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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


namespace KinectGestures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OneButtonWindow : Window
    {
        private KinectSensorChooser sensorChooser = new KinectSensorChooser();
        WriteableBitmap _colorBitmap; 

        public OneButtonWindow()
        {
            InitializeComponent();
            uxSensorChooser.KinectSensorChooser = sensorChooser;
            sensorChooser.KinectChanged += sensorChooser_KinectChanged;
            sensorChooser.Start();
        }

        void sensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                e.OldSensor.Stop();
            }
            if (e.NewSensor != null)
            {
                e.NewSensor.ColorStream.Enable();
                e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                //var parameters = new TransformSmoothParameters
                //{
                //    Smoothing = 0.3f,
                //    Correction = 0.0f,
                //    Prediction = 0.0f,
                //    JitterRadius = 1.0f,
                //    MaxDeviationRadius = 0.5f
                //};

                //e.NewSensor.SkeletonStream.Enable(parameters);
                e.NewSensor.SkeletonStream.Enable();
                e.NewSensor.DepthStream.Range = DepthRange.Near;
                e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                e.NewSensor.AllFramesReady += NewSensor_AllFramesReady;
                _colorBitmap = Coding4Fun.Kinect.Wpf.WriteableBitmapHelper.CreateWriteableBitmap(e.NewSensor.ColorStream);
                _image1.Source = _colorBitmap; 
            }
        }

        void NewSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (var colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return; 
                }

                Coding4Fun.Kinect.Wpf.WriteableBitmapHelper.WritePixelsForColorImageFrame(colorFrame, _colorBitmap); 
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sensorChooser != null)
            {
                sensorChooser.Stop(); 
            }
        }

        private void KinectCircleButton_Click_1(object sender, RoutedEventArgs e)
        {
            KinectCircleButton button = (KinectCircleButton)sender;
            button.Foreground = new SolidColorBrush(Colors.Red); 
        } 

    }
}
