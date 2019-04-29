using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace PizzaBoy
{
    class WebRequest_Wrapper
    {
        public static CookieContainer Cookies = new CookieContainer();

        #region " UserAgent."
        // Default
        private static string _UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
        public static string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        #endregion

        #region " Method."
        // Default
        private static string _Method = "GET";
        public static string Method
        {
            get { return _Method; }
            set { _Method = value.ToUpperInvariant(); }
        }
        #endregion

        #region " AllowAutoRedirect."
        // Default
        private static bool _AllowAutoRedirect = true;
        public static bool AllowAutoRedirect
        {
            get { return _AllowAutoRedirect; }
            set { _AllowAutoRedirect = value; }
        }
        #endregion

        #region " KeepAlive."
        // Default
        private static bool _KeepAlive = true;
        public static bool KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        #endregion

   
        public static object Request(webTask task)
        {
            String Host = task.URL;
            String Referer = task.URL;
            String POSTData = null;

            if (!task.retainCookies)
            {
                Cookies = new CookieContainer();
            }

            if (task.POSTData == null)
            {
                _Method = "GET";
            }
            else
            {
                _Method = "POST";
                POSTData = (String)task.POSTData;
            }
                        
            try
            {
                HttpWebRequest WebR = (HttpWebRequest)WebRequest.Create(Host);

                WebR.Method = _Method;
                WebR.CookieContainer = Cookies;
                WebR.AllowAutoRedirect = _AllowAutoRedirect;
                WebR.KeepAlive = _KeepAlive;
                WebR.UserAgent = _UserAgent;
                WebR.ContentType = "application/x-www-form-urlencoded";
                WebR.Referer = Referer;

                if ((_Method == "POST"))
                {
                    byte[] _PostData = null;
                    _PostData = System.Text.Encoding.Default.GetBytes(POSTData);
                    WebR.ContentLength = _PostData.Length;

                    System.IO.Stream StreamWriter = WebR.GetRequestStream();
                    StreamWriter.Write(_PostData, 0, POSTData.Length);
                    StreamWriter.Dispose();
                }

                HttpWebResponse WebResponse;
                string PageHTML; 

                try
                {
                   WebResponse =  (HttpWebResponse)WebR.GetResponse();
                   Cookies.Add(WebResponse.Cookies);
                   System.IO.StreamReader StreamReader = new System.IO.StreamReader(WebResponse.GetResponseStream());
                   PageHTML = StreamReader.ReadToEnd();
                }
                catch (WebException e)
                {
                    WebResponse response = e.Response;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        PageHTML = reader.ReadToEnd();
                    }
                }

                String pageTitle = System.Text.RegularExpressions.Regex.Match(PageHTML, @"<title>(.*<)/title>").Groups[1].ToString();

                if (task.exportCookies)
                {
                    try
                    {
                        CookieCollection toExport = Cookies.GetCookies(new Uri(task.URL));
                        foreach (Cookie toOutput in toExport)
                        {
                            System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + pageTitle + @" - Cookies.txt", toOutput.ToString() + "\r\n");
                        }
                    }catch(Exception ex)
                    {
                        System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\" + pageTitle + @" - Cookies.txt", ex.Message);
                    }
                }

                if (task.outputSource)
                {
                    if(PageHTML.Contains("<title> myicard.net Select</title>"))
                    {
                        return PageHTML;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("WebRequest exited with error: {0}", ex.Message));
                return null;
            }
        }

    }
}
