using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

        public void AutoGenerate(int i)
        {
            if (gener_class != null) gener_class.Static_gen(ss_ctl.transform.position.y, i);
        }
        public void AutoGenerate(bool rotate)
        {
            if (gener_class != null) gener_class.Random_gen(ss_ctl.transform.position.y, rotate, 0);
        }
        /// <summary>
        /// 切換預設的產生器類別，包含補給品產生。
        /// 這裡是設定要使用何種類別。
        /// 由於換個寫法，這裡暫時空置
        /// </summary>
        /// <param name="i">指定產生器內容，目前有的產生器如下：
        /// 0. 預設，產生 UFO 。
        /// 1. 產生 箱子 。
        /// 2. 產生 瓶子 。
        /// </param>
        public void AsignGenerate(int i)
        {
            string name = "";
            switch (i)
            {
                case 0: name = "ObjectGenerator"; break;
                case 1: name = "ObjectGenerator2"; break;
                default:break;
            }
            gener_class = GameObject.Find(name).GetComponent<Object_Generator>();
        }
        /// <summary>
        /// 產生附帶子物件的程式。
        /// 會先用一般生成的方式生成物件後，取得該物件的ID再依此生成子物件。
        /// 但如果使用預置物的話，物件會生成在子物件上。
        /// </summary>
        /// <param name="tg">哪種子物件要被生成</param>
        public void MeteoGenerate()
        {
            int obj = Random.Range(0, 3);
            int Gid = gener_class.Random_gen(ss_ctl.transform.position.y, false,obj); // Fist: Generate SubObject.
            // Second: Load Insub Prefabs.
            List<Object> pfabs = new()
            {
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/Empty.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal01.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal02.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_PurpCrystal01.prefab", typeof(Object))
            };
            
            // Third: Input on the subobject.
            gener_class.Random_Metro(Gid, pfabs);
        }
        public void ObjectDestory(GameObject obj)
        {
            gener_class.Destroys(obj);
        }
        /////////////////////場 景//////////////////////////////
        /// <summary>
        /// 預先產生物件方法，會依照目前的場景放置物件。
        /// i 代表目前的場景編號，可以在這裡指定場景為哪個時，用何種方式生成何種物件。
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

        public void test()
        {
            gener_class.r();
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
        /// <summary>
        /// 遊戲結束處理情況
        /// </summary>
        public void GameOver()
        {
            mEnd.enabled = true;
            rocket_ctl.enabled = false;
            rocket_ctl.GetComponent<Rigidbody>().isKinematic = true;
            ss_ctl.enabled = false;
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
            if (UI_fuel <= 0) GameOver(); //結束遊戲條件之一
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
