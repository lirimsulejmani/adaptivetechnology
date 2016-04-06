using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AT.Plugins
{
    public class LinkEnrichmentTechnique
    {
        public static void MakeAdaptation(HtmlDocument doc, string url,string[] readMoreLinks)
        {
            int i = 0;
            foreach (var s in readMoreLinks)
            {
              readMoreLinks[i] = s.ToLowerInvariant();
              i++;
            }

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            //foreach (HtmlNode link in doc.DocumentNode.Descendants("a"))
            {
                var linkContent = link.InnerHtml.ToLowerInvariant();
                var linkInnerText = link.InnerText;
                if (link.Attributes["title"] != null)
                { }
                else if (CheckWord(linkContent,readMoreLinks))
                {
                    var href = link.Attributes["href"].Value;
                    Regex regex = new Regex(@"(http://)?(www\.)?\w+\.(com|net|edu|org)");

                    //In case wehere link goes inside webpage
                    if (href.Contains("#"))
                    {
                        var node = doc.DocumentNode.SelectSingleNode("//*[@id=\" " + href + " \"]");
                        if (node != null)
                        {
                            var nodes = node.Descendants();
                            foreach (var item in nodes)
                            {
                                if (item.InnerText != "")
                                {
                                    link.Attributes.Add("aria-label", item.InnerText);
                                }
                            }
                        }
                    }
                    //In case wehere link goes to anether page
                    else if (regex.IsMatch(href.ToString()) || href.StartsWith("file:///"))
                    {
                        WebClient client = new WebClient();
                        string downloadString = client.DownloadString(href);

                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(downloadString);
                        var document = html.DocumentNode;
                        HtmlNode mdnode = html.DocumentNode.SelectSingleNode("//meta[@name='description']");

                        if (mdnode != null)
                        {
                            HtmlAttribute desc;

                            desc = mdnode.Attributes["content"];
                            string fulldescription = desc.Value;
                            if (fulldescription != "")
                            {
                                link.Attributes.Add("aria-label", fulldescription);
                            }
                            else
                            {
                                var regText = new Regex(@"[a-zA-Z0-9]");
                                var headings = document.SelectNodes("//h1") != null ? document.SelectNodes("//h1").ToList()
                                    : document.SelectNodes("//h2") != null ? document.SelectNodes("//h2").ToList() : document.SelectNodes("//h3") != null ? document.SelectNodes("//h3").ToList() : null;
                                if (headings != null)
                                {
                                    foreach (var item in headings.ToList())
                                    {
                                        if (regText.IsMatch(item.InnerText))
                                        {
                                            link.Attributes.Add("aria-label", item.InnerText);
                                            link.InnerHtml = link.InnerText + " about " + item.InnerText;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string newUrl = url + "/" + href;

                        WebClient client = new WebClient();
                        string downloadString = client.DownloadString(newUrl);

                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(downloadString);

                        HtmlNode mdnode = html.DocumentNode.SelectSingleNode("//meta[@name='description']");

                        if (mdnode != null)
                        {
                            HtmlAttribute desc;

                            desc = mdnode.Attributes["content"];
                            string fulldescription = desc.Value;
                            if (!string.IsNullOrEmpty(fulldescription))
                            {
                                link.Attributes.Add("aria-label", fulldescription);
                            }
                            else
                            {
                                var regText = new Regex(@"[a-zA-Z0-9]");
                                var headings = html.DocumentNode.SelectNodes("//title") != null
                                    ? html.DocumentNode.SelectNodes("//title").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h1") != null
                                    ? html.DocumentNode.SelectNodes("//h1").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h2") != null
                                    ? html.DocumentNode.SelectNodes("//h2").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h3") != null
                                    ? html.DocumentNode.SelectNodes("//h3").FirstOrDefault()
                                    : null;

                                if (headings != null)
                                {
                                    if (regText.IsMatch(headings.InnerText))
                                    {
                                        link.Attributes.Add("aria-label", headings.InnerText);
                                        link.InnerHtml = link.InnerText + " about " + headings.InnerText;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //var nodes = html.DocumentNode.SelectSingleNode("//body").DescendantNodes();
                            var nodes = doc.DocumentNode.SelectSingleNode("//*[@id=\"maincontent\"]").Descendants();
                            foreach (var node in nodes)
                            {
                                if (node is HtmlTextNode)
                                {
                                    var newNode = node as HtmlTextNode;
                                    if (node.InnerText != "" && !node.InnerText.Contains("\n"))
                                    {
                                        //link.Attributes.Add("aria-label", node.InnerText);
                                        link.ParentNode.ReplaceChild(HtmlNode.CreateNode("<a href='" + newUrl + "'> " + link.InnerText + " " + node.InnerText + "</a>"), link);
                                        break;
                                    }
                                }
                            }


                            //Find all nodes that contains text and change inner text                         
                            //var nodes = doc.DocumentNode.SelectNodes("//body//text()[(normalize-space(.) != '') and not(parent::script) and not(*)]");
                            //foreach (HtmlNode htmlNode in nodes)
                            //{
                            //    htmlNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(htmlNode.InnerText + "_translated"), htmlNode);
                            //}
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(linkInnerText))
                {
                    var href = link.Attributes["href"].Value;
                    //In case wehere link goes inside webpage
                    if (href.StartsWith("/"))
                    {
                        string newUrl = url + "/" + href;

                        WebClient client = new WebClient();
                        string downloadString = "";
                        try
                        {
                            downloadString = client.DownloadString(newUrl);
                        }
                        catch { }
                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(downloadString);

                        HtmlNode mdnode = html.DocumentNode.SelectSingleNode("//meta[@name='description']");

                        if (mdnode != null)
                        {
                            HtmlAttribute desc;

                            desc = mdnode.Attributes["content"];
                            string fulldescription = desc.Value;
                            if (!string.IsNullOrEmpty(fulldescription))
                            {
                                link.Attributes.Add("aria-label", fulldescription);
                            }
                            else
                            {
                                var regText = new Regex(@"[a-zA-Z0-9]");
                                var headings = html.DocumentNode.SelectNodes("//title") != null
                                    ? html.DocumentNode.SelectNodes("//title").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h1") != null
                                    ? html.DocumentNode.SelectNodes("//h1").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h2") != null
                                    ? html.DocumentNode.SelectNodes("//h2").FirstOrDefault()
                                    : html.DocumentNode.SelectNodes("//h3") != null
                                    ? html.DocumentNode.SelectNodes("//h3").FirstOrDefault()
                                    : null;

                                if (headings != null)
                                {
                                    if (regText.IsMatch(headings.InnerText))
                                    {
                                        link.Attributes.Add("aria-label", headings.InnerText);
                                        link.InnerHtml = link.InnerText + " about " + headings.InnerText;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //var nodes = html.DocumentNode.SelectSingleNode("//body").DescendantNodes();
                            var nodes = doc.DocumentNode.SelectSingleNode("//*[@id=\"maincontent\"]").Descendants();
                            foreach (var node in nodes)
                            {
                                if (node is HtmlTextNode)
                                {
                                    var newNode = node as HtmlTextNode;
                                    if (node.InnerText != "" && !node.InnerText.Contains("\n"))
                                    {
                                        //link.Attributes.Add("aria-label", node.InnerText);
                                        link.ParentNode.ReplaceChild(HtmlNode.CreateNode("<a href='" + newUrl + "'> " + link.InnerText + " " + node.InnerText + "</a>"), link);
                                        break;
                                    }
                                }
                            }


                            //Find all nodes that contains text and change inner text                         
                            //var nodes = doc.DocumentNode.SelectNodes("//body//text()[(normalize-space(.) != '') and not(parent::script) and not(*)]");
                            //foreach (HtmlNode htmlNode in nodes)
                            //{
                            //    htmlNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(htmlNode.InnerText + "_translated"), htmlNode);
                            //}
                        }
                    }
                }
            }
        }

        public static bool CheckWord(string text, string[] array)
        {
            int i = 0;
            foreach (var s in array)
            {
                if (text.Contains(s))
                {
                    return true;
                }
                i++;
            }

            return false;
        }
    }
}
