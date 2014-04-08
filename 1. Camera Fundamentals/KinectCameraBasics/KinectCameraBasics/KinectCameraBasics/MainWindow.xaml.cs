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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Coding4Fun.Kinect.Wpf;

namespace KinectCameraBasics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensorChooser _chooser = new KinectSensorChooser();
        WriteableBitmap _colorBitmap;

        public MainWindow()
        {
            InitializeComponent();
            
            _chooser.KinectChanged += _sensor_KinectChanged;
            _sensorChooserUI.KinectSensorChooser = _chooser; 
            _chooser.Start(); 
        }

        void _sensor_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                e.OldSensor.Stop();
                e.OldSensor.AllFramesReady -= NewSensor_AllFramesReady;
            }
            if (e.NewSensor != null)
            {
                e.NewSensor.AllFramesReady += NewSensor_AllFramesReady;
                e.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);


                //set image variables
                _colorBitmap = Coding4Fun.Kinect.Wpf.WriteableBitmapHelper.CreateWriteableBitmap(e.NewSensor.ColorStream);

                _image1.Source = _colorBitmap; 
            }
        }

        void NewSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            //auto clean up after copying pixels
            using (var colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    //lost frames
                    return; 
                }
                //colorFrame.CopyPixelDataTo(_colorPixels);

                Coding4Fun.Kinect.Wpf.WriteableBitmapHelper.WritePixelsForColorImageFrame(colorFrame, _colorBitmap); 
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _chooser.Stop(); 
        }

    }
}
