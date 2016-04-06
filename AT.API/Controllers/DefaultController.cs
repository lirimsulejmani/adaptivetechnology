using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AT.Plugins;
using System.Text;
using HtmlAgilityPack;
using System.Collections.Specialized;
using AT.API.Models;
using AT.Service.Implementations;
using AT.Repository.Enums;
using System.IO;
using System.Web;
using System.Threading;
using System.Web.UI.HtmlControls;
using OpenQA.Selenium.PhantomJS;

namespace AT.API.Controllers
{
    public class DefaultController : ApiController
    {

        public string GetResult(string url)
        {
            bool getData = false;
            string agent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";

            HtmlDocument htmlDoc = new HtmlDocument();
            if (getData)
            {
                using (var client = new WebClient())
                {
                    client.Headers["User-Agent"] = agent;
                    var data = client.DownloadData(url);
                    htmlDoc.LoadHtml(Encoding.UTF8.GetString(data));
                }
            }
            else
            {
                //  var content = GetContentResponse(url);
                //  htmlDoc.LoadHtml(content);

                //this code download all html
                /*
                var driver = new PhantomJSDriver();
                driver.Url = url;
                driver.Navigate();
                //the driver can now provide you with what you need (it will execute the script)
                //get the source of the page
                var source = driver.PageSource;
                //fully navigate the dom
                // var pathElement = driver.FindElementById("A1");
                htmlDoc.LoadHtml(source);
                */

                using (var client = new WebClient())
                {
                    client.Headers["User-Agent"] = agent;
                    client.Encoding = Encoding.UTF8;
                    Uri uri = new Uri(url);
                    string content = client.DownloadString(uri);
                    htmlDoc.LoadHtml(content);
                }
            }
            try
            {
                AdaptationService service = new AdaptationService();
                var adaptationTechniques = service.AppliedTechniques();
                //download all files
                //if (!url.StartsWith("file:///"))
                //{ DownloadFiles(url, htmlDoc); }

                //This method makes adaptation for navigation enrichment technique
                //which add a skip link on the top of the page
                if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Navigation.ToString()))
                {
                    NavigationEnrichmentTechnique.MakeAdaptation(htmlDoc);
                }

                //This method makes adaptation for links contains Read More text
                //An attribute is added to make link more undestandable
                if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Link.ToString()))
                {
                    string[] readMoreLinks = service.ListTextToRead();
                    LinkEnrichmentTechnique.MakeAdaptation(htmlDoc, url, readMoreLinks);
                }
                //This method makes adaptation for images
                if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Image.ToString()))
                {
                    ImageEnrichmentTechnique.MakeAdaptation(htmlDoc, url);
                }
                //This method makes adaptation for images
                if (adaptationTechniques.Contains(AdaptationTechniquesEnum.TextImage.ToString()))
                {
                    ImageEnrichmentTechnique.ImagesInsideLinks(htmlDoc, url);
                }

                if (htmlDoc.DocumentNode != null)
                {
                    HtmlNode scripts = htmlDoc.CreateElement("script");
                    scripts.Attributes.Add("type", "text/javascript");
                    scripts.AppendChild(htmlDoc.CreateTextNode("clearTimeout();"));
                    scripts.AppendChild(htmlDoc.CreateTextNode("clearInterval();"));
                    htmlDoc.DocumentNode.AppendChild(scripts);
                }

                return htmlDoc.DocumentNode.OuterHtml;
            }
            catch(Exception ex)
            {
                return "AdaptaitionFailed <br />" + ex.Message + "<br />" + ex.Source + "<br />";
            }
        }



        public static void DownloadFiles(string url, HtmlDocument doc)
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath(@"~\DownloadedContent\");
            string localhostPath = url.Replace("http://", "").Replace("www.", "").Replace(".com", "") + "_files";
            string subPath = filePath + localhostPath;
            bool exists = System.IO.Directory.Exists(subPath);
            string imgPath = subPath + "\\images\\";
            string cssPath = subPath + "\\css\\";
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(subPath);
                System.IO.Directory.CreateDirectory(imgPath);
                System.IO.Directory.CreateDirectory(cssPath);
            }
            #region Images
            //download images
            var urls = doc.DocumentNode.Descendants("img")
                                    .Select(e => e.GetAttributeValue("src", null))
                                    .Where(s => !String.IsNullOrEmpty(s));
            List<string> links = new List<string>();
            string agent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
            foreach (string link in urls)
            {
                links.Add(link.Contains("http") ? link : url + "/" + link);
            }
            foreach (string file in links)
            {
                string[] fileName = file.Split('/');
                if (file.Length > 0 || !file.Contains("http"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers["User-Agent"] = agent;
                        webClient.Encoding = Encoding.UTF8;
                        Uri uri = new Uri("https://" + file);
                        webClient.DownloadFile(uri, imgPath + fileName[fileName.Length - 1].ToString());
                    }
                }
            }
            //edit src of image to get virtualpath image
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//img/@src"))
            {
                var link = node.Attributes["src"].Value;
                if (!link.Contains("http"))
                {
                    var fileName = System.IO.Path.GetFileName(link);
                    node.Attributes["src"].Value = @"/DownloadedContent/" + localhostPath + "/images/" + fileName;
                }
            }

            #endregion

            #region Styles

            var styles = doc.DocumentNode.Descendants("link")
                                   .Select(e => e.GetAttributeValue("href", null))
                                   .Where(s => !String.IsNullOrEmpty(s));
            List<string> styleLinks = new List<string>();
            foreach (string link in styles)
            {
                styleLinks.Add(link.Contains("http") ? link : url + "/" + link);
            }
            foreach (string file in styleLinks)
            {
                string[] fileName = file.Split('/');
                if (file.Length > 0)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers["User-Agent"] = agent;
                        webClient.Encoding = Encoding.UTF8;
                        Uri uri = new Uri("https://" + file);
                        webClient.DownloadFile(uri, cssPath + fileName[fileName.Length - 1].ToString());
                    }
                }
            }
            //edit src of image to get virtualpath image
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//link/@href"))
            {
                var link = node.Attributes["href"].Value;
                if (!link.Contains("http"))
                {
                    var fileName = System.IO.Path.GetFileName(link);
                    node.Attributes["href"].Value = @"/DownloadedContent/" + localhostPath + "/css/" + fileName;
                }
            }

            #endregion
        }

        //public string GetContentResponse(string urlAddress)
        //{
        //    string agent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";

        //    string data = "";
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
        //    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        //    request.Method = "POST";
        //    request.ContentLength = 0;
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    response.Headers["User-Agent"] = agent;

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        Stream receiveStream = response.GetResponseStream();
        //        StreamReader readStream = null;

        //        if (response.CharacterSet == null)
        //        {
        //            readStream = new StreamReader(receiveStream);
        //        }
        //        else
        //        {
        //            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
        //        }

        //         data = readStream.ReadToEnd();

        //        response.Close();
        //        readStream.Close();
        //    }

        //    return data;
        //}

        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var html = e.Result;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
            }
        }

        private static string GetWebText(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = "A .NET Web Crawler";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string htmlText = reader.ReadToEnd();
            return htmlText;
        }
    }

}
