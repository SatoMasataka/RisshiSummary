using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RisshiSummary.Action
{
    /// <summary>
    /// Mecab日本語解析クラス
    /// </summary>
    public class MecabHandler
    {
        /// <summary>
        /// 解析結果を取得する
        /// </summary>
        /// <param name="txt"></param>
        public List<MecabResult> GetMecabResult(string txt)
        {
            if (string.IsNullOrEmpty(txt)) return null;

            txt = removeZenkakuKigo(txt);

            mecabResultList = new List<MecabResult>();

            //プロセスを作成し、MeCab.exeを指定します
            Process process = new Process();

            process.StartInfo.FileName = Startup.Configuration["AppPath:MecabPath"];// "C:\\Program Files (x86)\\MeCab\\bin\\mecab.exe";//Startup.Configuration.Get("AppPath:MecabPath");

            //UseShellExecuteプロパティをFalseに設定し、RedirectStandardOutputプロパティをTrueに設定しMeCabの出力をリダイレクトします。
            //さらにOutputDataReceivedにイベントハンドラを設定し、MeCabの出力を非同期で読み取れる設定にします。
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler(process_DataReceived);
            //process.Exited += new EventHandler(process_Exit);

            //RedirectStandardInputプロパティをTrueに設定し、MeCabへの入力をリダイレクトします。
            process.StartInfo.RedirectStandardInput = true;

            process.Start();
            process.BeginOutputReadLine();

            //標準入力で内容をMeCab.exeに入力します。
            StreamWriter sw = process.StandardInput;
            sw.AutoFlush = true;
            sw.WriteLine(txt);

            int prevAnalyseNum = -1;
            while (prevAnalyseNum != mecabResultList.Count())
            {
                prevAnalyseNum = mecabResultList.Count();
                Thread.Sleep(1000);                
            }

            return mecabResultList;

        }

        private List<MecabResult> mecabResultList { get; set; }

        /// <summary>
        /// Mecabからの結果を受け取る時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void process_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string[] s = ((string)e.Data).Split('\t');
            
            if (s.Length < 2 )　return;　//EOS対策

            string[] wData=s[1].Split(',');
            var mResu = new MecabResult() { Index = mecabResultList.Count(),
                                            Word = s[0],
                                            WordType = wData[0],
                                            WordData = wData,
                                            Yomi = (wData.Last()!="*") ? removeSmall( wData.Last()) : removeSmall(s[0]) };

            //Addしないパターン(記号・英数・半角)
            if (mResu.WordType.Contains("記号") || 
                Regex.IsMatch(mResu.Word, "^[ａ-ｚＡ-Ｚ０-９]+$") || 
                isHankaku(mResu.Word)) return;

            mecabResultList.Add(mResu);
        }

        /// <summary>
        /// 文字列から全角記号を削除
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private string removeZenkakuKigo(string txt)
        {
            List<string> zenkaku = new List<string> { "！","”","＃","＄","％","＆","’","（","）","＝","～","｜","‘",
                       "｛","＋","＊","｝","＜","＞","？","＿","－","＾","￥","＠","「","；","：","」","、","。","・" };　//全角記号

            foreach (string zen in zenkaku){ txt = txt.Replace(zen,""); }
            return txt;
        }


        /// <summary>
        /// 小さい文字を除いた文字数をかえす
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private int removeSmall(string txt)
        {
            List<string> komoji = new List<string> (){ "ャ", "ュ", "ョ", "ァ", "ィ", "ゥ", "ェ", "ォ" };
            foreach (string k in komoji){txt= txt.Replace(k, "");}
            return txt.Length;
        }

        /// <summary>
        /// 半角判定
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isHankaku(string str)
        {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            int num = sjisEnc.GetByteCount(str);
            return num == str.Length;
        }
    }

    /// <summary>
    /// 解析結果
    /// </summary>
    public class MecabResult
    {
        public int Index { get; set; }
        /// <summary>
        /// 単語
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// 品詞
        /// </summary>
        public string WordType { get; set; }
        /// <summary>
        /// 単語情報をsplit(",")で入れている
        /// </summary>
        public string[] WordData{ get; set; }
        /// <summary>
        /// 読み文字数
        /// </summary>
        public int Yomi{ get; set; }
    }


}
