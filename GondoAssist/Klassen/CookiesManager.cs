using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GondoAssist.Klassen
{
    public class CookiesManager
    {
        XDocument xmlDoc;
        string xml_path;

        public CookiesManager()
        {
           // xml_path = ParamsLib.BrwsrOptions.BrowserCookiesFile;

            xmlDoc = new XDocument();

            if (File.Exists(xml_path))
            {
                xmlDoc = XDocument.Load(xml_path);
            }
            else
            {
                var xmlBodyNode = new XElement("body", "");
                var xmlCList = new XElement("cookies_list", "");

                xmlBodyNode.Add(xmlCList);

                xmlBodyNode.Save(xml_path);
                xmlDoc = XDocument.Load(xml_path);
            }
        }

        public List<MyCookie> GetCookiesForUser(string user_name)
        {
            List<MyCookie> cookiesList = new List<MyCookie>();
            try
            {
                cookiesList = (from e in xmlDoc.Root.Elements("cookies_list")
                               let cookie = e.Element("cookie")
                               where (string)cookie.Attribute("user_name") == user_name
                               select new MyCookie
                               {
                                   name = (string)cookie.Attribute("c_name"),
                                   value = (string)cookie.Attribute("c_value"),
                                   domain = (string)cookie.Attribute("c_domain"),
                                   path = (string)cookie.Attribute("c_path"),
                                   expiries = (string)cookie.Attribute("c_expiries"),
                                   secure = (string)cookie.Attribute("c_secure"),
                               }).ToList();
            }
            catch
            { }

            return cookiesList;
        }

        public void AddCookiesForUser(string username, string cookieName, string cookieValue, string domainName, string path, string expiries, string secure)
        {
            var xmlNode = new XElement("cookie", new XAttribute("user_name", username),
                                new XAttribute("c_name", cookieName),
                                new XAttribute("c_value", cookieValue),
                                new XAttribute("c_domain", domainName),
                                new XAttribute("c_path", path),
                                new XAttribute("c_expiries", expiries),
                                new XAttribute("c_secure", secure)
            );

            xmlDoc.Element("body").Element("cookies_list").Add(xmlNode);
        }

        public void Save()
        {
            xmlDoc.Save(xml_path);
        }

        public void removeCookieForUser(string username)
        {
            try
            {
                xmlDoc.Element("body").Element("cookies_list").Descendants("cookie")
                                   .Where(x => (string)x.Attribute("user_name") == username)
                                   .Remove();
            }
            catch
            {
            }
        }


        public class MyCookie
        {
            public string name { get; set; }
            public string value { get; set; }
            public string domain { get; set; }
            public string path { get; set; }
            public string expiries { get; set; }
            public string secure { get; set; }
        }

    }
}
