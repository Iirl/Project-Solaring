using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 讀取語言資料系統
    /// 主要的功能有：
    /// 1. 將資料寫入到腳本資料中
    /// 2. 讀取語言編號並置換欄位內的語言資料
    /// </summary>
    public class ScriptRead : MonoBehaviour
    {
        [SerializeField, Header("語言資料")]
        private ScriptData landata;
        [SerializeField, Header("文字欄位")]
        private TMP_Text OriganilData;
        [SerializeField, Header("UI資料欄位"), NonReorderable]
        private TMProClass[] TextField;
        //
        private List<string> LoadData = new List<string>();
        [System.Serializable]
        public class TMProClass
        {
            public string label;
            public int id;
            public TMP_Text TmpUGUI;
        }
        #region 資料處理
        /// <summary>
        /// 從檔案讀取資料
        /// </summary>
        private void DataLoad()
        {
            WWW tmpHeader;
            switch (landata.platform)
            {
                case ScriptData.Platforms.PC:
                    // 將路徑中的內容讀取出來，在將資料切割
                    int i = 0;
                    foreach (var data in landata.Language)
                    {
                        if (data.filename == "") continue;
                        LoadData.Add(Resources.Load<TextAsset>($"{data.filename}").ToString());
                        data.datas = LoadData[i].Split('\n', StringSplitOptions.RemoveEmptyEntries).
                            Where(str => str.Substring(0, 3) != "---").ToArray();
                        i++;
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
        /// <summary>
        /// 透過語言標號取得語言資料內容
        /// </summary>
        /// <param name="lang">語言編號</param>
        private void DataWrite(int lang = 0)
        {
            float spices = landata.Language.Length;
            if (lang >= spices) return;     //防止溢位
            if (landata.Language[lang] != null) { 
                for (int i = 0; i < TextField.Length; i++)
                {
                    if (TextField[i].TmpUGUI != null) TextField[i].TmpUGUI.text = landata.Language[lang].datas[TextField[i].id];
                    if (landata.Language[lang].font != null) TextField[i].TmpUGUI.font = landata.Language[lang].font;

                }
            }
        }
        private void DataOrigial(int lang = 0)
        {
            float spices = landata.Language.Length;
            if (lang >= spices) return;     //防止溢位
            if (LoadData.Count > 0) OriganilData.text = LoadData[lang]; 

        }

        /// <summary>
        /// 將設定的資料與語言名稱相符時寫上內容
        /// </summary>
        /// <param name="str">語言名稱</param>
        private void DataWrite(string str)
        {            
            for (int i = 0; i< landata.Language.Length;i++) 
                if (landata.Language[i].label.Contains(str)) DataWrite(i);
        }
        #endregion

        // 公用方法
        public void LoadTextToData() => DataLoad();
        public void ChangeLanguage(int i) => DataWrite(i);
        public void ChangeLanguage(string label) => DataWrite(label);

        private void Awake()
        {
            DataLoad();
        }
        private void Start()
        {
            int lid = StaticSharp._LANG_ID;
            DataWrite(lid);
            if(OriganilData != null) DataOrigial(lid);
        }
    }

}