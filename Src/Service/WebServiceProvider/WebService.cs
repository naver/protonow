using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Web;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Service.WebServer
{
    public class WebService : IWebServer
    {
        #region Constructor

        public WebService()
        {
            running = false; //running flag

            charEncoder = Encoding.UTF8; // To encode string

            contentPath = System.IO.Path.GetTempPath() + @"Compass\Preview";

            serverPort = 32767;

            ServerURL = "http://localhost";
           // ServerURL = "http://127.0.0.1";

            extensions = new Dictionary<string, string>()
            { 
                //{ "extension", "content type" }
                { ".htm", "text/html" },
                { ".html", "text/html" },
                { ".xml", "text/xml" },
                { ".txt", "text/plain" },
                { ".css", "text/css" },
                { ".map", "application/json" },
                { ".svg", "image/svg+xml" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".jpg", "image/jpg" },
                { ".jpeg", "image/jpeg" },
                { ".ico", "image/x-icon" },
                { ".zip", "application/zip"},
                { ".js","application/javascript"},
                { ".json", "application/json" },
                };
        }

        #endregion

        #region Private Init

        private bool running; //running flag

        private Encoding charEncoder; // To encode string

        private string contentPath; // Root path of our contents

        private Dictionary<string, string> extensions;

        private int serverPort;

        private string ServerURL; // Root path of our contents

        HttpListener _httpListener;

        #endregion

        #region  Public Interface

        public bool StartWebService()
        {
            if (running)
            {
                return running; // If it is already running, exit.
            }

            Random rnd = new Random();
            int iCount = 0;

            while (true)
            {
                serverPort = rnd.Next(30000 + (iCount * 200), 30000 + ((iCount+1) * 200));

                contentPath = System.IO.Path.GetTempPath() + @"Compass\Preview\" + serverPort.ToString();

                if (start(ServerURL, serverPort, contentPath))
                {
                    CommonFunction.SetPreViewTempPath(contentPath);
                    
                    return true;
                }

                if (++iCount == 50)
                { 
                    CommonFunction.SetPreViewTempPath("");
                    System.Windows.MessageBox.Show(Naver.Compass.Common.Helper.GlobalData.FindResource("Error_Local_Server"));
                    return false;
                }
            }
        }

        public void StopWebService()
        {
            stop();
        }

        public string GetWebUrl()
        {
            return ServerURL+":" + serverPort.ToString() + "/";
        }

        public void Dispose()
        {
            stop();
        }

        #endregion

        #region private fuction

        //start web server
        private bool start(string ipAddress, int port, string contentPath)
        {

            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            try
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(ipAddress + ":" + port.ToString() + "/");
                _httpListener.Start();
                running = true;
                this.contentPath = contentPath;
            }
            catch (HttpListenerException ex)
            {
                return false;
            }

            // One thread that will listen connection requests and create new threads to handle them.
            Thread requestListenerT = new Thread(() =>
            {
                while (running)
                {

                    try
                    {
                        HttpListenerContext context = _httpListener.GetContext(); // get a context
                        // Create new thread to handle the request and continue to listen the socket.
                        Thread requestHandler = new Thread(() =>
                        {
                            try
                            {
                                handleTheRequest(context);
                            }
                            catch
                            {
                                System.Diagnostics.Debug.WriteLine("Handle Request error!");
                            }
                        });
                        requestHandler.Start();
                    }
                    catch { }
                }

                System.Diagnostics.Debug.WriteLine("requestListenerT ->is out!");
            });

            requestListenerT.Start();

            return true;
        }

        //stop web server
        private void stop()
        {
            if (running)
            {
                running = false;
                try
                {
                    if (_httpListener.IsListening)
                    {
                        _httpListener.Stop();
                        _httpListener.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Shutdown httpListener failed!");
                }
            }
        }

        private void handleTheRequest(HttpListenerContext context)
        {
            string requestedFile;
            byte[] msg;

            if (context.Request.HttpMethod.Equals("GET") || context.Request.HttpMethod.Equals("POST"))
            {
                requestedFile = (HttpUtility.UrlDecode(context.Request.RawUrl)).Split('?')[0];

                requestedFile = requestedFile.Replace("/", "\\").Replace("\\..", ""); // Not to go back

                string path = contentPath + requestedFile;

                if (requestedFile.CompareTo(@"\") == 0)//Get root path start file
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "text/html";
                    msg = File.ReadAllBytes(contentPath + "\\index.html");
                }
                else//get r
                {
                    if (!File.Exists(path))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        msg = GetNoFoundData();
                    }
                    else
                    {
                        string FileExt = Path.GetExtension(path);

                        if (extensions.ContainsKey(FileExt))
                        {
                            if (FileExt.CompareTo(".js") == 0)
                            {
                                context.Response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
                            }
                            context.Response.ContentType = extensions[FileExt];
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            msg = File.ReadAllBytes(path);
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            msg = GetNoSuppportData();
                        }
                    }
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                msg = GetNoImplementData();
            }

            try
            {
                context.Response.ContentLength64 = msg.Length;

                using (Stream s = context.Response.OutputStream)
                {
                    s.Write(msg, 0, msg.Length);
                }
            }
            catch { }
            finally
            {
                context.Response.KeepAlive = false;
                context.Response.OutputStream.Close();
            }
        }

        private byte[] GetNoSuppportData()
        {

            return Encoding.UTF8.GetBytes("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>protoNow Local Web Server</h2><div>404 -Not Support</div></body></html>");
        }


        private byte[] GetNoFoundData()
        {

            return Encoding.UTF8.GetBytes("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>protoNow Local Web Server</h2><div>404 -Not Found</div></body></html>");
        }

        private byte[] GetNoImplementData()
        {

            return Encoding.UTF8.GetBytes("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><h2>protoNow Local Web Server</h2><div>501 - Method Not Implemented</div></body></html>");
        }

        #endregion
    }
}
