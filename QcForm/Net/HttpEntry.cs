using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;
using System.Net.Cache;

namespace ILazy.Net
{
    public enum Mothed
    {
        POST,
        GET
    }
    /// <summary>
    /// Http基类
    /// </summary>
    public abstract class HttpBase
    {
        /// <summary>
        /// Http的请求地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Http请求发送数据
        /// </summary>
        public string Data { set; get; }
        /// <summary>
        /// Http请求类型
        /// </summary>
        public Mothed Mothed { set; get; }
        /// <summary>
        /// 初始化管理
        /// </summary>
        public HttpBase() {
            this.Mothed = Net.Mothed.GET;
            this.ContentType = "application/x-www-form-urlencoded";
            this.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322)";
        }
        public HttpBase(string url)
        {
            this.URL = url;
            this.Mothed = Net.Mothed.GET;
            this.ContentType = "application/x-www-form-urlencoded";
            this.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon; .NET CLR 1.1.4322)";
        }
        /// <summary>
        /// 执行http的请求
        /// </summary>
        public abstract void doRequest();

       /// <summary>
        /// 网络请求
        /// </summary>
        public HttpWebRequest HttpRequest;
        /// <summary>
        /// 网络回复
        /// </summary>
        public HttpWebResponse HttpResponse;
        /// <summary>
        /// 请求内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 网页的内容
        /// </summary>
        public string HtmlResult { get; set; }

        
    }

    /// <summary>
    /// Http协议
    /// </summary>
    public class HttpEntry : HttpBase
    {

        public delegate void Request(object sender);
        public event Request onRequest;

        public delegate void Error(string msg);
        public event Error OnError;

        public delegate void NotFound(string msg);
        public event NotFound OnNotFound;


        public HttpEntry() : base() { }

        public HttpEntry(string url)
            : base(url)
        {
 
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate,X509Chain chain, SslPolicyErrors errors)
        {  // 总是接受  
            return true;
        }

        /// <summary>
        /// 执行页面的请求
        /// </summary>
        public override void doRequest()
        {

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                this.HttpRequest = (HttpWebRequest)WebRequest.Create(this.URL);

                this.HttpRequest.Method = (Net.Mothed.POST == this.Mothed) ? "POST" : "GET";

                this.HttpRequest.ContentType = this.ContentType;

                this.HttpRequest.UserAgent = this.UserAgent;

                if (this.Mothed == Net.Mothed.POST)
                {
                    this.HttpRequest.ContentLength = Encoding.UTF8.GetByteCount(this.Data);

                    byte[] param = Encoding.UTF8.GetBytes(this.Data);

                    using (Stream reqStream = this.HttpRequest.GetRequestStream())
                    {
                        reqStream.Write(param, 0, param.Length);
                    }
                }

                this.HttpResponse = (HttpWebResponse)this.HttpRequest.GetResponse();

                Stream myResponseStream = this.HttpResponse.GetResponseStream();

                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

                string retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();

                myResponseStream.Close();

                this.HtmlResult = retString;

                if (this.onRequest != null)
                {
                    this.onRequest(retString);
                }
            }
            catch (WebException ex)
            {
                ILazy.Log.iError(ex.ToString());
                if (this.OnError != null)
                {
                    this.OnError(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                ILazy.Log.iError(ex.ToString());
                if (this.OnError != null)
                {
                    this.OnError(ex.ToString());
                }
            }

        }

        
    }
}
