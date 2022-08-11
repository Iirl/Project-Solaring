using UnityEngine;
using TMPro;


namespace solar_a
{
    /// <summary>
    /// 遊戲結束系統，呼叫系統UI。
    /// 放入標題文字，設定要顯示的訊息，就能在結束時隨機產生文字。
    /// When the End, it will display messege onto screen.
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;
        [SerializeField, Header("標題文字")]
        TMP_Text weclome;
        [SerializeField, Tooltip("顯示訊息")]
        string[] messege;

        #region 方法
        private void ShowInfo(TMP_Text tp, string player="")
        {
            float distance = mgCenter.UI_moveDistane;
            int onfuel = mgCenter.UI_fuel;
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
        }

        #endregion

        private void Start()
        {
            if (weclome!=null) ShowInfo(weclome);
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            //mgCenter.CheckGame(true);
            mgCenter.show_Menu();
        }
    }
}