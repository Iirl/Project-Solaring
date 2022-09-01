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
    public delegate void StatusMethod(bool trunOn);
    #region 面板控制效果
    [SerializeField, Tooltip("顯示在遊戲中")]
    private bool showWindows;
    public bool isShowed { get { return showWindows; } }
    [SerializeField, Header("異常狀態"), Tooltip("不會死亡")]
    private bool noDead;
    public bool NoDead { set { noDead = value; SendControl(); } get { return noDead; } }
    [SerializeField, Tooltip("不耗燃料")]
    private bool noFuel;
    public bool NoFuel { set { noFuel = value; SendControl(); } get { return noFuel; } }
    [SerializeField, Tooltip("不耗衝刺")]
    private bool noRush;
    public bool NoRush { set { noRush = value; SendControl(); } get { return noRush; } }
    [SerializeField, Tooltip("移到終點")]
    private bool toFinal;
    [SerializeField, Header("燃料物件")]
    private GameObject fuelObj;
    [SerializeField, Header("飛船物件")]
    private GameObject[] rockets;
    #endregion
    public void ShowDebug(bool isOpen) => showWindows = isOpen;
    public void CreateBorkenRocket() => uniGenerator(0);
    /// <summary>
    /// 狀態切換控制器
    /// </summary>
    /// <param name="id">異常狀態編號</param>
    /// <param name="sec">持續時間</param>
    /// <param name="sm">額外傳入方法</param>
    public void StateControaller(int id, int sec, StatusMethod sm = null)
    {
        switch ((RocketACondition)id)
        {
            case RocketACondition.Protect:
                sm = toNoDead;
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
        StartCoroutine(StatusTimer(sm, sec));
    }

    #region 狀態處理
    //
    private void toNoDead(bool funOn = false) => NoDead = funOn;
    private void toFullGage(bool funOn = false) => NoFuel = funOn;
    private void toFullRushGage(bool funOn = false) => NoRush = funOn;
    /// <summary>
    /// 將控制內容傳給主控中心
    /// </summary>
    private void SendControl()
    {
        if (mgc)
        {
            mgc.noDead = noDead ? true : false;
            mgc.noExhauFuel = noFuel ? true : false;
            mgc.noExhauRush = noRush ? true : false;
            mgc.toFinDest = toFinal ? true : false;
        }
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
    private void uniGenerator(int idx)
    {
        GameObject player = ManageCenter.rocket_ctl.gameObject;
        obGenerate = new Object_Generator.Generater(player, rockets[idx]);
        obGenerate.Create_v3 = player.transform.position + Vector3.back * 2;
        obGenerate.Generates();
    }
    /// <summary>
    /// 狀態計時器，會先執行 true 再執行 false.
    /// </summary>
    /// <param name="smd">要切換的方法</param>
    /// <param name="sec">設定秒數</param>
    /// <returns></returns>
    private IEnumerator StatusTimer(StatusMethod smd, int sec)
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
        SendControl();
    }

    [Header("顯示功能"), Space]
    GUIStyle focuss = new GUIStyle();
    private Rect windowRect = new Rect(20, 20, 200, 120);
    private Color onColor = Color.green;
    private Color offColor = Color.gray;
    private void OnGUI()
    {
        if (showWindows) windowRect = GUI.Window(0, windowRect, CheatWindow, "Debug Window");

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
        noDead = GUI.Toggle(new Rect(10, 25, 80, 25), noDead, "不會死亡");
        noFuel = (GUI.Toggle(new Rect(10, 55, 80, 25), noFuel, "不耗燃料"));
        noRush = (GUI.Toggle(new Rect(100, 25, 80, 25), noRush, "不耗衝刺"));
        toFinal = (GUI.Toggle(new Rect(100, 55, 80, 25), toFinal, "移到終點"));
        if (GUI.Button(new Rect(10, 85, 80, 25), "+20燃料")) mgc.FuelReplen(20);
        if (fuelObj) if (GUI.Button(new Rect(100, 85, 80, 25), "燃料箱")) GeneratorBlock();
        if (GUI.changed) SendControl();
        //
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
    #endregion
}
