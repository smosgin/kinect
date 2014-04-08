// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using Microsoft.Kinect.Toolkit;
using System.Threading.Tasks; 

namespace WorkingWithDepthData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double oldAvg = 0;
        Stopwatch timer = Stopwatch.StartNew();

        public MainWindow()
        {
            InitializeComponent();

            kinectSensorChooserUI1.KinectSensorChooser = _sensorChooser;
            kinectSensorChooserUI1.KinectSensorChooser.KinectChanged += new EventHandler<KinectChangedEventArgs>(KinectSensorChooser_KinectChanged);
            _sensorChooser.Start(); 
        }

        WriteableBitmap _depthBitmap;
        Int32Rect _imageSize;
        Byte[] _pixels;
        int _stride; 
        List<DepthImagePixel> _allPlayers = new List<DepthImagePixel>(); 

        KinectSensorChooser _sensorChooser = new KinectSensorChooser();


        void KinectSensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            KinectSensor oldSensor = e.OldSensor;

            //stop the old sensor
            if (oldSensor != null)
            {
                oldSensor.Stop();
            }

            //get the new sensor
            KinectSensor newSensor = e.NewSensor;
            if (newSensor == null)
            {
                return;
            }

            //turn on features that you need
            newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            //newSensor.DepthStream.Range = DepthRange.Near; 
            newSensor.SkeletonStream.Enable();

            //sign up for events if you want to get at API directly
            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);

            try
            {
                _depthBitmap = new WriteableBitmap(newSensor.DepthStream.FrameWidth,
                    newSensor.DepthStream.FrameHeight,
                    96, 96, PixelFormats.Bgr32, null);

                _imageSize = new Int32Rect(0, 0, newSensor.DepthStream.FrameWidth, 
                    newSensor.DepthStream.FrameHeight);
                _stride = newSensor.DepthStream.FrameWidth * 4; 
                _pixels = new byte[newSensor.DepthStream.FrameHeight * _stride];

                newSensor.Start();


                image1.Source = _depthBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message); 
            }
        }

        void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {            
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    return; 
                }

                byte[] pixels = GenerateColoredBytes(depthFrame);

                //create image
                _depthBitmap.WritePixels(_imageSize, pixels, _stride, 0); 
            }
        }

        const int BlueIndex = 0;
        const int GreenIndex = 1;
        const int RedIndex = 2;      
        private byte[] GenerateColoredBytes(DepthImageFrame depthFrame)
        {
            DepthImagePixel[] depthPoints = new DepthImagePixel[depthFrame.PixelDataLength];
            depthFrame.CopyDepthImagePixelDataTo(depthPoints); 
            

            //use depthFrame to create the image to display on-screen
            //depthFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)


            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency 
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions       
     
            
            //loop through all distances
            //pick a RGB color based on distance 

            _allPlayers.Clear();

            for (int depthIndex = 0, colorIndex = 0; 
                depthIndex < depthPoints.Length && colorIndex < _pixels.Length; 
                depthIndex++, colorIndex += 4)
            {

                //gets the depth value
                int depth = depthPoints[depthIndex].Depth;

                // 900 mm or 2.95'
                if (depth <= 900)
                {
                    //we are very close
                    _pixels[colorIndex + BlueIndex] = 255;
                    _pixels[colorIndex + GreenIndex] = 0;
                    _pixels[colorIndex + RedIndex] = 0;

                }
                // 900 mm - 2K+ mm or 2.95' - 6.56'
                else if (depth > 900 && depth <= 2000)
                {
                    //we are a bit further away
                    _pixels[colorIndex + BlueIndex] = 0;
                    _pixels[colorIndex + GreenIndex] = 255;
                    _pixels[colorIndex + RedIndex] = 0;
                }
                // 2K+ mm or 6.56'+
                else if (depth > 2000)
                {
                    //we are the farthest
                    _pixels[colorIndex + BlueIndex] = 0;
                    _pixels[colorIndex + GreenIndex] = 0;
                    _pixels[colorIndex + RedIndex] = 255;
                }

                // get the player (requires skeleton tracking enabled for values)
                int player = depthPoints[depthIndex].PlayerIndex;

                //Color all players "yellow"
                if (player > 0)
                {
                    _allPlayers.Add(depthPoints[depthIndex]);
                    _pixels[colorIndex + BlueIndex] = 18;
                    _pixels[colorIndex + GreenIndex] = 250;
                    _pixels[colorIndex + RedIndex] = 255;
                }
            }

            if (_allPlayers.Count > 0)
            {
                var avg = (from p in _allPlayers select Convert.ToDouble(p.Depth)).Average();
                timer.Stop();

                //Convert to feet and display
                //Calculate speed by change in distance over change in time
                //Subtract oldAvg from avg to get the displacement, then use the timer's
                //ElapsedMilliseconds variable to divide by the change in time
                var feet = ConvertToFeet(Math.Abs(avg - oldAvg));
                var speed = feet / (timer.ElapsedMilliseconds / 1000.0);
                txtDistance.Text = String.Format("{0:0.##} ft/s", speed);
                oldAvg = avg;
                timer.Reset();
                timer.Start();
            }

            return _pixels;
        }        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._sensorChooser.Stop(); 
        }

        //Sweet land O' Liberty
        private static double ConvertToFeet(double millimeters)
        {
            return (millimeters * 0.0032808); 
        }


    }
}

