using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV.Structure;
using Microsoft.Kinect;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Emgu.CV;
using Microsoft.Kinect.Toolkit;
using NAudio.Wave;
using KinectOpenCV.AudioCore;
using System.ComponentModel;
using System.Threading;


namespace KinectOpenCV
{
	public enum EffectType
	{
		None,
		Blur,
		Dilate,
		Erode,
		Edge
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        KinectSensorChooser _sensorChooser = new KinectSensorChooser(); 

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		private const int EffectSize = 200;

		// kinect objects
		private KinectSensor _sensor;
		private CoordinateMapper _mapper;

		// color data
		private byte[] _colorData;
		private WriteableBitmap _colorWriteableBitmap;
		private System.Drawing.Bitmap _drawingBitmap;
		private Image<Bgra, byte> _ocvColorImg;

		// skeleton data
		private Skeleton[] _skeletons;

		// which effects to apply
		private EffectType _colorEffect;

		// regions to which we apply the selected effect
		List<Rectangle> _effectRegions = new List<Rectangle>();

		public MainWindow()
		{
			InitializeComponent();

            _sensorChooser.KinectChanged += _sensorChooser_KinectChanged;
            _sensorChooser.Start(); 
		}

        void _sensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.OldSensor != null)
            {
                e.OldSensor.Stop();
                e.OldSensor.AllFramesReady -= this._sensor_AllFramesReady;
            }
            if (e.NewSensor != null)
            {
                #region Setup Kinect Sensor
                _sensor = e.NewSensor;

                //map points
                _mapper = new CoordinateMapper(_sensor);

                // create color data bitmaps and arrays
                _colorData = new byte[_sensor.ColorStream.FramePixelDataLength];
                _colorWriteableBitmap = new WriteableBitmap(_sensor.ColorStream.FrameWidth, _sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // create the skeleton arrays
                _skeletons = new Skeleton[_sensor.SkeletonStream.FrameSkeletonArrayLength];

                // Assign the writeable bitmap to the WPF color image display
                ColorImage.Source = _colorWriteableBitmap;

                // enable color, depth, skeleton
                _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                try
                {
                    _sensor.DepthStream.Range = DepthRange.Near;
                }
                catch
                {
                    // Kinect for Xbox, doesn't support this.
                }

                _sensor.SkeletonStream.Enable();
                _sensor.SkeletonStream.EnableTrackingInNearRange = true;
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

                // hook up our event
                _sensor.AllFramesReady += _sensor_AllFramesReady; 
                #endregion

                //System.Drawing.Bitmap and the OpenCvImage
                _drawingBitmap = new Bitmap(_sensor.ColorStream.FrameWidth, _sensor.ColorStream.FrameHeight, PixelFormat.Format32bppRgb);
                _ocvColorImg = new Image<Bgra, byte>(_drawingBitmap); 


            }
        }


		void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			_effectRegions.Clear();

			using(SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
			{
                if (skeletonFrame == null)
                {
                    return; 
                }

				// get the skeleton data
				skeletonFrame.CopySkeletonDataTo(_skeletons);

				// get the tracked skeletons		
				var tracked = (from s in _skeletons where s.TrackingState == SkeletonTrackingState.Tracked select s);
				foreach(Skeleton s in tracked)
				{
					// grab the head joint
					Joint j = s.Joints[JointType.Head];

					//map the joint to the color image coordinates
					ColorImagePoint colorPoint = _mapper.MapSkeletonPointToColorPoint(j.Position, ColorImageFormat.RgbResolution640x480Fps30);

					// create a rectangle around that area to apply the effect
					_effectRegions.Add(new Rectangle(colorPoint.X - (EffectSize/2), colorPoint.Y - (EffectSize/2), EffectSize, EffectSize));
				}
			}

			// if we have no rectangles, add an empty one
            if (_effectRegions.Count == 0)
            {
                _effectRegions.Add(Rectangle.Empty);
            }

			using(ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
			{
                #region Setup Left Image
                if (colorImageFrame == null)
                {
                    return;
                }

                // get color frame data
                colorImageFrame.CopyPixelDataTo(_colorData);

                _colorWriteableBitmap.WritePixels(
                    new Int32Rect(0, 0, _colorWriteableBitmap.PixelWidth, _colorWriteableBitmap.PixelHeight),
                    _colorData, _colorWriteableBitmap.PixelWidth * colorImageFrame.BytesPerPixel, 0);

                #endregion


				// apply the effect to the data and get back a bitmap source to display
				BitmapSource bitmapSource = ApplyEffect(_colorEffect, _colorData, _drawingBitmap, _ocvColorImg, _effectRegions);

				// display it
				ColorEffectImage.Source = bitmapSource;
			}
		}

		private BitmapSource ApplyEffect(EffectType effect, byte[] pixelData, System.Drawing.Bitmap bitmap, Image<Bgra, byte> ocvImage, List<Rectangle> effectRegions)
		{

			// lock the bitmap for writing
			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
												ImageLockMode.WriteOnly, bitmap.PixelFormat);

			// copy the data from pixelData to BitmapData
			Marshal.Copy(pixelData, 0, data.Scan0, pixelData.Length);

			// unlock the bitmap
			bitmap.UnlockBits(data);

			// assign the bitmap to the OpenCV image
			ocvImage.Bitmap = bitmap;

			if(effect != EffectType.None)
			{
				foreach(Rectangle effectRegion in effectRegions)
				{
					// set the Region of Interest based on the joint
					ocvImage.ROI = effectRegion;

					// temp image to hold effect output
					Image<Bgra, byte> ocvTempImg;

					switch(effect)
					{
						case EffectType.Blur:
							ocvTempImg = ocvImage.SmoothBlur(20, 20);
                            break;
						case EffectType.Dilate:
							ocvTempImg = ocvImage.Dilate(5);
							break;
						case EffectType.Erode:
                            ocvTempImg = ocvImage.Erode(5);
							break;
						case EffectType.Edge:
							Image<Gray, byte> gray = ocvImage.Convert<Gray, byte>();
							gray = gray.SmoothBlur(3, 3);
                            gray = gray.Canny(30.0f, 50.0f);
							ocvTempImg = gray.Convert<Bgra, byte>();
							break;
						default:
							throw new ArgumentOutOfRangeException("effect");
					}

					// copy the effect area to the final image
					CvInvoke.cvCopy(ocvTempImg, ocvImage, IntPtr.Zero);
				}
			}

			// reset the Region of Interest
			ocvImage.ROI = Rectangle.Empty;

            #region Convert System.Drawing.Bitmap to WPF BitmapSource
            // get a bitmap handle from the OpenCV image
            IntPtr hBitmap = ocvImage.ToBitmap().GetHbitmap();

            // convert that handle to a WPF BitmapSource
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                                                                BitmapSizeOptions.FromWidthAndHeight(
                                                                                bitmap.Width, bitmap.Height));
            // delete the bitmap
            DeleteObject(hBitmap); 
            #endregion

			return bitmapSource;
		}

		private void ColorEffect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_colorEffect = (EffectType)e.AddedItems[0];
		}

        WaveIn waveIn;
        WaveFileWriter writer;
        bool isStarted = false;
        BackgroundWorker worker;

        private void InitWaveRecorder()
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            waveIn.DeviceNumber = 0;

            waveIn.DataAvailable += waveIn_DataAvailable;
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void btnRecord_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isStarted)
            {
                if (worker != null && worker.IsBusy)
                    worker.CancelAsync();

                btnRecord.Content = "Stop Recording";
                isStarted = true;

                InitWaveRecorder();
                try
                {
                    writer = new WaveFileWriter("INPUT.WAV", waveIn.WaveFormat);

                    waveIn.StartRecording();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // Recording stopped.
                btnRecord.Content = "Start Recording";
                isStarted = false;

                waveIn.StopRecording();

                writer.Close();

                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WAVFile input = new WAVFile();
            input.Open("INPUT.wav", WAVFile.WAVFileMode.READ);

            

            WAVFile output = new WAVFile();
            output.Create("PITCHY_OUTPUT.wav", input.IsStereo, input.SampleRateHz, input.BitsPerSample);

            //shift audio
            while (input.NumSamplesRemaining > 0)
            {
                short sample = input.GetNextSampleAs16Bit();
                float sampleVal = (float)sample / 32768f;
                float[] indata = new float[] { sampleVal };

                PitchShifter.PitchShift(0.5f, 1, input.SampleRateHz, indata);

                output.AddSample_16bit((short)(indata[0] * 32768));
            }

            output.Close();


            //play sound
            var soundFile = "PITCHY_OUTPUT.wav";
            using (var wfr = new WaveFileReader(soundFile))
            using (WaveChannel32 wc = new WaveChannel32(wfr) { PadWithZeroes = false })
            using (var audioOutput = new DirectSoundOut())
            {
                audioOutput.Init(wc);

                audioOutput.Play();

                while (audioOutput.PlaybackState != PlaybackState.Stopped)
                {
                    Thread.Sleep(20);
                }

                audioOutput.Stop();
            }

        }
	}
}
