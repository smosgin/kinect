// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    var canvas;
    var localSketch;
    var sketch;

    var jsCanvasWidth = 1200;
    var jsCanvasHeight = 600;

    var handLX = 300;
    var handLY = 300;
    var handRX = 900;
    var handRY = 300;
    var handLDepth = 1.0;
    var handRDepth = 1.0;

    //Kinect variables
    var kinect = Coding4Fun.Kinect.KinectService.WinRTClient;
    var skeletonClient = new Coding4Fun.Kinect.KinectService.WinRTClient.SkeletonClient();
    var ipAddress = "127.0.0.1";


    app.onactivated = function(args) {
	    if (args.detail.kind === activation.ActivationKind.launch) {
		    if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
			    // TODO: This application has been newly launched. Initialize
			    // your application here.
		    } else {
			    // TODO: This application has been reactivated from suspension.
			    // Restore application state here.
		    }
          
	        //Register for skeletal tracking
		    skeletonClient.onskeletonframeready = skeletonFrameHandler;
		    skeletonClient.connect(ipAddress, 4532);

		    args.setPromise(WinJS.UI.processAll());
	    }

        // WE GRAB A REFERENCE TO THE CANVAS
	    canvas = document.getElementById("canvasRoot");

        // WE ARE NO LONGER PULLING A NATIVE SKETCH...
        //var uri = new Windows.Foundation.Uri('ms-appx:///sketch.pde');

        // BUT WE ARE NOW CREATING AN EMPTY SKETCH OBJECT
	    localSketch = new Processing.Sketch();
        // AND THEN PULLING IN JAVASCRIPT THAT WILL ADD FUNCTIONS TO THE ABOVE OBJECT ( clever! yes, I know...)
        // THIS LETS US WRITE PROCESSING IN A MORE JAVASCRIPT FRIENDLY SYNTAX FOR MORE TRADITIONAL MSFT DEVS
	    var uri = new Windows.Foundation.Uri('ms-appx:///sketch.js');

	    Windows.Storage.StorageFile.getFileFromApplicationUriAsync(uri).done(
		    function(data) {
			    Windows.Storage.FileIO.readTextAsync(data).done(
				    function(fileContent) {
				        // WE ARE NO LONGER COMPILING NATIVE PROCESSING, SO LOCALSKETCH
                        // WAS ALREADY CREATED AS AN EMPY SHELL ABOVE
				        //localSketch = Processing.compile(fileContent);
				        // BUT BY DOING AN EVAL ON THE ADDITIONAL JAVASCRIPT, WE FILL UP 
                        // LOCALSKETCH WITH THE PROPER LOGIC
				        eval(fileContent);
				        // LOCALSKETCH HAS NOW BEEN EXTENDED THROUGH THE EVAL CALL WITH THE 
				        // ADDITIONAL JAVASCRIPT SYNTAXED PROCESSING FUNCTIONS FROM THE FILE,
                        // SO SLAP IT ON THE CANVAS AND WE'RE DONE.
					    sketch = new Processing(canvas, localSketch);
                       
				    });
		    });


    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    var skeletonFrameHandler = function (ev) {
        //Debug.writeln("In skeleton frame handler"); 

        var skeleton = null;
        for (var i = 0; i < ev.skeletonArrayLength; i++)
        {
            if (ev.skeletons[i].trackingState == kinect.SkeletonTrackingState.tracked)
            {
                skeleton = ev.skeletons[i];
                break;
            }
        }

        if (skeleton != null)
        {
            //z = distance from camera in meters
            var distance = skeleton.joints[kinect.JointType.handRight].position.z;
            handLX = skeleton.joints[kinect.JointType.handLeft].position.x;
            handLY = skeleton.joints[kinect.JointType.handLeft].position.y;
            handRX = skeleton.joints[kinect.JointType.handRight].position.x;
            handRY = skeleton.joints[kinect.JointType.handRight].position.y;

            Debug.writeln("hand left" + handLX + ", " + handLY);

        }
    };

    app.start();
})();
