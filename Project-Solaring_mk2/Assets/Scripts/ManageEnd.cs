using UnityEngine;
using TMPro;


namespace solar_a
{
    /// <summary>
    /// �C�������t�ΡA�I�s�t��UI�C
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("�����t��")]
        ManageCenter mgCenter;
        [SerializeField, Header("���D��r")]
        TMP_Text weclome;

        #region ��k
        private void ShowInfo(TMP_Text tp, string messege="")
        {
            float distance = mgCenter.UI_moveDistane;
            int onfuel = mgCenter.UI_fuel;
            string[] msg = { "�P�@����","�A�ڭ��h����", "�ARIP" };
            tp.text = $"{messege} �b {distance} ���U{msg[Random.Range(0, msg.Length)]}";
        }

        #endregion

        private void Start()
        {
            ShowInfo(weclome);
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}