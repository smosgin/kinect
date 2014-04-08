using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KinectGestures.Data
{

    
    public class Channel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
    public class Item
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
    }

    //snippet taken from Web 
    public static class RssParser
    {
        private static IEnumerable<Channel> ParseRssXml(XDocument xdoc)
        { 
            return from channels in xdoc.Descendants("channel")
            select new Channel
            {
                Title = channels.Element("title") != null ? channels.Element("title").Value : "",
                Link = channels.Element("link") != null ? channels.Element("link").Value : "",
                Description = channels.Element("description") != null ? channels.Element("description").Value : "",
                Items = from items in channels.Descendants("item")
                    select new Item
                    {
                        Title = items.Element("title") != null ? items.Element("title").Value : "",
                        Link = items.Element("link") != null ? items.Element("link").Value : "",
                        Description = items.Element("description") != null ? items.Element("description").Value : "",
                        Guid = (items.Element("guid") != null ? items.Element("guid").Value : "")
                                 
                    }
            };

        }

        public static IEnumerable<Channel> GetFeed(string filePath)
        {
            if (File.Exists(filePath))
            {
                XDocument xdoc = XDocument.Load(filePath);
                return ParseRssXml(xdoc);               
            }
            else
            {
                throw new FileNotFoundException("File Path doesn't exist" + filePath);
            }
        }
        
        public static IEnumerable<Channel> GetFeed(Uri feedUri)
        {
            XDocument xdoc = XDocument.Load(new StreamReader(HttpWebRequest.Create(feedUri).GetResponse().GetResponseStream()));
            return ParseRssXml(xdoc); 
        }

    }

}
