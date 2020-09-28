using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;

namespace BrokenLinks
{
    class BrokenLinksChecker
    {
        private Uri m_link;
        private Dictionary<string, int> m_validLinks = new Dictionary<string, int>();
        private Dictionary<string, int> m_invalidLinks = new Dictionary<string, int>();
        private Queue<Uri> m_uris = new Queue<Uri>();
        public BrokenLinksChecker(string link)
        {
            try
            {
                if (link.Last() != '/')
                {
                    link += "/";
                }
                m_link = new Uri(link, UriKind.Absolute);
                m_uris.Enqueue(m_link);
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Specified empty link");
            }
            catch (UriFormatException)
            {
                throw new Exception("Couldn't recognize correct link");
            }
        }
        public void CheckLinks()
        {
            while (m_uris.Count != 0)
            {
                Uri link = m_uris.Dequeue();
                string HTML;
                int statusCode;
                if (IsAlreadyChecked(link))
                {
                    continue;
                }
                GetHtmlFromLink(link, out HTML, out statusCode);
                if (IsSuccessStatusCode(statusCode))
                {
                    m_validLinks.Add(link.AbsoluteUri, statusCode);
                    List<string> newLinks = FindAllLinksInHTML(HTML);
                    PushLinksInQueue(newLinks, link);
                }
                else
                {
                    m_invalidLinks.Add(link.AbsoluteUri, statusCode);
                }
            }
        }

        private static void GetHtmlFromLink(Uri link, out string HTML, out int statusCode)
        {
            HttpWebRequest req;
            HttpWebResponse res;
            statusCode = 0;
            HTML = "";
            try
            {
                req = WebRequest.Create(link) as HttpWebRequest;
                res = req.GetResponse() as HttpWebResponse;
                Stream stream = res.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                HTML = sr.ReadToEnd();
            }
            catch (WebException e)
            {
                res = e.Response as HttpWebResponse;
            }
            catch (Exception)
            {
                throw new Exception("Unexpected error");
            }

            statusCode = (int)res.StatusCode;
        }

        private static List<string> FindAllLinksInHTML(string HTML)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HTML);
            List<string> links = new List<string>();
            var collection = doc.DocumentNode.SelectNodes("//a");
            if (collection != null)
            {
                foreach (HtmlNode link in collection)
                {
                    string lnk = link.GetAttributeValue("href", null);
                    //string srcLink = link.Attributes["href"].Value;
                    links.Add(lnk);
                }
            }
            return links;
        }
        private void PushLinksInQueue(List<string> newLinks, Uri parent)
        {
            foreach (string link in newLinks)
            {
                Uri result;
                result = ConcatenateLinks(link, parent);
                if (result != null)
                {
                    if (CheckIsInSourceDomain(result) && !IsAlreadyInQueue(result))
                    {
                        m_uris.Enqueue(result);
                    }
                }
            }
        }

        private bool IsAlreadyInQueue(Uri link)
        {
            return m_uris.Contains(link);
        }

        private bool IsAlreadyChecked(Uri link)
        {
            return m_invalidLinks.ContainsKey(link.AbsoluteUri)
                || m_validLinks.ContainsKey(link.AbsoluteUri);
        }

        private static Uri ConcatenateLinks(string link, Uri parent)
        {
            Uri result = null;
            try
            {
                if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                {
                    result = new Uri(link, UriKind.Absolute);
                }
                else if (Uri.IsWellFormedUriString(link, UriKind.Relative))
                {
                    Uri relativeURL = new Uri(link, UriKind.Relative);
                    Uri newURL = new Uri(parent, relativeURL);
                    result = newURL;
                }
            }
            catch (Exception)
            {
                throw new Exception("Unexpected exception");
            }

            return result;
        }
        private static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= 200 && statusCode < 300;
        }

        private bool CheckIsInSourceDomain(Uri currentLink)
        {
            return currentLink.Host == m_link.Host;
        }

        public Dictionary<string, int> GetValidLinksList()
        {
            return m_validLinks;
        }
        public Dictionary<string, int> GetInvalidLinksList()
        {
            return m_invalidLinks;
        }

    }
}

namespace BrokenLinks
{
    class Program
    {
        static string ParseArgs(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Arguments should be: [link to site that will be checked]");
            }
            return args[0];
        }
        static void Main(string[] args)
        {
            try
            {
                string link = ParseArgs(args);
                BrokenLinksChecker checker = new BrokenLinksChecker(link);
                checker.CheckLinks();
                WriteValidLinksToFile(checker.GetValidLinksList());
                WriteInvalidLinksToFile(checker.GetInvalidLinksList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        static public void WriteValidLinksToFile(Dictionary<string, int> linksList)
        {

            StreamWriter validLinkWriter = new StreamWriter("../../../valid.txt", false, Encoding.UTF8);
            using (validLinkWriter)
            {
                foreach(var link in linksList)
                {
                    validLinkWriter.WriteLine(link.Key + "\tStatus code: " + link.Value);
                }
                validLinkWriter.WriteLine();
                validLinkWriter.WriteLine("Valid links count: " + linksList.Count.ToString());
                validLinkWriter.WriteLine("Check out date: " + DateTime.UtcNow);
            }

        }
        static public void WriteInvalidLinksToFile(Dictionary<string, int> linksList)
        {
            StreamWriter invalidLinkWriter = new StreamWriter("../../../invalid.txt", false, Encoding.UTF8);
            using (invalidLinkWriter)
            {
                foreach (var link in linksList)
                {
                    invalidLinkWriter.WriteLine(link.Key + "\tStatus code: " + link.Value);
                }
                invalidLinkWriter.WriteLine();
                invalidLinkWriter.WriteLine("Invalid links count: " + linksList.Count.ToString());
                invalidLinkWriter.WriteLine("Check out date: " + DateTime.UtcNow);
            }
        }
    }
}
