using UnityEngine;


namespace solar_a
{
    /// <summary>
    /// �C�������t�ΡA�I�s�t��UI�C
    /// </summary>
    public class ManageEnd : MonoBehaviour
    {

        [SerializeField, Header("�����t��")]
        ManageCenter mgCenter;

        #region ��k
        

        #endregion
        private void Start()
        {
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}