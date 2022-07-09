using UnityEngine;
using TMPro;

namespace solar_a
{
    /// <summary>
    /// 選單控制系統，調用其他系統的功能，控制與共同方法。
    /// 本系統應放在必定存在於場上的物件上
    /// 目前放在： GameManage_UI
    /// 會將可重複調用的方法放於此處，在其他物件上原有的方法如有需要重複使用也會放在這。
    /// </summary>
    public class ManageCenter : MonoBehaviour
    {
        [SerializeField, Header("系統功能總表"), Tooltip("控制系統")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("火箭控制系統")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("場景控制系統")]
        SceneStage_Control ss_ctl;
        [SerializeField, Tooltip("場景系統")]
        ManageScene ss_mag;
        [SerializeField, Tooltip("預設產生器類別，請指定一個產生器物件")]
        Object_Generator gener_class;
        [SerializeField, Tooltip("結束管理")]
        ManageEnd mEnd;
        [SerializeField, Header("介面UI控制"), Tooltip("UI 距離顯示")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI 燃料顯示")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI 相關(Read Only)")]
        private int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }

        #region 共用欄位 (Public Feild)
        public CanvasGroup canvas_select;
        #endregion

        #region 共用方法 (Public Method)

        ///////////// 火箭控制相關
        /// <summary>
        /// 燃料變化
        /// </summary>
        /// <param name="f">輸入目前燃料</param>
        /// <returns></returns>
        public float fuelChange(float f)
        {
            f -= Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;
            return f;
        }

        ///////////// 產生物件

        public void AutoGenerate()
        {
            //gener_class.Static_gen(ss_ctl.transform.position.y);
            gener_class.Random_gen(ss_ctl.transform.position.y, false);
        }
        /// <summary>
        /// 切換預設的產生器類別，包含補給品產生。
        /// </summary>
        /// <param name="i">指定產生器內容，目前有的產生器如下：
        /// 0. 預設，產生 UFO 。
        /// 1. 產生 箱子 。
        /// 2. 產生 瓶子 。
        /// </param>
        public void AutoGenerate(int i)
        {
            switch (i)
            {
                case 0: gener_class = GameObject.Find("UFOGenerator").GetComponent<Object_Generator>(); break;
                case 1: gener_class = GameObject.Find("BOXGenerator").GetComponent<Object_Generator>(); break;
                case 2: gener_class = GameObject.Find("BottleGenerator").GetComponent<Object_Generator>(); break;
                default: gener_class = null; break;
            }

            if (gener_class != null) gener_class.Random_gen(ss_ctl.transform.position.y, true);
        }
        /// <summary>
        /// 預先產生物件方法，會依照目前的場景放置物件
        /// </summary>
        public void PreOrderGen()
        {
            int i = ss_mag.LoadScenes();
            switch (i)
            {
                case 0: break;
                default: break;
            }
        }

        ///////////// 選單變化相關
        /// <summary>
        /// 淡入動畫
        /// </summary>
        public void FadeIn()
        {
            if (canvas_select.alpha < 1)
            {
                canvas_select.alpha += 0.1f;
            }
            else
            {
                canvas_select.alpha = 1;
                canvas_select.interactable = true;
                canvas_select.blocksRaycasts = true;
                CancelInvoke("FadeIn");
            }

        }
        /// <summary>
        /// 淡出動畫
        /// </summary>
        public void FadeOut()
        {
            if (canvas_select.alpha > 0)
            {
                canvas_select.alpha -= 0.1f;
            }
            else
            {
                canvas_select.alpha = 0;
                canvas_select.interactable = false;
                canvas_select.blocksRaycasts = false;
                CancelInvoke("FadeOut");
            }

        }
        #endregion

        #region 本地控制方法或事件
        /// <summary>
        /// 設定UI的提示
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int)ss_ctl.stage_position.y;
            UI_fuel = (int)rocket_ctl.GetRocketInfo().x;
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }
        private void Update()
        {
            _show_UI();
        }
        #endregion

    }
}
