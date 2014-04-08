using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

using Windows.Storage;
using Windows.Storage.Streams;

namespace ProcessingJSViewerWinPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		bool _isBrowserLoaded = false;
		bool _isJavascriptReady = false;
		string _sketchText = "";

		// Constructor
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
		{
			_isJavascriptReady = _isBrowserLoaded = false;
		}

		private void Browser_Loaded(object sender, RoutedEventArgs e)
		{
			_isBrowserLoaded = true;

			ValidateAndLoadSketch();
		}

		private void JSListener(object sender, NotifyEventArgs e)
		{
			_isJavascriptReady = true;

			ValidateAndLoadSketch();
		}

		private async void ValidateAndLoadSketch()
		{
			if (!_isJavascriptReady || !_isBrowserLoaded)
				return;

			var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///sketch.pde", UriKind.RelativeOrAbsolute));

			using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				using (var inStream = fs.GetInputStreamAt(0))
				{
					using (var dataReader = new DataReader(inStream))
					{
						await dataReader.LoadAsync((uint)fs.Size);
						_sketchText = dataReader.ReadString((uint)fs.Size);
						dataReader.DetachStream();
					}
				}
			}

			if (_sketchText != "")
				Browser.InvokeScript("loadSketch", new[] { _sketchText });
		}

		// Handle navigation failures.
		private void Browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			MessageBox.Show("Navigation to this page failed, check your internet connection");
		}
	}
}