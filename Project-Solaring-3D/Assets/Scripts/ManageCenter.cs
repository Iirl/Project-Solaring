using UnityEngine;
using TMPro;

namespace solar_a
{
    /// <summary>
    /// 選單控制系統，調用其他系統的功能，控制與共同方法。
    /// </summary>
    public class ManageCenter : MonoBehaviour
    {
        [SerializeField, Header("系統選單"), Tooltip("控制系統")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("火箭控制系統")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("場景管理系統")]
        SceneStage_Control ss_ctl;
        [SerializeField, Header("介面UI控制"), Tooltip("UI 距離顯示")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI 燃料顯示")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI 相關(Read Only)")]
        private int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }


        #region 共用欄位 (Public Feild)

        #endregion

        #region 共用方法 (Public Method)

        #endregion

        #region 本地控制方法
        /// <summary>
        /// 設定UI的提示
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int) ss_ctl.stage_position.y;
            UI_fuel = (int) rocket_ctl.GetRocketInfo().x;
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }

        #endregion

        #region 事件調用
        private void Update()
        {
            _show_UI();
        }
        #endregion
    }
}
