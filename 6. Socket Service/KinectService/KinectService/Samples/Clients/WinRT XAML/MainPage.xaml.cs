using System;
using System.Linq;
using System.IO;
using Coding4Fun.Kinect.KinectService.WinRTClient;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Coding4Fun.Kinect.KinectService.Samples.WinRTXaml
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly WriteableBitmap _colorBitmap = new WriteableBitmap(1,1);
		private WriteableBitmap _depthBitmap = new WriteableBitmap(1,1);

		private readonly ColorClient _colorClient;
		private readonly DepthClient _depthClient;
		private readonly SkeletonClient _skeletonClient;

		public MainPage()
		{
			this.InitializeComponent();

			_colorClient = new ColorClient();
			_colorClient.ColorFrameReady += client_ColorFrameReady;

			_depthClient = new DepthClient();
			_depthClient.DepthFrameReady += client_DepthFrameReady;

			_skeletonClient = new SkeletonClient();
			_skeletonClient.SkeletonFrameReady += client_SkeletonFrameReady;
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private async void StartColor_Click(object sender, RoutedEventArgs e)
		{
			try
			{
			if(!_colorClient.IsConnected)
				await _colorClient.Connect(ServerIp.Text, 4530);
			else
				_colorClient.Disconnect();
			}
			catch(Exception ex)
			{
				new MessageDialog(ex.Message).ShowAsync();
			}
		}

		private async void client_ColorFrameReady(object sender, ColorFrameData e)
		{
			InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
			await stream.WriteAsync(e.PixelBuffer);
			stream.Seek(0);
			_colorBitmap.SetSource(stream);
			this.Color.Source = _colorBitmap;
		}

		private async void StartDepth_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if(!_depthClient.IsConnected)
					await _depthClient.Connect(ServerIp.Text, 4531);
				else
					_depthClient.Disconnect();
			}
			catch(Exception ex)
			{
				new MessageDialog(ex.Message).ShowAsync();
			}
		}
	   
		private void client_DepthFrameReady(object sender, DepthFrameData e)
		{
			if(_depthBitmap == null || _depthBitmap.PixelWidth != e.ImageFrame.Width || _depthBitmap.PixelHeight != e.ImageFrame.Height)
			{
				_depthBitmap = new WriteableBitmap(e.ImageFrame.Width, e.ImageFrame.Height);
				this.Depth.Source = this._depthBitmap;
			}

			byte[] convertedDepthBits = ConvertDepthFrame(e.DepthData.ToArray(), e);

			InMemoryRandomAccessStream depthStream = new InMemoryRandomAccessStream();
			DataWriter depthWriter = new DataWriter(depthStream.GetOutputStreamAt(0));
			depthWriter.WriteBytes(convertedDepthBits);

			Stream s = _depthBitmap.PixelBuffer.AsStream();
			s.Write(convertedDepthBits, 0, convertedDepthBits.Length);
			_depthBitmap.Invalidate();
		}

		private byte[] ConvertDepthFrame(byte[] depthFrame, DepthFrameData args)
		{
			int[] intensityShiftByPlayerR = { 1, 2, 0, 2, 0, 0, 2, 0 };
			int[] intensityShiftByPlayerG = { 1, 2, 2, 0, 2, 0, 0, 1 };
			int[] intensityShiftByPlayerB = { 1, 0, 2, 2, 0, 2, 0, 2 };

			const int RedIndex = 2;
			const int GreenIndex = 1;
			const int BlueIndex = 0;

			byte[] depthFrame32 = new byte[args.ImageFrame.Width * args.ImageFrame.Height * 4];

			for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame.Length * 4; i16+=2, i32 += 4)
			{
				short val = (short)(depthFrame[i16] | (depthFrame[i16+1] << 8));
				int player = val & args.PlayerIndexBitmask;
				int realDepth = val >> args.PlayerIndexBitmaskWidth;
				
				// transform 13-bit depth information into an 8-bit intensity appropriate
				// for display (we disregard information in most significant bit)
				byte intensity = (byte)(~(realDepth >> 4));

				if (player == 0 && realDepth == 0)
				{
					// white 
					depthFrame32[i32 + RedIndex] = 255;
					depthFrame32[i32 + GreenIndex] = 255;
					depthFrame32[i32 + BlueIndex] = 255;
				}
				else
				{
					// tint the intensity by dividing by per-player values
					depthFrame32[i32 + RedIndex] = (byte)(intensity >> intensityShiftByPlayerR[player]);
					depthFrame32[i32 + GreenIndex] = (byte)(intensity >> intensityShiftByPlayerG[player]);
					depthFrame32[i32 + BlueIndex] = (byte)(intensity >> intensityShiftByPlayerB[player]);
				}
			}

			return depthFrame32;
		}

		private async void StartSkeleton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if(!_skeletonClient.IsConnected)
					await _skeletonClient.Connect(ServerIp.Text, 4532);
				else
					_skeletonClient.Disconnect();
			}
			catch(Exception ex)
			{
				new MessageDialog(ex.Message).ShowAsync();
			}
		}

		void client_SkeletonFrameReady(object sender, SkeletonFrameData e)
		{
			Skeleton skeleton = (from s in e.Skeletons
									 where s.TrackingState == SkeletonTrackingState.Tracked
									 select s).FirstOrDefault();

			if(skeleton == null)
				return;

			SetEllipsePosition(headEllipse, skeleton.Joints[(int)JointType.Head]);
			SetEllipsePosition(leftEllipse, skeleton.Joints[(int)JointType.HandLeft]);
			SetEllipsePosition(rightEllipse, skeleton.Joints[(int)JointType.HandRight]);
		}

		private void SetEllipsePosition(FrameworkElement ellipse, Joint joint)
		{
			var scaledJoint = ScaleTo(joint, (int)Skeleton.Width, (int)Skeleton.Height, .5f, .5f);

			Canvas.SetLeft(ellipse, scaledJoint.Position.X);
			Canvas.SetTop(ellipse, scaledJoint.Position.Y);            
		}

		private Joint ScaleTo(Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
		{
			SkeletonPoint pos = new SkeletonPoint()
			{
				X = Scale(width, skeletonMaxX, joint.Position.X),
				Y = Scale(height, skeletonMaxY, -joint.Position.Y),
				Z = joint.Position.Z,
			};

			Joint j = new Joint()
			{
				TrackingState = joint.TrackingState,
				Position = pos
			};

			return j;
		}

		private float Scale(int maxPixel, float maxSkeleton, float position)
		{
			float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel/2));
			if(value > maxPixel)
				return maxPixel;
			if(value < 0)
				return 0;
			return value;
		}
	}
}
