using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// �奻����� Script Data.
    /// �w�w�@���Ҧ��}������¦�����
    /// UI�P��ܷ|���O�H���P���t�ε{������
    /// </summary>
    [CreateAssetMenu(fileName = "New scenario", menuName = "SolarPrject/Language Script")]
    public class ScriptData : ScriptableObject
    {
        [SerializeField, Header("���x����")]
        public Platforms platform;
        [Header("Ū�����"),NonReorderable]
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
