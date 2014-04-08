using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;



namespace KinectGestures.Data
{
    public class TechEdSessions
    {
        string url = @"http://channel9.msdn.com/Events/TechEd/NorthAmerica/2013/RSS";
        string filePath = @"C:\Users\danielfe\SkyDrive\Documents\Projects\KinectDemos\_Final\4. Interactions\KinectGestures\KinectGestures\Data\TechEdNorthAmerica2013Sessions.xml";
        string imagePath = @"C:\Users\danielfe\SkyDrive\Documents\Projects\KinectDemos\_Final\4. Interactions\KinectGestures\KinectGestures\Data\default.png"; 
        IEnumerable<Channel> data; 

       public List<SessionInfo> ReturnAllSessions(bool useLocal)
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(imagePath);
            myBitmapImage.EndInit();

            if (useLocal)
            {
                data = RssParser.GetFeed(filePath);
            }
            else
            {
                data = RssParser.GetFeed(new Uri(url));
            }
           
            var list = data.FirstOrDefault().Items.ToList();

            var result = from c in data.FirstOrDefault().Items
                         orderby c.Title
                         select new SessionInfo() { Description = c.Description, SessionImage = myBitmapImage,  Name = c.Title, SessionUri = c.Link };
            return result.ToList();
        }


    }
}
