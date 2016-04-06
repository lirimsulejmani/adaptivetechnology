using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;

/// <summary>
/// Summary description for Check
/// </summary>
public class Check
{
	public Check()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static string AccessibilityApiUrl
    {
        get;
        set;
    }

    public static string ApiKey
    {
        get;
        set;

    }

    public static string Checks
    {
        get;
        set;
    }

    public static bool IgnoreValidatorWarnings
    {
        get;
        set;
    }

    public static bool IgnoreCheckerWarnings
    {
        get;
        set;
    }

    public static bool ShowErrors
    {
        get;
        set;
    }

    public static XmlDocument GetAccessibilityResults(string weblink,string guidelines)
    {
        IgnoreValidatorWarnings = false;
        IgnoreCheckerWarnings = true;
        ShowErrors = true;
        AccessibilityApiUrl = "http://achecker.ca/checkacc.php ";
        Checks = guidelines;
        ApiKey = "a4cdc8a18af33dabfd2c492044c65c6772689cc4";

        HttpWebRequest req = WebRequest.Create(AccessibilityApiUrl) as HttpWebRequest;
        req.KeepAlive = false;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        //&offset=10
        //string postData = "id=" + ApiKey + "&html=" + System.Web.HttpUtility.UrlEncode(contentToBeChecked) + "&guide=" + Checks + "&output=rest";
        string postData = "id=" + ApiKey + "&uri=" + weblink + "&guide=" + Checks + "&output=rest";
        //string postData = "id=" + ApiKey + "&uri=" + weblink + "&guide=" + Checks + "&output=html";
        byte[] buffer = Encoding.ASCII.GetBytes(postData);
        req.ContentLength = buffer.Length;
        Stream PostData = req.GetRequestStream();
        PostData.Write(buffer, 0, buffer.Length);
        PostData.Close();


        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
        Stream dataStream = resp.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        responseFromServer = responseFromServer.Replace("&Acirc;", "").Replace("&nbsp;", " ");

        if (resp.StatusCode == HttpStatusCode.OK)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseFromServer);
            return doc;
        }
        else
        {
            XmlDocument errorDoc = new XmlDocument();
            errorDoc.LoadXml("<xml><status>ERROR</status><error>" + resp.StatusCode.ToString() + "</error><message>" + HttpUtility.HtmlEncode(responseFromServer) + "</message></xml>");
            return errorDoc;
        }

    }

    public static string GetAccessibilityHtmlResults(string weblink)
    {
        IgnoreValidatorWarnings = false;
        IgnoreCheckerWarnings = false;
        ShowErrors = true;
        AccessibilityApiUrl = "http://achecker.ca/checkacc.php ";
        Checks = "WCAG2-A";
        ApiKey = "a4cdc8a18af33dabfd2c492044c65c6772689cc4";

        HttpWebRequest req = WebRequest.Create(AccessibilityApiUrl) as HttpWebRequest;
        req.KeepAlive = false;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        //&offset=10
        //string postData = "id=" + ApiKey + "&html=" + System.Web.HttpUtility.UrlEncode(contentToBeChecked) + "&guide=" + Checks + "&output=rest";
        //string postData = "id=" + ApiKey + "&uri=" + weblink + "&guide=" + Checks + "&output=rest";
        string postData = "id=" + ApiKey + "&uri=" + weblink + "&guide=" + Checks + "&output=html";
        byte[] buffer = Encoding.ASCII.GetBytes(postData);
        req.ContentLength = buffer.Length;
        Stream PostData = req.GetRequestStream();
        PostData.Write(buffer, 0, buffer.Length);
        PostData.Close();

        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
        Stream dataStream = resp.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        responseFromServer = responseFromServer.Replace("&Acirc;", "").Replace("&nbsp;", " ");

        if (resp.StatusCode == HttpStatusCode.OK)
        {
            return responseFromServer;
        }
        else
        {
          return  "<xml><status>ERROR</status><error>" + resp.StatusCode.ToString() + "</error><message>" + HttpUtility.HtmlEncode(responseFromServer) + "</message></xml>";             
        }

    }


}