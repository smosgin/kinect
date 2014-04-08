#pragma once

namespace Coding4Fun
{
	namespace Kinect
	{
		namespace KinectService
		{
			namespace WinRTClient
			{
				public enum class ImageFormat
				{
					Jpeg,
					Png,
					Raw
				};

				public enum class ColorImageFormat
				{
					Undefined,
					RgbResolution640x480Fps30,
					RgbResolution1280x960Fps12,
					YuvResolution640x480Fps15,
					RawYuvResolution640x480Fps15,
				};

				public enum class DepthImageFormat
				{
					Undefined,
					Resolution640x480Fps30,
					Resolution320x240Fps30,
					Resolution80x60Fps30,
				};


				public enum class FrameEdges
				{
					Bottom = 8,
					Left = 2,
					None = 0,
					Right = 1,
					Top = 4
				};

				public enum class SkeletonTrackingState
				{
					NotTracked,
					PositionOnly,
					Tracked
				};

				public enum class JointType
				{
					HipCenter,
					Spine,
					ShoulderCenter,
					Head,
					ShoulderLeft,
					ElbowLeft,
					WristLeft,
					HandLeft,
					ShoulderRight,
					ElbowRight,
					WristRight,
					HandRight,
					HipLeft,
					KneeLeft,
					AnkleLeft,
					FootLeft,
					HipRight,
					KneeRight,
					AnkleRight,
					FootRight
				};

				public enum class JointTrackingState
				{
					NotTracked,
					Inferred,
					Tracked
				};
			}
		}
	}
}
