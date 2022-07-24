using UnityEngine;


namespace solar_a
{
    /// <summary>
    /// 遊戲結束系統，呼叫系統UI。
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;

        #region 方法
        

        #endregion
        private void Start()
        {
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}