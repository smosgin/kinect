using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Media;
using System.Windows.Threading;

namespace Coding4Fun.Toolkit.Controls.Common
{
    public static class HueLightingWrapper
    {
        private static string HueLightIp = "192.168.1.100";
        private static string HueLightUser = "Coding4Fun";
        private static Color lastColor;

        public static void RegisterUserWithHue()
        {
            try
            {
                var client = new WebClient();

                //our uri to perform registration
                var uri = new Uri(string.Format("http://{0}/api", HueLightIp));

                //create our registration object, along with username and description
                var reg = new
                {
                    username = HueLightUser,
                    devicetype = "Coding4Fun Hue Kinect Light Project"
                };

                var jsonObj = JsonConvert.SerializeObject(reg);

                client.UploadStringCompleted += client_UploadStringCompleted;

                //Invoke a POST to the bridge
                client.UploadStringAsync(uri, jsonObj);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void ExecuteHue(Color color)
        {
            if (color == lastColor)
            {
                return;
            }

            try
            {
                var drawColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);

                //build our State object
                var state = new
                {
                    on = true,
                    hue = (int)(drawColor.GetHue() * 182.04), //we convert the hue value into degrees by multiplying the value by 182.04
                    sat = (int)(drawColor.GetSaturation() * 254)
                };

                //convert it to json:
                var jsonObj = JsonConvert.SerializeObject(state);

                for (int i = 1; i <= 3; i++)
                {
                    //set the api url to set the state
                    var uri = new Uri(string.Format("http://{0}/api/{1}/lights/{2}/state", HueLightIp, HueLightUser, i));

                    var client = new WebClient();

                    client.UploadStringCompleted += client_UploadStringCompleted;

                    //Invoke the PUT method to set the state of the bulb
                    client.UploadStringAsync(uri, "PUT", jsonObj, color);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        static void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.UserState != null)
                {
                    lastColor = (Color)e.UserState;
                }

                Debug.WriteLine(e.Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
