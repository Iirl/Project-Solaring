using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
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
        static public int UI_fuel = 100;
        static public float UI_moveDistane = 0;
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
        [SerializeField, Header("混音控制系統")]
        AudioMixer adM;
        #region 本地調閱欄位 (Private Feild)
        

        #endregion


        public void X_PowerMode()
        {
            StaticSharp.isPowerfullMode = !StaticSharp.isPowerfullMode;
            transform.Find("AudioBox").GetComponent<AudioSource>().Play();
        }
        /// <summary>
        /// 共用方法 (Public Method)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetStagePOS()
        {
            return ss_ctl.transform.position;
        }
        public Vector3 GetStageBorder()
        {
            return ss_ctl.GetBoxborder();
        }
        #region 火箭控制相關
        // Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;

        /// <summary>
        /// 火箭與場景移動，速度的快慢不會影響抵達距離，越快可以越早抵達目的。
        /// </summary>
        public void MoveAction()
        {
            float unit = Time.deltaTime * ss_ctl.speed; // 單位距離，使用 deltaTime 可以移除更新頻率的錯誤。
            //if (!rocket_ctl.rc_dtion.IsStay) 
            //    ss_ctl.transform.position += Vector3.up * unit / 2; // 場景移動1
            UI_moveDistane += unit;
            if (rocket_ctl.RocketS1.x > 0) rocket_ctl.PutRocketSyn(rocket_ctl.Unit_fuel * Time.deltaTime * ss_ctl.speed);   // 燃料變化
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
            if (StaticSharp.Conditions == State.End && nowFuel > 0) CancelInvoke("GameState");
        }
        #endregion
        #region 產生物件
        /*
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
            AsignGenerate(1);
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
        */
        /// <summary>
        /// 清除物件函數。
        /// 所有物件從畫面上移除都要經過這個函式。
        /// </summary>
        /// <param name="obj"></param>
        public void ObjectDestory(GameObject obj)
        {
            GenerateSystem objGS = obj.transform.GetComponentInParent<GenerateSystem>();
            if (!objGS) { Destroy(obj); return; }
            //print(objGS.name);
            objGS.Destroys(obj);
            //gener_class.Destroys(obj);
        }
        #endregion
        #region 場 景 相 關
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
        // 進入中繼站
        public void InToStation()
        {
            StaticSharp.Rocket_INFO = rocket_ctl.RocketS1;
            StaticSharp.Rocket_BASIC = rocket_ctl.RocketBasic;
            ss_mag.SaveLeveInform();
            ss_mag.LoadScenes("Station");
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
        private IEnumerator PauseFadeEffect(bool visable = true)
        {
            canvas_select = pauseUI.GetComponent<CanvasGroup>();
            switch (visable)
            {
                case true:
                    while (canvas_select.alpha < 1)
                    {
                        canvas_select.alpha += 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.x);
                    }
                    Time.timeScale = 0.05f;
                    condition.Next();
                    CanvasCtrl(canvas_select, visable);
                    break;
                default:
                    while (canvas_select.alpha > 0)
                    {
                        canvas_select.alpha -= 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.y);

                    }
                    Time.timeScale = 1f;
                    condition.Previous();
                    CanvasCtrl(canvas_select, visable);
                    break;
            }

            GameState();



        }
        #endregion

        StaticSharp.GameCondition condition = new StaticSharp.GameCondition();
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
            // 場景UI - 移動的寫法：先取得場景位置，然後再將位置送到UI裡。
            /*Vector3 stage_pos = GetStagePOS();
            UI_moveDistane = (int)stage_pos.y;  //*/
            // 改放在 MoveAction 中
            // 燃料UI
            UI_fuel = (int)rocket_ctl.RocketS1.x;
            if (UI_fuel <= 100) ui_fuelbar.fillAmount = UI_fuel / 100f;
            else
            {   // 超過 100 的部分用格狀血條顯示
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity);
                if (EnergyPlus.Length >= level)
                {
                    int count = 0;
                    foreach (GameObject g in EnergyPlus)
                    {
                        g.SetActive(level > count);
                        count++;
                    }
                }
            }
            // 標示文字UI內容
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// 暫停選單開關
        /// </summary>
        public void show_Menu()
        {
            //print($"{StaticSharp.Conditions} & {condition.isPause}");
            if (menus != null)
            {
                canvas_select = menus;
                if (condition.isEnd)
                {
                    StartCoroutine(PauseFadeEffect(true));
                }
                else if (!condition.isPause)
                {
                    // 顯現
                    condition.Next();
                    StartCoroutine(PauseFadeEffect(true));

                }
                else
                {
                    // 淡出
                    condition.Previous();
                    StartCoroutine(PauseFadeEffect(false));

                }
            }
        }
        /// <summary>
        /// 遊戲狀態處理情況(需要被Invoke)
        /// 根據目前的狀態切換遊戲進行狀況
        /// END = 結束遊戲
        /// Running 切換 = 開起暫停選單
        /// </summary>
        private void GameState()
        {
            if (condition.GetState() == "End")
            {// GameOver
                bool isEnd = true;
                transform.Find("UI_Pause").transform.Find("Btn_Back_en").gameObject.SetActive(!isEnd);
                mEnd.enabled = isEnd;
                ss_ctl.enabled = !isEnd;
                condition.Finish();
                rocket_ctl.ControlChange(!isEnd);
            }
            else if (StaticSharp.Conditions != State.Running)
            {// 如果不是執行狀態，則暫停空間，並呼叫暫停選單。
                space_ctl.enabled = !space_ctl.enabled;
                rocket_ctl.ControlChange(!condition.isPause);
            } else
            {
                space_ctl.enabled = !space_ctl.enabled;
                rocket_ctl.ControlChange(!condition.isPause);
            }
            if (pauseUI != null) pauseUI.SetActive(true);
            CancelInvoke("GameState");
        }

        /// <summary>
        /// 進入遊戲結束系統
        /// 若有以下情況則呼叫此程式：
        /// 1. 沒有燃料
        /// 2. 撞到任何物體
        /// </summary>
        private void StateEnd()
        {
            //CheckGame(true, 5f); //結束遊戲條件之一
            show_Menu();
        }

        private void Awake()
        {
            UI_moveDistane = 0;
        }
        private void Start()
        {
            //print($"目前場景編號為：{PlayerPrefs.GetInt(ss_mag.sceneID)}");
        }
        private void Update()
        {

            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    show_UI();
                    MoveAction();
                    break;
                case State.Loading:
                    break;
                case State.Pause:
                    break;
                case State.End:
                    StateEnd();
                    break;
                default:
                    break;
            }
            //print(StaticSharp.Conditions);  //狀態機檢查
            // 按鍵輸入偵測
            StaticSharp.SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftAlt),
                Input.GetKey(KeyCode.LeftShift)
                );

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


}
