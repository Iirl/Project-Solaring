using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 文本資料檔 Script Data.
    /// 預定作為所有腳本的基礎資料檔
    /// UI與對話會分別以不同的系統程式執行
    /// </summary>
    [CreateAssetMenu(fileName = "New scenario", menuName = "SolarPrject/Language Script")]
    public class ScriptData : ScriptableObject
    {
        [SerializeField, Header("平台類型")]
        public Platforms platform;
        [Header("讀取資料"),NonReorderable]
        public LanguageClass[] Language;
        //
        public enum Platforms { PC, Mobile }
        [System.Serializable]
        public class LanguageClass
        {
            public string label;
            public string filename;
            public string[] datas;

        }
        
    }

    


}
