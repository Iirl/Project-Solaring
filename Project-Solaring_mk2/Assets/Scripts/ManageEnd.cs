using UnityEngine;


namespace solar_a
{
    /// <summary>
    /// 笴栏挡╰参㊣╰参UI
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("い北╰参")]
        ManageCenter mgCenter;

        #region よ猭
        

        #endregion
        private void Start()
        {
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}