using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT.Plugins
{
    public class NavigationEnrichmentTechnique
    {
        public static void MakeAdaptation(HtmlDocument doc)
        {
            //bootstrap advice
            //<a class="sr-only sr-only-focusable" href="#content">Skip to main content</a>
            
            // Wrapper acts as a root element
            string newContent = "<a href='#maincontent' style='background:#444; color:white; z-index:999999999;'> Skip link </a> <br />";

            // Create new node from newcontent
            HtmlNode newNode = HtmlNode.CreateNode(newContent);

            // Get body node
            HtmlNode body = doc.DocumentNode.SelectSingleNode("//body");

            // Add new node as first child of body
            body.PrependChild(newNode);

            HtmlAttribute valueAttribute = body.Attributes["class"];

            if (valueAttribute != null)
            {
                var id = valueAttribute.Value;
            }

            var findclasses = body.Descendants("div").Where(d =>
                (d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("main")) || (d.Attributes.Contains("id") && d.Attributes["id"].Value.Contains("main")));

            if (findclasses.Any())
            {
                if (findclasses.FirstOrDefault().Attributes["id"] != null)
                {
                    findclasses.FirstOrDefault().Attributes["id"].Value = "maincontent";
                }
                else
                {
                    findclasses.FirstOrDefault().Attributes.Add("id", "maincontent");
                }
            }
        }
    }
}
