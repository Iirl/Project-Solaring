using System.IO;
using UnityEngine;

namespace solar_a
{
    public class LangRead : MonoBehaviour
    {
        [SerializeField , Header("�y�����")]
        ScriptData landata;
        /// <summary>
        /// �q�ɮ�Ū�����
        /// </summary>
        private void DataLoad()
        {
            string tmpdata;
            WWW tmpHeader;
            print(Resources.Load("/CH.txt").ToString());
            switch (landata.platform)
            {
                case ScriptData.Platforms.PC:
                    // �N���|�������eŪ���X�ӡA�b�N��Ƥ���
                    foreach (var data in landata.Language)
                    {
                        if (data.filename == "") continue;
                        //tmpdata = File.ReadAllText($"{Resources.Load($"\\{data.filename}.txt")}");
                        //data.datas = tmpdata.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

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



        private void Awake()
        {
            DataLoad();
        }

    }

}