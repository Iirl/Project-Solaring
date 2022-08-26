using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace solar_a
{
    public class LangRead : MonoBehaviour
    {
        [SerializeField, Header("�y�����")]
        private ScriptData landata;
        [SerializeField, Header("������"), NonReorderable]
        private TMProClass[] TextField;
        //

        [System.Serializable]
        public class TMProClass
        {
            public string label;
            public TMP_Text tmpText;
        }
        /// <summary>
        /// �q�ɮ�Ū�����
        /// </summary>
        private void DataLoad()
        {
            string tmpdata;
            WWW tmpHeader;
            switch (landata.platform)
            {
                case ScriptData.Platforms.PC:
                    // �N���|�������eŪ���X�ӡA�b�N��Ƥ���
                    foreach (var data in landata.Language)
                    {
                        if (data.filename == "") continue;
                        tmpdata = Resources.Load<TextAsset>($"{data.filename}").ToString();
                        data.datas = tmpdata.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).
                            Where(str => str.Substring(0, 3) != "---").ToArray();

                    }
                    break;
                case ScriptData.Platforms.Mobile:
                    // �N���|�ন���}�A�b�N��Ƥ���
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
            if (lang >= spices) return;     //�����
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