using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Unity.VisualScripting;
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
        [Header("中控系統")]
        static public ManageCenter mgCenter;
        [SerializeField, Header("各程式管理系統"),Tooltip("場景系統")]
        private ManageScene MgScene;
        static public ManageScene mgScene;
        [SerializeField, Tooltip("結束管理")]
        private ManageEnd MgEnd;
        static public ManageEnd mgEnd;
        [SerializeField, Tooltip("火箭控制系統")]
        private Rocket_Controll Rocket_CTL;
        static public Rocket_Controll rocket_ctl;
        [SerializeField,Tooltip("場景控制系統")]
        private SceneStage_Control SS_CTL;
        static public SceneStage_Control ss_ctl;
        [Tooltip("空間空至系統")]
        static public Space_Controll space_ctl;
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
        [SerializeField, Tooltip("用盡燃料後的掙扎時間"),Range(0,10)]
        private float fuelExhaustedTime=5;
        /// <summary>
        /// 選單畫布控制
        /// </summary>
        [SerializeField, Header("暫停選單")]
        GameObject pauseUI;
        [SerializeField, Tooltip("暫停選單畫布系統")]
        CanvasGroup pauseMenus;
        [SerializeField, Header("畫布淡化速度")]
        Vector2 fadeSpeed = Vector2.zero + Vector2.one * 0.01f;
        [SerializeField, Header("混音控制系統")]
        AudioMixer adM;
        #region 本地調閱欄位 (Private Feild)
        [Tooltip("程式控制選擇目前的畫布，可以不用設定。")]
        private CanvasGroup canvas_select;
        private AudioSource AudioBox;
        private BoxCollider FinishBox;
        [HideInInspector]
        public bool noExhauFuel, noExhauRush, noDead, toFinDest;
        #endregion


        public void X_PowerMode()
        {
            
            noDead = !noDead;
            transform.Find("AudioBox").GetComponent<AudioSource>().Play();
        }
        /// <summary>
        /// 共用方法 (Public Method)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetStagePOS() => ss_ctl.transform.position;
        public Vector3 GetStageBorder() => ss_ctl.GetBoxborder();
        public void SetRockBasicInfo(float x, float y=0, float z=0) {
            x = x != 0 ? x: rocket_ctl.RocketBasic.x;
            y = y != 0 ? y: rocket_ctl.RocketBasic.y;
            z = z != 0 ? z: rocket_ctl.RocketBasic.z;
            rocket_ctl.SetBasicInfo(x,y,z);
                }
        //除錯功能
        public void GetState() => print(condition.GetState());

        #region 火箭控制與計數相關
        private IEnumerator DeathCounter(float counter=0)
        {            
            for (int i=0; i< counter;i++) yield return new WaitForSeconds(1);
            if(rocket_ctl.RocketS1.x <1) condition.Dead();
            yield return null;
        }
        /// <summary>
        /// 火箭與場景移動。
        /// 舊版：調整場景的 Y軸數值。
        /// 新版：直接修改UI數值。
        /// </summary>
        public void MoveAction()
        {
            //if (!rocket_ctl.rc_dtion.IsStay) 
            //    ss_ctl.transform.position += Vector3.up * unit / 2; // 場景移動1
            float unit = Time.deltaTime * ss_ctl.speed; // 單位距離，使用 deltaTime 可以移除更新頻率的錯誤。            
            if (toFinDest) UI_moveDistane = ss_ctl.finishDistane;
            else UI_moveDistane += unit;
            if (UI_moveDistane >= ss_ctl.finishDistane)
            {
                UI_moveDistane = ss_ctl.finishDistane;
                FinishBox.enabled = true;
            }

            float fueldown = rocket_ctl.Unit_fuel * Time.deltaTime * ss_ctl.speed;
            //print(rocket_ctl.rc_dtion.IsBoost);
            //if (rocket_ctl.rc_dtion.IsBoost)  // fueldown -= (rocket_ctl.RocketS1.z * rocket_ctl.Unit_fuel) * Time.deltaTime;
            if (rocket_ctl.RocketS1.x > 0 && !noExhauFuel) rocket_ctl.PutRocketSyn(fueldown);   // 燃料變化
            else if (!noExhauFuel) StartCoroutine(DeathCounter(fuelExhaustedTime));   // 燃料用盡，死亡倒數。

        }
        /// <summary>
        /// 燃料補充函數，輸入定值增加燃料。
        /// </summary>
        /// <param name="f">指定要補的值</param>
        public void FuelReplen(int f)
        {
            float nowFuel = rocket_ctl.RocketS1.x;
            rocket_ctl.PutRocketSyn(f);
            rocket_ctl.ADOClipControl(0);
            if (StaticSharp.Conditions == State.End && nowFuel > 0) CancelInvoke("GameState");
        }
        #endregion
        #region 物件通用函數
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
        /// <param name="obj">碰撞區域回傳的物件</param>
        GenerateSystem objGS;
        public void ObjectDestory(GameObject obj, bool hasDesTime=false)
        {
            objGS = obj.transform.GetComponentInParent<GenerateSystem>();
            //print($"名稱: {objGS.name} hasdes:{hasDesTime}");  // 測試是否有讀取到物件，讀不到則直接銷毀避免錯誤。
            if (!objGS) { Destroy(obj); return; }
            objGS.Destroys(obj, hasDesTime);
            //gener_class.Destroys(obj);
        }
        #endregion
        #region 場 景 相 關
        /// <summary>
        /// 進入中繼站
        /// 儲存火箭的資料
        /// </summary>
        public void InToStation()
        {
            StaticSharp.Rocket_INFO = rocket_ctl.RocketS1;
            StaticSharp.Rocket_BASIC = rocket_ctl.RocketBasic;
            mgScene.SaveLeveInform();
            mgScene.LoadScenes("Station");            
        }

        ///////////// 選單變化相關
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
                    condition.Next();
                    canvas_select.CanvansFadeControl(visable);
                    break;
                default:
                    while (canvas_select.alpha > 0)
                    {
                        canvas_select.alpha -= 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.y);
                    }
                    condition.Previous();
                    canvas_select.CanvansFadeControl(visable);
                    break;
            }
            GameState();
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
            if (UI_fuel <= 100)
            {
                ui_fuelbar.fillAmount = UI_fuel / 100f;
                if(EnergyPlus[0].activeSelf) EnergyPlus[0].SetActive(false);
            }
            else
            {   // 超過 100 的部分用格狀血條顯示
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity) + 1;
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
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane.ToString("0")}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// 暫停選單開關
        /// </summary>
        public void show_Menu()
        {
            //print($"{StaticSharp.Conditions} & {condition.isPause}");
            if (pauseMenus != null)
            {
                canvas_select = pauseMenus;
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
        #endregion
        /// <summary>
        /// 遊戲狀態處理情況(需要被Invoke)
        /// 根據目前的狀態切換遊戲進行狀況
        /// END = 結束遊戲
        /// Running 切換 = 開起暫停選單
        /// </summary>
        private void GameState()
        {
            if (pauseUI != null && !pauseUI.activeSelf) pauseUI.SetActive(true);
            if (condition.GetState() == "End")
            {   // GameOver
                bool isEnd = true;
                pauseUI.transform.Find("Btn_Back_en").gameObject.SetActive(!isEnd);
                mgEnd.enabled = isEnd;
                ss_ctl.enabled = !isEnd;
                // 關閉音效
                StartCoroutine(rocket_ctl.ControlChange(!isEnd));
                Simple_move[] simple_s = FindObjectsOfType<Simple_move>();
                foreach (Simple_move simple in simple_s) StartCoroutine(simple.Mute());
                condition.Finish();
            }
            else if (StaticSharp.Conditions != State.Running)
            {// 如果不是執行狀態，則暫停空間，並呼叫暫停選單。
                StartCoroutine(rocket_ctl.ControlChange(false));
            }
            else StartCoroutine(rocket_ctl.ControlChange(true));
            CancelInvoke("GameState");
        }
        StaticSharp.GameCondition condition = new StaticSharp.GameCondition();
        #region 本地控制方法或事件
        /// <summary>
        /// 進入遊戲結束系統，若有以下情況則呼叫此程式：
        /// 1. 沒有燃料
        /// 2. 撞到任何物體
        /// </summary>
        private void StateEnd() => StartCoroutine(PauseFadeEffect(true));

        #endregion

        private void Awake()
        {
            mgCenter = GetComponent<ManageCenter>();
            FinishBox = GameObject.Find("NextStage").GetComponent<BoxCollider>();
        }
        private void Start()
        {
            if (pauseMenus == null) pauseMenus = pauseUI.GetComponent<CanvasGroup>();
            if (MgEnd) mgEnd = MgEnd;
            else mgEnd = FindObjectOfType<ManageEnd>();
            mgScene = MgScene ?? FindObjectOfType<ManageScene>();
            ss_ctl = SS_CTL ?? FindObjectOfType<SceneStage_Control>();
            rocket_ctl = Rocket_CTL ?? FindObjectOfType<Rocket_Controll>();
            
            //print($"目前場景編號為：{PlayerPrefs.GetInt(ss_mag.sceneID)}");
            UI_moveDistane = 0;
        }
        private void Update()
        {

            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    if (Time.timeScale != 1) Time.timeScale = 1;
                    show_UI();
                    if (UI_moveDistane <= ss_ctl.finishDistane) MoveAction();
                    break;
                case State.Pause:
                    if (Time.timeScale > 0.1f) Time.timeScale = 0.05f;
                    break;
                case State.End:
                    if (noDead)
                    {
                        condition.Run();
                        break;
                    }
                    StateEnd();
                    break;
                case State.Finish:
                    if (Time.timeScale > 0.1f) Time.timeScale = 0f;
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
            if (Input.GetKeyDown(KeyCode.Escape)) show_Menu();
        }

        #region 彩蛋相關
        /*Ctrl+b 開啟黑洞隨機跳關：需要黑洞生成的動畫或圖片
        Shift+c 火箭換成貨機：需要一架貨機
        Shift+u 火箭換飛碟
        Alt+n 火箭隱形
        Alt+r 重新生成補給與障礙物*/

        #endregion
    }


}
