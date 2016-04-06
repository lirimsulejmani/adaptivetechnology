using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Web;
using System.Drawing;
using AT.Plugins.OCR;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Linq;


namespace AT.Plugins
{
    public class ImageEnrichmentTechnique
    {
        public static void MakeAdaptation(HtmlDocument doc, string url)
        {
            //string localImagePath = ImageEnrichmentTechnique.ImagesInsideLinks(doc, url);
            try
            {
              //  var imgQuery = "//img[substring(@src, string-length(@src) - string-length('.gif') +1) != '.gif']";
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//img/@src"))
                {
                    if (node.Attributes["alt"] == null)
                    {
                        var resultGoogle = GetSourceImage(node, url);
                        if (!string.IsNullOrEmpty(resultGoogle))
                        {
                            node.Attributes.Add("alt", resultGoogle);
                        }
                    }
                    else if (string.IsNullOrEmpty(node.Attributes["alt"].Value))
                    {
                        var resultGoogle = GetSourceImage(node, url);
                        if (!string.IsNullOrEmpty(resultGoogle))
                        {
                            node.SetAttributeValue("alt", resultGoogle);
                        }
                    }
                }

                //if control is image button
                var imageButtons = doc.DocumentNode.SelectNodes("//input[@type='image']");
                string localImagePath = "";
                foreach (HtmlNode node in imageButtons)
                {
                    if (node.Attributes["src"] != null && node.GetAttributeValue("alt","") == "")
                    {
                        
                            var src = node.Attributes["src"].Value;
                            var uri = new Uri(url);
                            var link = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                            string imagePath = src.StartsWith("http") ? src : link + "/" + src;
                            localImagePath = GetFiles(src, imagePath);
                            var result = ExtractTextFromImage(localImagePath);

                            if (!string.IsNullOrEmpty(result))
                            {
                                node.Attributes.Add("alt", result);
                            }
                            else
                            {
                            var resultGoogle = SearchImagesByGoogle(imagePath);
                                if (!string.IsNullOrEmpty(resultGoogle))
                                {
                                    node.Attributes.Add("alt", resultGoogle);
                                }
                            }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetSourceImage(HtmlNode node,string url)
        {
            string localImagePath = "";

            var src = node.Attributes["src"].Value;
            if (src.Contains("?"))
            { src = src.Substring(0, src.IndexOf('?')); }
            if (!src.EndsWith(".gif"))
            {

                string imagePath = src.StartsWith("http") ? src : url + src;
                if (!DoesUrlExist(imagePath))
                {
                    var uri = new Uri(imagePath);
                    var baseUrl = uri.Scheme + "://" + uri.Authority + "/";
                    imagePath = baseUrl + src;
                }
                string imageLink = node.Attributes["src"].Value;
                if (url.StartsWith("file:///"))
                {
                    var fileName = System.IO.Path.GetFileName(url);
                    imagePath = url.Replace("file:///", "").Replace(fileName, "") + src;
                    localImagePath = imagePath;
                    imageLink = UploadImage(localImagePath);
                }
                else if (!imageLink.StartsWith("http") || !Path.HasExtension(imageLink))
                {
                    localImagePath = GetFiles(src, imagePath);
                    imageLink =localImagePath == "" ? "" : UploadImage(localImagePath);
                }
                if (imageLink != "")
                {
                    if (imageLink.Contains("?"))
                    { imageLink = imageLink.Substring(0, imageLink.IndexOf('?')); }

                    //google result
                    if (IsLocalPath(imageLink) || imageLink.StartsWith("http"))
                    {
                        var resultGoogle = SearchImagesByGoogle(imageLink);
                        return resultGoogle;


                        /*
                         bing result
                         var resultBing = SearchImagesByBing(imageLink);
                         node.Attributes.Add("alt", resultBing);
                         bing result
                         var newNode = doc.CreateElement("label");
                         newNode.InnerHtml = resultBing;
                         doc.DocumentNode.AppendChild(newNode);
                        */
                    }
                }
            }

            return "";
        }
        public static string ImagesInsideLinks(HtmlDocument doc,string url)
        {
            string localImagePath = "";
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//img/@src"))
            {
                if (node.ParentNode != null)
                {
                    if (node.ParentNode.Name == "a" && node.ParentNode.GetAttributeValue("href", true))
                    {
                        if (!node.GetAttributeValue("alt", false))
                        {
                            var src = node.Attributes["src"].Value;
                            string imagePath = src.StartsWith("http") ? src : url + "/" + src;
                            if (url.StartsWith("file:///"))
                            {
                                var fileName = System.IO.Path.GetFileName(url);
                                imagePath = url.Replace("file:///","").Replace(fileName, "") + src;
                                localImagePath = imagePath;
                            }
                            else
                            {
                              localImagePath = GetFiles(src, imagePath);
                            }
                            var result = ExtractTextFromImage(localImagePath);
                            if (!string.IsNullOrEmpty(result))
                            {
                                node.Attributes.Add("alt", result);
                                var newNode = doc.CreateElement("label");
                                newNode.InnerHtml = "Text of image link is: " + result + " <br /> <br />";
                                doc.DocumentNode.AppendChild(newNode);
                                doc.CreateTextNode("<br />");
                            }
                        }
                    }
                }
            }

            return localImagePath;
        }

        /*
        public static string SearchImagesByBing(string queryString)
        { 
            // Create a Bing container.
            string bingRootUrl = "https://api.datamarket.azure.com/Bing/Search";
            string BingAccountKey = "mRgR6AzXI5JVilVEhakOtJzN80ZS6CysO3ggvqRbvHU=";
            string BingMarket = "en-us";
            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search/v1/Image";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(BingAccountKey, BingAccountKey);

            // Build the query, limiting to 10 results.
            var imageQuery =
                bingContainer.Image(queryString, null, BingMarket, "Moderate", null, null, "Size:Small");
            imageQuery = imageQuery.AddQueryOption("$top", 1);
            // Run the query and display the results.
            var imageResults = imageQuery.Execute();
            StringBuilder searchResult = new StringBuilder();
           // Label lblResults = new Label();

            foreach (ImageResult iResult in imageResults)
            {
                searchResult.Append(string.Format("Image Title: <a href={1}>{0}</a><br />Image Url: {1}<br /><br />",
                    iResult.Title,
                    iResult.MediaUrl));
            }

            return searchResult.ToString();
            //lblResults.Text = searchResult.ToString();
            //Panel1.Controls.Add(lblResults);
        }
        */
        public static string SearchImagesByGoogle(string queryString)
        {
            //web request and regex
            string requestUrl = "http://images.google.com/searchbyimage?image_url=" + queryString;

            var response = Request(new Uri(requestUrl));
            var match = Regex.Match(response, "<div class=\"_hUb\">([^>]+)>(.+?)</div>");
            string result = match.Value;
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("</a></div>", "");
                result = result.Substring(result.LastIndexOf(">") + 1);
            }
            else
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response);
                var nodes = doc.DocumentNode.SelectNodes("//*[@class='r']/a");
                if (nodes == null)
                    return "";

                if (nodes.Count > 0)
                {
                    result = nodes.FirstOrDefault().InnerText;
                }

            }
            return result;
        }
        private static string Request(Uri uri)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                httpWebRequest.Method = "GET";

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                    return response;
                }
            }
            catch (WebException e)
            {
                throw e.InnerException;
            }
        }

        public static string ExtractTextFromImage(string imagePath)
        {
            using (var ocrEngine = new OnenoteOcrEngine())
            using (var image = Image.FromFile(imagePath))
            {
                var text = ocrEngine.Recognize(image);
                if (text == null)
                    return "";
                else
                   return text;
            }
        }

        public static string GetFiles(string url,string path)
        {
           WebClient Client = new WebClient();
            string fileName = Path.GetFileName(path);
            if (!Path.HasExtension(fileName))
            {
                fileName += ".png";
            }
           var localPath = HttpContext.Current.Server.MapPath(@"~\DownloadedContent\" + fileName);
            //  var filePath = HttpContext.Current.Server.MapPath(url);
            try
            {
                Client.DownloadFile(path, localPath);
            }
            catch(Exception ex) {
                return "";
            }
           return localPath;
        }
       
        public static string UploadImage(string image)
        {
            //imgur upload keys
            string ClientId = "cc7aeeee1df1e40";
            string ClientSecret = "8c38b1f8370a6234b5ad49c9c1c5978ad7d50b6b";

            WebClient w = new WebClient();
            w.Headers.Add("Authorization", "Client-ID " + ClientId);
            try
            {
                var values = new NameValueCollection
                {
                    { "key", ClientSecret },
                    { "image", Convert.ToBase64String(File.ReadAllBytes(image)) }
                };

              //  Keys.Add("image", Convert.ToBase64String(File.ReadAllBytes(image)));
                byte[] responseArray = w.UploadValues("https://api.imgur.com/3/image", values);
                String result = Encoding.ASCII.GetString(responseArray);
                Regex reg = new Regex("link\":\"(.*?)\"");
                Match match = reg.Match(result);
                string url = match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/");
                return url;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Something went wrong. " + s.Message);
                return ex.Message;
            }
             
        }

        public static string GetDataDir(Type t)
        {
            string c = t.FullName;
            c = c.Replace("AdaptationPlugins.", "");
            c = c.Replace('.', Path.DirectorySeparatorChar);
            string p = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Data", c));
            p += Path.DirectorySeparatorChar;
            //Console.WriteLine("Using Data Dir {0}", p);
            return p;
        }

        public static bool IsValidGDIPlusImage(string filename)
        {
            try
            {
                using (var bmp = new Bitmap(filename))
                {
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private static bool IsLocalPath(string p)
        {
            return new Uri(p).IsFile;
        }

        public static bool DoesUrlExist(string uriName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriName);
            request.Method = WebRequestMethods.Http.Head;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool pageExists = response.StatusCode == HttpStatusCode.OK;
                return true;
            }
            catch {
                return false;
            }
        }

    }
}
