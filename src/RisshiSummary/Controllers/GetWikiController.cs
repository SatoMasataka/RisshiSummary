using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Linq;
using System.Web;
using RisshiSummary.Action;

namespace RisshiSummary.Controllers
{
    [Route("RisshiSummary/[controller]")]
    public class GetWikiController : Controller
    {
        // GET: api/values
        [HttpGet()]
        public RetVal Get()
        {
            return new RetVal() { WikiContent = "Wikiから取得できませんでした。" };
        }

        // GET api/values/5
        [HttpGet("{keyword}")]
        public RetVal Get(string keyword)
        {
            //string keyword = id;
            string content = GetFromWikipedia(keyword);
            if (string.IsNullOrEmpty(content))
                return new RetVal() { WikiContent = "Wikiから取得できませんでした。" };
            else
                return new RetVal() { WikiContent = content };
        }

        /// <summary>
        /// 戻り値格納用クラス
        /// </summary>
        public class RetVal
        {
            /// <summary>
            /// ウィキ内容
            /// </summary>
            public string WikiContent { get; set; }
        }

        /// <summary>
        /// 取得するテキストの最大文字数
        /// </summary>
        private const int MaxTextLength = 1000;

        /// <summary>
        /// Wikiから取得処理コア
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private string GetFromWikipedia(string keyword)
        {
            try
            {
                WebClient wc = new WebClient();
                NameValueCollection nameValue = new NameValueCollection();
                nameValue.Add("action", "query");
                nameValue.Add("prop", "revisions");
                nameValue.Add("rvprop", "content");
                nameValue.Add("redirects", "1");
                nameValue.Add("format", "xml");
                nameValue.Add("titles", HttpUtility.UrlEncode(keyword, Encoding.UTF8));
                wc.QueryString = nameValue;

                wc.Headers.Add("user-agent",
                    "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                wc.Headers.Add("Content-type", "charset=UTF-8"); ;

                string result = wc.DownloadString(@"http://ja.wikipedia.org/w/api.php");


                result = result.Replace(@"\", @"%");
                result = System.Web.HttpUtility.UrlDecode(result);

                var xmldoc = XDocument.Parse(result);
                var rev = xmldoc.Root.Descendants("rev").FirstOrDefault();
                if (rev == null)
                    return null;

                /////ここから加工処理
                //取得したテキストのうち、句点を含まない行は削除
                List<string> contentLines = rev.Value.Split('\n').ToList<string>();
                var r = (from l in contentLines
                         where l.Contains("。")
                         select l).ToArray<string>();

                //リスト化したテキストを再集結
                string returnString = string.Join("", r).Replace("[[", "").Replace("]]", "").Replace("<br />", "");

                //ここで文字数を少し減らして処理を省く
                returnString = (returnString.Length > MaxTextLength * 3) ? returnString.Substring(0, MaxTextLength * 3) : returnString;

                returnString = RemoveTag(returnString, "<ref", "</ref>");
                returnString = RemoveTag(returnString, "<small", "</small>");
                returnString = RemoveTag(returnString, "<!--", "-->");

                return (returnString.Length > MaxTextLength) ? returnString.Substring(0, MaxTextLength) : returnString;
            }
            catch (Exception e)
            {
                Log.OutputExceptionLog(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// タグを中身ごと取り除きます。
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private string RemoveTag(string txt,string tagStr,string tagEnd)
        {
            string retVal = txt;
            try
            {
                while (true)
                {
                    int removeStartPos = retVal.IndexOf(tagStr);
                    if (removeStartPos < 0) break;
                    int remveEndPos = retVal.IndexOf(tagEnd, removeStartPos) + tagEnd.Length - 1;
                    if (remveEndPos < 0) break;

                    retVal = retVal.Remove(removeStartPos, remveEndPos - removeStartPos + 1);
                }
            }
            catch { }
            return retVal;
        }
    }
}
