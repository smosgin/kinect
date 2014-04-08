using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using KinectGestures.Data; 


namespace KinectGestures
{
    public class DemoDataSource
    {
        public static List<SessionInfo> GetAllData()
        {
            TechEdSessions TechedWrapper = new TechEdSessions();
            return TechedWrapper.ReturnAllSessions(true);
        }
    }
}
