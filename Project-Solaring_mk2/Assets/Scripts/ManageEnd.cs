using UnityEngine;
using TMPro;


namespace solar_a
{
    /// <summary>
    /// �C�������t�ΡA�I�s�t��UI�C
    /// ��J���D��r�A�]�w�n��ܪ��T���A�N��b�������H�����ͤ�r�C
    /// When the End, it will display messege onto screen.
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("�����t��")]
        ManageCenter mgCenter;
        [SerializeField, Header("���D��r")]
        TMP_Text weclome;
        [SerializeField, Tooltip("��ܰT��")]
        string[] messege;

        #region ��k
        private void ShowInfo(TMP_Text tp, string player="")
        {
            float distance = ManageCenter.UI_moveDistane;
            int onfuel = ManageCenter.UI_fuel;
            string[] tmps = { "�P�@����", "�A�ڭ��h����", "�ARIP" };
            string[] msg = new string[messege.Length + tmps.Length];
            for (int i =0; i < messege.Length + tmps.Length; i++)
            {
                if (i < tmps.Length) msg[i] = tmps[i];
                else {
                    msg[i] = messege[i - tmps.Length];
                }
            }
            //foreach (string s in msg) print(s);
            tp.text = $"{player} �b {distance} ���U{msg[Random.Range(0, msg.Length)]}";
        }

        #endregion

        private void Start()
        {
            if (weclome!=null) ShowInfo(weclome);                   //��ܵ�����r
            //mgCenter.canvas_select = GetComponent<CanvasGroup>();   //�]�w�e������
            //mgCenter.CheckGame(true);
            mgCenter.show_Menu();
        }
    }
}