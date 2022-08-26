using System.IO;
using UnityEngine;

namespace solar_a
{
    public class LangRead : MonoBehaviour
    {
        [SerializeField , Header("語言資料")]
        ScriptData landata;
        /// <summary>
        /// 從檔案讀取資料
        /// </summary>
        private void DataLoad()
        {
            string tmpdata;
            WWW tmpHeader;
            print(Resources.Load("/CH.txt").ToString());
            switch (landata.platform)
            {
                case ScriptData.Platforms.PC:
                    // 將路徑中的內容讀取出來，在將資料切割
                    foreach (var data in landata.Language)
                    {
                        if (data.filename == "") continue;
                        //tmpdata = File.ReadAllText($"{Resources.Load($"\\{data.filename}.txt")}");
                        //data.datas = tmpdata.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                    }
                    break;
                case ScriptData.Platforms.Mobile:
                    // 將路徑轉成網址，在將資料切割
                    foreach (var data in landata.Language)
                    {
                        tmpHeader = new WWW($"{Application.streamingAssetsPath}/{data.filename}.txt");
                        data.datas = tmpHeader.text.Split('\n');
                    }
                    break;
            }
        }



        private void Awake()
        {
            DataLoad();
        }

    }

}