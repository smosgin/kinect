using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
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
using KinectGestures.Data;
using Microsoft.Kinect.Toolkit.Controls;
using System.Diagnostics;


namespace KinectGestures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser = new KinectSensorChooser();

        public MainWindow()
        {
            InitializeComponent();
            uxSensorChooser.KinectSensorChooser = sensorChooser;
            sensorChooser.KinectChanged += sensorChooser_KinectChanged;

            sensorChooser.Start();

            //Databind the list of sessions
            this.DataContext = DemoDataSource.GetAllData();            
        }

        void sensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                e.OldSensor.Stop();
            }
            if (e.NewSensor != null)
            {
                e.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30); 
                e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                e.NewSensor.SkeletonStream.Enable();
                e.NewSensor.DepthStream.Range = DepthRange.Near;
                e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sensorChooser != null)
            {
                sensorChooser.Stop(); 
            }
        }

        private void KinectItemsControl_ItemClick(object sender, RoutedEventArgs e)
        {
            //button that raised the event
            KinectTileButton button = (KinectTileButton)e.OriginalSource;
            SessionInfo session = (SessionInfo)button.DataContext;

            //start up Internet Explorer using the session Uri
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = session.SessionUri;
            p.StartInfo = info;
            info.FileName = @"C:\Program Files\Internet Explorer\iexplore.exe";
            p.Start();                    
        }
    }
}
