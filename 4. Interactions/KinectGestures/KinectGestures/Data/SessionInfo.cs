using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KinectGestures.Data
{
    public class SessionInfo
    {
        public string Name { get; set; }
        public string Speaker { get; set; }
        public BitmapImage SessionImage{ get; set; }
        public string Description { get; set; }
        public string SessionUri { get; set; }
    }
}
