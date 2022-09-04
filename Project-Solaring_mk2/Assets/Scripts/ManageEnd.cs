using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

namespace solar_a
{
    /// <summary>
    /// 遊戲結束系統，呼叫系統UI。
    /// 放入標題文字，設定要顯示的訊息，就能在結束時隨機產生文字。
    /// When the End, it will display messege onto screen.
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {
        ManageCenter mgc;
        [SerializeField, Header("標題文字")]
        public TMP_Text messageLog;
        [SerializeField, Tooltip("顯示訊息")]
        string[] message;
        // data Content

        #region 方法
        private void ShowInfo(TMP_Text tp, string player="")
        {
            int level = StaticSharp._LEVEL;
            float distance = ManageCenter.UI_moveDistane;
            Vector3 pos = mgc.GetRocketPosition();
            string dataRecord;
            string[] tmps = { "R.I.P" };
            string[] msg = new string[message.Length + tmps.Length];
            for (int i =0; i < message.Length + tmps.Length; i++)
            {
                if (i < tmps.Length) msg[i] = tmps[i];
                else {
                    msg[i] = message[i - tmps.Length];
                }
            }
            //foreach (string s in msg) print(s);
            if (StaticSharp._LANG_ID == 1) tp.text = $"Player {player} at {distance.ToString("0")} Crashed.\n{msg[Random.Range(0, msg.Length)]}";
            else tp.text = $"You {player} 在 {distance.ToString("0")} 結束了旅途。\n{msg[Random.Range(0, msg.Length)]}";
            //紀錄資訊
            pos = distance > StaticSharp._SCORE ? pos : StaticSharp._RECORDPOS;
            distance = RecordScores(pos,(int)(distance));

            dataRecord = $"{level}@{distance}@{pos.x}@{pos.y}@{pos.z}";
            DataSave(dataRecord);
        }
        private float RecordScores(Vector3 pop3,int score)
        {
            int preScroe = StaticSharp._SCORE;
            if (preScroe < score)
            {
                StaticSharp._RECORDPOS = pop3;
                StaticSharp._SCORE = score;
            }
            else score = preScroe; //若沒有超過之前的紀錄，將之前的分數塞回去寫入。
            return score;
        }
        private void DataSave(string data)
        {
            string path = Application.persistentDataPath + "/scores.txt";
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write); // 開啟檔案
            StreamWriter sw = new StreamWriter(file);   // 寫入模式
            sw.WriteLine(data);  // 寫入資料
            sw.Close();
            file.Close();
        }
        #endregion


        private void Awake()
        {
            mgc = GetComponent<ManageCenter>();
        }
        private void Start()
        {
            if (messageLog!=null) ShowInfo(messageLog);                   //顯示結束文字
        }
    }
}