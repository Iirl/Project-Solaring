using UnityEngine;
using TMPro;

namespace solar_a
{
    /// <summary>
    /// ��汱��t�ΡA�եΨ�L�t�Ϊ��\��A����P�@�P��k�C
    /// </summary>
    public class ManageCenter : MonoBehaviour
    {
        [SerializeField, Header("�t�ο��"), Tooltip("����t��")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("���b����t��")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("�����޲z�t��")]
        SceneStage_Control ss_ctl;
        [SerializeField, Tooltip("�����޲z")]
        ManageEnd mEnd;
        [SerializeField, Header("����UI����"), Tooltip("UI �Z�����")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI �U�����")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI ����(Read Only)")]
        private int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }


        #region �@����� (Public Feild)

        #endregion

        #region �@�Τ�k (Public Method)

        ///////////// ���b�������
        /// <summary>
        /// �U���ܤ�
        /// </summary>
        /// <param name="f">��J�ثe�U��</param>
        /// <returns></returns>
        public float fuelChange(float f)
        {
            f -= Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;
            return f;
        }

        ///////////// ����ܤƬ���
        /// <summary>
        /// �H�J�ʵe
        /// </summary>
        /// <param name="cg">�Щ�J�t�� CanvasGroup ��UI�e��</param>
        public void FadeIn(CanvasGroup cg)
        {
            if (cg.alpha <1) {
                cg.alpha += 0.1f;
            } else
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }

        }
        /// <summary>
        /// �H�X�ʵe
        /// </summary>
        /// <param name="cg">�Щ�J�t�� CanvasGroup ��UI�e��</param>
        public void FadeOut(CanvasGroup cg)
        {
            if (cg.alpha > 0)
            {
                cg.alpha -= 0.1f;
            }
            else
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }

        }
        #endregion

        #region ���a�����k
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int) ss_ctl.stage_position.y;
            UI_fuel = (int) rocket_ctl.GetRocketInfo().x;
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }
        #endregion

        #region �ƥ�ե�
        private void Update()
        {
            _show_UI();
        }
        #endregion
    }
}
