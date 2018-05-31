using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SS.Reward.Core
{
    public static class Utils
    {
        public static string GetMessageHtml(string message, bool isSuccess)
        {
            return isSuccess
                ? $@"<div class=""alert alert-success"" role=""alert"">{message}</div>"
                : $@"<div class=""alert alert-danger"" role=""alert"">{message}</div>";
        }

        public static void SelectListItems(ListControl listControl, params string[] values)
        {
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    item.Selected = false;
                }
                foreach (ListItem item in listControl.Items)
                {
                    foreach (var value in values)
                    {
                        if (string.Equals(item.Value, value))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        public static string GetUrlWithoutQueryString(string rawUrl)
        {
            string urlWithoutQueryString;
            if (rawUrl != null && rawUrl.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                var queryString = rawUrl.Substring(rawUrl.IndexOf("?", StringComparison.Ordinal));
                urlWithoutQueryString = rawUrl.Replace(queryString, "");
            }
            else
            {
                urlWithoutQueryString = rawUrl;
            }
            return urlWithoutQueryString;
        }

        public static string AddQueryString(string url, NameValueCollection queryString)
        {
            if (queryString == null || url == null || queryString.Count == 0)
                return url;

            var builder = new StringBuilder();
            foreach (string key in queryString.Keys)
            {
                builder.Append($"&{key}={HttpUtility.UrlEncode(queryString[key])}");
            }
            if (url.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                if (builder.Length > 0) builder.Remove(0, 1);
                return string.Concat(url, "?", builder.ToString());
            }
            if (url.EndsWith("?"))
            {
                if (builder.Length > 0) builder.Remove(0, 1);
            }
            return string.Concat(url, builder.ToString());
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            return string.Equals(a.Trim().ToLower(), b.Trim().ToLower());
        }

        private static object Eval(object dataItem, string name)
        {
            object o = null;
            try
            {
                o = DataBinder.Eval(dataItem, name);
            }
            catch
            {
                // ignored
            }
            if (o == DBNull.Value)
            {
                o = null;
            }
            return o;
        }

        public static int EvalInt(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToInt32(o);
        }

        public static decimal EvalDecimal(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToDecimal(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o?.ToString() ?? string.Empty;
        }

        public static DateTime EvalDateTime(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            if (o == null)
            {
                return DateTime.MinValue;
            }
            return (DateTime)o;
        }

        public static bool EvalBool(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o != null && Convert.ToBoolean(o.ToString());
        }

        public static string ReplaceNewline(string inputString, string replacement)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append(replacement);
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string SwalError(string title, string text)
        {
            var script = $@"swal({{
  title: '{title}',
  text: '{ReplaceNewline(text, string.Empty)}',
  icon: 'error',
  button: '关 闭',
}});";

            return script;
        }

        public static string SwalWarning(string title, string text, string btnCancel, string btnSubmit, string scripts)
        {
            var script = $@"swal({{
  title: '{title}',
  text: '{ReplaceNewline(text, string.Empty)}',
  icon: 'warning',
  buttons: {{
    cancel: '{btnCancel}',
    catch: '{btnSubmit}'
  }}
}})
.then(function(willDelete){{
  if (willDelete) {{
    {scripts}
  }}
}});";
            return script;
        }
    }
}
