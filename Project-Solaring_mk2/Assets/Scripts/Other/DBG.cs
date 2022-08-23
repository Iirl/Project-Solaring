using solar_a;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Debug function.
/// 除錯用程式，把需要的功能放在這裡，透過控制中控中心的布林值達到操控的效果。
/// </summary>
public class DBG : MonoBehaviour
{
    ManageCenter mgc;
    //
    [SerializeField, Tooltip("顯示在遊戲中")]
    private bool showWindows;
    public bool isShowed { get { return showWindows; } }
    [SerializeField, Header("不會死亡")]
    private bool noDead;
    [SerializeField, Header("不耗燃料")]
    private bool noFuel;
    [SerializeField, Header("不耗衝刺")]
    private bool noRush;
    [SerializeField, Header("移到終點")]
    private bool toFinal;
    //
    private void NoDie(bool on = false) => noDead = on;
    private void NoFuel(bool on = false) => noFuel = on;
    private void NoRush(bool on = false) => noRush = on;
    private void ToFinal(bool on = false) => toFinal = on;
    public void ShowDebug(bool isOpen) => showWindows = isOpen;
    //
    private void Awake()
    {
        mgc = FindObjectOfType<ManageCenter>();

    }
    private void Update()
    {
        mgc.noDead = noDead ? true : false;
        mgc.noExhauFuel = noFuel ? true : false;
        mgc.noExhauRush = noRush ? true : false;
        mgc.toFinDest = toFinal ? true : false;

    }

    [Header("顯示功能"), Space]
    private Rect windowRect = new Rect(20, 20, 200, 100);
    private void OnGUI()
    {
        if (showWindows) windowRect = GUI.Window(0, windowRect, DoMyWindow, "Debug Window");

    }
    void DoMyWindow(int windowID)
    {
        // Make a very long rect that is 20 pixels tall.
        // This will make the window be resizable by the top
        // title bar - no matter how wide it gets.
        if (GUI.Button(new Rect(10, 20, 80, 25), "不會死亡")) NoDie(!noDead);
        if (GUI.Button(new Rect(10, 55, 80, 25), "不耗燃料")) NoFuel(!noFuel);
        if (GUI.Button(new Rect(100, 20, 80, 25), "不耗衝刺")) NoRush(!noRush);
        if (GUI.Button(new Rect(100, 55, 80, 25), "移到終點")) ToFinal(!toFinal);
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
