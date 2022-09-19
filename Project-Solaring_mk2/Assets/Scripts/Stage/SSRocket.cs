using System.Collections;
using solar_a;
using UnityEngine;

/// <summary>
/// Secret Specil Rocket.
/// 除錯用程式，把需要的功能放在這裡，透過控制中控中心的布林值達到操控的效果。
/// </summary>
public class SSRocket : MonoBehaviour
{
    ManageCenter mgc;
    Rocket_Controll rct;
    public delegate void StatusMethod(bool trunOn);
    #region 面板控制效果
    [SerializeField, Tooltip("顯示在遊戲中")]
    private bool showWindows, showPassFiled;

    public bool isShowed { get { return showWindows; } }
    [SerializeField, Header("異常狀態"), Tooltip("不會死亡")]
    private bool protectx;
    public bool NoDead { set { protectx = value; SendControl(); } get { return protectx; } }
    [SerializeField, Tooltip("不耗燃料")]
    private bool noFuel;
    public bool NoFuel { set { noFuel = value; SendControl(); } get { return noFuel; } }
    [SerializeField, Tooltip("不耗衝刺")]
    private bool noRush;
    public bool NoRush { set { noRush = value; SendControl(); } get { return noRush; } }
    [SerializeField, Tooltip("移到終點")]
    private bool toFinal;
    [SerializeField, Header("負面效果"),Tooltip("降低速度效果"), Range(0, 1)]
    private float slowDown = 0.5f;
    [SerializeField, Header("燃料物件")]
    private GameObject fuelObj;
    [SerializeField, Header("飛船物件")]
    private GameObject[] rockets;
    #endregion
    public void ShowDebug(bool isOpen) => showPassFiled = isOpen;
    public void ChangeToBorkenRocket() => PlayerChange(0);
    public void ChangeToUFO() => PlayerChange(0);
    /// <summary>
    /// 狀態切換控制器
    /// </summary>
    /// <param name="id">異常狀態編號</param>
    /// <param name="sec">持續時間</param>
    /// <param name="sm">額外傳入方法</param>
    public void StateControaller(int id, int sec, StatusMethod sm = null)
    {
        //print($"執行編號:{id}, 效果為{((RocketACondition)id)}");
        switch ((RocketACondition)id)
        {
            case RocketACondition.Protect:
                if (!protectx)
                {
                    sm = toProtect;
                    StartCoroutine(StatusTimer(rct.StateToShield, sec));
                }
                else return;
                break;
            case RocketACondition.FullGage:
                sm = toFullGage;
                break;
            case RocketACondition.FullRush:
                sm = toFullRushGage;
                break;
            default:
                break;
        }
        if (sm != null) StartCoroutine(StatusTimer(sm, sec));
    }
    public void DebufferController(int id, float sec, StatusMethod sm = null)
    {
        switch ((RocketDCondition)id)
        {
            case RocketDCondition.Slowdown:
                if (rct.Speed_slow != 0) break;
                sm = slowDownSpd;
                break;
            default:
                break;
        }
        if(sm != null) StartCoroutine(StatusTimer(sm, sec));

    }

    #region 狀態處理
    //
    private void toProtect(bool funOn = false) => ManageCenter.mgCenter.protect = funOn;
    private void toNoDead(bool funOn = false) => NoDead = funOn;
    private void toFullGage(bool funOn = false) => NoFuel = funOn;
    private void toFullRushGage(bool funOn = false) => NoRush = funOn;
    //
    private void slowDownSpd(bool dbf= false) => rct.Speed_slow = dbf ? slowDown : 0;
    /// <summary>
    /// 將控制內容傳給主控中心
    /// </summary>
    private void SendControl()
    {
        if (!mgc) mgc = FindObjectOfType<ManageCenter>();
        if (mgc)
        {
            mgc.protect = protectx ? true : false;
            mgc.noExhauFuel = noFuel ? true : false;
            mgc.noExhauRush = noRush ? true : false;
            mgc.toFinDest = toFinal ? true : false;
        }
        rct.StateToShield(protectx);
    }
    /// <summary>
    /// 額外生成物件
    /// </summary>
    Object_Generator.Generater obGenerate;
    private void GeneratorBlock()
    {
        obGenerate = new Object_Generator.Generater(ManageCenter.rocket_ctl.gameObject, fuelObj);
        obGenerate.Create_v3 += Vector3.up * 20;
        obGenerate.Create_r3 = Random.rotation;
        obGenerate.destoryTime = 5;
        obGenerate.Generates();
    }
    private void PlayerChange(int idx)
    {
        StaticSharp.Rocket_INFO = rct.RocketVarInfo;
        obGenerate = new Object_Generator.Generater(transform.parent.gameObject, rockets[idx]);
        obGenerate.Create_v3 = transform.position + Vector3.up;
        GameObject newPlayer = (GameObject)obGenerate.Generates();
        newPlayer.MoveComponent(rct.GetComponent<ColliderSystem>());
        //StartCoroutine(LoadCompTimer(newPlayer));
        ManageCenter.rocket_SSR = newPlayer.GetComponent<SSRocket>();
        ManageCenter.rocket_ctl = newPlayer.GetComponent<Rocket_Controll>();
        rct = newPlayer.GetComponent<Rocket_Controll>();
       
        Destroy(gameObject);
    }
    /// <summary>
    /// 狀態計時器，會先執行 true 再執行 false.
    /// </summary>
    /// <param name="smd">要切換的方法</param>
    /// <param name="sec">設定秒數</param>
    /// <returns></returns>
    private IEnumerator StatusTimer(StatusMethod smd, float sec)
    {
        smd(true);
        yield return new WaitForSeconds(sec);
        smd(false);
    }
    #endregion
    #region 事件欄位
    private void Awake()
    {
        mgc = FindObjectOfType<ManageCenter>();
        rct = GetComponent<Rocket_Controll>();
        SendControl();
    }

    [Header("顯示功能"), Space]
    private Rect windowRect = new Rect(20, 100, 200, 120);
    private Rect passWindowRect = new Rect(20, 20, 110, 45);
    private void OnGUI()
    {
        if (showPassFiled) GUI.Window(0, passWindowRect, CodeInWindows, "Code");
        if (showWindows) windowRect = GUI.Window(1, windowRect, CheatWindow, "Super Special Rocket");
    }
    /// <summary>
    /// 密碼視窗
    /// </summary>
    string[] password = { "2", "5", "4", "1" };
    bool[] inputCheck = new bool[4];
    bool isPassWindows = false;
    private void CodeInWindows(int windowID)
    {
        if (!isPassWindows) { isPassWindows = true; StartCoroutine(CodeTimer()); }
        if (GUI.Button(new Rect(90, 5, 10, 10), "X")) { showPassFiled = !showPassFiled; }
        for (int i = 0; i < password.Length; i++)
        {
            GUI.Toggle(new Rect(15 + (i * 20), 20, 50, 25), inputCheck[i], "");
        }
    }
    private void CodeEraser()
    {
        for (int i = 0; i < inputCheck.Length; i++) inputCheck[i] = false;
    }
    // 用來處理計算輸入密碼的程式，如果輸入錯誤一定次數會重置。
    // 次數不宜太低，避免才剛按下去就被重置。
    private IEnumerator CodeTimer()
    {
        int count = 0, fail=0;
        while (!inputCheck[count])
        {
            inputCheck[count] = Input.GetKey(password[count]);
            fail = Input.anyKey ? fail + 1 : fail;
            if (fail > 16) {CodeEraser(); fail = 0; count = 0; }
            if (inputCheck[count] && count < inputCheck.Length - 1) { count++; fail = 0; }
            yield return null;
        }
        yield return null;
        if (!showWindows) showWindows = inputCheck[0] && inputCheck[1] && inputCheck[2] && inputCheck[3];
        showPassFiled = false;
        isPassWindows = false;
        yield break;
    }
    /// <summary>
    /// 作弊視窗
    /// </summary>
    private void CheatWindow(int windowID)
    {
        // Make a very long rect that is 20 pixels tall.
        // This will make the window be resizable by the top
        // title bar - no matter how wide it gets.
        if (GUI.Button(new Rect(180, 5, 10, 10), "X")) showWindows = !showWindows;
        protectx = GUI.Toggle(new Rect(10, 25, 80, 25), protectx, "不會死亡");
        noFuel = (GUI.Toggle(new Rect(10, 55, 80, 25), noFuel, "不耗燃料"));
        noRush = (GUI.Toggle(new Rect(100, 25, 80, 25), noRush, "不耗衝刺"));
        //toFinal = (GUI.Toggle(new Rect(100, 55, 80, 25), toFinal, ""));
        if (GUI.Button(new Rect(10, 85, 80, 25), "+20燃料")) mgc.FuelReplen(20);
        if (fuelObj) if (GUI.Button(new Rect(100, 85, 80, 25), "移到終點")) toFinal = true;
        if (GUI.changed) SendControl();
        //
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
        // 設定歸零
        toFinal = false;
    }
    #endregion
}
