using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace solar_a
{
    /// <summary>
    /// persistentDataPath 的檔案存取
    /// 本檔案只會在 Menu 開始時存取，因為設定資料放在Menu中。
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        [SerializeField, Header("檔案名稱")]
        private string setting = "setting.txt";
        private string scores = "scores.txt";
        [SerializeField, Header("設定資料")]
        private Slider volSlider;
        [SerializeField]
        private Toggle[] langToggle;
        [SerializeField]
        private Button[] moveBTN;
        [SerializeField]
        private Button[] spineBTN;
        // 本地屬性
        private string path;
        #region 存取方法
        /// <summary>
        /// 從路徑檔案讀取設定資料
        /// </summary>
        /// <param name="path">包含檔案名稱的路徑資料</param>
        /// <returns>讀取的字串資料</returns>
        private string DataLoad(string path) => File.ReadAllText(path);
        private int DataLoadInt(string path) { 
            if (File.ReadAllText(path) != "") return Convert.ToInt32(File.ReadAllText(path));
            return -1;
        }
        /// <summary>
        /// 將輸入的資料存到檔案中
        /// </summary>
        /// <param name="datas"></param>
        private void DataSave(string data, string path)
        {
            FileStream file = new FileStream(path, FileMode.Open); // 開啟檔案
            StreamWriter sw = new StreamWriter(file);   // 寫入模式
            sw.WriteLine(data);  // 寫入資料
            sw.Close();
            file.Close();
        }
        /// <summary>
        /// 取得資料路徑
        /// </summary>
        /// <returns>回傳 persistent 路徑結合檔案名稱</returns>
        private string GetPath(string filename)
        {
            path = Application.persistentDataPath + "/" + filename;
            if (!File.Exists(path))
            {
                FileStream file = new FileStream(path, FileMode.Create); // 沒有檔案就建一個
                file.Close();
            }
            return path;
        }
        /// <summary>
        /// 取得目前選單的設定
        /// </summary>
        /// <returns>將所有設定併成一個字串資料</returns>
        private string TakeSetting()
        {
            string data = "";
            data += volSlider.value + "@";
            foreach (var lang in langToggle)
            {
                data += lang.isOn + "@";
            }
            return data;
        }
        /// <summary>
        /// 將設定資料放到暫存記憶體
        /// </summary>
        /// <param name="setData">要讀進的資料，可以配合 DataLoad 使用。</param>
        private void PutSetting(string setData)
        {
            string[] datas = setData.Split('@');
            StaticSharp._VOLUME = float.Parse(datas[0]);
            volSlider.value = StaticSharp._VOLUME;
            langToggle[0].isOn = (datas[1].ToLower().Contains("true") ? true : false);
            langToggle[1].isOn = (datas[2].ToLower().Contains("true") ? true : false);
            for (int i = 0; i < langToggle.Length; i++) if (langToggle[i].isOn) StaticSharp._LANG_ID = i;
        }
        private void PutSources(string setData)
        {
            string[] datas = setData.Split('@');
            StaticSharp._LEVEL = Convert.ToInt32(datas[0]);
            StaticSharp._SCORE = Convert.ToInt32(datas[1]);
            StaticSharp._RECORDPOS = new Vector3(float.Parse(datas[2]), float.Parse(datas[3]), float.Parse( datas[4]));
        }
        #endregion
        // 公用存取方法
        public void SaveSettingData() => DataSave(TakeSetting(), GetPath(setting));
        public void SaveScoreData(int src) => DataSave(src.ToString(), GetPath(scores));
        public void LoadSettingData() => PutSetting(DataLoad(GetPath(setting))); //從檔案讀取設定資料
        public void LoadScoreData() => PutSources(DataLoad(GetPath(scores))); //從檔案讀取分數資料

        private void Awake()
        {
            LoadSettingData();
            LoadScoreData();
            print(GetPath(setting));
        }
    }
}

// <筆記>
// 執行順序為：
// * 存檔
//      1. 先取得檔案路徑
//      2. 將要存檔的資料與路徑填入 DataSave
// * 讀取
//      1. 先取得檔案路徑
//      2. DataLoad 會回傳一個未切割的字串資料
// 