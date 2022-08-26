using TMPro;
using UnityEngine;

namespace solar_a
{
    [CreateAssetMenu(fileName = "New scenario", menuName = "SolarPrject/Language Script")]
    public class ScriptData : ScriptableObject
    {
        [SerializeField, Header("平台類型")]
        public Platforms platform;
        [Header("讀取資料"),NonReorderable]
        public LanguageClass[] Language;
        [Header("資料欄位"), NonReorderable]
        public TMProClass[] TextField;
        //
        public enum Platforms { PC, Mobile }
        [System.Serializable]
        public class LanguageClass
        {
            public string label;
            public string filename;
            public string[] datas;

        }
        [System.Serializable]
        public class TMProClass
        {
            public string label;
            public TextMeshProUGUI text;

        }
        
    }

    


}
