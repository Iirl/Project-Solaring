using TMPro;
using UnityEngine;

namespace solar_a
{
    [CreateAssetMenu(fileName = "New scenario", menuName = "SolarPrject/Language Script")]
    public class ScriptData : ScriptableObject
    {
        [SerializeField, Header("���x����")]
        public Platforms platform;
        [Header("Ū�����"),NonReorderable]
        public LanguageClass[] Language;
        [Header("������"), NonReorderable]
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
