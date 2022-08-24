using solar_a;
using UnityEngine;

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
    [SerializeField, Header("�U�ƪ���")]
    private GameObject fuelObj;
    //
    private void NoDie(bool on = false) => noDead = on;
    private void NoFuel(bool on = false) => noFuel = on;
    private void NoRush(bool on = false) => noRush = on;
    private void ToFinal(bool on = false) => toFinal = on;
    public void ShowDebug(bool isOpen) => showWindows = isOpen;
    private void SendControl() //�N����e�ǵ��D������
    {
        mgc.noDead = noDead ? true : false;
        mgc.noExhauFuel = noFuel ? true : false;
        mgc.noExhauRush = noRush ? true : false;
        mgc.toFinDest = toFinal ? true : false;
    }    
    /// <summary>
    /// �B�~�ͦ�����
    /// </summary>
    Object_Generator.Generater obGenerate;
    private void GeneratorBlock()
    {
        obGenerate = new Object_Generator.Generater(ManageCenter.rocket_ctl.gameObject,fuelObj);
        obGenerate.Create_v3 += Vector3.up * 20;
        obGenerate.Create_r3 = Random.rotation;
        obGenerate.destoryTime = 5;
        obGenerate.Generates();
        print(obGenerate.Create_v3);
    }

    private void Awake()
    {
        mgc = FindObjectOfType<ManageCenter>();
        SendControl();
    }

    [Header("��ܥ\��"), Space]
    GUIStyle focuss = new GUIStyle();
    private Rect windowRect = new Rect(20, 20, 200, 120);
    private Color onColor = Color.green;
    private Color offColor = Color.gray;
    private void OnGUI()
    {
        if (showWindows) windowRect = GUI.Window(0, windowRect, CheatWindow, "Debug Window");

    }
    /// <summary>
    /// �@������
    /// </summary>
    private void CheatWindow(int windowID)
    {
        // Make a very long rect that is 20 pixels tall.
        // This will make the window be resizable by the top
        // title bar - no matter how wide it gets.
        noDead = GUI.Toggle(new Rect(10, 25, 80, 25), noDead, "���|���`");
        noFuel = (GUI.Toggle(new Rect(10, 55, 80, 25), noFuel, "���ӿU��"));
        noRush = (GUI.Toggle(new Rect(100, 25, 80, 25), noRush, "���ӽĨ�"));
        toFinal = (GUI.Toggle(new Rect(100, 55, 80, 25), toFinal, "������I")) ;
        if (GUI.Button(new Rect(10, 85, 80, 25), "+20�U��")) mgc.FuelReplen(20);
        if (fuelObj) if (GUI.Button(new Rect(100, 85, 80, 25), "�U�ƽc")) GeneratorBlock();

        if (GUI.changed) SendControl();
        //
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
