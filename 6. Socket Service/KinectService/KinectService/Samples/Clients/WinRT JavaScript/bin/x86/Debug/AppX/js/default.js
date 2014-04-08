// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
	"use strict";

	WinJS.Binding.optimizeBindingReferences = true;

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;
	var imaging = Windows.Graphics.Imaging;
	var kinect = Coding4Fun.Kinect.KinectService.WinRTClient;

	var colorClient = new Coding4Fun.Kinect.KinectService.WinRTClient.ColorClient();
	var depthClient = new Coding4Fun.Kinect.KinectService.WinRTClient.DepthClient();
	var skeletonClient = new Coding4Fun.Kinect.KinectService.WinRTClient.SkeletonClient();

	var sekeletonCanvas;
	var depthCanvas;

	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
				// TODO: This application has been newly launched. Initialize
				// your application here.
			} else {
				// TODO: This application has been reactivated from suspension.
				// Restore application state here.
			}
			document.getElementById("startColor").addEventListener("click", startColorHandler);
			//document.getElementById("startDepth").addEventListener("click", startDepthHandler);
			document.getElementById("startSkeleton").addEventListener("click", startSkeletonHandler);
			depthCanvas = document.getElementById("depthCanvas");
			sekeletonCanvas = document.getElementById("skeletonCanvas");
			args.setPromise(WinJS.UI.processAll());
		}
	};

	var startColorHandler = function (ev) {
		colorClient.oncolorframeready = colorFrameHandler;
		colorClient.connect(ipAddress.value, 4530);
	};

	var colorFrameHandler = function (ev) {
		var reader = Windows.Storage.Streams.DataReader.fromBuffer(ev.pixelBuffer);
		var bytes = new Uint8Array(ev.pixelBuffer.length);
		reader.readBytes(bytes);
		var blob = new Blob([bytes], { type: 'image/jpeg' });
		document.getElementById('colorImg').src = URL.createObjectURL(blob, { oneTimeOnly: true });
	};

	var startDepthHandler = function (ev) {
		depthClient.ondepthframeready = depthFrameHandler;
		depthClient.connect(ipAddress.value, 4531);
	};

	var depthFrameHandler = function (ev) {
		var reader = Windows.Storage.Streams.DataReader.fromBuffer(ev.depthData);
		var bytes = new Uint8Array(ev.depthData.length);
		reader.readBytes(bytes);

		var ctx = depthCanvas.getContext("2d");

		var imageData = ctx.getImageData(0, 0, depthCanvas.width, depthCanvas.height);
		var data = imageData.data;

		convertDepthFrame(data, bytes, ev);

		ctx.putImageData(imageData, 0, 0);
	};

	var convertDepthFrame = function(out, depthFrame, args)
	{
		var intensityShiftByPlayerR = [ 1, 2, 0, 2, 0, 0, 2, 0 ];
		var intensityShiftByPlayerG = [ 1, 2, 2, 0, 2, 0, 0, 1 ];
		var intensityShiftByPlayerB = [ 1, 0, 2, 2, 0, 2, 0, 2 ];

		var RedIndex = 0;
		var GreenIndex = 1;
		var BlueIndex = 2;
		var AlphaIndex = 3;

		for (var i16 = 0, i32 = 0; i16 < depthFrame.length && i32 < depthFrame.length * 4; i16+=2, i32 += 4)
		{
			var val = (depthFrame[i16] | (depthFrame[i16+1] << 8));
			var player = val & args.playerIndexBitmask;
			var realDepth = val >> args.playerIndexBitmaskWidth;

			// transform 13-bit depth information into an 8-bit intensity appropriate
			// for display (we disregard information in most significant bit)
			var intensity = (~(realDepth >> 4)) & 0xff;

			if (player == 0 && realDepth == 0)
			{
				// white 
				out[i32 + RedIndex]   = 255;
				out[i32 + GreenIndex] = 255;
				out[i32 + BlueIndex]  = 255;
				out[i32 + AlphaIndex] = 255;
			}
			else
			{
				// tint the intensity by dividing by per-player values
				out[i32 + RedIndex]   = (intensity >> intensityShiftByPlayerR[player]) & 0xff;
				out[i32 + GreenIndex] = (intensity >> intensityShiftByPlayerG[player]) & 0xff;
				out[i32 + BlueIndex]  = (intensity >> intensityShiftByPlayerB[player]) & 0xff;
				out[i32 + AlphaIndex] = 255;
			}
		}
	};

	var startSkeletonHandler = function (ev) {
		skeletonClient.onskeletonframeready = skeletonFrameHandler;
		skeletonClient.connect(ipAddress.value, 4532);
	};

	var skeletonFrameHandler = function (ev) {
		var context = sekeletonCanvas.getContext("2d");
		context.save();
		context.clearRect(0, 0, sekeletonCanvas.width, sekeletonCanvas.height);

		var skeleton = null;
		for(var i = 0; i < ev.skeletonArrayLength; i++)
		{
			if(ev.skeletons[i].trackingState == kinect.SkeletonTrackingState.tracked)
			{
				skeleton = ev.skeletons[i];
				break;
			}
		}

		if(skeleton != null)
		{
		    
			drawJoint(context, skeleton.joints[kinect.JointType.handLeft], "#FF0000");
			drawJoint(context, skeleton.joints[kinect.JointType.handRight], "#00FF00");
			drawJoint(context, skeleton.joints[kinect.JointType.head], "#0000FF");
            
		}

		context.restore();
	};

	var drawJoint = function(context, joint, color)
	{
		var scaledJoint = scaleTo(joint, sekeletonCanvas.width, sekeletonCanvas.height, 0.5, 0.5);
		context.fillStyle = color;
		context.fillRect(scaledJoint.position.x, scaledJoint.position.y, 20, 20);
	};

	var scaleTo = function(joint, width, height, skeletonMaxX, skeletonMaxY)
	{
		var pos = new kinect.SkeletonPoint();
		pos.x = scale(width, skeletonMaxX, joint.position.x);
		pos.y = scale(height, skeletonMaxY, -joint.position.y);
		pos.z = joint.position.z;

		var j = new kinect.Joint();
		j.trackingState = joint.trackingState;
		j.position = pos;

		return j;
	};

	var scale = function(maxPixel, maxSkeleton, position)
	{
		var value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel/2));
		if(value > maxPixel)
			return maxPixel;
		if(value < 0)
			return 0;
		return value;
	};

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state
		// that needs to persist across suspensions here. You might use the
		// WinJS.Application.sessionState object, which is automatically
		// saved and restored across suspension. If you need to complete an
		// asynchronous operation before your application is suspended, call
		// args.setPromise().
	};

	app.start();
})();
