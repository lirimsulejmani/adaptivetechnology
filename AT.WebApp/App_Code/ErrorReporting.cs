using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for ErrorReporting
/// </summary>
public class ErrorReporting
{
	    public static string AddErrorMsgToTag(string Html, string ErrorType, string Message, string Repair, string CheckId, XmlNode error)
        {
            var endTag = Html.IndexOf(">");
            var tag = GetTag(Html);
            var desc = error.SelectSingleNode("description").InnerText;
            var procedure = error.SelectSingleNode("procedure").InnerText;
            var reason = error.SelectSingleNode("reason").InnerText;

            var replacementTag = AddAttribute(tag, "emessage", ErrorType + "@" + HttpUtility.HtmlEncode(Message + "@" + FixCodeTags(Repair) + "@" + FixCodeTags(desc) + "@" + FixCodeTags(procedure) + "@" + FixCodeTags(reason)), true, "|");

            if (ErrorType.ToUpper() == "ERROR")
            {
                replacementTag = AddAttribute(replacementTag, "style", "border:1px dashed red !important;background-color:#FECCCB !important", false, ";");
            }
            else
            {
                replacementTag = AddAttribute(replacementTag, "style", "border:1px dashed red;background-color:#F5F8CB", true, ";");
            }
            replacementTag = AddAttribute(replacementTag, "onclick", "parent.showWarning(this); return false;", false, "");
            replacementTag = AddAttribute(replacementTag, "xid", CheckId, true, "|");

            return replacementTag + Html.Substring(endTag + 1);
        }

        public static string FixCodeTags(string text)
        {
            return text.Replace("<code>", "&lt;").Replace("</code>", "&gt;");
        }

        public static string GetTag(string Html)
        {
            int endTag = Html.IndexOf(">");
            return Html.Substring(0, endTag + 1);
        }

        public static string GetAttribute(string Html, string Attribute)
        {
            var startAttrib = Html.IndexOf(Attribute);

            if (startAttrib == -1)
            {
                return string.Empty;
            }
            else
            {
                var startPos = (startAttrib + Attribute.Length + 2);
                return Html.Substring(startPos, Html.IndexOf("\"", startPos) - startPos);
            }
        }

        public static string RemoveAttribute(string Html, string Attribute)
        {
            var startAttrib = Html.IndexOf(Attribute);

            if (startAttrib == -1)
            {
                return Html;
            }
            else
            {
                return Html.Substring(0, startAttrib) + Html.Substring(Html.IndexOf("\"", Attribute.Length + startAttrib + 3) + 1);
            }
        }

        public static string AddAttribute(string Html, string Attribute, string Value, bool KeepValues, string Seperator)
        {
            if (KeepValues)
            {
                var curValue = GetAttribute(Html, Attribute);
                if (curValue != string.Empty)
                {
                    Value = curValue + Seperator + Value;
                }
            }

            Html = RemoveAttribute(Html, Attribute);

            var space = Html.IndexOf(" ");
            if (space != -1)
            {
                return Html.Insert(space, " " + Attribute + "=\"" + Value + "\" ");
            }
            else
            {
                var endSlash = Html.IndexOf("/");
                if (endSlash > 0)
                {
                    return Html.Insert(endSlash - 1, " " + Attribute + "=\"" + Value + "\" ");
                }
                else
                {
                    return Html.Insert(Html.Length - 1, " " + Attribute + "=\"" + Value + "\" ");
                }
            }
        }

        public static string GetMsg(string ErrorMsg)
        {
            return ErrorMsg.Substring(ErrorMsg.IndexOf("target=\"_new\">") + 14).Replace("</a>", "").Replace("<code>", "&lt;").Replace("</code>", "&gt;");
        }

        public static string GetErrorID(string Html)
        {
            int startTag = Html.IndexOf("id=");
            int endTag = Html.IndexOf("\"", startTag);
            return Html.Substring(startTag + 3).Substring(0, endTag - startTag - 3);
        }
    }
