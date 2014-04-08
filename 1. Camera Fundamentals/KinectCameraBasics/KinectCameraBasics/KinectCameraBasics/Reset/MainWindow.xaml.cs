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
        Int32Rect _imageSize;
        int _stride;
        byte[] _colorPixels; 
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
                _imageSize = new Int32Rect(0, 0, e.NewSensor.ColorStream.FrameWidth, e.NewSensor.ColorStream.FrameHeight);
                _colorPixels = new byte[e.NewSensor.ColorStream.FramePixelDataLength];
                _stride = e.NewSensor.ColorStream.FrameWidth * e.NewSensor.ColorStream.FrameBytesPerPixel;
                _colorBitmap = new WriteableBitmap(e.NewSensor.ColorStream.FrameWidth, e.NewSensor.ColorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);

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
                colorFrame.CopyPixelDataTo(_colorPixels); 

                _colorBitmap.WritePixels(_imageSize, _colorPixels, _stride, 0);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _chooser.Stop(); 
        }

    }
}
