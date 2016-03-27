using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RisshiSummary.Action
{
    public class KuMaker
    {
        /// <summary>
        /// 句作成メイン処理
        /// </summary>
        /// <param name="inputedTxt"></param>
        /// <returns></returns>
        public HaikuResult CreateKu(string inputedTxt,string mode)
        {
            //エラーの場合の戻り値
            var errorRet = (mode == "0") ? new HaikuResult() { Part1 = "この文じゃ", Part2 = "俳句は俳句は", Part3 = "作れない" } :
                 new HaikuResult() { Part1 = "残念ながら", Part2 = "この文章じゃ", Part3 = "都都逸都都逸", Part4 = "作れない" };
          
            MecabHandler mecab = new MecabHandler();
            AllList = mecab.GetMecabResult(inputedTxt);

            try
            {
                string[] bannedMeshi = new string[] { "接尾", "非自立" };

                //リスト生成
                MeshiList = (from r in AllList
                             where (r.WordType == "名詞" &&  !bannedMeshi.Contains(r.WordData[1])) 
                             select r).ToList();
                DoshiList = (from r in AllList
                             where r.WordType == "動詞" && r.WordData[1] == "自立" && r.WordData[6] != "する"
                             select r).ToList();

                //スタート候補が閾値以下ならエラー
                if (MeshiList.Count < 5) { return errorRet; }

                HaikuResult retVal = new HaikuResult();

                if (mode == "0")
                {
                    //俳句
                    retVal.Part1 = kuMake(5, true);
                    retVal.Part2 = kuMake(7, false);
                    retVal.Part3 = kuMake(5, false);

                    if (string.IsNullOrEmpty(retVal.Part1) || string.IsNullOrEmpty(retVal.Part2) || string.IsNullOrEmpty(retVal.Part3))
                        return errorRet;
                }
                else
                {
                    //都都逸
                    retVal.Part1 = kuMake(7, true);
                    retVal.Part2 = kuMake(7, false);
                    retVal.Part3 = kuMake(7, false);
                    retVal.Part4 = kuMake(5, false);

                    if (string.IsNullOrEmpty(retVal.Part1) || string.IsNullOrEmpty(retVal.Part2)
                        || string.IsNullOrEmpty(retVal.Part3) || string.IsNullOrEmpty(retVal.Part4))
                        return errorRet;
                }
                return retVal;
            }
            catch (Exception e)
            {
                return errorRet;
            }
        }

        /// <summary>
        /// 全単語リスト
        /// </summary>
        private List<MecabResult> AllList;
        /// <summary>
        /// 名詞リスト
        /// </summary>
        private List<MecabResult> MeshiList;
        /// <summary>
        /// 動詞リスト
        /// </summary>
        private List<MecabResult> DoshiList;
        /// <summary>
        /// 名詞動詞リスト
        /// </summary>
        private List<MecabResult> MeshiDoshiList { get { return MeshiList.Union(DoshiList).ToList<MecabResult>(); } }


        /// <summary>
        /// 各句を作る
        /// </summary>
        /// <param name="startFrom"></param>
        /// <param name="mojisu"></param>
        /// <returns></returns>
        private string kuMake(int mojisu ,bool isHokku)
        {
            string retstr = "";
            int yomiCount = 0;
            MecabResult lasW = new MecabResult();//LastWord用ワーク

            //発句なら初期化
            if (isHokku) { Saved.Refresh(); }

            Random rnd = new System.Random();

            List<MecabResult> startList =((Saved.LastWord.WordData[1] !=null && Saved.LastWord.WordData[1].Contains("連体")) 
                                                                            || Saved.LastWord.WordType == "形容詞") ?
                MeshiList.OrderBy(_ => rnd.Next()).Where(x => !Saved.UsedWord.Contains(x)).ToList<MecabResult>():
                MeshiDoshiList.OrderBy(_ => rnd.Next()).Where(x => !Saved.UsedWord.Contains(x)).ToList<MecabResult>();

            //前の句がをがで終わっているときはこの句に動詞が含まれないとだめ
            bool mustContainDoshi=(Saved.LastWord.Word == "を" || Saved.LastWord.Word == "が");

            //スタート候補単語でループ
            for (int i=0; i < startList.Count; i++)
            {
                bool  containDoshi = false;
                //bool containDoshi = false;//この句の中に動詞を含んでいるかどうか
                yomiCount = 0;
                retstr = "";

                //スタート候補単語に後ろの単語をくっつけてゆく
                for (int j = startList[i].Index; yomiCount + AllList[j].Yomi <= mojisu; j++)
                {
                    retstr += AllList[j].Word;
                    yomiCount += AllList[j].Yomi;
                    if (AllList[j].WordType == "動詞") { containDoshi = true; }

                   lasW = AllList[j];
                }

                // 指定文字数になったらリターン
                if (yomiCount == mojisu)
                {
                    if (mustContainDoshi && !containDoshi) { continue; }　//動詞を含まないといけないのに動詞が入っていない
                    if(lasW.WordData[5].Contains("連用") || lasW.WordData[5].Contains("接続")) { continue; }　//半端な語で句が終わっている
                    Saved.UsedWord.Add(startList[i]);
                    Saved.LastWord = lasW;
                    return retstr;
                }                   
            }
            return string.Empty;
        }

        /// <summary>
        /// 句の前後関係保持用静的クラス
        /// </summary>
        static class Saved
        {
            public static MecabResult LastWord { get; set; }
            public static List<MecabResult> UsedWord { get; set; }
           

            /// <summary>
            /// 発句用セーブ値リフレッシュ
            /// </summary>
            public static void Refresh()
            {
                LastWord = new MecabResult();
                LastWord.WordData = new string[9]; //OutOfIndex対策
                UsedWord = new List<MecabResult>();
                
            }
        }
    }

    /// <summary>
    /// 俳句格納モデル
    /// </summary>
    public class HaikuResult
    {
        public string Part1 { get; set; }
        public string Part2 { get; set; }
        public string Part3 { get; set; }
        public string Part4 { get; set; }
        public string Part5 { get; set; }
    }
}
