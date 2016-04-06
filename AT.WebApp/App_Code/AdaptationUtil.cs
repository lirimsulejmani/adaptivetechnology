using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Text;
using AT.Plugins;
using AT.Repository.Model;
using AT.Repository.Enums;
using AT.Service.Implementations;

public class AdaptationUtil
{
    

	public AdaptationUtil()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static string GetWebClient(string url)
    {
        WebClient client = new WebClient();
        string downloadString = client.DownloadString(url);
        return downloadString;
    }

    public static string GetWebClientData(string url)
    {
        WebClient client = new WebClient();
        var data = client.DownloadData(url);
        var html = Encoding.UTF8.GetString(data);
        return html;
    }

    public static HtmlDocument GetHtmlContent(string url,bool getData = false)
    {

        WebClient client = new WebClient();
        client.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";

        HtmlDocument doc = new HtmlDocument();
        if (getData)
        {
            var data = client.DownloadData(url);
            doc.LoadHtml(Encoding.UTF8.GetString(data));
        }
        else
        {
            doc.LoadHtml(client.DownloadString(url));
        }
        return doc;
    }
   
    public static string GetHtmlDocument(string url)
    {
        var html = GetHtmlContent(url);
        AdaptationService service = new AdaptationService();
        var adaptationTechniques = service.AppliedTechniques();

        //download all files
        if (!url.StartsWith("file:///"))
        { DownloadFiles(url, html); }

        //This method makes adaptation for navigation enrichment technique
        //which add a skip link on the top of the page
        if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Navigation.ToString()))
        {
            NavigationEnrichmentTechnique.MakeAdaptation(html);
        }

        //This method makes adaptation for links contains Read More text
        //An attribute is added to make link more undestandable
        if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Link.ToString()))
        {
            string[] readMoreLinks = service.ListTextToRead();
            LinkEnrichmentTechnique.MakeAdaptation(html, url, readMoreLinks);
        }
        //This method makes adaptation for images
        if (adaptationTechniques.Contains(AdaptationTechniquesEnum.Image.ToString()))
        {
            ImageEnrichmentTechnique.MakeAdaptation(html, url);
        }
       

        return html.DocumentNode.OuterHtml;
    }

    public static void DownloadFiles(string url,HtmlDocument doc)
    {
        var filePath = HttpContext.Current.Server.MapPath(@"~\DownloadedContent\");
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
        foreach (string link in urls)
        {
            links.Add(link.Contains("http") ? link : url + "/" + link);
        }
        foreach (string file in links)
        {
            string[] fileName = file.Split('/');
            if (file.Length > 0 ||  !file.Contains("http"))
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(file, imgPath + fileName[fileName.Length - 1].ToString());
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
            styleLinks.Add(link.Contains("http") ? link : url+ "/" + link);
        }
        foreach (string file in styleLinks)
        {
            string[] fileName = file.Split('/');
            if (file.Length > 0)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(file, cssPath + fileName[fileName.Length - 1].ToString());
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
}