using solar_a;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Debug function.
/// �����ε{���A��ݭn���\���b�o�̡A�z�L��������ߪ����L�ȹF��ޱ����ĪG�C
/// </summary>
public class DBG : MonoBehaviour
{
    ManageCenter mgc;
    //
    [SerializeField, Tooltip("��ܦb�C����")]
    private bool showWindows;
    public bool isShowed { get { return showWindows; } }
    [SerializeField, Header("���|���`")]
    private bool noDead;
    [SerializeField, Header("���ӿU��")]
    private bool noFuel;
    [SerializeField, Header("���ӽĨ�")]
    private bool noRush;
    [SerializeField, Header("������I")]
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

    [Header("��ܥ\��"), Space]
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
        if (GUI.Button(new Rect(10, 20, 80, 25), "���|���`")) NoDie(!noDead);
        if (GUI.Button(new Rect(10, 55, 80, 25), "���ӿU��")) NoFuel(!noFuel);
        if (GUI.Button(new Rect(100, 20, 80, 25), "���ӽĨ�")) NoRush(!noRush);
        if (GUI.Button(new Rect(100, 55, 80, 25), "������I")) ToFinal(!toFinal);
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
