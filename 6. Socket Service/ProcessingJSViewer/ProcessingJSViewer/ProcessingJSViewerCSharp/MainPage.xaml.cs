using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProcessingJSViewerCSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		bool _isBrowserLoaded = false;
		bool _isJavascriptReady = false;
		
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
			var processingJsFile = "";
			using (var fs = await file.OpenReadAsync())
			{
				using (var inStream = fs.GetInputStreamAt(0))
				{
					using (var dataReader = new DataReader(inStream))
					{
						await dataReader.LoadAsync((uint)fs.Size);
						processingJsFile = dataReader.ReadString((uint)fs.Size);
						dataReader.DetachStream();
					}
				}
			}

			if (processingJsFile != "")
				Browser.InvokeScript("loadSketch", new[] { processingJsFile });
		}

		// Handle navigation failures.
		private async void Browser_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
		{
			await new MessageDialog("Navigation to this page failed, check your internet connection", "warning").ShowAsync();
		}

    }
}
