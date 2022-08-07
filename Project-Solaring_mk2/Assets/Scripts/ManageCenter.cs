using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        #region 管理控制系統
        [SerializeField, Header("系統功能總表"), Tooltip("控制系統")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("火箭控制系統")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("場景控制系統")]
        SceneStage_Control ss_ctl;
        [SerializeField, Tooltip("場景系統")]
        ManageScene ss_mag;
        [SerializeField, Tooltip("結束管理")]
        ManageEnd mEnd;
        #endregion
        /// <summary>
        /// 產生器控制
        /// </summary>
        [SerializeField, Header("預設產生器類別，請指定一個產生器物件")]
        Object_Generator gener_class;
        [SerializeField, Tooltip("設定產生器物件")]
        private string[] objectName = { "補給品", "隕石" };
        [SerializeField, Tooltip("設定多少距離產生一次物件")]
        private int[] objectDistance = { 50, 30 };
        /// <summary>
        /// GameUI Interface.
        /// </summary>
        [SerializeField, Header("介面UI控制"), Tooltip("UI 距離顯示")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI 燃料文字")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI 燃料條")]
        Image ui_fuelbar;
        [SerializeField]
        GameObject[] EnergyPlus;
        [SerializeField, Tooltip("UI 相關(Read Only)")]
        public int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }
        /// <summary>
        /// 選單畫布控制
        /// </summary>
        [SerializeField, Header("暫停選單")]
        GameObject pauseUI;
        [SerializeField, Tooltip("暫停選單畫布系統")]
        CanvasGroup menus;
        [Tooltip("程式控制選擇目前的畫布，可以不用設定。")]
        public CanvasGroup canvas_select;
        [SerializeField, Header("畫布淡化速度")]
        Vector2 fadeSpeed = Vector2.zero + Vector2.one * 0.01f;
        #region 本地調閱欄位 (Private Feild)
        private bool isGen = false;

        #endregion

        #region 共用方法 (Public Method)

        public Vector3 GetStagePOS()
        {
            return ss_ctl.transform.position;
        }
        ///////////// 火箭控制相關
        // Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;

        /// <summary>
        /// 火箭與場景移動，速度的快慢不會影響抵達距離，越快可以越早抵達目的。
        /// </summary>
        public void MoveAction()
        {
            float unit = Time.deltaTime * ss_ctl.speed; // 單位距離，使用 deltaTime 可以移除更新頻率的錯誤。
            if (!rocket_ctl.rc_dtion.IsStay) ss_ctl.transform.position += Vector3.up * unit / 2; // 場景移動
            if (rocket_ctl.RocketS1.x > 0) rocket_ctl.PutRocketSyn(rocket_ctl.Unit_fuel * Time.deltaTime);   // 燃料變化
            //else rocket_ctl.PutRocketSyn(0, rocket_ctl.GetBasicInfo().y / 2);               // 燃料用盡，移動懲罰

        }
        /// <summary>
        /// 燃料補充函數，輸入定值增加燃料。
        /// </summary>
        /// <param name="f">指定要補的值</param>
        public void FuelReplen(int f)
        {
            float nowFuel = rocket_ctl.RocketS1.x;
            rocket_ctl.PutRocketSyn(f, rocket_ctl.RocketBasic.y);
            rocket_ctl.ADOClipControl(0);
            if (condition.state == GameCondition.State.End && nowFuel > 0) CancelInvoke("GameState");
        }

        ///////////// 產生物件

        /// <summary>
        /// 物件生成系統
        /// 指定距離隨機生成物件
        /// </summary>
        /// <param name="i"></param>
        public void GenerAuto(int idx = 0)
        {
        }

        /// <summary>
        /// 切換預設的產生器類別，包含補給品產生。
        /// 這裡是設定要使用何種類別。
        /// 由於換個寫法，這裡暫時空置
        /// </summary>
        /// <param name="i">指定產生器內容，目前有的產生器如下：
        /// 0. 預設，產生 一般物件類別 。
        /// 1. 產生 隕石類別 。
        /// 2. 產生 敵機類別 。
        /// </param>
        public void AsignGenerate(int i)
        {

            try { gener_class = GameObject.Find(objectName[i]).GetComponent<Object_Generator>(); }
            catch { print("類別物件不存在，請重新設定"); }

        }
        /// <summary>
        /// 調用自動產生補給品
        /// </summary>
        /// <param name="i">補給品類別</param>
        /// <param name="rotate">是否隨機生成轉向</param>
        public void AutoGenerate(int i, bool rotate = false)
        {
            AsignGenerate(0); //切換成 補品類別
            Vector3 st_border = ss_ctl.GetBoxborder();
            if (gener_class != null) gener_class.Static_gen(GetStagePOS().y, i, Random.Range(-st_border.x / 2, st_border.x / 2), Random.Range(st_border.y, st_border.y * 2), rotate);
        }
        /// <summary>
        /// 產生附帶子物件的程式。
        /// 會先用一般生成的方式生成物件後，取得該物件的ID再依此生成子物件。
        /// 但如果使用預置物的話，物件會生成在子物件上。
        /// </summary>
        public void MeteoGenerate()
        {
            AsignGenerate(1); //切換成 隕石類別
            int obj = Random.Range(0, 3);
            int Gid = gener_class.Random_gen(GetStagePOS().y + 40, false, obj); // Fist: Generate SubObject.
            // Second: Load Insub Prefabs.
            List<Object> pfabs = new()
            {
#if UNITY_EDITOR
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/Empty.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal01.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal02.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_PurpCrystal01.prefab", typeof(Object))
#endif
            };

            // Third: Input on the subobject.
            gener_class.Random_Metro(Gid, pfabs);
        }
        /// <summary>
        /// 清除物件函數。
        /// 所有物件從畫面上移除都要經過這個函式。
        /// </summary>
        /// <param name="obj"></param>
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
            int i = ss_mag.GetScenes();
            switch (i)
            {
                case 0: break;
                default: break;
            }
        }

        public void test(int idx = 0)
        {
            // Change Asign Object to List String.
            print(condition.GetState());
        }
        ///////////// 選單變化相關
        ///
        public void GetState()
        {
            print(condition.GetState());
        }
        /// <summary>
        /// 選單淡入動畫
        /// </summary>
        public void FadeIn()
        {
            if (canvas_select.alpha < 1)
            {
                canvas_select.alpha += 0.1f;
            }
            else
            {
                condition.Next();
                GameState();
                CanvasCtrl(canvas_select, true);
                CancelInvoke("FadeIn");
                Time.timeScale = 0.05f;
            }
        }
        /// <summary>
        /// 選單淡出動畫
        /// </summary>
        public void FadeOut()
        {
            if (canvas_select.alpha > 0)
            {
                canvas_select.alpha -= 0.1f;
            }
            else
            {
                condition.Previous();
                GameState();
                Time.timeScale = 1f;
                CanvasCtrl(canvas_select);
                CancelInvoke("FadeOut");
            }

        }

        /// <summary>
        /// 遊戲結束判定系統
        /// 1. 沒有燃料
        /// 2. 撞到任何物體
        /// </summary>
        /// <param name="end"></param>
        public void CheckGame(bool end = false, float times = 0.2f)
        {
            if (end && ((int)condition.state) != 4)
            {
                // 檢查是否正在暫停，若是的話強制結束暫停
                if (menus.alpha != 0 && menus != null)
                {
                    CanvasCtrl(menus);
                }
                condition.Dead();
                Invoke("GameState", times);
            }

        }
        #endregion

        GameCondition condition = new GameCondition();
        #region 本地控制方法或事件
        /// <summary>
        /// 畫布群組開關
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="on"></param>
        private void CanvasCtrl(CanvasGroup cvs, bool on = false)
        {
            cvs.alpha = on ? 1 : 0;
            cvs.interactable = on;
            cvs.blocksRaycasts = on;
        }
        /// <summary>
        /// 設定UI的提示
        /// </summary>
        private void show_UI()
        {
            Vector3 stage_pos = GetStagePOS();
            UI_moveDistane = (int)stage_pos.y;
            UI_fuel = (int)rocket_ctl.RocketS1.x;
            print (UI_fuel / 100f);
            if (UI_fuel <= 100) ui_fuelbar.fillAmount = UI_fuel / 100f;
            else
            {   // 超過 100 的部分用格狀血條顯示
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity);                
                if ( EnergyPlus.Length >= level)
                {
                    int count = 0;
                    foreach (GameObject g in EnergyPlus)
                    {
                        g.SetActive( level >count);
                        count++;
                    }
                }
            }
            if (UI_fuel <= 0 && (int)condition.state != 3) { CheckGame(true, 5f); }//結束遊戲條件之一
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// 暫停選單開關
        /// </summary>
        public void show_Menu()
        {
            if (menus != null)
            {
                canvas_select = menus;
                if (condition.state != GameCondition.State.Pause)
                {
                    // 顯現
                    condition.Next();
                    InvokeRepeating("FadeIn", 0, fadeSpeed.x);
                }
                else
                {
                    // 淡出
                    condition.Previous();
                    InvokeRepeating("FadeOut", 0, fadeSpeed.y);
                }
            }
        }
        /// <summary>
        /// 遊戲狀態處理情況
        /// </summary>
        private void GameState()
        {
            bool isEnd = false;
            if (condition.GetState() == "End")
            {// GameOver
                isEnd = true;
                mEnd.enabled = true;
                ss_ctl.enabled = false;
                rocket_ctl.enabled = false;
            }
            else if (condition.state != GameCondition.State.Running)
            {// 如果不是執行狀態，則暫停空間，並呼叫暫停選單。
                space_ctl.enabled = !space_ctl.enabled;
            }
            if (pauseUI != null) pauseUI.SetActive(true);
            rocket_ctl.ControlChange(!isEnd);
            CancelInvoke("GameState");
        }
        private IEnumerator PathAutoGenerObject(int idx = 0)
        {

            isGen = true;
            switch (idx)
            {
                case 0: AutoGenerate(0, true); break;
                case 1:
                    MeteoGenerate(); ; break;
                default: break;
            }

            yield return new WaitForSeconds(1);
            if (UI_moveDistane % objectDistance[idx] == idx) ss_ctl.transform.Translate(Vector3.up);
            isGen = false;

        }
        /// <summary>
        /// 特殊指令，當玩家輸入快速鍵的時候會出現的封弊功能。
        /// </summary>
        private void SpecialistKeyInput(bool isCtrl, bool isAtl, bool isLS)
        {
            if (!isCtrl && !isAtl && !isLS) return;
            else
            {
                bool kCtrl = Input.GetKey(KeyCode.LeftControl);
                bool kAtl = Input.GetKey(KeyCode.LeftAlt);
                bool kLS = Input.GetKey(KeyCode.LeftShift);
                bool kB = Input.GetKeyDown(KeyCode.B);
                bool kC = Input.GetKeyDown(KeyCode.C);
                bool kN = Input.GetKeyDown(KeyCode.N);
                bool kM = Input.GetKeyDown(KeyCode.M);
                bool kO = Input.GetKeyDown(KeyCode.O);
                bool kP = Input.GetKeyDown(KeyCode.P);
                bool kQ = Input.GetKeyDown(KeyCode.Q);
                bool kR = Input.GetKeyDown(KeyCode.R);
                bool kU = Input.GetKeyDown(KeyCode.U);
                if (isCtrl)
                {
                    if (kAtl && kO)
                    {
                        //print("Debug");
                        //GameObject testOB = ((Transform)Resources.InstanceIDToObject(27318)).gameObject;
                        CanvasGroup testOB = GameObject.Find("TestObject").GetComponent<CanvasGroup>();
                        testOB.alpha = testOB.alpha != 0 ? 0 : 1;
                        testOB.interactable = !testOB.interactable;
                        testOB.blocksRaycasts = !testOB.blocksRaycasts;

                    }
                    if (kLS) print("C+S button");
                    else if (kB) print("B button");
                    else if (kM) print("M button");
                    else if (kO) print("O button");
                    else if (kP) print("P button");
                    else if (kQ) print("Q button");
                }
                else if (isAtl)
                {
                    if (kLS) print("A+S button");
                    else if (kN) print("N button");
                    else if (kR) print("R button");
                }
                else if (isLS)
                {
                    if (kC) print("C button");
                    else if (kU) print("U button");
                }
            }

        }

        private void Update()
        {
            show_UI();
            SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftAlt),
                Input.GetKey(KeyCode.LeftShift)
                );
            //// ---路徑自動生成物件---
            ///
                for (int i = 0; i < objectName.Length; i++)
                {
                    if (UI_moveDistane % objectDistance[i] == i && !isGen) StartCoroutine(PathAutoGenerObject(i));
                }
        }
        #endregion

        #region 彩蛋相關
        /*Ctrl+b 開啟黑洞隨機跳關：需要黑洞生成的動畫或圖片
        Shift+c 火箭換成貨機：需要一架貨機
        Shift+u 火箭換飛碟
        Alt+n 火箭隱形
        Alt+r 重新生成補給與障礙物*/

        #endregion
    }

    /// <summary>
    /// 遊戲狀態機
    /// 目前先設定為：執行中、讀取、暫停及結束遊戲。
    /// ※與之前的寫法相比，不需要個別設定布林值，狀態變化只要下要變的函數就可以，而且順序可以固定或指定。
    /// </summary>
    public class GameCondition
    {
        public enum State { Running, Loading, Pause, End }
        public State state = State.Running;

        public void Next()
        {
            if (state < State.End - 1) state++;
        }
        public void Previous()
        {
            if (state != 0) state--;
        }
        public void Dead()
        {
            state = State.End;
        }
        public string GetState()
        {
            return state.ToString();
        }
    }
}
