using UnityEngine;
using TMPro;
using System.IO;

namespace solar_a
{
    /// <summary>
    /// 遊戲結束系統，呼叫系統UI。
    /// 放入標題文字，設定要顯示的訊息，就能在結束時隨機產生文字。
    /// When the End, it will display messege onto screen.
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {
        [SerializeField, Header("標題文字")]
        TMP_Text weclome;
        [SerializeField, Tooltip("顯示訊息")]
        string[] messege;

        #region 方法
        private void ShowInfo(TMP_Text tp, string player="")
        {
            float distance = ManageCenter.UI_moveDistane;
            int onfuel = ManageCenter.UI_fuel;
            string[] tmps = { "與世長辭", "，我們懷念它", "，RIP" };
            string[] msg = new string[messege.Length + tmps.Length];
            for (int i =0; i < messege.Length + tmps.Length; i++)
            {
                if (i < tmps.Length) msg[i] = tmps[i];
                else {
                    msg[i] = messege[i - tmps.Length];
                }
            }
            //foreach (string s in msg) print(s);
            tp.text = $"{player} 在 {distance} 停下{msg[Random.Range(0, msg.Length)]}";
            //紀錄資訊
            RecordScores((int)(distance));
        }
        private void RecordScores(int score)
        {
            int preScroe = StaticSharp._SCORE;
            if (preScroe < score) StaticSharp._SCORE = score;
            else score = preScroe; //若沒有超過之前的紀錄，將之前的分數塞回去寫入。
            DataSave(score.ToString());
        }
        private void DataSave(string data)
        {
            string path = Application.persistentDataPath + "/scores.txt";
            FileStream file = new FileStream(path, FileMode.Open); // 開啟檔案
            StreamWriter sw = new StreamWriter(file);   // 寫入模式
            sw.WriteLine(data);  // 寫入資料
            sw.Close();
            file.Close();
        }
        #endregion

        private void Start()
        {
            if (weclome!=null) ShowInfo(weclome);                   //顯示結束文字
        }
    }
}