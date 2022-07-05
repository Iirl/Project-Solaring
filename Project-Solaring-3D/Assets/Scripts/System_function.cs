using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

namespace solar_a
{
    public class System_function : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("取得場景")]
        GameObject Stage;
        BoxCollider Stage_boxBorder;
        public float stage_x { get; set; }
        public float stage_y { get; set; }
        public float stage_z { get; set; }
        [SerializeField, Header("UI 相關")]
        private int UI_moveDistane = 0, UI_fuel = 100;
        [SerializeField]
        TMP_Text ui_Dist, ui_fuel;



        CinemachineVirtualCamera cinemachine;
        #endregion
        #region 方法
        /// <summary>
        /// 設定UI的提示
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int)(Stage.transform.position.y);
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }

        /// <summary>
        /// 程式啟動時或者調整大小時要呼叫此函數調整邊界。
        /// </summary>
        private void Box_border()
        {
            Camera cv_mc = Stage_boxBorder.GetComponentInChildren<Camera>();
            stage_x = cv_mc.aspect * cinemachine.m_Lens.OrthographicSize * 2;     //width
            stage_y = (1 / cv_mc.aspect) * stage_x; //heigh
            stage_z = stage_y + 2;
            Stage_boxBorder.size = new Vector3(stage_x, stage_y, stage_z);
           

        }
        #endregion

        #region 全域控制方法
        /// <summary>
        /// 重讀場景
        /// </summary>
        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public float stageX() { return stage_x; }

        #endregion

        #region 事件
        private void Awake()
        {
            cinemachine = Stage.GetComponentInChildren<CinemachineVirtualCamera>();
            Stage_boxBorder = Stage.GetComponent<BoxCollider>();
            Box_border();
        }
        private void Update()
        {
            _show_UI();
            print(stage_x);
        }
        private void FixedUpdate()
        {

        }
        #endregion
    }
}