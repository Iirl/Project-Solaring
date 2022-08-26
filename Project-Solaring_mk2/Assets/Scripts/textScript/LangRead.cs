using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace solar_a
{
    public class LangRead : MonoBehaviour
    {
        [SerializeField, Header("語言資料")]
        private ScriptData landata;
        [SerializeField, Header("資料欄位"), NonReorderable]
        private TMProClass[] TextField;
        //

        [System.Serializable]
        public class TMProClass
        {
            public string label;
            public TMP_Text tmpText;
        }
        /// <summary>
        /// 從檔案讀取資料
        /// </summary>
        private void DataLoad()
        {
            string tmpdata;
            WWW tmpHeader;
            switch (landata.platform)
            {
                case ScriptData.Platforms.PC:
                    // 將路徑中的內容讀取出來，在將資料切割
                    foreach (var data in landata.Language)
                    {
                        if (data.filename == "") continue;
                        tmpdata = Resources.Load<TextAsset>($"{data.filename}").ToString();
                        data.datas = tmpdata.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).
                            Where(str => str.Substring(0, 3) != "---").ToArray();

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
        private void DataWrite(int lang = 0)
        {
            float spices = landata.Language.Length;
            if (lang >= spices) return;     //防止溢位
            if (landata.Language[lang] != null)
            {
                for (int i = 0; i < TextField.Length; i++)
                {
                    if (TextField[i].tmpText != null)
                    {
                        TextField[i].tmpText.text = landata.Language[lang].datas[i];
                    }
                }
            }


        }
        private void DataWrite(string str)
        {            
            for (int i = 0; i< landata.Language.Length;i++) 
                if (landata.Language[i].label.Contains(str)) DataWrite(i);
        }
        //
        public void ChangeLanguage(int i) => DataWrite(i);
        public void ChangeLanguage(string label) => DataWrite(label);

        private void Awake()
        {
            DataLoad();
            DataWrite();
        }

    }

}