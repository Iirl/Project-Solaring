using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace solar_a
{
    /// <summary>
    /// 選單控制系統，調用其他系統的功能，控制與共同方法。
    /// 本系統應放在必定存在於場上的物件上
    /// 目前放在： GameManage_UI
    /// 會將可重複調用的方法放於此處，在其他物件上原有的方法如有需要重複使用也會放在這。
    /// </summary>
    [RequireComponent(typeof(ManageScene), typeof(ManageEnd))]
    public class ManageCenter : MonoBehaviour
    {
        #region 管理控制系統
        [Header("中控系統")]
        static public ManageCenter mgCenter;
        static public ManageScene mgScene;
        static public ManageDisco mgDsko;
        static public ManageEnd mgEnd;
        static public Rocket_Controll rocket_ctl;
        static public SSRocket rocket_SSR;
        static public SceneStage_Control ss_ctl;
        static public Space_Controll space_ctl;
        [SerializeField, Header("各程式管理系統")]
        private ManageScriptClass ManageSystemController;
        [System.Serializable]
        public class ManageScriptClass
        {
            [SerializeField, Tooltip("場景系統")]
            public ManageScene MgScene;
            [SerializeField, Tooltip("聲音管理")]
            public ManageDisco MgDisco;
            [SerializeField, Tooltip("結束管理")]
            public ManageEnd MgEnd;
            [SerializeField, Tooltip("火箭控制系統")]
            public Rocket_Controll Rocket_CTL;
            [SerializeField, Tooltip("場景控制系統")]
            public SceneStage_Control SS_CTL;
            [SerializeField, Tooltip("空間控制系統")]
            public Space_Controll Space_CTL;
        }

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
        [SerializeField, Header("玩家資訊")]
        private GameObject[] RocketModel;
        [SerializeField, Tooltip("機體外觀設定")]
        private RocketPreviews RocketOutfit;
        [SerializeField, Tooltip("用盡燃料後的掙扎時間"), Range(0, 10)]
        private float fuelExhaustedTime = 5;
        /// <summary>
        /// 選單畫布控制
        /// </summary>
        [SerializeField, Header("暫停選單")]
        GameObject pauseUI;
        [SerializeField, Tooltip("暫停選單畫布系統")]
        CanvasGroup pauseMenus;
        [SerializeField, Header("畫布淡化速度")]
        Vector2 fadeSpeed = Vector2.zero + Vector2.one * 0.01f;
        /// <summary>
        /// 關卡層級控制
        /// </summary>
        [SerializeField, Header("關卡設定")]
        public StageInformation[] stInfo;
        [SerializeField]
        private int levelBuildSetting = 2;
        private int levelNow;
        [System.Serializable]
        public class StageInformation
        {

            [SerializeField, Header("名稱")]
            public string label;
            [SerializeField, Header("移動速度")]
            public float speed;
            [SerializeField, Header("結束距離")]
            public float finishDistane;
        }
        #region 本地調閱欄位 (Private Feild)
        [Tooltip("程式控制選擇目前的畫布，可以不用設定。")]
        private CanvasGroup canvas_select;
        [HideInInspector] //作弊控制開關
        public bool noExhauFuel, noExhauRush, protect, toFinDest;
        bool runtime = false;
        #endregion

        public void X_PowerMode() { protect = !protect; rocket_ctl.StateToShield(protect); StaticSharp.isProtected = protect; ; }
        /// <summary>
        /// 共用方法 (Public Method)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetStagePOS() => StageInfo(1);
        public Vector3 GetStageBorder() => StageInfo(2);
        public void SetRockBasicInfo(float x, float y = 0, float z = 0)
        {
            x = x != 0 ? x : rocket_ctl.RocketBasic.x;
            y = y != 0 ? y : rocket_ctl.RocketBasic.y;
            z = z != 0 ? z : rocket_ctl.RocketBasic.z;
            rocket_ctl.SetBasicInfo(x, y, z);
        }
        //除錯功能
        public void GetState() => print(condition.GetState());

        #region 火箭控制與計數相關
        public Vector3 GetRocketPosition() => rocket_ctl.transform.position;
        public void FuelReplen(int f) => FuelReplens(f, rocket_ctl.RocketVarInfo.x);
        public void RocketStop(bool stop) => rocket_ctl.rc_dtion.onStop(stop);

        private void PutPlayerOBJ()
        {
            //In Awake 事件，判斷是否需要生成玩家物件
            //或者可以修改 RocketOutfit 後再一次呼叫
            if (!GameObject.FindGameObjectWithTag("Player"))
            {
                //print("需要生成物件");
                try { Instantiate(RocketModel[(int)RocketOutfit], Vector3.down * 11, Quaternion.identity); }
                catch (System.IndexOutOfRangeException) { print("找不到物件，請確定MangeUI上是否有設定玩家物件"); }
                catch (System.Exception) { print("RocketModel 的資料不存在，請放入火箭模型"); }
            }
            rocket_ctl = ManageSystemController.Rocket_CTL ? ManageSystemController.Rocket_CTL : FindObjectOfType<Rocket_Controll>();
            rocket_SSR = rocket_ctl != null ? rocket_ctl.GetComponent<SSRocket>() : null;
            //else print("不用生成物件");
        }

        /// <summary>
        /// 火箭與場景移動。
        /// 舊版：調整場景的 Y軸數值。
        /// 新版：直接修改UI數值。
        /// </summary>
        private void MoveAction()
        {
            //if (!rocket_ctl.rc_dtion.IsStay) 
            //    ss_ctl.transform.position += Vector3.up * unit / 2; // 場景移動1
            float unit = Time.deltaTime * stInfo[levelNow].speed; // 單位距離，使用 deltaTime 可以移除更新頻率的錯誤。
            if (toFinDest) { UI_moveDistane = stInfo[levelNow].finishDistane; toFinDest = false; }
            else UI_moveDistane += unit;
            if (UI_moveDistane >= stInfo[levelNow].finishDistane)
            {
                UI_moveDistane = stInfo[levelNow].finishDistane;
                if (FinishArea.finishState) FindObjectOfType<FinishArea>().enabled = true;
            }

            float fueldown = rocket_ctl.Unit_fuel * Time.deltaTime * stInfo[levelNow].speed;
            //print(rocket_ctl.rc_dtion.IsBoost);
            if (rocket_ctl.rc_dtion.IsBoost) fueldown += (rocket_ctl.RocketVarInfo.z * rocket_ctl.Unit_fuel) * Time.deltaTime;
            if (rocket_ctl.RocketVarInfo.x > 0 && !noExhauFuel) rocket_ctl.PutRocketSyn(fueldown);   // 燃料變化
            else if (!noExhauFuel) StartCoroutine(DeathCounter(fuelExhaustedTime));   // 燃料用盡，死亡倒數。
            //print(fueldown);

        }
        /// <summary>
        /// 燃料補充函數，輸入定值增加燃料。
        /// </summary>
        /// <param name="f">指定要補的值</param>
        /// <param name="nowFuel">目前的燃料值</param>
        private void FuelReplens(int f, float nowFuel)
        {
            rocket_ctl.PutRocketSyn(f);
            if (StaticSharp.Conditions == State.End && nowFuel > 0) CancelInvoke("GameState");
        }
        /// <summary>
        /// 燃料耗盡的倒數
        /// </summary>
        /// <param name="counter">設定時間</param>
        private IEnumerator DeathCounter(float counter = 0)
        {
            for (int i = 0; i < counter; i++) yield return new WaitForSeconds(1);
            if (rocket_ctl.RocketVarInfo.x < 1) condition.Dead();
            yield return null;
        }
        #endregion
        #region 物件通用函數
        /// <summary>
        /// 清除物件函數。
        /// 所有物件從畫面上移除都要經過這個函式。
        /// </summary>
        /// <param name="obj">碰撞區域回傳的物件</param>
        GenerateSystem objGS;
        public void ObjectDestory(GameObject obj, bool hasDesTime = false)
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
        /// SS_ctl 參數取得
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private Vector3 StageInfo(int idx)
        {
            if (!ss_ctl) return Vector3.zero;
            if (idx == 1) return ss_ctl.transform.position;
            else if (idx == 2) return ss_ctl.GetBoxborder();
            return Vector3.zero;
        }
        /// <summary>
        /// 進入中繼站
        /// 儲存火箭的資料
        /// </summary>
        public void InToStation()
        {
            mgScene.ReloadToAndClear();
            mgScene.SaveLeveInform(levelNow);
            mgScene.LoadScenes("Station");
        }
        /// <summary>
        /// 切換到下一關的檢查
        /// </summary>
        private void StartChageScene()
        {
            mgScene.SaveLeveInform(levelNow);
            mgScene.SceneChageEvent(true);
        }
        ///////////// 選單變化相關
        private IEnumerator PauseFadeEffect(bool visable = true)
        {
            if (condition.isEnd)
            {
                canvas_select = pauseMenus;
                yield return new WaitForSeconds(1.6f);
            }
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
            UI_fuel = (int)rocket_ctl.RocketVarInfo.x;
            if (UI_fuel <= 100)
            {
                ui_fuelbar.fillAmount = UI_fuel / 100f;
                if (EnergyPlus[0].activeSelf) EnergyPlus[0].SetActive(false);
            }
            else
            {   // 超過 100 的部分用格狀血條顯示
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity) + 1;
                EnergyAnimator(level - 1); //能量動畫控制
                if (EnergyPlus.Length > 0)
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
        private void EnergyAnimator(int i)
        {
            string State = "onState";
            i = (i < EnergyPlus.Length) ? i : EnergyPlus.Length - 1;
            if (EnergyPlus[i].activeSelf) if (EnergyPlus[i].GetComponent<Animator>().GetBool(State)) return;
            for (int j = 0; j <= i; j++) if (EnergyPlus[j].activeSelf) EnergyPlus[j].GetComponent<Animator>().SetBool(State, i == j);
            //print($"第 {i} 號燃料燃燒中");
        }

        /// <summary>
        /// 暫停選單開關
        /// </summary>
        public void Show_Menu(CanvasGroup cvs)
        {
            //print($"{StaticSharp.Conditions} & {condition.isPause}");
            if (pauseMenus != null)
            {
                canvas_select = cvs;
                if (condition.isEnd)
                {
                    pauseUI.SetActive(true);
                    StartCoroutine(PauseFadeEffect(true));
                }
                else if (!condition.isPause)
                {
                    // 顯現
                    condition.Next();
                    if (!pauseUI.activeSelf && canvas_select == pauseMenus) pauseUI.SetActive(true);
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
        public void ShowCanvas(CanvasGroup cvs)
        {
            if (pauseUI.activeSelf) cvs.CanvansFadeControl(!cvs.blocksRaycasts);
            else Show_Menu(cvs);
        }
        #endregion
        public int GetLevel() => levelNow;
        /// <summary>
        /// 遊戲狀態處理情況(需要被Invoke)
        /// 根據目前的狀態切換遊戲進行狀況
        /// END = 結束遊戲
        /// Running 切換 = 開起暫停選單
        /// </summary>
        private void GameState()
        {
            //if (pauseUI != null && !pauseUI.activeSelf) pauseUI.SetActive(true);
            if (condition.GetState() == "End")
            {   // GameOver
                bool isEnd = true;
                pauseUI.transform.Find("Btn_Back_en").gameObject.SetActive(!isEnd);
                mgEnd.enabled = isEnd;
                ss_ctl.enabled = !isEnd;
                // 關閉音效
                Simple_move[] simple_s = FindObjectsOfType<Simple_move>();
                foreach (Simple_move simple in simple_s) StartCoroutine(simple.Mute());
                condition.Finish();
            }
            CancelInvoke("GameState");
        }
        #region 本地控制方法或事件
        /// <summary>
        /// 進入遊戲結束系統，若有以下情況則呼叫此程式：
        /// 1. 沒有燃料
        /// 2. 撞到任何物體
        /// </summary>
        private void StateEnd() => Show_Menu(pauseMenus);

        #endregion

        #region 啟動事件
        StaticSharp.GameCondition condition = new StaticSharp.GameCondition();
        private void Awake()
        {
            mgCenter = GetComponent<ManageCenter>();
            if (pauseMenus == null) pauseMenus = pauseUI.GetComponent<CanvasGroup>();
            if (ManageSystemController.MgEnd) mgEnd = ManageSystemController.MgEnd;
            else mgEnd = FindObjectOfType<ManageEnd>();
            mgScene = ManageSystemController.MgScene ? ManageSystemController.MgScene : FindObjectOfType<ManageScene>();
            mgDsko = ManageSystemController.MgDisco ? ManageSystemController.MgDisco : FindObjectOfType<ManageDisco>();
            ss_ctl = ManageSystemController.SS_CTL ? ManageSystemController.SS_CTL : FindObjectOfType<SceneStage_Control>();
            space_ctl = ManageSystemController.Space_CTL ? ManageSystemController.Space_CTL : FindObjectOfType<Space_Controll>();
            PutPlayerOBJ();  // 放置玩家火箭

        }

        private void Start()
        {
            //print($"目前場景編號為：{PlayerPrefs.GetInt(ss_mag.sceneID)}");
            StaticSharp.isChangeScene = false;
            if( StaticSharp.isProtected ) X_PowerMode();
            // 取得距離數值，如果沒有則從零開始
            UI_moveDistane = StaticSharp.DistanceRecord > 0 ? StaticSharp.DistanceRecord : 0;
            UI_fuel = (int)rocket_ctl.RocketVarInfo.x;
            levelNow = mgScene.GetScenes() - levelBuildSetting + 1;
            if (mgScene.GetScenesName().Contains("Tutorail")) levelNow = 0;
            else if (levelNow > 1) UI_moveDistane = Mathf.Clamp(UI_moveDistane, stInfo[levelNow - 1].finishDistane, stInfo[levelNow].finishDistane);
            //print($"目前關卡:{mgScene.GetScenes()}/ BuildSetting: {levelBuildSetting}");            
        }
        #endregion
        private void Update()
        {
            // 狀態機執行功能
            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    if (StaticSharp.isChangeScene)
                    {
                        StaticSharp.isChangeScene = false;
                        StartChageScene();
                    }
                    if (Time.timeScale != 1) Time.timeScale = 1;
                    show_UI();
                    if (mgScene.GetScenesName().Contains("Tutorail")) { if (UI_moveDistane <= 2000) MoveAction(); }
                    else if (UI_moveDistane <= stInfo[levelNow].finishDistane) { MoveAction(); }
                    break;
                case State.Loading:
                    Time.timeScale = 0.5f;
                    break;
                case State.Pause:
                    if (Time.timeScale > 0.1f) Time.timeScale = 0f;
                    break;
                case State.End:
                    if (!runtime)
                    {
                        runtime = true;
                        StaticSharp._LEVEL = levelNow;
                        mgEnd.messageLog.text = "";
                        rocket_ctl.StateToBorken(true);
                        StateEnd();
                    }
                    rocket_ctl.rc_dtion.state = RocketState.Crashed;
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
            if (Input.GetKeyDown(KeyCode.Escape)) Show_Menu(pauseMenus);
        }

        #region 彩蛋相關
        /*Ctrl+b 開啟黑洞隨機跳關：需要黑洞生成的動畫或圖片
        Shift+c 火箭換成貨機：需要一架貨機
        Shift+u 火箭換飛碟ˇ
        Alt+n 火箭隱形ˇ
        Alt+r 重新生成補給與障礙物=>原意為清空物件，現改成生成物件ˇ*/

        #endregion
    }


}
