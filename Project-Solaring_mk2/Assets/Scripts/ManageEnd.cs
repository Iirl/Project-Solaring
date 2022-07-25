using UnityEngine;
using TMPro;


namespace solar_a
{
    /// <summary>
    /// 遊戲結束系統，呼叫系統UI。
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;
        [SerializeField, Header("標題文字")]
        TMP_Text weclome;

        #region 方法
        private void ShowInfo(TMP_Text tp, string messege="")
        {
            float distance = mgCenter.UI_moveDistane;
            int onfuel = mgCenter.UI_fuel;
            string[] msg = { "與世長辭","，我們懷念它", "，RIP" };
            tp.text = $"{messege} 在 {distance} 停下{msg[Random.Range(0, msg.Length)]}";
        }

        #endregion

        private void Start()
        {
            ShowInfo(weclome);
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}